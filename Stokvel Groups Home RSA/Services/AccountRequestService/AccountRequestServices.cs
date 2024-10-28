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

        var accountList = await _unitOfWork.AccountsRepository.GetAllAsync(
            x => x.Id == userId,
            includeProperties: "Group"
        );

        var applicationAccount = new ApplicationAccount
        {
            ApplicationUsers = new List<ApplicationUser>(),
            Account = accountList.ToList()
        };

        return applicationAccount;
    }


    private ApplicationAccount GetGroupMembers(List<int>? groupId)
    {
        List<Account> account = new();
        List<ApplicationUser> applicationUsers = new();

        foreach (var id in groupId)
        {
            var j = _unitOfWork.GroupsRepository.GetList().Where(x => x.GroupId == id);
            var group = j
                .Include(a => a.Accounts)
                .ThenInclude(au => au.ApplicationUser)
                .ToList();

            account.AddRange(group.Where(x => x.GroupId == id).SelectMany(x => x.Accounts?.ToList()).ToList());
            applicationUsers.AddRange(group.Where(x => x.GroupId == id).SelectMany(x => x.Accounts?.Select(x => x.ApplicationUser)).ToList());

            ApplicationAccount = new()
            {
                Account = account.ToList(),
                ApplicationUsers = applicationUsers.Distinct().ToList(),
            };

            ApplicationAccount.ApplicationUsers.DistinctBy(x => x.Id).ToList();


        }
        return ApplicationAccount;
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
        if (userId == null) return;

        var accountProfiles = await _unitOfWork.AccountProfileRepository.GetAllAsync();
        var groups = await _unitOfWork.GroupsRepository.GetAllAsync();
        var memberGroup = groups.FirstOrDefault(x => x.VerifyKey == account.GroupVerifyKey).GroupId;

        if (memberGroup == null)
        {
            throw new InvalidOperationException("Group not found for the provided verification key.");
        }

        account.Id = userId;
        account.AccountCreated = DateTime.Now;
        account.GroupId = memberGroup;

        if (!accountProfiles.Any(x => x.Id == userId))
        {
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
    }


    public bool CheckIfAccountExists(string userId, string verifyKey)
    {
        return _unitOfWork.AccountsRepository.AccountExists(userId, verifyKey);
    }























}
