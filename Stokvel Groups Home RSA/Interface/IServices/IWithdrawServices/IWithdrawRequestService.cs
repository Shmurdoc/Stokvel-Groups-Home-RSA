using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices
{
    public interface IWithdrawRequestService
	{
        Task<List<Deposit>> GetDeposits(int groupId);
        Task<List<Deposit>> GetDepositsByGroupId(int groupId);
        Task<List<ApplicationUser>> GetApplicationUsers(List<Deposit> deposits, int groupId);
        Task<List<Account>> GetAccounts(List<Deposit> deposits);
        Task<List<Invoice>> GetInvoicesForGroup(List<Deposit> deposits, List<Account> currentMonthAccounts);
        Task<List<Account>> GetCurrentMonthAccounts(List<Deposit> deposits);
        Task<decimal> GetOutstandingAmount(List<Invoice> invoices, int groupId);
        Task<List<Invoice>> GroupInvoicesByDay(List<Invoice> invoices);
        Task<decimal> GetSumOfInvoices(List<Account> allAccounts);
        Task<decimal> GetTargetAmount(int groupId);
        Task ApplyPenaltiesAsync();
    }
}
