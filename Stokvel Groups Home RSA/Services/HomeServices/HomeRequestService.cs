using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IHomeService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using System.Text.RegularExpressions;

namespace Stokvel_Groups_Home_RSA.Services.HomeService
{
    public class HomeRequestService : IHomeRequestService
    {

        private readonly IUnitOfWork _unitOfWork;
       

        public HomeRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ApplicationAccount> GetApplicationAccountDetailsAsync(int groupId)
        {
            var homeRequest = await GetDepositDetailsAsync(groupId);
            var accounts = await _unitOfWork.AccountsRepository.GetList()
                .Include(a => a.ApplicationUser)
                .Where(a => a.GroupId == groupId)
                .ToListAsync();

            if (homeRequest == null && accounts == null)
            {
                return null;
            }

            var applicationAccount = new ApplicationAccount();

            if (homeRequest != null)
            {
                applicationAccount.ApplicationUsers = homeRequest
                    .SelectMany(d => d.Invoices.Select(i => i.Account.ApplicationUser))
                    .ToList();
                applicationAccount.Account = homeRequest
                    .SelectMany(d => d.Invoices.Select(i => i.Account))
                    .ToList();
            }
            else
            {
                applicationAccount.ApplicationUsers = accounts
                    .Select(a => a.ApplicationUser)
                    .ToList();
                applicationAccount.Account = accounts;
            }

            return applicationAccount;
        }




        public async Task<IEnumerable<Deposit>> GetDepositDetailsAsync(int groupId)
        {
            // Fetch deposits and include related entities
            var deposits = await _unitOfWork.DepositRepository.GetList()
                .Include(d => d.Invoices)
                    .ThenInclude(i => i.Account)
                        .ThenInclude(a => a.ApplicationUser)
                .Where(d => d.Invoices.Any(i => i.Account.GroupId == groupId))
                .ToListAsync();

            return deposits.ToList();
        }
    }
}
