using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IHomeService
{
    public interface IHomeRequestService
    {


        Task<ApplicationAccount> GetApplicationAccountDetailsAsync(int groupId);
        Task<IEnumerable<Deposit>> GetDepositDetailsAsync(int groupId);

    }
}
