using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Stokvel_Groups_Home_RSA.Interface.IRepo;

namespace Stokvel_Groups_Home_RSA.ViewComponents
{
    public class MiniImageViewComponent : ViewComponent
    {
        private IUnitOfWork _unitOfWork;
        public MiniImageViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = User.Identity.GetUserId();

            ViewBag.image = "~/wwwroot/images/Profile";

            //use unitOfWork to get data
            var profilePic = await _unitOfWork.ApplicationUserRepository.GetAllAsync(x => x.Id == userId);

            return await Task.FromResult<IViewComponentResult>(View(profilePic));
        }
    }
}
