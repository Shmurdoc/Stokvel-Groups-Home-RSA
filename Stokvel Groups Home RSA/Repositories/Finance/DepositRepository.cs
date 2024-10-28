using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Models;
namespace Stokvel_Groups_Home_RSA.Repositories.Finance
{
    public class DepositRepository : Repository<Deposit>, IDepositRepository
    {
        private readonly ApplicationDbContext _context;

        public DepositRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Deposit deposit)
        {
            _context.Deposits.Update(deposit);
        }

    }
}
