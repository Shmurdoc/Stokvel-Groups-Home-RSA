using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Repositories.UserArea
{
    public class AccountProfileRepository : Repository<AccountProfile>, IAccountProfileRepository
    {
        private ApplicationDbContext _context;

        public AccountProfileRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public void Update(AccountProfile? accountProfile)
        {
            _context.Update(accountProfile);
        }
    }
}
