using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;
using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;
using System.Text.RegularExpressions;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositService;

public class DepositService : IDepositService
{
    public IDepositRequestServices? DepositRequestServices { get; private set; }
    private readonly IUnitOfWork? _unitOfWork;
    private readonly IDepositSet? _depositSet;
    private readonly IPreDepositRequestServices _preDepositRequestServices;
    private readonly IWalletRequestServices _walletRequestServices;
    private readonly IAccountProfileRequestServices _accountProfileRequestServices;
    private readonly IGroupRequestServices _groupRequestServices;


    public DepositService(IDepositSet depositSet, IUnitOfWork unitOfWork, IPreDepositRequestServices preDepositRequestServices, IWalletRequestServices walletRequestServices, IAccountProfileRequestServices accountProfileRequestServices, IGroupRequestServices groupRequestServices)
    {
        _unitOfWork = unitOfWork;
        _depositSet = depositSet;
        _preDepositRequestServices = preDepositRequestServices;
        _walletRequestServices = walletRequestServices;
        _accountProfileRequestServices = accountProfileRequestServices;
        _groupRequestServices = groupRequestServices;
    }

    public async Task DepositRequestAsync(Deposit deposit, string? description, int accountId,string? userId, string? dropdownValue)
    {
        DepositRequestServices = new DepositRequest(_depositSet);
        await DepositRequestServices.DepositAsync(deposit, description, accountId, userId, dropdownValue);
    }

    public async Task PreDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue)
    {
        DepositRequestServices = new PreDepositDepositRequest(_preDepositRequestServices, _depositSet, _unitOfWork);
        await DepositRequestServices.DepositAsync(deposit, description, accountId, userId, dropdownValue);
    }

    public async Task WalletDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue)
    {
        DepositRequestServices = new WalletDepositRequest(_unitOfWork, _depositSet, _walletRequestServices,_groupRequestServices);
        await DepositRequestServices.DepositAsync(deposit, description, accountId, userId, dropdownValue);
    }



    public async Task<decimal> GetPreDepositAmount(int accountId)
    {
        var preDepo = await _unitOfWork.AccountsRepository.GetAllAsync(x => x.AccountId == accountId, includeProperties: "PreDeposit");
        return preDepo?.Select(x => x.PreDeposit?.Amount).FirstOrDefault() ?? 0.00m;
    }

    public async Task<int> GetMemberDescription(int groupId)
    {
        var groupAccount = await _unitOfWork.AccountsRepository.GetAllAsync(g => g.GroupId == groupId);
        return groupAccount.Where(x => x.AccountQueueStart.Month == DateTime.Now.Month)
                           .Select(x => x.AccountQueue).FirstOrDefault();
    }

    
    public async Task<decimal> CalculateExcess(Deposit? deposit, decimal memberDepoTarget, int memberDescription, int accountId)
    {
        decimal memberAmountTotal = 0m;

        // If accountId is zero, return zero as no valid account exists
        if (accountId == 0)
        {
            return 0;
        }

        // Fetch the deposit list from the repository, filtered by the current month and the specified reference
        var depositMemberList = _unitOfWork.DepositRepository.GetList()
            .Where(x => x.Invoices.Any(inv => inv.Account.AccountId == accountId));

        var depositList = depositMemberList
            .Where(x => x.DepositDate.Month == DateTime.Now.Month &&
                        x.DepositReference == "Deposit" + memberDescription)
            .ToList();

        // Get preDepositAmount for the specified account
        var preDepositAmount = await this.GetPreDepositAmount(accountId);

        // If the preDepositAmount matches the memberDepoTarget and memberDescription is not zero,
        // calculate and return the excess amount
        if (preDepositAmount == memberDepoTarget && memberDescription == 0)
        {
            memberAmountTotal = deposit.DepositAmount;
            return memberAmountTotal;
        }
        else { 
            return 0m;
        }

        // If no deposits found for the given criteria, return 0 (no excess)
        if (depositList == null || !depositList.Any())
        {
            return 0m;
        }
        else
        {
            // Calculate the total deposit amount for the current month and the specific deposit reference
            var AmountTotal = depositList.Sum(x => x.DepositAmount);


            // If the total amount matches the memberDepoTarget, calculate and return the excess amount
            if (AmountTotal+deposit.DepositAmount >= memberAmountTotal)
            {
                var newTotal = AmountTotal + deposit.DepositAmount;
                var excess = newTotal - memberDepoTarget;
                return excess;
            }

        }

        return 0;
    }




    public async Task<bool> IsTargetMet(int groupId, decimal depositAmount)
    {
        var memberTarget = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
        return depositAmount >= memberTarget.AccountTarget;
    }

    public async Task ProcessDeposit(Deposit deposit, string description, int accountId, string userId, decimal excess, int memberDescription, decimal memberDepoTarget, string dropdownValue)
    {
        var memberPreDepoAccountList = await _unitOfWork.PreDepositRepository.GetAllAsync(x => x.AccountId == accountId);
        var profileStatus = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
        var memberExpectedDepoAmount = await _preDepositRequestServices.PreDepoMembersAsync(accountId);
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
            await DepositToWallet(deposit, description, accountId, userId, excess, memberDepoTarget, dropdownValue);
        }
        else
        {
            if (profileStatus.AccountProfile.StatusRank != MemberStatuses.PendingPayment)
            {
                if (memberExpectedDepoAmount.PreDeposit == null || memberExpectedDepoAmount?.PreDeposit?.Amount + deposit.DepositAmount <= memberPreDepoTarget)
                {
                    description = "PreDeposit";
                    await this.PreDepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
                }
                else
                {
                    await this.DepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
                }
            }
            else
            {
                dropdownValue = "PendingPayment";
                await this.PreDepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
            }
        }
    }

    public async Task DepositToWallet(
Deposit deposit,
string? description,
int accountId,
string? userId,
decimal excess,
decimal memberDepoTarget,
string dropdownValue)
    {
        var account = await GetAccountWithGroupAsync(accountId);
        var groupTotal = CalculateGroupTotal(account.ToList());
        description = "Wallet"; // Always assign this to "Wallet"
        var memberStatus = await GetMemberStatusAsync(userId);
        var pendingPaymentTotal = CalculatePendingPaymentTotal(memberDepoTarget, groupTotal);

        if (memberStatus?.AccountProfile?.StatusRank == MemberStatuses.PendingPayment)
        {
            await HandlePendingPayment(deposit, description, accountId, userId, excess, pendingPaymentTotal, dropdownValue);
        }
        else
        {
            await HandleNonPendingPayment(deposit, description, accountId, userId, excess, dropdownValue);
        }
    }

    // Helper method to get account details
    public async Task<IEnumerable<Account>> GetAccountWithGroupAsync(int accountId)
    {
        return await _unitOfWork.AccountsRepository.GetAllAsync(x => x.AccountId == accountId, includeProperties: "Group");
    }

    // Helper method to calculate the group total members (excluding the current member)
    public int CalculateGroupTotal(List<Account> account)
    {
        return account.Select(x => x.Group.TotalGroupMembers).FirstOrDefault() - 1;
    }

    // Helper method to get the member's status
    public async Task<ApplicationUserAccountProfile> GetMemberStatusAsync(string? userId)
    {
        return await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
    }

    // Helper method to calculate pending payment per member
    public decimal CalculatePendingPaymentTotal(decimal memberDepoTarget, int groupTotal)
    {
        return memberDepoTarget / groupTotal;
    }

    // Helper method to handle pending payment logic
    public async Task HandlePendingPayment(
        Deposit deposit,
        string description,
        int accountId,
        string userId,
        decimal excess,
        decimal pendingPaymentTotal,
        string dropdownValue)
    {
        if (excess <= pendingPaymentTotal)
        {
            await ProcessDepositRequest(deposit, description, accountId, userId, excess, dropdownValue);
        }
        else if (excess > pendingPaymentTotal && excess != 0)
        {
            var newExcess = excess - pendingPaymentTotal;
            await ProcessExcessDeposit(deposit, description, accountId, userId, newExcess, dropdownValue);
        }
    }

    // Helper method to process the wallet deposit request when excess and has no pending payments
    public async Task ProcessWalletDepositRequest(
        Deposit deposit,
        string description,
        int accountId,
        string userId,
        decimal excess,
        string dropdownValue)
    {
        var excessDeposit = new Deposit { DepositAmount = excess };
        await this.WalletDepositRequestAsync(excessDeposit, description, accountId, userId, dropdownValue);
        deposit.DepositAmount -= excess;
        await this.DepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
    }

    // Helper method to process the deposit request when excess is less than or equal to pending payment
    public async Task ProcessDepositRequest(
        Deposit deposit,
        string description,
        int accountId,
        string userId,
        decimal excess,
        string dropdownValue)
    {
        var excessDeposit = new Deposit { DepositAmount = excess };
        await this.PreDepositRequestAsync(excessDeposit, description, accountId, userId, dropdownValue);
        deposit.DepositAmount -= excess;
        await this.DepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
    }

    // Helper method to process excess deposit when excess is greater than pending payment
    public async Task ProcessExcessDeposit(
        Deposit deposit,
        string description,
        int accountId,
        string userId,
        decimal newExcess,
        string dropdownValue)
    {
        var excessDeposit = new Deposit { DepositAmount = newExcess };
        var walletMoney = new Wallet { Id = userId, Amount = newExcess };

        // Send the excess amount to wallet
        await this.PreDepositRequestAsync(excessDeposit, description, accountId, userId, dropdownValue);
        await this.DepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
        await Wallet(walletMoney); // Sending excess to wallet
        deposit.DepositAmount -= newExcess;
        await this.PreDepositRequestAsync(deposit, description, accountId, userId, dropdownValue);
    }

    // Helper method to handle non-pending payment logic
    public async Task HandleNonPendingPayment(
        Deposit deposit,
        string description,
        int accountId,
        string userId,
        decimal excess,
        string dropdownValue)
    {
        
        if (excess > 0)
        {
            // new deposit amount is the excess amount
            var depo = new Deposit { DepositAmount = excess };
            await ProcessWalletDepositRequest(depo, description, accountId, userId, excess, dropdownValue);
        }
    }
      


    public async Task Wallet(Wallet walletMoney)
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

}
