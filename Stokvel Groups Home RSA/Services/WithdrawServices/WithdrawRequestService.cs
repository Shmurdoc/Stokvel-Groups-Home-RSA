using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet;

namespace Stokvel_Groups_Home_RSA.Services.WithdrawServices;

public class WithdrawRequestService : IWithdrawRequestService
{

	private readonly IUnitOfWork _unitOfWork;
    private readonly IWithdrawServices _withdrawServices;
    private readonly IGroupRequestServices _groupRequestServices;

	public WithdrawRequestService(IUnitOfWork unitOfWork, IWithdrawServices withdrawServices, IGroupRequestServices groupRequestServices)
	{
		_unitOfWork = unitOfWork;
        _withdrawServices = withdrawServices;
        _groupRequestServices = groupRequestServices;
	}


    public async Task ApplyPenaltiesAsync()
    {
        var groupIdList = await _unitOfWork.GroupsRepository.GetList().Where(g => g.Active == true).Select(g => g.GroupId).ToListAsync();
        var accountList = await _unitOfWork.AccountsRepository.GetAllAsync();

        foreach (var groupId in groupIdList)
        {
            var groupTargetAmount = await _groupRequestServices.CalculateAmountTarget(groupId);
            var accountIdList = accountList.Where(a => a.GroupId == groupId).Select(a => a.AccountId).ToList();
            var deposits = await GetDeposits(groupId);
            var currentMonthAccounts = await GetCurrentMonthAccounts(deposits);
            var invoicesForGroup = await GetInvoicesForGroup(deposits, currentMonthAccounts);

            var outstandingAmount = await this.GetOutstandingAmount(invoicesForGroup, groupId);
            var outstandingAmountPerPerson = outstandingAmount / currentMonthAccounts.Count;

            List<Account> overdueMembers = new();

            foreach (var accountId in accountIdList)
            {
                var memberInvoice = currentMonthAccounts.Where(i => i.AccountId == accountId).ToList();

                if (memberInvoice.Select(x => x.Invoices.Sum(x => x.TotalAmount)).FirstOrDefault() != groupTargetAmount)
                {
                    overdueMembers.Add(memberInvoice.FirstOrDefault());
                }

                if (overdueMembers.Count > 0)
                {
                    //await _withdrawServices.SendPenaltyEmail(overdueMembers);
                    await _withdrawServices.PenaltiesAsync(overdueMembers, groupTargetAmount);
                }
            }
        }
    }



    // Fetch deposits related to the given groupId and include related entities
    public async Task<List<Deposit>> GetDeposits(int groupId)
    {
        return  await _unitOfWork.DepositRepository.GetList()
                    .Include(d => d.Invoices)
                        .ThenInclude(i => i.Account)
                            .ThenInclude(a => a.ApplicationUser)
                    .Where(d => d.Invoices.Any(i => i.Account.GroupId == groupId))
                    .ToListAsync();
    }

    public async Task<List<Deposit>> GetDepositsByGroupId(int groupId)
    {
        return await _unitOfWork.DepositRepository.GetList()
            .Include(d => d.Invoices)
                .ThenInclude(i => i.Account)
                    .ThenInclude(a => a.ApplicationUser)
            .Where(d => d.Invoices.Any(i => i.Account.GroupId == groupId))
            .ToListAsync();
    }

    // Fetch application users for the current month's account queue
    public async Task<List<ApplicationUser>> GetApplicationUsers(List<Deposit> deposits, int groupId)
    {
        
        return deposits
            .SelectMany(d => d.Invoices)
            .Where(i => i.Account.GroupId == groupId)
            .GroupBy(i => i.Account.Id)
            .Select(g => g.First().Account.ApplicationUser)
            .ToList();
    }

    // Get accounts for the current month's queue
    public async Task<List<Account>> GetAccounts(List<Deposit> deposits)
    {
        return deposits
            .SelectMany(d => d.Invoices)
            .Select(i => i.Account)
            .Distinct()
            .ToList();
    }

    // Get invoices related to the selected accounts
    public async Task<List<Invoice>> GetInvoicesForGroup(List<Deposit> deposits, List<Account> currentMonthAccounts)
    {
        return deposits
                    .Where(d => d.DepositReference == "Deposit" + currentMonthAccounts
                        .Select(x => x.AccountQueue)
                        .SingleOrDefault())
                    .SelectMany(d => d.Invoices)
                    .ToList();
    }

    // Get accounts for the current month's queue
    public async Task<List<Account>> GetCurrentMonthAccounts(List<Deposit> deposits)
    {
        return deposits
                    .SelectMany(d => d.Invoices)
                    .Where(i => i.Account.AccountQueueStart.Month == DateTime.Today.Month)
                    .Select(i => i.Account)
                    .Distinct()
                    .ToList();
    }

    public async Task<decimal> GetOutstandingAmount(List<Invoice> invoices, int groupId)
    {
        if (invoices == null)
        {
            throw new ArgumentNullException(nameof(invoices), "Invoices list cannot be null.");
        }

        if (groupId == 0)
        {
            throw new ArgumentException("Group ID cannot be zero.", nameof(groupId));
        }

        var targetAmount = await GetTargetAmount(groupId);
        var totalSum = invoices.Sum(ta => ta.TotalAmount);

        return targetAmount - totalSum;
    }



    // Group invoices by the day of the invoice date for the current month
    public async Task<List<Invoice>> GroupInvoicesByDay(List<Invoice> invoices)
    {
        return invoices
                    .GroupBy(x => x.InvoiceDate.Value.Day)
                    .Select(x => x.FirstOrDefault())
                    .ToList();
    }

    // Get the sum of invoices for accounts in the current month, excluding the ones from the current month
    public async Task<decimal> GetSumOfInvoices(List<Account> allAccounts)
    {
        // Ensure that allAccounts is not null and contains items
        if (allAccounts == null || !allAccounts.Any())
        {
            return 0m; // Return 0 if there are no accounts
        }

        // Filter and calculate the sum of invoices for accounts not starting in the current month
        var invoiceSum = allAccounts
            .Where(account => account.AccountQueueStart.Month != DateTime.Now.Month)
            .SelectMany(account => account.Invoices)
            .Where(invoice => invoice.InvoiceDate.HasValue && invoice.InvoiceDate.Value.Month == DateTime.Now.Month)
            .Sum(invoice => invoice.TotalAmount);

        return invoiceSum;
    }



    public async Task<decimal> GetTargetAmount(int groupId)
    {
        return await _unitOfWork.GroupRequestServices.CalculateAmountTarget(groupId);
    }

}
