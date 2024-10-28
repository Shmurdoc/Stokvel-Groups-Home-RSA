

using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;

public interface IAccountRequestServices
{

    Task<ApplicationAccount>? MembersOfAccountList(string? userId);
    Task<ApplicationAccount?> InAccountListAsync(string? userId, AccountType? accountType);
    bool CheckIfAccountExists(string userId, string verifyKey);
    Task<List<PreDeposit?>> GroupFeePreDepoAsync(List<Account?> accounts);
    Task AddAccountToGroupAsync(Account account, string? userId);
}


