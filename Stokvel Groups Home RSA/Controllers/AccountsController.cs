using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.common.Alert.TempData;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using System.Globalization;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class AccountsController : Controller
    {

        private readonly ILogger<AccountsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRequestServices _accountRequestService;
        public readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        public readonly IGroupRequestServices _groupRequestService;

        public static ApplicationAccount? ApplicationAccount { get; private set; }
        public static PreDepositMembers? PreDepositMembers { get; private set; }


        public AccountsController(

            ILogger<AccountsController> logger,
            IUnitOfWork unitOfWork,
            IAccountRequestServices accountRequestService,
            Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager,
            IGroupRequestServices groupRequestServices
            )
        {
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _accountRequestService = accountRequestService;
            _groupRequestService = groupRequestServices;
        }
        public async Task<ApplicationAccount?> ListOfAccounts(ApplicationAccount applicationAccount)
        {
            var accounts = new List<Account>();
            var applicationUsers = await _unitOfWork.ApplicationUserRepository.GetAllAsync();

            if (applicationUsers == null)
            {
                return null; // Return null if no application users are found
            }

            applicationAccount = new ApplicationAccount
            {
                Account = accounts,
                ApplicationUsers = new List<ApplicationUser>()
            };

            foreach (var member in applicationUsers)
            {
                var identityUser = await _userManager.FindByNameAsync(member.UserName);
                if (identityUser != null)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(identityUser, "Admin");
                    if (!isAdmin)
                    {
                        applicationAccount.ApplicationUsers.Add(member);
                    }
                }
            }

            // Retrieve accounts for each non-admin user
            foreach (var user in applicationAccount.ApplicationUsers)
            {
                var userAccounts = await _unitOfWork.AccountsRepository.GetAllAsync(
                    x => x.Id == user.Id,
                    includeProperties: "Group"
                );
                accounts.AddRange(userAccounts);
            }

            return applicationAccount;
        }


        //GET: Accounts
        [HttpGet]
        public async Task<IActionResult> IndexAdmin()
        {
            var userId = User.Identity.GetUserId();
           

            // Retrieve all application users
            var applicationUsers =  await ListOfAccounts(ApplicationAccount);

            if (applicationUsers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicationUsers' is null.");
            }

            // Group accounts by TypeAccount and select the first account in each group
            applicationUsers.Account = applicationUsers.Account
                .GroupBy(x => x.Group.TypeAccount)
                .Select(g => g.FirstOrDefault())
                .ToList();

            return View(applicationUsers);
        }
        [HttpGet]
        public async Task<IActionResult> AdminToAcccount(AccountType accountType)
        {
            var userId = User.Identity.GetUserId();

            //in group by type
            // Retrieve all application users
            var applicationUsers = await ListOfAccounts(ApplicationAccount);
            if (applicationUsers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicationUsers' is null.");
            }

            // Group accounts by TypeAccount and select the first account in each group
            applicationUsers.Account = applicationUsers.Account
                .GroupBy(x => x.Group.TypeAccount)
                .Select(g => g.FirstOrDefault())
                .ToList();

            var groupMembers = applicationUsers.Account?.GroupBy(x => x.Group?.GroupName).Select(x => x.FirstOrDefault()).ToList();
            applicationUsers.Account = groupMembers;
            return applicationUsers != null ?
                          View(applicationUsers) :
                          Problem("Entity set 'ApplicationDbContext.Accounts'  is null.");
        }

        //[HttpGet]
        public async Task<IActionResult> AdminMembersDashboard(AccountType? accountType, string? groupName, int groupId)
        {
            ViewBag.image = "/wwwroot/images/Profile";
            var userId = User.Identity.GetUserId();
            var userAccounts = new List<ApplicationUser>();

            // Get all members in the specified account type
            var allMembersInTypeAccount = await ListOfAccounts(ApplicationAccount);

            var memberAccount = allMembersInTypeAccount.Account?
                .Where(x => x.Group?.GroupName == groupName)
                .ToList();

            // Get pre-deposit list for the group fee
            var preDepositList = await _accountRequestService.GroupFeePreDepoAsync(memberAccount);

            if (memberAccount != null)
            {
                foreach (var account in memberAccount)
                {
                    var accountUser = allMembersInTypeAccount.ApplicationUsers?
                        .FirstOrDefault(x => x.Id == account.Id);
                    if (accountUser != null)
                    {
                        userAccounts.Add(accountUser);
                    }
                }
            }

            var preDepositMembers = new PreDepositMembers
            {
                ApplicationUser = userAccounts,
                Account = memberAccount,
                PreDeposit = preDepositList
            };

            // ViewBag Area
            ViewBag.StokvelActive = preDepositMembers.Account.Select(x=>x.Group.Active).FirstOrDefault();
            ViewBag.UserId = userId;

            return View(preDepositMembers);
        }


        // GET: Accounts
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(IndexAdmin));
            }

            var userId = User.Identity.GetUserId();

            // Request service to get group type
            var requestAccountType = await _accountRequestService.MembersOfAccountList(userId);

            if (requestAccountType == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Accounts' is null.");
            }

            requestAccountType.Account = requestAccountType.Account?
                .GroupBy(x => x.Group.TypeAccount)
                .Select(x => x.FirstOrDefault())
                .ToList();

            return View(requestAccountType);
        }



        [HttpGet]
        public async Task<IActionResult> Index1(AccountType? accountType, string? groupName, int groupId)
        {
            ViewBag.image = "/wwwroot/images/Profile";
            var userId = User.Identity.GetUserId();
            var userAccounts = new List<ApplicationUser>();

            ViewBag.UserId = userId;

            var allMembersInTypeAccount = await _accountRequestService.InAccountListAsync(userId, accountType);
            var memberAccount = allMembersInTypeAccount.Account?
                .Where(x => x.Group.GroupName == groupName)
                .ToList();

            var groupInfo = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
            string? resultLocalPage = null;

            if (memberAccount != null)
            {
                foreach (var account in memberAccount)
                {
                    var accountUser = allMembersInTypeAccount.ApplicationUsers?
                        .FirstOrDefault(x => x.Id == account.Id);
                    if (accountUser != null)
                    {
                        userAccounts.Add(accountUser);
                    }
                }
            }

            var applicationAccount = new ApplicationAccount
            {
                ApplicationUsers = userAccounts,
                Account = memberAccount
            };

            if (groupInfo.TotalGroupMembers == applicationAccount.Account?.Count(x => x.Accepted == true))
            {
                resultLocalPage = $"/Accounts/AcceptedMembersDashboard?groupId={groupId}&AccountType={accountType}&GroupName={groupName}";
            }

            if (resultLocalPage != null)
            {
                return LocalRedirect(resultLocalPage);
            }

            return View(applicationAccount);
        }



        [HttpGet]
        public async Task<IActionResult> AcceptedMembersDashboard(AccountType? accountType, string? groupName, int groupId)
        {
            ViewBag.image = "/wwwroot/images/Profile";
            var userId = User.Identity.GetUserId();
            var userAccounts = new List<ApplicationUser>();

            // Get all members in the specified account type
            var allMembersInTypeAccount = await _accountRequestService.InAccountListAsync(userId, accountType);

            var memberAccount = allMembersInTypeAccount.Account?
                .Where(x => x.Group?.GroupName == groupName)
                .ToList();

            // Get pre-deposit list for the group fee
            var preDepositList = await _accountRequestService.GroupFeePreDepoAsync(memberAccount);

            if (memberAccount != null)
            {
                foreach (var account in memberAccount)
                {
                    var accountUser = allMembersInTypeAccount.ApplicationUsers?
                        .FirstOrDefault(x => x.Id == account.Id);
                    if (accountUser != null)
                    {
                        userAccounts.Add(accountUser);
                    }
                }
            }

            var preDepositMembers = new PreDepositMembers
            {
                ApplicationUser = userAccounts,
                Account = memberAccount,
                PreDeposit = preDepositList
            };

            // ViewBag Area
            ViewBag.StokvelActive = preDepositMembers.Account.Select(x=>x.Group.Active).FirstOrDefault();
            ViewBag.UserId = userId;
            ViewData["GroupId"] = preDepositMembers.Account.Select(x => x.GroupId).FirstOrDefault();


            return View(preDepositMembers);
        }



        [HttpGet]
        public async Task<IActionResult> StartStokvel(int accountId, int groupId)
        {
            var userId = User.Identity.GetUserId();
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);

            if (group == null)
            {
                this.AddAlertDanger("Group not found. Please try again.");
                return LocalRedirect("/Accounts/AcceptedMembersDashboard");
            }

            var groupMembers = await GetGroupMembersAsync(groupId);
            var (paidMembers, notPaidMembers) = await CategorizeMembersAsync(groupMembers, groupId);

            if (notPaidMembers.Any())
            {
                NotifyUnpaidMembers(notPaidMembers);
                return RedirectToDashboard(group);
            }

            if (paidMembers.Count == group.TotalGroupMembers)
            {
                await UpdateMemberQueueAsync(paidMembers, groupId);
                await UpdateGroupStatus(groupId);
            }
            else
            {
                this.AddAlertDanger("Failed! Something went wrong. Please try again.");
            }

            return RedirectToDashboard(group);
        }

        private async Task<List<Account>> GetGroupMembersAsync(int groupId)
        {
            var accounts = await _unitOfWork.AccountsRepository.GetAllAsync(includeProperties: "PreDeposit");
            return accounts.Where(x => x.GroupId == groupId).ToList();
        }

        private async Task<(List<ApplicationUser> paidMembers, List<ApplicationUser> notPaidMembers)> CategorizeMembersAsync(List<Account> groupMembers, int groupId)
        {
            List<ApplicationUser> paidMembers = new();
            List<ApplicationUser> pendingMember = new();
            List<ApplicationUser> notPaidMembers = new();

            foreach (var memberAccount in groupMembers)
            {
                var applicationUser = await _unitOfWork.ApplicationUserRepository.GetAllAsync(a => a.Id == memberAccount.Id, includeProperties: "AccountProfiles");
                var userTarget = await _groupRequestService.CalculateAmountTarget(groupId);

                if (memberAccount.PreDeposit != null && memberAccount.PreDeposit.Amount == userTarget)
                {
                    paidMembers.Add(applicationUser.SingleOrDefault());
                }else if (applicationUser.All(x => x.AccountProfiles.StatusRank == MemberStatuses.PendingPayment))
                {
                    pendingMember.Add(applicationUser.SingleOrDefault());
                }
                else
                {
                    notPaidMembers.Add(applicationUser.SingleOrDefault());
                }
            }

            paidMembers = paidMembers.Concat(pendingMember).OrderByDescending(x=>x.AccountProfiles.MembershipRank).ToList();

            return (paidMembers, notPaidMembers);
        }

        private void NotifyUnpaidMembers(List<ApplicationUser> notPaidMembers)
        {
            foreach (var member in notPaidMembers)
            {
                this.AddAlertDanger($"{member.UserName} has not yet paid the deposit. Please remind the member to pay.");
            }
        }

        private async Task UpdateMemberQueueAsync(List<ApplicationUser> paidMembers, int groupId)
        {
          

            var orderedPaidMembers = paidMembers.OrderByDescending(x => x.AccountProfiles.Id).ToList();
            int memberCount = 0;
            foreach (var member in orderedPaidMembers)
            {
                var memberInDb = await _unitOfWork.AccountsRepository.GetAllAsync(x=>x.GroupId == groupId);
                var memberInList = memberInDb.Where(a => a.Id == member.Id).FirstOrDefault();

                if (memberInList == null)
                {
                    throw new Exception("Member not found in the list.");
                }
                
                memberCount = memberCount+1;
                memberInList.AccountQueue = memberCount;
                DateTime dayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dayEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 25);
                DateTime dayStartModify = dayStart.AddMonths(memberCount);
                DateTime dayEndModify = dayEnd.AddMonths(memberCount);


                memberInList.AccountQueueStart = dayStartModify;
                memberInList.AccountQueueEnd = dayEndModify;

                _unitOfWork.AccountsRepository.Update(memberInList);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task UpdateGroupStatus(int groupId)
        {
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);

            group.Active = true;

            _unitOfWork.GroupsRepository.Update(group);
            await _unitOfWork.SaveChangesAsync();
        }

        private IActionResult RedirectToDashboard(Group group)
        {
            var resultUrl = $"/Accounts/AcceptedMembersDashboard?AccountType={group.TypeAccount}&GroupName={group.GroupName}";
            return LocalRedirect(resultUrl);
        }



        [HttpGet]
        public async Task<IActionResult> Index2(AccountType accountType)
        {
            var userId = User.Identity.GetUserId();

            //in group by type
            ApplicationAccount = await _accountRequestService.InAccountListAsync(userId, accountType);
            
            return ApplicationAccount != null ?
                          View(ApplicationAccount) :
                          Problem("Entity set 'ApplicationDbContext.Accounts'  is null.");
        }


        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(id);
            if (id == null || account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Id,GroupVerifyKey,AccountCreated")] Account? account)
        {
            if (account == null)
            {
                return BadRequest("Account cannot be null.");
            }

            var userId = User.Identity.GetUserId();
            var accountExists = _unitOfWork.AccountsRepository.AccountExists(userId, account.GroupVerifyKey);
            var groupExists = _unitOfWork.GroupsRepository.GroupExists(account.GroupVerifyKey);

            if (!accountExists)
            {
                if (groupExists)
                {
                    var accountProfiles = await _unitOfWork.AccountProfileRepository.GetAllAsync();

                    if (!accountProfiles.Any(x => x.Id == userId))
                    {
                        await _accountRequestService.AddAccountToGroupAsync(account, userId);
                    }
                    else
                    {
                        var updateProfile = accountProfiles.FirstOrDefault(x => x.Id == userId);
                        if (updateProfile != null)
                        {
                            updateProfile.GroupsJoined += 1;
                            _unitOfWork.AccountProfileRepository.Update(updateProfile);
                        }
                        account.AccountCreated = DateTime.Now;
                        await _unitOfWork.AccountsRepository.Add(account);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("GroupVerifyKey", "Group does not exist. Please provide a correct Group Verification Key.");
                }
            }
            else
            {
                ModelState.AddModelError("GroupVerifyKey", "Verification Key has already been used.");
            }

            return View(account);
        }



        // GET: Accounts/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(id);

            if (id == 0 || account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Id,GroupVerifyKey,AccountCreated")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    account.Accepted = true;
                    _unitOfWork.AccountsRepository.Update(account);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!Exists(account.AccountId))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(id);
            if (id == null || account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(id);
            if (account == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Accounts'  is null.");
            }
            else
            {
                await _unitOfWork.AccountsRepository.RemoveAsync(account);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }




        [HttpGet]
        public async Task<IActionResult> AcceptedMembers(int id, string groupName, int groupId, AccountType accountType, bool acceptStatus)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }
           var allMembersInTypeAccount = new ApplicationAccount();

            if (User.IsInRole("Admin"))
            {
                allMembersInTypeAccount = await this.ListOfAccounts(ApplicationAccount);
            }
            else
            {
                allMembersInTypeAccount = await _accountRequestService.InAccountListAsync(userId, accountType);
            }
             
            if (allMembersInTypeAccount == null)
            {
                return NotFound("No members found for the specified account type.");
            }

            var memberAccounts = allMembersInTypeAccount.Account?
                .Where(x => x.Group.GroupName == groupName)
                .ToList();

            if (memberAccounts == null || !memberAccounts.Any())
            {
                return NotFound("No members found in the specified group.");
            }

            var userAccounts = memberAccounts
                .Select(account => allMembersInTypeAccount.ApplicationUsers?
                    .FirstOrDefault(x => x.Id == account.Id))
                .Where(accountUser => accountUser != null)
                .ToList();

            var applicationAccount = new ApplicationAccount
            {
                ApplicationUsers = userAccounts,
                Account = memberAccounts
            };

            if (applicationAccount.ApplicationUsers.Count <= 12)
            {
                var account = await _unitOfWork.AccountsRepository.GetByIdAsync(id);
                if (account == null)
                {
                    return NotFound("Account not found.");
                }

                try
                {
                    account.Accepted = acceptStatus;
                    _unitOfWork.AccountsRepository.Update(account);
                    await _unitOfWork.SaveChangesAsync();

                    string status = "Success!";
                    TempData["AcceptStatus"] = acceptStatus;

                    if (acceptStatus)
                    {
                        this.AddAlertSuccess($"{status} A new member has been added successfully.");
                    }
                    else
                    {
                        this.AddAlertSuccess($"{status} A member has been removed successfully.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    string status = "Failed!";
                    this.AddAlertDanger($"{status} Something went wrong, please try again.");
                }

                groupName = groupName.Replace(" ", "+");
                var result = $"/Accounts/Index1?groupId={groupId}&AccountType={accountType}&GroupName={groupName}";
                return LocalRedirect(result);
            }

            return View(applicationAccount);
        }

        //private bool Exists(int? id)
        //{
        //	return (_accountsCRUDService.Exists(id));
        //}



        [HttpGet]
        public async Task<IActionResult> JoinGroup(string key)
        {
            Account account = new()
            {
                Id = User.Identity.GetUserId(),
                GroupVerifyKey = key,
                AccountCreated = DateTime.Now
            };


            var details = await _unitOfWork.AccountsRepository.GetAllAsync(x => x.Id == User.Identity.GetUserId());
            var exists = details.Any(x => x.GroupVerifyKey == key);

            if (exists)
            {
                //await this.Create(account);
            }
            else
            {
                // Add Error message
                string status = status = "Failed!";
                this.AddAlertDanger($"{status} Something went wrong, Please try again.");
            }
            return RedirectToAction("Index", "Groups");

        }

    }

}


