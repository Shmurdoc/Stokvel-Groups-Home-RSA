using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices
{
    public interface IAccountProfileRequestServices
    {

        Task<ApplicationUserAccountProfile> AccountProfileInfoAsync(string? id);
    }
}
