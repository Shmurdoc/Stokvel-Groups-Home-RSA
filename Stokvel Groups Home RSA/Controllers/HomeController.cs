
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models;
using System.Diagnostics;

namespace Stokvel_Groups_Home_RSA.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IUnitOfWork _unitOfWork;
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {


        // get groupIds of active Accounts  
        //var groupId = await _homeRequestService.MemberAccountGroupId(userId);

        var id = User.Identity.GetUserId();


        var accountUser = await _unitOfWork.ApplicationUserRepository.GetByIdAsync(id);
        if (accountUser is null)
        {
            return RedirectToAction("Create", "AccountUsers");
        }

        //if (roleAccount)
        //{
        //    return RedirectToAction("AdminIndex");
        //}
        return View();
    }

    //public async Task<string> GetData()
    //{
    //    IEnumerable<Calendar> events = await _calendarRepository.GetAll();
    //    var eventData = events.Select(x => new Calendar { CalendarId = x.CalendarId, Title = x.Title, Start = x.Start, AllDay = x.AllDay, ClassName = x.ClassName, End = x.End, GroupId = x.GroupId, }).ToList();
    //    return Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
    //}



    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
