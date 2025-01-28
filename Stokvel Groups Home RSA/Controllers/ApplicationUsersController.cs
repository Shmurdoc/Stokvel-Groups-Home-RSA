using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using System.Data;
using System.Reflection.Metadata;


namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ILogger<ApplicationUsersController> _logger;
        private IUnitOfWork _unitOfWork;
        private IAccountRequestServices _accountRequestServices;
       

        public ApplicationUsersController(IUnitOfWork unitOfWork, 
            IAccountRequestServices accountRequestServices,
            ILogger<ApplicationUsersController> logger)
        {
        
            _unitOfWork = unitOfWork;
            _accountRequestServices = accountRequestServices;
            _logger = logger;
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
            var userId = User.Identity.GetUserId();
            var newUserVerify = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);

             if (newUserVerify.AcceptedUserAccount == true)
            {
                return RedirectToAction("Index", "Home");
            }
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
    
   
        public async Task<IActionResult> Create(ApplicationUserPersonal? applicationUserPersonal , IFormFile fileUpload)
        {
           

    

            // Get User Id
            var userId = User.Identity.GetUserId();

            // Check if User Already Exists
            var existingUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(userId);

            
            if (existingUser == null)
            {
                throw new ArgumentNullException(nameof(existingUser), "Existing user cannot be null.");
            }
            if (applicationUserPersonal.ApplicationUser == null)
            {
                throw new ArgumentNullException(nameof(applicationUserPersonal.ApplicationUser), "Application user personal cannot be null.");
            }



            if (HttpContext.Request.Form.Count > 0 && HttpContext.Request.Form.Files.Count > 0)
            {
                var uploadedImage = HttpContext.Request.Form.Files[0];
                await _unitOfWork.InputFoldersRepository.UploadImage(applicationUserPersonal.ApplicationUser, uploadedImage);
            }
            else
            {
                applicationUserPersonal.ApplicationUser.Date = DateTime.Now;
                applicationUserPersonal.ApplicationUser.MemberPhotoPath = "~/wwwroot/images/Profile";
                await _unitOfWork.ApplicationUserRepository.Add(applicationUserPersonal.ApplicationUser);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }


        // GET: AccountUsers/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            var accountUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(id);

            // get status of application Account
            ViewBag.AcceptedUser = accountUser?.AcceptedUserAccount;
           

            if ( accountUser == null)
            {
                return NotFound();
            }

           /* if (accountUser.AcceptedUserAccount == true)
            {
                return RedirectToAction("Index", "Home");
            }*/
            return View(accountUser);
        }

        // POST: AccountUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, [Bind("Id,FirstName,LastName,MemberPhotoPath,MemberFileName,Address,City,Province,Zip,Date,AcceptedUserAccount,PhoneNumber,SecondAddress,SecondStreetAddress,SecondCity,SecondProvince,SecondPostalCode,DateOfBirth,EmployerFirstName,EmployerLastName,EmailAddress,EmployerAddress,EmployerStreetAddress,EmployerCity,EmployerProvince,EmployerPostalCode,AnnualIncome,RentPayment,Loans,LoanPurpose,MemberIdPath,MemberIdFileName,MemberBankStatementPath,MemberBankStatementFileName")] ApplicationUser? applicationUser)
        {
            if (applicationUser == null)
            {
                throw new ArgumentNullException(nameof(applicationUser), "Application user personal cannot be null.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (HttpContext.Request.Form.Count > 0 && HttpContext.Request.Form.Files.Count > 0)
                    {
                        var uploadedImage = HttpContext.Request.Form.Files[0];
                        if (IsValidImage(uploadedImage))
                        {
                            await _unitOfWork.InputFoldersRepository.UploadImage(applicationUser, uploadedImage);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "Invalid image file.");
                            return View(applicationUser);
                        }
                    }
                    else
                    {
                        var existingEntity = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(id);
                        if (existingEntity == null)
                        {
                            return NotFound();
                        }

                        applicationUser.Date = DateTime.Now;
                        applicationUser.MemberPhotoPath = "~/wwwroot/images/Profile";
                        _unitOfWork.ApplicationUserRepository.Update(applicationUser);

                        var changes = await _unitOfWork.TrackAsync();

                        if (!changes.Any())
                        {
                            ModelState.AddModelError(string.Empty, "No changes detected.");
                            return View(applicationUser);
                        }

                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                catch (DBConcurrencyException e)
                {
                    _logger.LogError(e, "Concurrency error occurred while editing user.");
                    throw new ArgumentException("A concurrency error occurred. Please try again.", nameof(applicationUser));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while editing user.");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    throw;
                }
            }
            return View(applicationUser);
        }



        private bool IsValidImage(IFormFile file)
        {
            // Implement image validation logic here
            return true;
        }


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
