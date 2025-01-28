using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Models;
using System.Xml.Linq;

namespace Stokvel_Groups_Home_RSA.Repositories.UserArea
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(ApplicationUser? applicationUser)
        {
            if (applicationUser == null)
            {
                throw new ArgumentNullException(nameof(applicationUser));
            }

            var existingUser = _context.ApplicationUsers.Find(applicationUser.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Update specific fields
            existingUser.FirstName = applicationUser.FirstName;
            existingUser.LastName = applicationUser.LastName;
            existingUser.Address = applicationUser.Address;
            existingUser.City = applicationUser.City;
            existingUser.Province = applicationUser.Province;
            existingUser.Zip = applicationUser.Zip;
            existingUser.Date = applicationUser.Date;
            existingUser.AcceptedUserAccount = applicationUser.AcceptedUserAccount;
            existingUser.SecondAddress = applicationUser.SecondAddress;
            existingUser.SecondStreetAddress = applicationUser.SecondStreetAddress;
            existingUser.SecondCity = applicationUser.SecondCity;
            existingUser.SecondProvince = applicationUser.SecondProvince;
            existingUser.SecondPostalCode = applicationUser.SecondPostalCode;
            existingUser.DateOfBirth = applicationUser.DateOfBirth;
            existingUser.EmployerFirstName = applicationUser.EmployerFirstName;
            existingUser.EmployerLastName = applicationUser.EmployerLastName;
            existingUser.EmailAddress = applicationUser.EmailAddress;
            existingUser.EmployerAddress = applicationUser.EmployerAddress;
            existingUser.EmployerStreetAddress = applicationUser.EmployerStreetAddress;
            existingUser.EmployerCity = applicationUser.EmployerCity;
            existingUser.EmployerProvince = applicationUser.EmployerProvince;
            existingUser.EmployerPostalCode = applicationUser.EmployerPostalCode;
            existingUser.AnnualIncome = applicationUser.AnnualIncome;
            existingUser.RentPayment = applicationUser.RentPayment;
            existingUser.Loans = applicationUser.Loans;
            existingUser.LoanPurpose = applicationUser.LoanPurpose;
            existingUser.MemberPhotoPath = applicationUser.MemberPhotoPath;
            existingUser.MemberFileName = applicationUser.MemberFileName;
            existingUser.MemberIdPath = applicationUser.MemberIdPath;
            existingUser.MemberIdFileName = applicationUser.MemberIdFileName;
            existingUser.MemberBankStatementPath = applicationUser.MemberBankStatementPath;
            existingUser.MemberBankStatementFileName = applicationUser.MemberBankStatementFileName;
            existingUser.PhoneNumber = applicationUser.PhoneNumber;

            // Handle file uploads if any
            if (applicationUser.ProfilePicture != null)
            {
                var profilePicturePath = Path.Combine("wwwroot/images/Profile", applicationUser.ProfilePicture.FileName);
                using (var stream = new FileStream(profilePicturePath, FileMode.Create))
                {
                    await applicationUser.ProfilePicture.CopyToAsync(stream);
                }
                existingUser.MemberPhotoPath = profilePicturePath;
                existingUser.MemberFileName = applicationUser.ProfilePicture.FileName;
            }

            if (applicationUser.IdFile != null)
            {
                var idFilePath = Path.Combine("wwwroot/documents/IDs", applicationUser.IdFile.FileName);
                using (var stream = new FileStream(idFilePath, FileMode.Create))
                {
                    await applicationUser.IdFile.CopyToAsync(stream);
                }
                existingUser.MemberIdPath = idFilePath;
                existingUser.MemberIdFileName = applicationUser.IdFile.FileName;
            }

            if (applicationUser.BankStatementFile != null)
            {
                var bankStatementPath = Path.Combine("wwwroot/documents/BankStatements", applicationUser.BankStatementFile.FileName);
                using (var stream = new FileStream(bankStatementPath, FileMode.Create))
                {
                    await applicationUser.BankStatementFile.CopyToAsync(stream);
                }
                existingUser.MemberBankStatementPath = bankStatementPath;
                existingUser.MemberBankStatementFileName = applicationUser.BankStatementFile.FileName;
            }

            _context.Entry(existingUser).State = EntityState.Modified;
        }


    }
}
