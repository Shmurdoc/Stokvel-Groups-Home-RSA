﻿using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo
{
    public class PreDepositInfo : IPreDepositRequestServices
    {

        private readonly IUnitOfWork _unitOfWork;
        public DepositToAccount? DepositToAccount { get; set; }
        public AccountPreDeposit? AccountPreDeposit { get; set; }
        public PreDepositInfo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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




        public async Task<DepositToAccount?> PreDepoMembersAsync(int accountId)
        {
            var memberListPaid = await _unitOfWork.PreDepositRepository.GetList()
                .Where(x => x.AccountId == accountId)
                .Include(x => x.Account)
                .ThenInclude(a => a.Invoices)
                .ThenInclude(i => i.Deposit)
                .ToListAsync();

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



        public async Task UpdatePreDeposit(Deposit? deposit, int accountId)
        {
            var memberPreDepoAccountList = await _unitOfWork.PreDepositRepository?.GetAllAsync();

            var preDeposit = new PreDeposit
            {
                AccountId = accountId,
                PreDepositDate = DateTime.Now,
                Amount = deposit.DepositAmount
            };

            if (!memberPreDepoAccountList.Any(x => x.AccountId == accountId))
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
