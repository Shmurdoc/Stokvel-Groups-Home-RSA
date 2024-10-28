using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.Finance
{
    public interface IBankDetailsRepository
    {

        Task<List<BankDetails>>? GetAll();

        Task<BankDetails>? GetById(int? id);
        Task Insert(BankDetails? bankDetails);

        Task Edit(BankDetails? bankDetails);

        Task Delete(int? id);

        Task SaveAsync();

        bool BankDetailsExists(int? id);

    }
}
