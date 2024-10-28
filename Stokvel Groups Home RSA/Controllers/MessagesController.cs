using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Models;
using System.Diagnostics;
using System.Web.Mvc;

namespace Stokvel_Groups_Home_RSA.Controllers
{
    public class MessagesController
    {

        //      // GET: Messages
        //public async Task<IActionResult> Index(int Id, int groupId, string userIdEx)
        //      {
        //          string? status;

        //          TempData["groupId"] = groupId;
        //          TempData.Keep("groupId");
        //          var userId = await _userManager.GetUserAsync(User);
        //          var members = await _accountRequestService.PendingMembersInGroup();
        //          var details = await _accountUserCRUDService.GetById(userId.Id);

        //          var managerId = members.Where(x => x.Group.GroupId == groupId).Select(x => x.Group.ManagerId).FirstOrDefault();
        //          var memberId = members.Where(x => x.AccountUser.Id == userId.Id).Select(x => x.AccountUser.Id).FirstOrDefault();


        //          var memberName = details.FirstName + " " + details.LastName;


        //          var memberImageName = details.MemberFileName;
        //          var memberImagePath = details.MemberPhotoPath;


        //          if (userIdEx == null)
        //          {
        //              ViewBag.MemberId = managerId;
        //              ViewBag.ManagerId = memberId;
        //          }
        //          else
        //          {
        //              ViewBag.MemberId = userIdEx;
        //          }


        //          if (User.IsInRole("Admin"))
        //          {
        //              var adminId = await _userManager.GetUserAsync(User);

        //              var admin = await _accountUserCRUDService.GetById(adminId.Id);
        //              ViewBag.GroupId = groupId;
        //              ViewBag.CurrentUser = await _userManager.GetUserAsync(User);
        //              ViewBag.MemberName = admin.FirstName + " " + admin.LastName;
        //              ViewBag.FileName = admin.MemberFileName;
        //              ViewBag.PathName = admin.MemberPhotoPath;
        //          }
        //          else
        //          {
        //              ViewBag.GroupId = groupId;
        //              ViewBag.CurrentUser = await _userManager.GetUserAsync(User);
        //              ViewBag.MemberName = memberName;
        //              ViewBag.FileName = memberImageName;
        //              ViewBag.PathName = memberImagePath;
        //              ViewBag.image = "~/wwwroot/images/Profile";
        //              ViewBag.Status = "PrivateGroup";
        //              ViewBag.AccountId = Id;
        //          }


        //          if (Id > 0)
        //          {
        //              ViewBag.Option = "memberMessages";
        //          }
        //          else
        //          {
        //              ViewBag.Option = "userMessages";
        //          }


        //          var currentUser = await _userManager.GetUserAsync(User);
        //          if (User.Identity.IsAuthenticated)
        //          {
        //              ViewBag.CurrentUserName = currentUser.UserName;
        //          }

        //          // check if member of group or just a user
        //          status = _messageRequestService.Status(Id);

        //          // display user or member data
        //          if (status == "userMessages")
        //          {
        //              var userMessages = await _messageRequestService.UserMessages(userId.Id, managerId, groupId);
        //              return View(userMessages);
        //          }
        //          else if (status == "memberMessages")
        //          {
        //              var memberMessages = await _messageRequestService.MemberMessages(userId.Id, managerId, groupId);
        //              return View(memberMessages);
        //          }

        //          return View();
        //      }


        //      // POST: Messages/Create
        //      // To protect from overposting attacks, enable the specific properties you want to bind to.
        //      // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //      public async Task<IActionResult> Create(Message message)
        //      {

        //          {
        //              message.UserName = User.Identity.Name;
        //              var sender = await _userManager.GetUserAsync(User);
        //              message.UserID = sender.Id;
        //              await _context.Messages.AddAsync(message);
        //              await _context.SaveChangesAsync();
        //              return Ok();
        //          }
        //          return Error();
        //      }

        //      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //      public IActionResult Error()
        //      {
        //          return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //      }
        //  }
    }
}

