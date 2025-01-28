using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Services.AccountRequestService;

public class AccountRequestServices : IAccountRequestServices
{


    private IUnitOfWork? _unitOfWork;

    public ApplicationAccount? ApplicationAccount { get; private set; }

    public AccountRequestServices(IUnitOfWork? unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public static ApplicationUser? ApplicationUser { get; private set; }
    public static Account? Account { get; private set; }

    public async Task<ApplicationAccount?> GroupAccountsAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        // Fetch all groups with their associated accounts
        var groups = await _unitOfWork.GroupsRepository.GetAllAsync(
            includeProperties: "Accounts"
        );

        // Filter the groups where the specified user is a member
        var groupIdsWithUser = groups
            .Where(group => group.Accounts.Any(account => account.Id == userId))
            .Select(group => group.GroupId)
            .ToList();

        // Initialize a list to hold all members from the matching groups
        var memberAccounts = new List<Account>();

        // Retrieve and accumulate members for each group that contains the user
        foreach (var groupId in groupIdsWithUser)
        {
            var groupMembers = await _unitOfWork.AccountsRepository.GetAllAsync(
                account => account.GroupId == groupId
            );
            memberAccounts.AddRange(groupMembers);
        }

        // Create the application account object with associated users and account members
        var applicationAccount = new ApplicationAccount
        {
            ApplicationUsers = new List<ApplicationUser>(), // Populate as needed
            Account = memberAccounts
        };

        return applicationAccount;
    }


    // must move this to group request 
    private ApplicationAccount GetGroupMembers(List<int>? groupIds)
    {
        if (groupIds == null || !groupIds.Any())
        {
            return new ApplicationAccount
            {
                Account = new List<Account>(),
                ApplicationUsers = new List<ApplicationUser>()
            };
        }

        var accounts = new List<Account>();
        var applicationUsers = new List<ApplicationUser>();

        // Fetch group data and associated accounts and users
        foreach (var groupId in groupIds)
        {
            var group = _unitOfWork.GroupsRepository
                .GetList()
                .Where(g => g.GroupId == groupId)
                .Include(g => g.Accounts)
                    .ThenInclude(a => a.ApplicationUser)
                .ToList();

            // Collect accounts and associated users
            accounts.AddRange(group.SelectMany(g => g.Accounts ?? new List<Account>()));
            applicationUsers.AddRange(group.SelectMany(g => g.Accounts?.Select(a => a.ApplicationUser) ?? new List<ApplicationUser>()));
        }

        // Remove duplicate application users based on their Id
        var distinctApplicationUsers = applicationUsers.DistinctBy(u => u.Id).ToList();

        // Create and return the ApplicationAccount object
        return new ApplicationAccount
        {
            Account = accounts.Distinct().ToList(),
            ApplicationUsers = distinctApplicationUsers
        };
    }



    public async Task<ApplicationAccount?> InAccountListAsync(string? userId, AccountType? accountType)
    {
        if (string.IsNullOrEmpty(userId) || accountType == null)
        {
            return null;
        }

        var memberInGroup = await GroupAccountsAsync(userId);
        if (memberInGroup == null)
        {
            return null;
        }

        var members = memberInGroup.Account
            .Where(item => item.Group.TypeAccount == accountType)
            .ToList();

        var groupIds = members
            .Select(x => x.Group.GroupId)
            .ToList();

        var groupsByType = GetGroupMembers(groupIds);

        return groupsByType;
    }


    public async Task<ApplicationAccount>? MembersOfAccountList(string? userId)
    {

        List<AccountType> accountTypes = new();

        var memberInGroup = await GroupAccountsAsync(userId);

        memberInGroup.Account = memberInGroup?.Account?.GroupBy(static item => item.Group.TypeAccount).Select(group => group.First()).ToList();
        var groupsByType = GetGroupMembers(memberInGroup?.Account?.Select(x => x.GroupId).ToList());

        return groupsByType;
    }

    //public AccountInvoice AccountInvoice(int groupId)
    //{

    //    var j = _unitOfWork.DepositRepository.GetList();


    //    var deposits = j
    //        .Include(i => i.Invoices.Where(x => x.Account.GroupId == groupId))
    //        .ThenInclude(a => a.Account)
    //        .ThenInclude(au => au.ApplicationUser)
    //        .ToList();

    //    return
    //}

    // check if member paid preDeposit
    public async Task<List<PreDeposit?>> GroupFeePreDepoAsync(List<Account?> accounts)
    {
        int selfRemoved = 1;
        var preDeposits = new List<PreDeposit?>();
        var preDepositMembers = await _unitOfWork.PreDepositRepository.GetAllAsync();
        var accountProfiles = await _unitOfWork.AccountProfileRepository.GetAllAsync();

        foreach (var account in accounts)
        {
            if (account == null) continue;

            var paidMember = preDepositMembers.FirstOrDefault(x => x.AccountId == account.AccountId);
            var status = accountProfiles.FirstOrDefault(x => x.Id == account.Id);

            if (account.Group == null || account.Group.TotalGroupMembers == 0) continue;

            var amount = account.Group.AccountTarget / account.Group.TotalGroupMembers - selfRemoved;

            if (paidMember != null && (amount == paidMember.Amount || status?.StatusRank == MemberStatuses.PendingPayment))
            {
                preDeposits.Add(paidMember);
            }
        }

        return preDeposits;
    }

    public async Task AddAccountToGroupAsync(Account account, string? userId)
    {
        if (string.IsNullOrEmpty(userId)) return;

        var accountProfiles = await _unitOfWork.AccountProfileRepository.GetAllAsync();
        var groups = await _unitOfWork.GroupsRepository.GetAllAsync();
        var memberGroup = groups.FirstOrDefault(x => x.VerifyKey == account.GroupVerifyKey)?.GroupId;

        if (memberGroup == null)
        {
            throw new InvalidOperationException("Group not found for the provided verification key.");
        }

        account.Id = userId;
        account.AccountCreated = DateTime.Now;
        account.GroupId = memberGroup.Value;

       
            var accountProfile = new AccountProfile
            {
                Id = userId,
                GroupsJoined = 1,
                GroupsLeft = 0,
                EmergencyCancel = 0,
                StatusRank = MemberStatuses.Bronze,
                MembershipRank = 0,
                TotalAmountDeposited = 0,
                TotalPenaltyFee = 0,
                GroupWarnings = 0,
            };

            await _unitOfWork.AccountProfileRepository.Add(accountProfile);
        

        await _unitOfWork.AccountsRepository.Add(account);
        await _unitOfWork.SaveChangesAsync();
    }



    public bool CheckIfAccountExists(string userId, string verifyKey)
    {
        return _unitOfWork.AccountsRepository.AccountExists(userId, verifyKey);
    }























}
