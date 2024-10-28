using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;


namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class ApplicationUsersController : Controller
    {

        private IUnitOfWork _unitOfWork;
        public ApplicationUserPersonal applicationUserPersonal { get; private set; }
        private readonly IInputFoldersRepository _inputFoldersRepository;
        public ApplicationUsersController(IUnitOfWork unitOfWork, IInputFoldersRepository inputFoldersRepository)
        {
            _inputFoldersRepository = inputFoldersRepository;
            _unitOfWork = unitOfWork;
        }

        //// GET: AccountUsers
        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public async Task<ViewResult> AdminIndex(string sortOrder, string currentFilter, string searchString, int? page)
        //{
        //	if (!User.IsInRole("Admin"))
        //	{
        //		return View(RedirectToAction("Index", "Home"));

        //	}

        //          ViewBag.image = "/wwwroot/images/Profile";
        //          ViewBag.CurrentSort = sortOrder;
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

        //	var accountUsers = await _accountUserRequestServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);

        //	return View(accountUsers.ToPagedList());
        //}



        // GET: ApplicationUsers
        public async Task<ViewResult> Index()
        {
            var id = User.Identity.GetUserId();
            var accountUsers = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(id);

            ViewBag.image = "/wwwroot/images/Profile";

            return View(accountUsers);
        }

        //// GET: AccountUsers/Details/5
        //[HttpGet]
        //public async Task<IActionResult> Details(string? id)
        //{
        //	var accountUser = await _accountUserCRUDService.GetById(id);
        //	if (id == null || accountUser == null)
        //	{
        //		return NotFound();
        //	}

        //	return View(accountUser);
        //}

        // GET: AccountUsers/Create
        public async Task<IActionResult> Create()
        {
            var UserId = User.Identity.GetUserId();
            var newUserVerify = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(UserId);

            //if (newUserVerify != null)
            //{
            //    return RedirectToAction(nameof(AdminIndex));
            //}
            return View();
        }

        // POST: AccountUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,MemberPhotoPath,MemberFileName,Address,City,Province,Zip,Date")] ApplicationUserPersonal applicationUserPersonal)
        {
            if (!ModelState.IsValid)
            {
                return View(applicationUserPersonal);
            }

            // Get User Id
            var userId = User.Identity.GetUserId();

            // Check if User Already Exists
            var existingUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);

            if (existingUser == null && applicationUserPersonal == null)
            {
                return View(applicationUserPersonal);
            }

            applicationUserPersonal.ApplicationUser.Id = userId;
            applicationUserPersonal.ApplicationUser.Date = DateTime.Now;

            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var uploadedImage = HttpContext.Request.Form.Files[0];
                await _inputFoldersRepository.UploadImage(applicationUserPersonal.ApplicationUser, uploadedImage);
            }
            else
            {
                applicationUserPersonal.ApplicationUser.MemberPhotoPath = "~/wwwroot/images/Profile";
                await _unitOfWork.ApplicationUserRepository.Add(applicationUserPersonal.ApplicationUser);
                await _unitOfWork.AccountUserPersonalRepository.Add(applicationUserPersonal.AccountUserPersonal);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }


        //// GET: AccountUsers/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{

        //	var accountUser = await _accountUserCRUDService.GetById(id);
        //	if (id == null || accountUser == null)
        //	{
        //		return NotFound();
        //	}

        //          return View(accountUser);
        //}

        //// POST: AccountUsers/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,MemberPhotoPath,MemberFileName,Address,City,Province,Zip,Date")] ApplicationUser applicationUser)
        //{
        //	if (id != applicationUser.Id)
        //	{
        //		return NotFound();
        //	}

        //	if (ModelState.IsValid)
        //	{
        //		try
        //		{

        //                  applicationUser.Date = DateTime.Now;
        //                  applicationUser.MemberPhotoPath = "~/wwwroot/images/Profile";
        //                  _accountUserCRUDService.Edit(applicationUser);
        //			await _accountUserCRUDService.SaveAsync();
        //		}
        //		catch (DbUpdateConcurrencyException)
        //		{
        //			if (!AccountUserExists(applicationUser.Id))
        //			{
        //				return NotFound();
        //			}
        //			else
        //			{
        //				throw;
        //			}
        //		}
        //		return RedirectToAction(nameof(Index));
        //	}
        //	return View(applicationUser);
        //}

        //// GET: AccountUsers/Delete/5
        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Delete(string id)
        //{
        //	var accountUser = await _accountUserCRUDService.GetById(id);
        //	if (id == null || accountUser == null)
        //	{
        //		return NotFound();
        //	}

        //	return View(accountUser);
        //}

        //// POST: AccountUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //	var accountUser = await _accountUserCRUDService.GetById(id);

        //	if (id == null || accountUser == null)
        //	{
        //		return Problem("Entity set 'ApplicationDbContext.AccountUsers'  is null.");
        //	}
        //	await _accountUserCRUDService.Delete(id);
        //	await _accountUserCRUDService.SaveAsync();


        //	return RedirectToAction(nameof(Index));
        //}

        //private bool AccountUserExists(string id)
        //{
        //	return (_accountUserCRUDService.AccountUserExists(id));
        //}
    }
}
