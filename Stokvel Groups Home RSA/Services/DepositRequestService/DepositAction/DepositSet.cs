using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet
{
    public class DepositSet : IDepositSet
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountProfileRequestServices _accountProfileRequestServices;
        public AccountProfile AccountProfile { get; private set; }
        
        public DepositSet(IUnitOfWork unitOfWork, IAccountProfileRequestServices accountProfileRequestServices)
        {
            _unitOfWork = unitOfWork;
            _accountProfileRequestServices = accountProfileRequestServices;
        }

        public async Task DepositToAccountAsync(Deposit? deposit, string? description, int accountId, string? userId)
        {
            if (deposit == null)
            {
                throw new ArgumentNullException(nameof(deposit));
            }

            var newDeposit = new Deposit
            {
                DepositId = deposit.DepositId,
                DepositDate = DateTime.Now,
                DepositReference = description,
                DepositAmount = deposit.DepositAmount
            };

            await _unitOfWork.DepositRepository.Add(newDeposit);
            await _unitOfWork.SaveChangesAsync();

            var depositToInvoice = new Invoice
            {
                DepositId = newDeposit.DepositId,
                AccountId = accountId,
                InvoiceDate = DateTime.Now,
                TotalAmount = deposit.DepositAmount,
                Description = description
            };

            await _unitOfWork.InvoicesRepository.Add(depositToInvoice);
            await _unitOfWork.SaveChangesAsync();

            await UpdateAccountProfile(deposit, description, accountId, userId);
        }

        public async Task UpdateAccountProfile(Deposit deposit, string? description, int accountId, string? userId)
        {
            var memberInfoProfile = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);

            AccountProfile = memberInfoProfile.AccountProfile;

            //var rank = statusRank;
            //var pointCount = deposit.DepositAmount / 100;
            //decimal dtot = deposit.DepositAmount;

            //accountProfiles.AccountProfileId = accountProfileId;
            //accountProfiles.Id = userId;
            //accountProfiles.StatusRank = rank;
            //accountProfiles.MembershipRank = membershipRank + Convert.ToInt32(pointCount);
            //accountProfiles.TotalAmoutDeposited = totalAmountDeposited + dtot;
        }


    }
}
