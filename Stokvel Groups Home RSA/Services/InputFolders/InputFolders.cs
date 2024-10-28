using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.InputFolders
{


    public class InputFolders : IInputFoldersRepository
    {

        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public InputFolders(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task UploadImage(ApplicationUser? applicationUser, IFormFile? uploadedImage)
        {


            if (uploadedImage != null)
            {
                string? uniqueFileName = null;

                string? ImageUploadedFoleder = Path.Combine
                    (_webHostEnvironment.WebRootPath, "images/MemberProfile");

                uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedImage.FileName;


                string? filepath = Path.Combine(ImageUploadedFoleder, uniqueFileName);

                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    uploadedImage.CopyTo(fileStream);
                }

                applicationUser.MemberPhotoPath = "~/wwwroot/images/MemberProfile";
                applicationUser.MemberFileName = uniqueFileName;

                //await _context.applicationUser.AddAsync(applicationUser);
                //await _context.SaveChangesAsync();
            }

        }

    }
}

