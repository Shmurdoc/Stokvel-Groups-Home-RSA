using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Repositories.Finance
{
    public class PreDepositRepository : Repository<PreDeposit>, IPreDepositRepository
    {
        private readonly ApplicationDbContext _context;

        public PreDepositRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(PreDeposit preDeposit)
        {
            _context?.PreDeposits?.Update(preDeposit);
        }



    }
}
