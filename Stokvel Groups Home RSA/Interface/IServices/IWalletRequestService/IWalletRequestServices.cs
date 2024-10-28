using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService
{
    public interface IWalletRequestServices
    {

        Task<ApplicationUserWallet> WalletGetAmountAsync(int accountId);

    }
}
