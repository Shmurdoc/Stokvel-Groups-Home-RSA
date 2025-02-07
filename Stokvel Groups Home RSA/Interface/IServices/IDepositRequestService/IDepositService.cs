using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService
{
    public interface IDepositService
    {
        Task DepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue);
        Task PreDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId,  string? dropdownValue);
        Task WalletDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue);
        Task<decimal> GetPreDepositAmount(int accountId);
        Task<int> GetMemberDescription(int groupId);
        Task<decimal> CalculateExcess(Deposit? deposit, decimal memberDepoTarget, int memberDescription, int accountId);
        Task<bool> IsTargetMet(int groupId, decimal depositAmount);
        Task ProcessDeposit(Deposit deposit, string description, int accountId, string userId, decimal excess, int memberDescription, decimal memberDepoTarget, string dropdownValue);
        Task DepositToWallet(Deposit deposit, string? description, int accountId, string? userId, decimal excess, decimal memberDepoTarget, string dropdownValue);
        Task<IEnumerable<Account>> GetAccountWithGroupAsync(int accountId);
        int CalculateGroupTotal(List<Account> account);
        Task<ApplicationUserAccountProfile> GetMemberStatusAsync(string? userId);
        decimal CalculatePendingPaymentTotal(decimal memberDepoTarget, int groupTotal);
        Task HandlePendingPayment(Deposit deposit, string description, int accountId, string userId, decimal excess, decimal pendingPaymentTotal, string dropdownValue);
        Task ProcessDepositRequest(Deposit deposit, string description, int accountId, string userId, decimal excess, string dropdownValue);
        Task ProcessExcessDeposit(Deposit deposit, string description, int accountId, string userId, decimal newExcess, string dropdownValue);
        Task HandleNonPendingPayment(Deposit deposit, string description, int accountId, string userId, decimal excess, string dropdownValue);
        Task Wallet(Wallet walletMoney);
    }
}
