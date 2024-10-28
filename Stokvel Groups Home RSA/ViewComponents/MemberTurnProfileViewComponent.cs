using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.ViewComponents;

public class MemberTurnProfileViewComponent : ViewComponent
{

    private readonly IUnitOfWork _unitOfWork;
    public AccountInvoice AccountInvoice { get; private set; }

    public MemberTurnProfileViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [HttpGet]
    public async Task<IViewComponentResult> InvokeAsync(int groupId)
    {
        // Display the next member turn profile
        var deposits = _unitOfWork.DepositRepository.GetList()
            .Include(i => i.Invoices.Where(x => x.Account.GroupId == groupId))
            .ThenInclude(a => a.Account)
            .ThenInclude(au => au.ApplicationUser)
            .ToList();

        var applicationUser = deposits?
            .Select(x => x.Invoices
                .Where(i => i.Account.AccountQueueStart.Month == DateTime.Today.Month)
                .Select(i => i.Account?.ApplicationUser)
                .SingleOrDefault())
            .SingleOrDefault();

        var accounts = deposits?
            .Select(x => x.Invoices?
                .Where(i => i.Account.AccountQueueStart.Month == DateTime.Today.Month)
                .Select(i => i.Account)
                .SingleOrDefault())
            .ToList();

        var invoices = deposits?
            .SelectMany(x => x.Invoices)
            .ToList();

        var memberDeposits = deposits?
            .Where(x => x.DepositReference == applicationUser?.FirstName)
            .ToList();

        var accountInvoice = new AccountInvoice
        {
            ApplicationUser = applicationUser,
            Account = accounts,
            Invoice = invoices,
            Deposit = memberDeposits
        };

        return await Task.FromResult<IViewComponentResult>(View(accountInvoice));
    }


}
