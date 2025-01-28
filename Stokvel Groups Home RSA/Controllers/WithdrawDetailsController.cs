using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet;
using System.Text.RegularExpressions;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class WithdrawDetailsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WithdrawDetailsController> _logger;
        private readonly IWithdrawRequestService _withdrawRequestService;

        public WithdrawDetailsController(IUnitOfWork unitOfWork, ILogger<WithdrawDetailsController> logger, IWithdrawRequestService withdrawRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _withdrawRequestService = withdrawRequestService;
        }


        public async Task<IActionResult> MemebrResultList(int id, int groupId)
        {
            return View();
        }

        public async Task<IActionResult> Payment(int id, int groupId)
        {
            //var memberInDb = await _accountRequestService.DisplayMemberTurnProfile();
            //var userAccount = await _accountRequestService.MembersInGroup();
            //decimal memberTotalAmount = 0;




            //var currnetMember = memberInDb.Where(x => x.GroupMembers.GroupId == groupId).ToList();

            //List<string> MemberSet = new();
            //List<string> MemberList = new();


            //var accountIdList = await _accountRequestService.AcceptedGroupMembers(groupId);

            //var paidMemberName = currnetMember.Where(x => x.Account.AccountId == id).Select(x => x.AccountUser.FirstName).FirstOrDefault();

            //foreach (var accountId in accountIdList)
            //{
            //	memberTotalAmount = memberTotalAmount + memberInDb.Where(x => x.Account.AccountId == accountId && x.Invoice.Discription == @ViewBag.AccountPayName).Sum(x => x.Invoice.TotalAmount);
            //}
            //DateTime dateTime = DateTime.Now;
            //string month = dateTime.ToString("MMMM");

            //ViewBag.MonthlyBill = memberTotalAmount;
            //ViewBag.DuePaymentDate = month + " " + 28;
            //ViewBag.GroupTarget = userAccount.Where(x => x.Account.AccountId == id).Select(x => x.Group.AccountTarget).FirstOrDefault();

            //ViewBag.AccountIdList = accountIdList;
            //ViewBag.AccountPayName = paidMemberName;

            //memberInDb

            return View();
        }

        // GET: InvoiceDetails
        public async Task<IActionResult> Index(int groupId)
        {
            try
            {
                // Fetch deposits related to the given groupId and include related entities
                var deposits = await _withdrawRequestService.GetDeposits(groupId);

                // Store deposits in ViewBag for use in the view
                ViewBag.Deposit = deposits;

                // Fetch application users for the current month's account queue
                var applicationUsers = await _withdrawRequestService.GetApplicationUsers(deposits,groupId);

                // Get accounts for the current month's queue
                var currentMonthAccounts = await _withdrawRequestService.GetCurrentMonthAccounts(deposits);

                // Get all accounts for the group
                var allAccounts = await _withdrawRequestService.GetAccounts(deposits);

                // Get invoices related to the selected accounts
                var invoicesForGroup = await _withdrawRequestService.GetInvoicesForGroup(deposits,currentMonthAccounts);

                // Fetch member deposit information
                var memberDeposits = deposits.ToList();

                // Create the AccountInvoice model object
                var accountInvoice = new AccountInvoice
                {
                    ApplicationUser = applicationUsers,
                    Account = allAccounts,
                    Invoice = invoicesForGroup,
                    Deposit = memberDeposits
                };

                // Group invoices by the day of the invoice date for the current month
                var memberInvoice = await _withdrawRequestService.GroupInvoicesByDay(invoicesForGroup);

                // Get the sum of invoices for accounts in the current month, excluding the ones from the current month
                var invoiceSummary = await _withdrawRequestService.GetSumOfInvoices(allAccounts);

                // Get the current user's ID
                var userId = User.Identity.GetUserId();

                // Populate ViewBag with necessary data
                ViewBag.TotalDepositedAmount = invoiceSummary;
                ViewBag.DepositedAmount = invoicesForGroup
                    .Where(invoice => invoice.Account.Id == userId)
                    .Sum(ta => ta.TotalAmount);

                // Get the list of invoice dates for the timeline (sorted by day)
                ViewBag.Invoice = memberInvoice
                    .OrderBy(invoice => invoice.InvoiceDate.Value.Day)
                    .Select(invoice => invoice.InvoiceDate.Value)
                    .ToList();

                // Get the outstanding amount for the user
                ViewBag.Outstanding = await _withdrawRequestService.GetOutstandingAmount(
                    invoicesForGroup.Where(x => x.Account.Id == userId).ToList(),
                    groupId
                );

                // Get the target amount for the group
                ViewBag.TargetAmount = await _withdrawRequestService.GetTargetAmount(groupId);

                // Set the profile image path for the user
                ViewBag.Image = "~/wwwroot/images/Profile";

                // Return the view with the model
                return View(accountInvoice);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging library like Serilog, NLog, etc.)
                _logger.LogError(ex, "An error occurred while fetching the invoice data.");

                // Display a generic error message to the user
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";

                // Return the view with an empty model or a fallback view
                return View(new AccountInvoice());
            }
        }


        //GET: InvoiceDetails/Create
        public async Task<IActionResult> Create(int groupId)
        {
            // Fetch deposits related to the given groupId and include related entities
            var deposits = await _withdrawRequestService.GetDeposits(groupId);

            // Get accounts for the current month's queue
            var currentMonthAccounts = await _withdrawRequestService.GetCurrentMonthAccounts(deposits);

            // Get invoices related to the selected accounts
            var invoicesForGroup = await _withdrawRequestService.GetInvoicesForGroup(deposits, currentMonthAccounts);
            var userId = User.Identity.GetUserId();

            ViewBag.MemberList = 
            //var memberName = memberList.Where(x=>x.)
            //total pay collection of funds for the month
            ViewBag.MonthlyAmountTotalPaid = await _withdrawRequestService.GetOutstandingAmount(
                    invoicesForGroup.Where(x => x.Account.Id == userId).ToList(),
                    groupId
                );
            return View();
        }
        //// POST: InvoiceDetails/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        ////public async Task<IActionResult> Create([Bind("DetailedId,InvoiceId,CreditAmount,TaxID,PaymentId")] WithdrawDetails invoiceDetails)
        ////{
        ////	if (ModelState.IsValid)
        ////	{
        ////		 _withdrawRequestService.CreditMember(invoiceDetails);
        ////		return RedirectToAction(nameof(Index));
        ////	}
        ////	return View(invoiceDetails);
        ////}

    }
}
