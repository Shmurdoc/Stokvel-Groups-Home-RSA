using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass
{
    public class WalletDepositRequest : IDepositRequestServices
    {

        private readonly IUnitOfWork? _unitOfWork;
        private readonly IDepositSet? _depositSet;
        private readonly IWalletRequestServices? _walletRequestServices;
        private readonly IGroupRequestServices _groupRequestServices;

        public WalletDepositRequest(IUnitOfWork? unitOfWork, IDepositSet? depositSet, IWalletRequestServices? walletRepositoryServices, IGroupRequestServices groupRequestServices)
        {
            _unitOfWork = unitOfWork;
            _depositSet = depositSet;
            _walletRequestServices = walletRepositoryServices;
            _groupRequestServices = groupRequestServices;
        }

        // deposit to wallet
        public async Task DepositAsync(Deposit deposit, string description, int accountId, string? userId, string? dropdownValue)
        {
            var memberWallet = await _walletRequestServices?.WalletGetAmountAsync(accountId);

            if (memberWallet == null)
            {
                throw new InvalidOperationException("Member wallet not found.");
            }
           
            var wallet = new Wallet
            {
                Id = memberWallet.ApplicationUser?.Id
            };

            if (memberWallet.Wallet == null)
            {
                

                await _unitOfWork.WalletRepository.Add(wallet);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                 memberWallet.Wallet.Amount += (deposit.DepositAmount);
                _unitOfWork.WalletRepository.Update(memberWallet.Wallet);
                await _unitOfWork.SaveChangesAsync();
            }

            await _depositSet.DepositToAccountAsync(deposit, description, accountId, userId, dropdownValue);
        }


    }
}
