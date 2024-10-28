using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea
{
    public interface IAccountUserPersonalRepository : IRepository<AccountUserPersonal>
    {

        void Update(AccountUserPersonal accountUserPersonal);
    }
}
