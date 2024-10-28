using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Repositories.UserArea
{
    public class AccountUserPersonalRepository : Repository<AccountUserPersonal>, IAccountUserPersonalRepository
    {
        private ApplicationDbContext _context;

        public AccountUserPersonalRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public void Update(AccountUserPersonal accountUserPersonal)
        {
            _context.Update<AccountUserPersonal>(accountUserPersonal);
        }
    }
}
