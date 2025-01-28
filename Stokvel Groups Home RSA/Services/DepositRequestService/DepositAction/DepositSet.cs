using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet
{
    public class DepositSet : IDepositSet
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountProfileRequestServices _accountProfileRequestServices;
        public readonly IPreDepositRequestServices _preDepositRequestServices;
        public readonly IGroupRequestServices _groupRequestServices;
        public AccountProfile AccountProfile { get; private set; }
        
        public DepositSet(IUnitOfWork unitOfWork, IAccountProfileRequestServices accountProfileRequestServices, IPreDepositRequestServices preDepositRequestServices, IGroupRequestServices groupRequestServices )
        {
            _unitOfWork = unitOfWork;
            _accountProfileRequestServices = accountProfileRequestServices;
            _preDepositRequestServices = preDepositRequestServices;
            _groupRequestServices = groupRequestServices;
        }

        public async Task DepositToAccountAsync(Deposit? deposit, string? description, int accountId, string? userId, string? dropdownValue)
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
            await UpdateAccountProfile(deposit, description, accountId, userId, dropdownValue);
        }

        public async Task UpdateAccountProfile(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue)
        {
            var memberStatusRank = new AccountProfile();
            var memberInfoProfile = await _accountProfileRequestServices.AccountProfileInfoAsync(userId);
            decimal preDepoBalance = 0;
            if (memberInfoProfile == null)
            {
                throw new Exception("Member profile not found.");
            }

            var accountProfile = memberInfoProfile.AccountProfile;
            decimal depositAmount = deposit.DepositAmount;
            var pointCount = depositAmount / 100;

            accountProfile.AccountProfileId = memberInfoProfile.AccountProfile.AccountProfileId;
            accountProfile.Id = userId;
            accountProfile.MembershipRank += Convert.ToInt32(pointCount);
            accountProfile.TotalAmountDeposited += depositAmount;

            
                var predepoInfo = await _preDepositRequestServices.CheckPreDepositStatusDepositAsync(accountId);
            if(description == "PreDeposit")
            {
                if(predepoInfo != null)
                {
                    preDepoBalance = deposit.DepositAmount;
                }
                 memberStatusRank = await UpdatePendingStatusRank(accountProfile, preDepoBalance, accountId, accountProfile.MembershipRank, dropdownValue);
            }
            else
            {
                 memberStatusRank = await UpdatePendingStatusRank(accountProfile, preDepoBalance, accountId, accountProfile.MembershipRank, dropdownValue);
            }
                
                accountProfile.StatusRank = memberStatusRank.StatusRank;
            

            if (memberInfoProfile != null)
            {
                _unitOfWork.AccountProfileRepository.Update(accountProfile);
            }
            else
            {
                await _unitOfWork.AccountProfileRepository.Add(accountProfile);
            }

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<AccountProfile> UpdatePendingStatusRank(AccountProfile accountProfile, decimal preDepositAmount, int accountId, decimal totalPoints, string? dropdownValue)
        {
            // Fetch pre-deposit list and account details
            var preDepositList = await _unitOfWork.PreDepositRepository.GetAllAsync(a=>a.AccountId == accountId);
            var memberPreDeposit = preDepositList.Select(x => x).FirstOrDefault();
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(accountId);

            // Calculate group target pre-deposit amount
            var groupTargetPreDeposit = await _groupRequestServices.CalculateAmountTarget(account.GroupId);

            if (preDepositList.Count() != 0)
            {
                // Initialize pre-deposit amount from database
                decimal preDepositAmountInDb = memberPreDeposit.Amount;

                // Update account profile status rank based on pre-deposit amount and group target
                if (preDepositAmount + preDepositAmountInDb < groupTargetPreDeposit && dropdownValue == "PendingPayment")
                {
                    accountProfile.StatusRank = MemberStatuses.PendingPayment;
                }
                else
                {
                    accountProfile.StatusRank = await UpdateStatusRank(totalPoints);
                }
            }
            else
            {
                // Handle case where pre-deposit list is null
                if (preDepositAmount < groupTargetPreDeposit && dropdownValue == "PendingPayment")
                {
                    accountProfile.StatusRank = MemberStatuses.PendingPayment;
                }
                else
                {
                    accountProfile.StatusRank = await UpdateStatusRank(totalPoints);
                }
            }

            return accountProfile;
        }




        public async Task<MemberStatuses> UpdateStatusRank(decimal totalPoints)
        {
            int intValue = (int)totalPoints;
            MemberStatuses memberStatus;

            if (intValue > 0 && intValue < 2500)
            {
                memberStatus = MemberStatuses.Bronze;
            }
            else if (intValue <= 5000)
            {
                memberStatus = MemberStatuses.Silver;
            }
            else if (intValue <= 75000)
            {
                memberStatus = MemberStatuses.Gold;
            }
            else if (intValue > 10000)
            {
                memberStatus = MemberStatuses.Platinum;
            }
            else
            {
                throw new Exception("Invalid membership rank.");
            }

            return memberStatus;
        }


    }
}
