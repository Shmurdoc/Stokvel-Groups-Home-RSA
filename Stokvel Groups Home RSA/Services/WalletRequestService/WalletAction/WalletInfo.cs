using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using System.Threading.Tasks;

namespace Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet
{
    public class WalletInfo : IWalletRequestServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WalletInfo> _logger;

        public WalletInfo(IUnitOfWork unitOfWork, ILogger<WalletInfo> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        public async Task<ApplicationUserWallet> WalletGetAmountAsync(int accountId)
        {
            // Fetch account and wallet asynchronously
            var memberAccount = await _unitOfWork.AccountsRepository
                                                 .GetByIdAsync(accountId);

            // If account is not found, return null or throw exception as per your design choice
            if (memberAccount == null)
            {
                // Log the error (you could use your logging framework here)
                _logger.LogWarning("Account with ID {AccountId} not found.", accountId);
                return null; // or throw custom exception: throw new AccountNotFoundException(accountId);
            }

            // Fetch the wallet based on the ApplicationUser's Id (assuming the Wallet's Id is related to ApplicationUser)
            var wallet = await _unitOfWork.WalletRepository
                                          .GetAllAsync(w => w.Id == memberAccount.Id);

            // If wallet is not found, return null
            if (wallet == null)
            {
                _logger.LogWarning("Wallet for ApplicationUser {UserId} not found.", memberAccount.ApplicationUser.Id);
                return null;
            }

            // Create and return the ApplicationUserWallet object
            var applicationUserWallet = new ApplicationUserWallet
            {
                ApplicationUser = memberAccount.ApplicationUser,
                Account = memberAccount,
                Wallet = wallet.SingleOrDefault()
            };

            return applicationUserWallet;
        }

    }
}
