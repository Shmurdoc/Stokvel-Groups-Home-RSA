using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;

public interface IAccountProfileRepository : IRepository<AccountProfile>
{

    void Update(AccountProfile? accountProfile);

}
