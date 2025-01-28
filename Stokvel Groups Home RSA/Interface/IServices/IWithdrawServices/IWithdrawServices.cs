using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;


public interface IWithdrawServices
{
    Task PenaltiesAsync(List<Account> overdueMembers, decimal groupTargetAmount);
    Task<decimal> CalculateTotalAmountAsync(Account member, decimal groupTargetAmount);
}
