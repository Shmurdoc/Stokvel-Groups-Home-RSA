using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices
{
	public interface IWithdrawCRUDService
	{

		Task Insert(WithdrawDetails? invoiceDetails);

		Task Edit(WithdrawDetails? invoiceDetails);

		Task Delete(int? id);

		Task<List<WithdrawDetails>>? GetAll();
		bool InvoiceDetailsExists(int? id);

	}
}
