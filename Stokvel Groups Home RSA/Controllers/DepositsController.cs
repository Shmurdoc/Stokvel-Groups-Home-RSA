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
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;
using Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet;

namespace Stokvel_Groups_Home.Controllers
{
    public class DepositsController : Controller
    {

        private readonly IAccountRequestServices _accountRequestServices;
        private readonly IDepositService _depositService;
        private readonly WalletInfo _walletInfo;
        private readonly PreDepositInfo _preDepositInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupRequestServices _groupRequestServices;
        private readonly IAccountProfileRequestServices _accountProfileRequestServices;



        public DepositsController(IUnitOfWork unitOfWork, IDepositService depositService, IAccountRequestServices accountRequestServices,
            WalletInfo walletInfo, PreDepositInfo preDepositInfo, IGroupRequestServices groupRequestServices, IAccountProfileRequestServices accountProfileRequestServices)
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
            TempData["accountId"] = accountId;
            TempData["groupId"] = groupId;

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
                totalAccountFund.PreDeposit.Amount = 0;
            }

            ViewBag.TargetAmount = groupMemberTarget;

            if (groupMemberTarget == memberAccount?.PreDeposit?.Amount ||
                totalAccountFund?.AccountProfile?.StatusRank == MemberStatuses.Platinum)
            {
                ViewBag.PreDepo = 0;
                ViewBag.Wallet = wallet?.Wallet?.Amount;
            }
            else if (totalAccountFund?.AccountProfile?.StatusRank == MemberStatuses.PendingPayment)
            {
                var preDepoAmount = groupMemberTarget - (groupMemberTarget / 5);
                ViewBag.PreDepo = preDepoAmount;
                ViewBag.Wallet = wallet?.Wallet?.Amount;
            }
            else
            {
                var preDepoAmount = groupMemberTarget - totalAccountFund?.PreDeposit?.Amount;
                ViewBag.PreDepo = preDepoAmount;
                ViewBag.Wallet = wallet?.Wallet?.Amount;
            }

            return View();
        }


        // POST: Deposits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepositId,InvoiceId,DepositAmount,DepositDate,MethodId,PaymentStatusId,DepositReference,GroupVerifyKey")] int accountId, int groupId, Deposit deposit)
        {
            accountId = GetTempDataValue("accountId", accountId);
            groupId = GetTempDataValue("groupId", groupId);

            var userId = User.Identity.GetUserId();
            var memberStatus = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
            string description = "deposit";

            try
            {
                var preDepositAmount = await GetPreDepositAmount(accountId);
                var memberDepoTarget = await _groupRequestServices.CalculateAmountTarget(groupId);
                var memberDepoInfo = await _preDepositInfo.PreDepoMembersAsync(accountId);
                var memberDescription = await GetMemberDescription(_preDepositInfo, accountId);
                var excess = CalculateExcess(_preDepositInfo, deposit, memberDepoTarget, memberDescription);


                if (ModelState.IsValid)
                {
                    if (await IsTargetMet(groupId, preDepositAmount))
                    {
                        deposit.DepositAmount = deposit.DepositAmount - excess;
                        await ProcessDeposit(deposit, description, accountId, userId, excess, memberDescription, memberDepoTarget);
                        this.AddAlertSuccess("Success! Your Deposit was successful.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        description = "PreDeposit";

                        if (memberStatus?.AccountProfile?.StatusRank == MemberStatuses.PendingPayment && excess == 0)
                        {
                            var pendingAmount = memberDepoTarget + (memberDepoTarget / 5);

                            ModelState.AddModelError("DepositAmount", "Deposit amount must be" + pendingAmount + ".");
                            return View(deposit);
                        }
                        else
                        {
                            await ProcessDeposit(deposit, description, accountId, userId, excess, memberDescription, memberDepoTarget);
                            this.AddAlertSuccess("Success! Your Deposit was successful.");
                        }
                    }
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception
                this.AddAlertDanger("Failed! The Stokvel has not yet begun; please try again when it is active.");
            }

            return View(deposit);
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

        private async Task<decimal> GetPreDepositAmount(int accountId)
        {
            var preDepo = await _unitOfWork.AccountsRepository.GetAllAsync(x => x.AccountId == accountId, includeProperties: "PreDeposit");
            return preDepo?.Select(x => x.PreDeposit?.Amount).FirstOrDefault() ?? 0.00m;
        }

        private async Task<int> GetMemberDescription(PreDepositInfo? memberDepoInfo, int accountId)
        {
            var groupAccount = await _unitOfWork.AccountsRepository.GetAllAsync();
            return groupAccount.Where(x => x.GroupId == memberDepoInfo?.AccountPreDeposit?.Account?.GroupId && x.AccountQueueStart.Month == DateTime.Now.Month)
                               .Select(x => x.AccountQueue).FirstOrDefault();
        }

        private decimal CalculateExcess(
         PreDepositInfo? memberDepoInfo,
         Deposit? deposit,
         decimal memberDepoTarget,
         int memberDescription)
        {
            var memberAmountTotal = memberDepoInfo?.DepositToAccount?.Deposit?
                .Where(x => x.DepositDate.Month == DateTime.Now.Month &&
                            x.DepositReference == "Deposit" + memberDescription)
                .Sum(x => x.DepositAmount) ?? 0m;

            memberAmountTotal += deposit?.DepositAmount ?? 0m;

            return memberAmountTotal > memberDepoTarget
                ? memberAmountTotal - memberDepoTarget
                : 0m;
        }


        private async Task<bool> IsTargetMet(int groupId, decimal preDepositAmount)
        {
            var groupMember = await _unitOfWork?.GroupsRepository?.GetByIdAsync(groupId);
            return groupMember.AccountTarget == preDepositAmount;
        }

        private async Task ProcessDeposit(Deposit deposit, string description, int accountId, string userId, decimal excess, int memberDescription, decimal memberDepoTarget)
        {
            var memberPreDepoAccountList = await _unitOfWork.PreDepositRepository.GetAllAsync(x => x.AccountId == accountId);
            var profileStatus = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
            var memberExpectedDepoAmount = await _preDepositInfo.PreDepoMembersAsync(accountId);
            var pendingPaymentTotal = memberDepoTarget / 5;
            decimal memberPreDepoTarget = 0;

            if (memberDescription != 0)
            {
                description = description + memberDescription;
                if (memberDescription < 5)
                {
                    memberPreDepoTarget = memberDepoTarget / (5 - memberDescription);
                }
            }

            if (excess > 0)
            {
                await DepositToWallet(deposit, description, accountId, userId, excess, memberDepoTarget);
            }
            else
            {
                if (profileStatus.AccountProfile.StatusRank != MemberStatuses.PendingPayment)
                {
                    if (memberExpectedDepoAmount?.PreDeposit?.Amount + deposit.DepositAmount <= memberPreDepoTarget)
                    {
                        description = "PreDeposit";
                        await _depositService.PreDepositRequestAsync(deposit, description, accountId, userId);
                    }
                    else
                    {
                        await _depositService.PreDepositRequestAsync(deposit, description, accountId, userId);
                    }
                }
                else
                {
                    await _depositService.PreDepositRequestAsync(deposit, description, accountId, userId);
                }
            }
        }

        private async Task DepositToWallet(Deposit deposit, string? description, int accountId, string? userId, decimal excess, decimal memberDepoTarget)
        {
            description = "Wallet";
            var memberStatus = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
            string? depositDescription = description;
            var pendingPaymentTotal = memberDepoTarget / 5;
            var excessDeposit = deposit;
            var walletMoney = new Wallet { Id = userId, Amount = new() };

            if (memberStatus?.AccountProfile?.StatusRank == MemberStatuses.PendingPayment)
            {
                if (excess <= pendingPaymentTotal)
                {
                    excessDeposit.DepositAmount = excess;
                    await _depositService.PreDepositRequestAsync(excessDeposit, description, accountId, userId);
                    deposit.DepositAmount -= excess;
                    await _depositService.DepositRequestAsync(deposit, description, accountId, userId);
                }
                else if (excess > pendingPaymentTotal && excess != 0)
                {
                    var newExcess = excess - pendingPaymentTotal;
                    description = "Wallet";
                    excessDeposit.DepositAmount = newExcess;
                    walletMoney.Amount = newExcess;
                    await _depositService.PreDepositRequestAsync(excessDeposit, description, accountId, userId); // Send to Wallet
                    excessDeposit.DepositAmount = pendingPaymentTotal;
                    await _depositService.DepositRequestAsync(deposit, description, accountId, userId);
                    await Wallet(walletMoney); // Sending to Deposit
                    deposit.DepositAmount -= excess;
                    await _depositService.PreDepositRequestAsync(deposit, depositDescription, accountId, userId);
                }
            }
            else
            {
                if (excess > pendingPaymentTotal && excess != 0)
                {
                    description = "Wallet";
                    excessDeposit.DepositAmount = excess;
                    walletMoney.Amount = excess;
                    await _depositService.PreDepositRequestAsync(excessDeposit, description, accountId, userId); // Sending to Deposit
                    deposit.DepositAmount -= excess;
                    await _depositService.PreDepositRequestAsync(deposit, depositDescription, accountId, userId);
                }
            }
        }



        private async Task Wallet(Wallet walletMoney)
        {
            var walletData = await _unitOfWork?.WalletRepository?.GetAllAsync();
            if (walletData.Any(w => w.Id == walletMoney.Id))
            {
                _unitOfWork?.WalletRepository?.Update(walletMoney);
            }
            else
            {
                _unitOfWork?.WalletRepository?.Add(walletMoney);
            }
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
            if (id != deposit.DepositId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepositRepository.Update(deposit);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!DepositExists(deposit.DepositId))
                    //{
                    //	return NotFound();
                    //}
                    //else
                    //{
                    //	throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }

            return View(deposit);
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