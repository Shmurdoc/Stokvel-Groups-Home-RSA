using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet
{
    public class WalletInfo : IWalletRequestServices
    {


        private readonly IUnitOfWork? _unitOfWork;
        private static ApplicationUserWallet? ApplicationUserWallet { get; set; }
        public WalletInfo(IUnitOfWork? unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ApplicationUserWallet> WalletGetAmountAsync(int accountId)
        {
            var accountList = await _unitOfWork.AccountsRepository.GetList()
                .Include(x => x.ApplicationUser)
                .ThenInclude(x => x.Wallets)
                .ToListAsync();

            var memberAccount = accountList.FirstOrDefault(x => x.AccountId == accountId);

            if (memberAccount == null)
            {
                return null;
            }

            var applicationUserWallet = new ApplicationUserWallet
            {
                ApplicationUser = memberAccount.ApplicationUser,
                Account = memberAccount,
                Wallet = memberAccount.ApplicationUser?.Wallets?.SingleOrDefault()
            };

            return applicationUserWallet;
        }

    }
}
