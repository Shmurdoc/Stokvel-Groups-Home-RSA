using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.Infrastructure
{
    public interface IInputFoldersRepository : IRepository<ApplicationUser>
    {
        Task UploadImage(ApplicationUser applicationUser, IFormFile uploadedImage);

    }
}
