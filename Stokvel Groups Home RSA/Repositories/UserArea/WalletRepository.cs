using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Models;


namespace Stokvel_Groups_Home_RSA.Repositories.UserArea
{

    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {

        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Wallet? wallet)
        {
            _context.Update(wallet);
        }
    }
}
