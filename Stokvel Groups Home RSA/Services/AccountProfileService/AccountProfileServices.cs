using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
using Stokvel_Groups_Home_RSA.Models.GroupedTables;

namespace Stokvel_Groups_Home_RSA.Services.AccountProfileService
{
    public class AccountProfileServices : IAccountProfileRequestServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountProfileServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUserAccountProfile> AccountProfileInfoAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty", nameof(id));
            }

            var member = _unitOfWork.ApplicationUserRepository.GetList().Where(x => x.Id == id);

            var applicationUser = await member.FirstOrDefaultAsync();
            var accountProfile = await member.Select(x => x.AccountProfiles).FirstOrDefaultAsync();

            if (applicationUser == null || accountProfile == null)
            {
                return null;
            }

            return new ApplicationUserAccountProfile
            {
                ApplicationUser = applicationUser,
                AccountProfile = accountProfile
            };
        }


    }

}
