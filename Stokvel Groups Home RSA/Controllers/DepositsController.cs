using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.common.Alert.TempData;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class DepositsController : Controller
    {

        private readonly IAccountRequestServices _accountRequestServices;
        private readonly IDepositService _depositService;
        private readonly IWalletRequestServices _walletInfo;
        private readonly IPreDepositRequestServices _preDepositInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupRequestServices _groupRequestServices;
        private readonly IAccountProfileRequestServices _accountProfileRequestServices;
        private readonly ILogger<DepositsController> _logger;


        public DepositsController(IUnitOfWork unitOfWork, IDepositService depositService, IAccountRequestServices accountRequestServices,
            IWalletRequestServices walletInfo, IPreDepositRequestServices preDepositInfo, IGroupRequestServices groupRequestServices,
            IAccountProfileRequestServices accountProfileRequestServices)
        {
            _unitOfWork = unitOfWork;
            _depositService = depositService;
            _accountRequestServices = accountRequestServices;
            _walletInfo = walletInfo;
            _preDepositInfo = preDepositInfo;
            _groupRequestServices = groupRequestServices;
            _accountProfileRequestServices = accountProfileRequestServices;
        }

        // GET: Deposits
        public async Task<IActionResult> Index()
        {

            var listAllDeposits = await _unitOfWork.DepositRepository.GetAllAsync();
            return View(listAllDeposits);
        }

        // GET: Deposits/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var deposit = await _unitOfWork.DepositRepository.GetByIdAsync(id);

            if (deposit == null)
            {
                return NotFound();
            }
            return View(deposit);
        }

        // GET: Deposits/Create
        [HttpGet]
        public async Task<IActionResult> Create(int accountId, int groupId)
        {
            bool preDepositExists = false;

            TempData["accountId"] = accountId;
            TempData["groupId"] = groupId;
            int pendingPaymentRound = 0;

            var memberDescription = await _depositService.GetMemberDescription(accountId);
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);

            if (memberDescription >= 5)
            {
                pendingPaymentRound = memberDescription;
            }


            var memberAccount = await _unitOfWork.AccountsRepository.GetByIdAsync(accountId);
            var preDeposits = await _unitOfWork.AccountsRepository.GetAllAsync(
                a => a.AccountId == accountId,
                includeProperties: "PreDeposit"
            );

            var groupMemberTarget = await _groupRequestServices.CalculateAmountTarget(groupId);
            var wallet = await _walletInfo.WalletGetAmountAsync(accountId);


            var totalAccountFund = await _preDepositInfo.CheckPreDepositStatusDepositAsync(accountId);
            if (totalAccountFund?.PreDeposit?.Amount != null)
            {
                ViewBag.PreDepo = totalAccountFund.PreDeposit.Amount;
                preDepositExists = true;
            }

            ViewBag.TargetAmount = groupMemberTarget;

            if (groupMemberTarget == memberAccount?.PreDeposit?.Amount ||
                totalAccountFund?.AccountProfile?.StatusRank == MemberStatuses.Platinum)
            {
                ViewBag.PreDepo = totalAccountFund.PreDeposit.Amount;
                ViewBag.Wallet = wallet?.Wallet?.Amount ?? 0;
                ViewBag.AllowPendingPayment = await AllowPendingPayment(groupId);
                ViewBag.Group = group.Active;
            }
            else if (totalAccountFund?.AccountProfile?.StatusRank == MemberStatuses.PendingPayment)
            {
                var preDepoAmount = groupMemberTarget - (groupMemberTarget / 5 - pendingPaymentRound);
                ViewBag.PreDepo = preDepoAmount;
                ViewBag.Wallet = wallet?.Wallet?.Amount ?? 0;
                ViewBag.AllowPendingPayment = await AllowPendingPayment(groupId);
            }
            else
            {
                var preDepoAmount = groupMemberTarget - totalAccountFund?.PreDeposit?.Amount ?? 0;

                ViewBag.PreDepo = preDepoAmount;
                ViewBag.Wallet = wallet?.Wallet?.Amount;
                ViewBag.AllowPendingPayment = await AllowPendingPayment(groupId);
            }

            return View();
        }

        public async Task<bool> AllowPendingPayment(int groupId)
        {
            bool preDepositPendingSpace = false;
            var accountMembers = await _unitOfWork.AccountsRepository.GetAllAsync(g => g.GroupId == groupId);
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);

            int countPendingMembers = 0;
            int memberPendingPaymentLimit = 0;

            foreach (var member in accountMembers)
            {
                var memberAccountProfile = await _accountProfileRequestServices.AccountProfileInfoAsync(member.Id);

                if (memberAccountProfile.AccountProfile.StatusRank == MemberStatuses.PendingPayment)
                {
                    countPendingMembers++;
                }
            }

            switch (group.TotalGroupMembers)
            {
                case 5:
                case 6:
                    memberPendingPaymentLimit = 1;
                    break;
                case 7:
                    memberPendingPaymentLimit = 2;
                    break;
                case 8:
                    memberPendingPaymentLimit = 3;
                    break;
                case 9:
                    memberPendingPaymentLimit = 3;
                    break;
                case 10:
                case 11:
                    memberPendingPaymentLimit = 4;
                    break;
                case 12:
                    memberPendingPaymentLimit = 5;
                    break;
                default:
                    memberPendingPaymentLimit = 0;
                    break;
            }

            if (countPendingMembers <= memberPendingPaymentLimit)
            {
                preDepositPendingSpace = true;
            }

            return preDepositPendingSpace;
        }


        // POST: Deposits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepositId,InvoiceId,DepositAmount,DepositDate,MethodId,PaymentStatusId,DepositReference,GroupVerifyKey")] int accountId, int groupId, Deposit deposit, string dropdownValue)
        {
            accountId = GetTempDataValue("accountId", accountId);
            groupId = GetTempDataValue("groupId", groupId);

            var userId = User.Identity.GetUserId();
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
            var memberStatus = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
            string description = "Deposit";

            try
            {
                var preDepositAmount = await _depositService.GetPreDepositAmount(accountId);
                var memberDepoTarget = await _groupRequestServices.CalculateAmountTarget(groupId);
                var memberDepoInfo = await _preDepositInfo.PreDepoMembersAsync(accountId);
                var memberDescription = await _depositService.GetMemberDescription(groupId);
                var excess = await _depositService.CalculateExcess(deposit, memberDepoTarget, memberDescription, accountId);


                int pendingPaymentRound = 0;

                if (memberDescription >= 5)
                {
                    pendingPaymentRound = memberDescription;
                }

                if (ModelState.IsValid)
                {

                    if (await _depositService.IsTargetMet(groupId, deposit.DepositAmount))
                    {
                        deposit.DepositAmount = deposit.DepositAmount - excess;
                        await _depositService.ProcessDeposit(deposit, description, accountId, userId, excess, memberDescription, memberDepoTarget, dropdownValue);
                        this.AddAlertSuccess("Success! Your Deposit was successful.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {


                        if (memberStatus?.AccountProfile?.StatusRank == MemberStatuses.PendingPayment && excess == 0 && group.Active == true)
                        {
                            description = "PreDeposit";
                            var pendingAmount = (memberDepoTarget - preDepositAmount) + (memberDepoTarget / 5 - memberDescription);

                            ModelState.AddModelError("DepositAmount", "Deposit amount must be R" + pendingAmount + ".");
                            return View(deposit);
                        }
                        else //
                        {
                            await _depositService.ProcessDeposit(deposit, description, accountId, userId, excess, memberDescription, memberDepoTarget, dropdownValue);
                            /*this.AddAlertSuccess("Success! Your Deposit was successful.");*/
                        }
                    }
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception
                this.AddAlertDanger("Failed! The Stokvel has not yet begun; please try again when it is active.");
            }
            return RedirectToAction("Index", "Deposits");
        }

        private int GetTempDataValue(string key, int defaultValue)
        {
            if (TempData[key] != null)
            {
                defaultValue = (int)TempData[key];
                TempData.Keep(key);
            }
            return defaultValue;
        }


        // GET: Deposits/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var editDepositInDb = await _unitOfWork.DepositRepository.GetByIdAsync(id);

            if (id == 0 || editDepositInDb == null)
            {
                return NotFound();
            }

            return View(editDepositInDb);
        }

        // POST: Deposits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepositId,InvoiceId,DepositAmount,DepositDate,PrepaymentId,MethodId,PaymentStatusId,DepositReference")] Deposit deposit)
        {
            // Ensure the ID in the URL matches the deposit ID
            if (id != deposit.DepositId)
            {
                return NotFound();
            }

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Update the deposit in the repository
                    _unitOfWork.DepositRepository.Update(deposit);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Check if the deposit still exists
                    if (!await DepositExists(deposit.DepositId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Log the exception and rethrow it
                        _logger.LogError(ex, "Concurrency error while updating deposit with ID {DepositId}.", deposit.DepositId);
                        throw;
                    }
                }
                // Redirect to the index action after successful update
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, return the view with the deposit object
            return View(deposit);
        }


        private async Task<bool> DepositExists(int id)
        {
            return await _unitOfWork.DepositRepository.GetByIdAsync(id) != null;
        }


        // GET: Deposits/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var displayDelDepositInDb = await _unitOfWork.DepositRepository.GetByIdAsync(id);
            if (id < 0 || displayDelDepositInDb == null)
            {
                return NotFound();
            }


            return View(displayDelDepositInDb);
        }

        // POST: Deposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delDepositInDb = await _unitOfWork.DepositRepository.GetByIdAsync(id);
            if (id < 0 || delDepositInDb == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Deposits'  is null.");
            }

            _unitOfWork.DepositRepository.RemoveAsync(delDepositInDb);
            return RedirectToAction(nameof(Index));
        }

        //private bool DepositExists(int id)
        //{
        //	var exists = _depositCRUDService.DepositExists(id);
        //	return (exists);
        //}









































































    }
}