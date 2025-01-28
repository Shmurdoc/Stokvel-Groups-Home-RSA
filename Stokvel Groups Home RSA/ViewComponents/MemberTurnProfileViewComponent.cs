using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stokvel_Groups_Home_RSA.ViewComponents;

public class MemberTurnProfileViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
   

    public MemberTurnProfileViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IViewComponentResult> InvokeAsync(int groupId)
    {
        if (!int.TryParse(ViewData["GroupId"]?.ToString(), out groupId))
        {
            groupId = 0; // Default value if conversion fails
        }

        // Fetch deposits and include related entities
        var deposits = await _unitOfWork.DepositRepository.GetList()
            .Include(d => d.Invoices)
                .ThenInclude(i => i.Account)
                    .ThenInclude(a => a.ApplicationUser)
            .Where(d => d.Invoices.Any(i => i.Account.GroupId == groupId))
            .ToListAsync();

        ViewBag.Deposit = deposits;

        // Get the application user for the current month's account queue
        var applicationUser = deposits
            .SelectMany(d => d.Invoices)
            .Where(a => a.Account.GroupId == groupId)
            .GroupBy(a => a.Account.Id)
            .Select(g => g.First().Account.ApplicationUser)
            .ToList();

        // Get the accounts for the current month's account queue
        var accounts = deposits
            .SelectMany(d => d.Invoices)
            .Where(i => i.Account.AccountQueueStart.Month == DateTime.Today.Month)
            .Select(i => i.Account)
            .Distinct()
            .ToList();

        // Get all invoices
        var invoices = deposits
            .Where(x => x.DepositReference == "Deposit" + accounts
                .Where(d => d.AccountQueueStart.Month == DateTime.Now.Month)
                .Select(x => x.AccountQueue)
                .SingleOrDefault())
            .SelectMany(d => d.Invoices)
            .ToList();

        // Get member deposits
        var memberDeposits = deposits.ToList();

        // Create an AccountInvoice object
        var accountInvoice = new AccountInvoice
        {
            ApplicationUser = applicationUser,
            Account = accounts,
            Invoice = invoices,
            Deposit = memberDeposits
        };

        var memberInvoice = invoices
            .GroupBy(x => x.InvoiceDate.Value.Day)
            .Select(x => x.FirstOrDefault())
            .ToList();

        var userId = User.Identity.GetUserId();

        // ViewBag Area
        // Get deposited amount
        ViewBag.DepositedAmount = invoices
            .Where(x => x.Account.Id == userId)
            .Sum(ta => ta.TotalAmount);

        // Get group by date for timeline and select one of each
        ViewBag.Invoice = memberInvoice
            .OrderBy(x => x.InvoiceDate.Value.Day)
            .Select(x => x.InvoiceDate.Value)
            .ToList();

        // Get outstanding amount
        ViewBag.Outstanding = await Outstanding(
            invoices.Where(x => x.Account.Id == userId).ToList(),
            groupId
        );

        // Get target amount
        ViewBag.TargetAmount = await TargetAmount(groupId);

        // Set the ViewBag image
        ViewBag.Image = "~/wwwroot/images/Profile";

        // Return the view component result
        return View(accountInvoice);
    }


    public async Task<decimal> Outstanding(List<Invoice> invoices, int groupId)
	{
		if (invoices == null)
		{
			throw new ArgumentNullException(nameof(invoices), "Invoices list cannot be null.");
		}

		if (groupId == 0)
		{
			throw new ArgumentException("Group ID cannot be zero.", nameof(groupId));
		}

        var targetAmount = await TargetAmount(groupId);
		var totalSum = invoices.Sum(ta => ta.TotalAmount);

		return targetAmount - totalSum;
	}

    public async Task<decimal> TargetAmount(int groupId)
    {
		return await _unitOfWork.GroupRequestServices.CalculateAmountTarget(groupId);
	}


}
