using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService
{
    public interface IPreDepositRequestServices
    {
        Task<DepositToAccount?> PreDepoMembersAsync(int accountId);
        Task<AccountPreDeposit?> CheckPreDepositStatusDepositAsync(int accountId);
        Task UpdatePreDeposit(Deposit deposit, int accountId);
    }
}
