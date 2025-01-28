using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Repositories;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.Finance
{
	public interface IWithdrawRepository : IRepository<WithdrawDetails>
	{

		void Update(WithdrawDetails? withdrawDetails);

	}
}
