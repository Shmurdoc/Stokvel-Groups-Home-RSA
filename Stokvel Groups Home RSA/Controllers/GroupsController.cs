using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Models;
using X.PagedList;
using Microsoft.AspNet.Identity;
using Stokvel_Groups_Home_RSA.common.Alert.TempData;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IGroupRequestServices _groupRequestServices;
        private readonly IAccountRequestServices _accountRequestService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IUnitOfWork unitOfWork,
            IGroupRequestServices groupRequestServices,
            IAccountRequestServices accountRequestService,
            ILogger<GroupsController> logger)
        {
            _groupRequestServices = groupRequestServices;
            _accountRequestService = accountRequestService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Helper Methods (for shared functionality like Alerts, etc.)
        private void AddAlert(string status, string message)
        {
            if (status == "Success!")
                this.AddAlertSuccess(message);
            else
                this.AddAlertDanger(message);
        }

        private async Task<bool> GroupExists(string verifyKey)
        {
            var group = await _unitOfWork.GroupsRepository.GetAllAsync(x => x.VerifyKey == verifyKey);
            return group.Any();
        }

        // Action Methods

        // GET: My Groups Index
        [HttpGet]
        public async Task<IActionResult> MyGroupsIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            string userId = User.Identity.GetUserId();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UserId = userId;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var accountUsers = await _groupRequestServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);
            return View(accountUsers.ToPagedList());
        }

        // GET: Private Groups Index
        [HttpGet]
        public async Task<IActionResult> PrivateGroupsIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            string userId = User.Identity.GetUserId();

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UserId = userId;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var accountUsers = await _groupRequestServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);
            return View(accountUsers.ToPagedList());
        }

        // GET: Group Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id.Value);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // GET: Create Group
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            return View();
        }

        // POST: Create Group
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,ManagerId,GroupName,VerifyKey,TypeAccount,TotalGroupMembers,GroupDate,AccountTarget,GroupStatus,Private")] Group group, string accountNumber, string paymentMethod)
        {
            var userId = User.Identity.GetUserId();

            if (await GroupExists(group.VerifyKey))
            {
                AddAlert("Failed!", "Group already exists. Please change Group Name and VerifyKey.");
                return View(group);
            }

            if (!ModelState.IsValid)
                return View(group);

            // Account creation logic
            var account = new Account
            {
                Id = userId,
                GroupId = group.GroupId,
                GroupVerifyKey = group.VerifyKey,
                AccountCreated = DateTime.Now,
                Accepted = true,
                AccoutNumber = accountNumber,
                PaymentMethod = paymentMethod,
            };

            try
            {
                group.ManagerId = userId;
                group.GroupDate = DateTime.Now;
                group.Private = true;
                group.GroupStatus = true;

                await _unitOfWork.GroupsRepository.Add(group);
                await _unitOfWork.SaveChangesAsync();

                await _accountRequestService.AddAccountToGroupAsync(account, userId);

                TempData["GroupStatusCreate"] = true;
                AddAlert("Success!", "You have created a new group successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating group.");
                AddAlert("Failed!", "Something went wrong. Please try again.");
            }

            return RedirectToAction("Index", "Accounts");
        }

        // GET: Edit Group
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id.Value);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: Edit Group
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,ManagerId,GroupName,VerifyKey,TypeAccount,TotalGroupMembers,GroupDate,AccountTarget,GroupStatus,Private")] Group group)
        {
            if (id != group.GroupId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(group);

            try
            {
                var userId = User.Identity.GetUserId();

                group.ManagerId = userId;
                group.GroupDate = DateTime.Now;
                group.Private = true;
                group.GroupStatus = true;

                _unitOfWork.GroupsRepository.Update(group);
                await _unitOfWork.SaveChangesAsync();

                AddAlert("Success!", "Group updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                AddAlert("Failed!", "Something went wrong while updating the group.");
                throw;
            }

            return RedirectToAction("PrivateGroupsIndex");
        }

        // GET: Delete Group
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id.Value);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: Delete Group
        [HttpPost, ActionName("DeleteGroup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroupConfirmed(int id)
        {
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id);
            if (group == null)
                return NotFound();

            _unitOfWork.GroupsRepository.RemoveAsync(group);
            await _unitOfWork.SaveChangesAsync();

            AddAlert("Success!", "Group deleted successfully.");
            return RedirectToAction("PrivateGroupsIndex");
        }

        //// GET: Groups
        //public async Task<IActionResult> ManagerIndex()
        //{
        //	var userId = User.Identity.GetUserId();
        //	var groups = await _accountsCRUDService.GetByUserId(userId);
        //	var groupsInDb = await _groupMembersCRUDService.GetAllAsync();
        //	var allGroupsCreated = await _groupsCRUDService.GetAllAsync();
        //          var accountUser = await _accountUserCRUDService.GetById(userId);
        //	List<int> groupIdList = new();

        //	foreach (var item in groups)
        //	{
        //		var result = groupsInDb.Where(x => x.AccountId == item.AccountId).Select(x => x.GroupId).FirstOrDefault();
        //		groupIdList.Add(result);

        //	}
        //          ViewBag.image = "~/wwwroot/images/Profile";
        //	ViewBag.MemberPhotoPath = accountUser.MemberPhotoPath;
        //          ViewBag.GroupIdList = groupIdList;

        //	return View(allGroupsCreated);
        //}

        //// GET: Groups
//public async Task<IActionResult> AdminIndex(string sortOrder, string currentFilter, string searchString, int? page)
//{
//	if (!User.IsInRole("Admin"))
//	{
//		return View(RedirectToAction("Index", "Home"));

//	}
//	ViewBag.CurrentSort = sortOrder;
//	ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
//	ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";


//	if (searchString != null)
//	{
//		page = 1;
//	}
//	else
//	{
//		searchString = currentFilter;
//	}

//	ViewBag.CurrentFilter = searchString;

//	var accountUsers = await _groupGroupRequestServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);

//	return View(accountUsers.ToPagedList());
//}
    }
}
