//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Stokvel_Groups_Home.Interface.IServices.IHomeService;

//namespace Stokvel_Groups_Home.ViewComponents
//{
//    public class MemberQueueDatesViewComponent : ViewComponent
//    {
//        private readonly IHomeRequestService _homeRequestService;
//        public MemberQueueDatesViewComponent(IHomeRequestService homeRequestService)
//        {
//            _homeRequestService = homeRequestService;
//        }

//        public async Task<IViewComponentResult> InvokeAsync()
//        {
//            var userId = User.Identity.GetUserId();
//            var groupId = await _homeRequestService.MemberAccountGroupId(userId);

//            var dateToNextInQueue = await _homeRequestService.MemberQueueList();
//            if (groupId.Count > 1)
//            {
//                var NextInQueue = dateToNextInQueue.Where(x => x.GroupMembers.GroupId == groupId[1]).DistinctBy(x => x.AccountUser.Id).ToList();

//                return await Task.FromResult<IViewComponentResult>(View(dateToNextInQueue));
//            }
//            return View(null);
//        }
//    }
//}
