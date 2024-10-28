using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass
{
    public class WalletDepositRequest : IDepositRequestServices
    {

        private readonly IUnitOfWork? _unitOfWork;
        private readonly IDepositSet? _depositSet;
        private readonly IWalletRequestServices? _walletRepositoryServices;

        public WalletDepositRequest(IUnitOfWork? unitOfWork, IDepositSet? depositSet, IWalletRequestServices? walletRepositoryServices)
        {
            _unitOfWork = unitOfWork;
            _depositSet = depositSet;
            _walletRepositoryServices = walletRepositoryServices;
        }

        // deposit to wallet
        public async Task DepositAsync(Deposit deposit, string description, int accountId, string? userId)
        {
            var memberWallet = await _walletRepositoryServices?.WalletGetAmountAsync(accountId);

            if (memberWallet == null)
            {
                throw new InvalidOperationException("Member wallet not found.");
            }

            var wallet = new Wallet
            {
                Amount = deposit.DepositAmount,
                Id = memberWallet.ApplicationUser?.Id
            };

            if (memberWallet.Wallet == null)
            {
                await _unitOfWork.WalletRepository.Add(wallet);
            }
            else
            {
                wallet.Amount += deposit.DepositAmount;
                _unitOfWork.WalletRepository.Update(wallet);
            }

            await _depositSet.DepositToAccountAsync(deposit, description, accountId, userId);
        }


    }
}
