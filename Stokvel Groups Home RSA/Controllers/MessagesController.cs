using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Interface.IServices.IMessageServices;
using Stokvel_Groups_Home_RSA.Interface.IRepo;

namespace Stokvel_Groups_Home_RSA.Controllers;

public class MessagesController : Controller
{

    public readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
    public readonly IUnitOfWork _unitOfWork;
  

    public MessagesController(Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager,
		IUnitOfWork unitOfWork 
       )
    {
       
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

	// GET: Messages
	public async Task<IActionResult> Index(int accountId, int groupId, string userIdEx)
	{
		if (groupId <= 0 || string.IsNullOrEmpty(userIdEx))
		{
			return BadRequest("Invalid parameters provided.");
		}

		TempData["groupId"] = groupId;
		TempData.Keep("groupId");

		var currentUser = await _userManager.GetUserAsync(User);
		var account = await _unitOfWork.AccountsRepository.GetAllAsync(
			x => x.Id == userIdEx && x.GroupId == groupId,
			includeProperties: "ApplicationUser"
		);
		var details = account.Select(x => x.ApplicationUser).FirstOrDefault();
		var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);

		if (details == null || group == null)
		{
			return NotFound("Account or group not found.");
		}

		var managerId = group.ManagerId;
		var memberName = $"{details.FirstName} {details.LastName}";
		var memberImageName = details.MemberFileName;
		var memberImagePath = details.MemberPhotoPath;

		ViewBag.MemberId = userIdEx ?? managerId;
		ViewBag.GroupId = groupId;
		ViewBag.CurrentUser = currentUser;
		ViewBag.MemberName = memberName;
		ViewBag.FileName = memberImageName;
		ViewBag.PathName = memberImagePath;
		ViewBag.image = "~/wwwroot/images/Profile";
		ViewBag.Status = "PrivateGroup";
		ViewBag.AccountId = accountId;
		ViewBag.Option = accountId > 0 ? "memberMessages" : "userMessages";
		ViewBag.CurrentUserName = currentUser.UserName;

		var status = await MessageStatus(accountId);

		if (status == "userMessages")
		{
			var userMessages = await _unitOfWork.MessageRepository.GetAllAsync(x => x.UserID == userIdEx && x.UserID == managerId && x.Group == groupId.ToString());
			return View(userMessages);
		}
		else if (status == "memberMessages")
		{
			var messages = await _unitOfWork.MessageRepository.GetAllAsync();
			return View(messages);
		}

		return View();
	}

	public async Task<string> MessageStatus(int accountId)
	{
			string? result;
			if (accountId != 0)
			{
				result = "memberMessages";
			}
			else
			{
				result = "memberMessages";
			}
			return result;
	
	}



	// POST: Messages/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

	public async Task<IActionResult> Create(Message message)
    {

            message.UserName = User.Identity.Name;
            var sender = await _userManager.GetUserAsync(User);
            message.UserID = sender.Id;
            await _unitOfWork.MessageRepository.Add(message);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
       
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

