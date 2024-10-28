using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.Finance
{
    public interface IInvoicesRepository : IRepository<Invoice>
    {
        void Update(Invoice? invoice);
    }
}