using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IHomeService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;
using System.Linq;


namespace Stokvel_Groups_Home_RSA.ViewComponents
{
    public class DailyGroupUpdatesViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public DailyGroupUpdatesViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = User.Identity.GetUserId();
            List<Deposit> displayDisplayRecentDeposit = new();
            var account = await _unitOfWork.AccountsRepository.GetAllAsync(u => u.Id == userId, includeProperties: "ApplicationUser");
            var groupId = account.Where(x => x.Accepted == true).Select(x => x.GroupId).ToList();

            if (groupId.Count == 1)
            {
                return View(null);
            }

            var memberList = await _unitOfWork.HomeRequestService.GetApplicationAccountDetailsAsync(groupId[1]);
            var groupMembers = await _unitOfWork.HomeRequestService.GetDepositDetailsAsync(groupId[1]);
            var order = groupMembers.OrderBy(x => x.DepositDate).Take(5).ToList();

            return await Task.FromResult<IViewComponentResult>(View(order.AsEnumerable().ToList())) ?? null;
        }


    }
}
