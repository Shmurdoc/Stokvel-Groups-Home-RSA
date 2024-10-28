using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.Finance
{
    public interface IWithdrawRepository
    {
        Task<List<WithdrawDetails>>? GetAll();

        Task<WithdrawDetails>? Details(int? id);
        Task Insert(WithdrawDetails? invoiceDetails);

        Task Edit(WithdrawDetails? invoiceDetails);

        Task Delete(int? id);

        bool InvoiceDetailsExists(int? id);
    }
}
