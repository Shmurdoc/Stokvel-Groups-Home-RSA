using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Stokvel_Groups_Home_RSA.common.Alert.TempData;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using Stokvel_Groups_Home_RSA.Services.GroupServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;

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

        public ApplicationUserPersonal? ApplicationUserPersonal { get; private set; }

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
            ViewBag.StokvelActive = false;
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
            ViewBag.StokvelActive = false;
            ViewBag.UserId = userId;

            return View(preDepositMembers);
        }



         [HttpGet]
         public async Task<IActionResult> StartStokvel(int accountId, int groupId)
         {
         	var userId = User.Identity.GetUserId();
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
            List<ApplicationUser> paidMembers = new();
            List<ApplicationUser> pending = new();
            List<ApplicationUser> notPaid = new();

            if (groupId > 0)
         	{
              

                var account = await _unitOfWork.AccountsRepository.GetAllAsync(includeProperties:"Group");
                var groupMembers = account.Where(x=>x.Group.GroupId == groupId).ToList();
                
                var applicationUserAccount = new ApplicationAccount()
                {
                    ApplicationUsers = new(),
                    Account = groupMembers
                };

                // member not paid
                ApplicationAccount = new()
                {
                    ApplicationUsers = new(),
                    Account = new()
                };

                foreach(var members in applicationUserAccount.Account)
                {
                    var applicationUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(members.Id);

                    if (members.PreDeposit != null || applicationUser.AccountProfiles.StatusRank != MemberStatuses.PendingPayment)
                    {
                        var userTarget = await _groupRequestService.CalculateAmountTarget(groupId);

                        // filter members not paid
                        if (members.PreDeposit.Amount != userTarget || applicationUser.AccountProfiles.StatusRank != MemberStatuses.PendingPayment)
                        {
                            notPaid.Add(applicationUser);
                        }
                        else
                        {
                            paidMembers.Add(applicationUser);
                        }
                    }
                    else
                    {
                            notPaid.Add(applicationUser);
                    }
                }
                if (notPaid.Count > 0)
                {
                    foreach (var member in notPaid)
                    {
                        this.AddAlertDanger($"{member} Has Not Yet Paid The Deposit, Please Remind Member To Pay");
                    }
                    var resultStart = "/Accounts" + "/AcceptedMembersDashboard?" + "AccountType=" + group.TypeAccount.ToString() + "&" + "GroupName=" + group.GroupName;
                    return LocalRedirect(resultStart);
                }

                if (paidMembers.Count + pending.Count == group.TotalGroupMembers)
                {
                    var paidList = paidMembers.OrderByDescending(x => x.AccountProfiles.Id).ToList();
                    var pendingList = pending.OrderByDescending(x => x.AccountProfiles.MembershipRank).ToList();

                    applicationUserAccount.ApplicationUsers.AddRange(paidList.Concat(pendingList).ToList());

                    foreach(var member in applicationUserAccount.Account)
                    {
                        member.AccountQueue += 1;
                        member.AccountQueueStart = new(year: DateAndTime.Now.Year, month: DateAndTime.Now.Month + member.AccountQueue , 1);
                        member.AccountQueueStart = new(year: DateAndTime.Now.Year, month: DateAndTime.Now.Month + member.AccountQueue, 25);
                         _unitOfWork.AccountsRepository.Update(member);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }
            else
            {
                string status = status = "Failed!";
                this.AddAlertDanger($"{status} Something went wrong, Pleace try again.");
            }
            var resultDone = "/Accounts" + "/AcceptedMembersDashboard?" + "AccountType=" + group.TypeAccount.ToString() + "&" + "GroupName=" + group.GroupName;
            return LocalRedirect(resultDone);
        }

        [HttpGet]
        public async Task<IActionResult> Index2(AccountType accountType)
        {
            var userId = User.Identity.GetUserId();

            //in group by type
            ApplicationAccount = await _accountRequestService.InAccountListAsync(userId, accountType);
            var groupMembers = ApplicationAccount.Account?.GroupBy(x => x.Group?.GroupName).Select(x => x.FirstOrDefault()).ToList();
            ApplicationAccount.Account = groupMembers;
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


