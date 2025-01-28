using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Repositories
{
    public class PenaltyFeeRepository : Repository<PenaltyFee>,IPenaltyFeeRepository
	{

		private readonly ApplicationDbContext _context;
		public PenaltyFeeRepository(ApplicationDbContext context): base(context)
		{
			_context = context;
		}
		
		public void Update(PenaltyFee? penaltyFee)
		{
			_context.Update(penaltyFee);
		}
	}
}
