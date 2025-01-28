using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models;


namespace Stokvel_Groups_Home_RSA.ViewComponents
{
    public class GroupChatMemberViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;

		public GroupChatMemberViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
            int? groupId = TempData["groupId"] as int?;
            TempData.Keep("groupId");

			var membersInGroup = await _unitOfWork.MessageRepository.GetAllAsync();
			var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
			List<ApplicationUser> applicationUsers = new();

			var j = _unitOfWork.GroupsRepository.GetList().Where(x => x.GroupId == groupId);
			var groupMessage = j
				.Include(a => a.Accounts)
				.ThenInclude(au => au.ApplicationUser)
				.ThenInclude(m=>m.Messages)
			.ToList();

			 applicationUsers.AddRange(groupMessage.Where(x => x.GroupId == groupId).SelectMany(x => x.Accounts?.Select(x => x.ApplicationUser)).ToList());

			var groupManager = membersInGroup.Where(g=>g.Group == groupId.ToString()).Select(x => x.UserID).FirstOrDefault();

			if (groupManager == group.ManagerId)
			{
				ViewBag.GroupManager = groupManager;
			}
			ViewBag.image = "/wwwroot/images/Profile";


			if (groupId > 0)
			{
				return await Task.FromResult<IViewComponentResult>(View(applicationUsers));
			}
			return View();
		}


	}
}
