using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.common.Alert.TempData;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Models;
using X.PagedList;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class GroupsController : Controller
    {

        private readonly IGroupRequestServices _groupGroupRequestServices;
        private readonly IAccountRequestServices _accountRequestService;
        private IUnitOfWork _unitOfWork;


        public GroupsController(IUnitOfWork unitOfWork, IGroupRequestServices groupRequestServices, IAccountRequestServices accountRequestService)
        {
            _groupGroupRequestServices = groupRequestServices;
            _accountRequestService = accountRequestService;
            _unitOfWork = unitOfWork;
        }

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

        // GET: Groups
        [HttpGet]
        public async Task<IActionResult> MyIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {

            var userId = User.Identity.GetUserId();


            var accountUser = await _unitOfWork.AccountsRepository.GetAllAsync(x => x.Id == userId);



            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UserId = userId;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var accountUsers = await _groupGroupRequestServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);

            return View(accountUsers.ToPagedList());
        }

        [HttpGet]
        public async Task<IActionResult> PrivateIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var userId = User.Identity.GetUserId();


            var accountUser = _unitOfWork.AccountsRepository.GetAllAsync();



            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UserId = userId;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var accountUsers = await _groupGroupRequestServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);

            return View(accountUsers.ToPagedList());
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

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id);
            if (id == null || group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Id = userId;
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,ManagerId,GroupName,VerifyKey,TypeAccount,TotalGroupMembers,GroupDate,AccountTarget,GroupStatus,Private")] Group @group)
        {
            var userId = User.Identity.GetUserId();

            var accountUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);

            var groupList = await _unitOfWork.GroupsRepository.GetAllAsync();

            var groupExists = groupList.Any(x => x.VerifyKey == @group.VerifyKey || x.GroupName == @group.GroupName);

            if (groupExists != true)
            {
                if (ModelState.IsValid)
                {

                    List<int> MemberList = new();

                    var accountList = await _unitOfWork.AccountsRepository.GetAllAsync(x => x.Id == userId);

                    foreach (var item in accountList)
                    {
                        if (item == null) continue;
                        var accountId = accountList.Where(x => x.Id == item.Id && x.Accepted == true).Select(x => x.AccountId).FirstOrDefault();
                        MemberList.Add(accountId);

                    }

                    if (MemberList.Count < 2)
                    {
                        // Create Group
                        @group.ManagerId = userId;
                        @group.GroupDate = DateTime.Now;
                        @group.Private = true;
                        @group.GroupImage = accountUser.MemberFileName;
                        @group.GroupStatus = true;
                        await _unitOfWork.GroupsRepository.Add(@group);
                        await _unitOfWork.SaveChangesAsync();

                        Account account = new()
                        {
                            Id = userId,
                            GroupId = group.GroupId,
                            GroupVerifyKey = group.VerifyKey,
                            AccountCreated = DateTime.Now,
                            Accepted = true,
                        };


                        try
                        {
                            // Create Manager Account Accepted
                            TempData["GroupStatusCreate"] = true;
                            string status = "Success!";
                            this.AddAlertSuccess($"{status} You have created a new group successfully.");

                            // add member to group
                            await _accountRequestService.AddAccountToGroupAsync(account, userId);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            string status = "Failed!";
                            if (await GroupExists(@group.VerifyKey)!)
                            {

                                this.AddAlertSuccess($"{status} Please try again.");
                                throw;
                            }
                            else
                            {
                                this.AddAlertSuccess($"{status} Something went wrong.");
                            }
                        }

                        return RedirectToAction("Index", "Accounts");
                    }
                    else
                    {
                        string status = "Failed!";
                        this.AddAlertDanger($"{status} You Have Reached Your Limit Of Creating New Groups At Your Current Level.");
                    }

                }
            }
            else
            {
                string status = "Failed!";
                this.AddAlertSuccess($"{status} Group Already Exists. Place change Group Name and VerifyKey");
            }
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id);
            if (id == null || group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(int id, [Bind("GroupId,ManagerId,GroupName,VerifyKey,TypeAccount,TotalGroupMembers,GroupDate,AccountTarget,GroupStatus,Private")] Group group)
        {
            if (id != group.GroupId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(group);
            }

            try
            {
                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var accountUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);
                if (accountUser == null)
                {
                    return NotFound("User not found.");
                }

                group.ManagerId = userId;
                group.GroupDate = DateTime.Now;
                group.Private = true;
                group.GroupImage = accountUser.MemberFileName;
                group.GroupStatus = true;

                _unitOfWork.GroupsRepository.Update(group);
                await _unitOfWork.SaveChangesAsync();

                this.AddAlertSuccess("Success! You have successfully updated a document.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await GroupExists(group.VerifyKey)!)
                {
                    this.AddAlertDanger("Failed! Something went wrong, data was not saved.");
                    return RedirectToAction(nameof(Edit));
                }
                else
                {
                    this.AddAlertDanger("Failed! Something went wrong.");
                    throw;
                }
            }

            return RedirectToAction(nameof(PrivateIndex));
        }


        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var @group = await _unitOfWork.GroupsRepository.GetByIdAsync(id);
            if (id == null || @group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(id);
            if (group == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Accounts'  is null.");
            }
            else
            {
                await _unitOfWork.GroupsRepository.RemoveAsync(group);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(PrivateIndex));
            }

        }

        private async Task<bool> GroupExists(string id)
        {
            var exists = await _unitOfWork.GroupsRepository.GetAllAsync(x => x.VerifyKey == id);
            return exists.Any();
        }
    }
}
