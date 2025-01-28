using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models;
using System.Diagnostics;
using System.Net;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var memberTotalAmount = new List<decimal>();

            var accountUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);
            if (accountUser == null || !accountUser.AcceptedUserAccount)
            {
                var encodedUserId = WebUtility.UrlEncode(userId);
                return RedirectToAction("Edit", "ApplicationUsers", new { id = encodedUserId });
            }

            var accounts = await _unitOfWork.AccountsRepository.GetAllAsync(u => u.Id == userId, includeProperties: "ApplicationUser");
            var groupIds = accounts.Where(x => x.Accepted).Select(x => x.GroupId).ToList();

            if (!groupIds.Any())
            {
                return View(new List<Deposit>());
            }

            var displayRecentDeposits = new List<Deposit>();
            var userInvoices = new List<Deposit>();
            var monthMemberNumbers = new List<int>();
            var lastDepositDate = new List<string>();
            var amountDue = new List<decimal>();

            foreach (var groupId in groupIds)
            {
                var groupMembers = await _unitOfWork.HomeRequestService.GetDepositDetailsAsync(groupId);
                userInvoices.AddRange(groupMembers.Where(x => x.Invoices.Any(y => y.Account.Id == userId)).ToList());

                var uniqueMonthMembers = groupMembers
                    .Where(x => x.Invoices.Any(y => y.Account.AccountQueueStart.Month == DateTime.Now.Month))
                    .SelectMany(x => x.Invoices)
                    .GroupBy(y => y.Account.AccountQueue)
                    .Select(g => g.First().Account.AccountQueue)
                    .FirstOrDefault();

                memberTotalAmount.Add(await TotalAmountTarget(groupId));
                lastDepositDate.Add(await LastDeposit(groupMembers, userId));
                monthMemberNumbers.Add(uniqueMonthMembers);
                amountDue.Add(await TotalAmountDue(groupMembers, uniqueMonthMembers, groupId));
            }

            ViewBag.MemberGroups = groupIds;
            ViewBag.TotalDepositDue = amountDue;
            ViewBag.totalAmount = memberTotalAmount;
            ViewBag.lastDepositDate = lastDepositDate;

            return View(displayRecentDeposits);
        }

        public async Task<decimal> TotalAmountTarget(int groupId)
        {
            var groupMembers = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
            var groupMembersTarget = await _unitOfWork.GroupRequestServices.CalculateAmountTarget(groupId);
            return groupMembersTarget * (groupMembers.TotalGroupMembers - 1);
        }

        public async Task<List<int>> MemberNumberDate(IEnumerable<Deposit> groupMembers)
        {
            var monthMemberNumbers = groupMembers
                .Where(x => x.Invoices.Any(y => y.Account.AccountQueueStart.Month == DateTime.Now.Month))
                .SelectMany(x => x.Invoices)
                .GroupBy(y => y.Account.AccountQueueStart)
                .Select(g => g.First().Account.AccountQueue)
                .ToList();

            return monthMemberNumbers;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<string> LastDeposit(IEnumerable<Deposit> groupMembers, string userId)
        {
            if (groupMembers == null)
            {
                return "0.00";
            }

            var result = groupMembers
                .Where(x => x.Invoices.Any(y => y.Account.Id == userId))
                .Select(x => x.Invoices.Select(i => i.TotalAmount).LastOrDefault())
                .LastOrDefault()
                .ToString("0.00");

            return result;
        }

        public async Task<decimal> TotalAmountDue(IEnumerable<Deposit> groupMembers, int monthMemberNumbers, int groupId)
        {
            var totalAmount = await _unitOfWork.GroupRequestServices.CalculateAmountTarget(groupId);
            var result = groupMembers
                .Where(x => x.DepositReference == "Deposit" + monthMemberNumbers)
                .Sum(x => x.DepositAmount);

            return totalAmount - result;
        }
    }
}
