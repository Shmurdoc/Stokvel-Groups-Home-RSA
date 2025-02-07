using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using Stokvel_Groups_Home_RSA.Services.GroupServices;

namespace Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo
{
    public class PreDepositInfo : IPreDepositRequestServices
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupRequestServices _groupRequestServices;
        public DepositToAccount? DepositToAccount { get; set; }
        public AccountPreDeposit? AccountPreDeposit { get; set; }
        
        public PreDepositInfo(IUnitOfWork unitOfWork, IGroupRequestServices groupRequestServices)
        {
            _unitOfWork = unitOfWork;
            _groupRequestServices = groupRequestServices;
        }



        public async Task<AccountPreDeposit?> CheckPreDepositStatusDepositAsync(int accountId)
        {
            var memberYetToDeposit = await _unitOfWork.PreDepositRepository.GetList()
                .Include(x => x.Account)
                .ThenInclude(a => a.ApplicationUser)
                .ThenInclude(u => u.AccountProfiles)
                .SingleOrDefaultAsync(x => x.AccountId == accountId);

            if (memberYetToDeposit == null)
            {
                return null;
            }

            var accountPreDeposit = new AccountPreDeposit
            {
                Account = memberYetToDeposit.Account,
                ApplicationUser = memberYetToDeposit.Account?.ApplicationUser,
                AccountProfile = memberYetToDeposit.Account?.ApplicationUser?.AccountProfiles,
                PreDeposit = memberYetToDeposit
            };

            return accountPreDeposit;
        }


        public async Task<List<PreDeposit>> GetMemberListPaidAsync(int accountId)
        {
            var memberListPaid = _unitOfWork.PreDepositRepository
                .GetList();

                var result =  memberListPaid
                 .Where(x => x.AccountId == accountId)
            .Include(x => x.Account)
            .ThenInclude(a => a.Invoices)
            .ThenInclude(i => i.Deposit)
            .ToList();  // Fetch the result asynchronously

            return result;
        }



        public async Task<DepositToAccount?> PreDepoMembersAsync(int accountId)
        {
            

            var memberListPaid = await GetMemberListPaidAsync(accountId);



            var account = memberListPaid.Select(x => x.Account).FirstOrDefault();
            var invoices = memberListPaid.SelectMany(x => x.Account.Invoices).ToList();
            var deposits = invoices.Select(i => i.Deposit).ToList();
            var preDeposit = memberListPaid.FirstOrDefault();

            var depositToAccount = new DepositToAccount
            {
                Account = account,
                Invoice = invoices,
                Deposit = deposits,
                PreDeposit = preDeposit
            };

            return depositToAccount;
        }



        public async Task UpdatePreDepositAsync(Deposit? deposit, int accountId)
        {
            if (deposit == null)
            {
                throw new ArgumentNullException(nameof(deposit));
            }

            var memberPreDepoAccountList = await PreDepoMembersAsync(accountId);
            decimal updateDepositAmount = deposit.DepositAmount;

            if (memberPreDepoAccountList?.PreDeposit != null)
            {
                updateDepositAmount += memberPreDepoAccountList.PreDeposit.Amount;
            }

            var preDeposit = new PreDeposit
            {
                AccountId = accountId,
                PreDepositDate = DateTime.Now,
                Amount = updateDepositAmount
            };

            if (memberPreDepoAccountList == null)
            {
                await _unitOfWork.PreDepositRepository.Add(preDeposit);
            }
            else
            {
                _unitOfWork.PreDepositRepository.Update(preDeposit);
            }

            await _unitOfWork.SaveChangesAsync();
        }





    }
}
