using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;

public interface IAccountsRepository : IRepository<Account>
{

    void Update(Account account);
    bool AccountExists(string userId, string VerifyKey);

}
