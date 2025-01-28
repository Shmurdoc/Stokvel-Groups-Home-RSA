using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class ApplicationUser : IdentityUser
{

    [Required(ErrorMessage = "Please Fill In Your Name")]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    [Column("FirstName")]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }


    [Required(ErrorMessage = "Please Fill In Your Lastname")]
    [Display(Name = "Last Name")]
    [StringLength(50)]
    public string? LastName { get; set; }

    [NotMapped]
    [DisplayName("Profile Picture")]
    [DataType(DataType.Upload)]
    [FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg")]
    public IFormFile? ProfilePicture { get; set; }
    public string? MemberPhotoPath { get; set; }
    public string? MemberFileName { get; set; }


    [Required(ErrorMessage = "Please Fill In Your address")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "Please Fill In Your City")]
    public string? City { get; set; }


    [Required(ErrorMessage = "Please Fill In Your Province")]
    public string? Province { get; set; }
    [Required(ErrorMessage = "Please Fill In Your Zip")]
    public int Zip { get; set; }


    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    [Display(Name = "Accepted User Account")]
    public bool AcceptedUserAccount { get; set; } = false;


    public virtual IEnumerable<Account?>? UserAccounts { get; set; }
    public virtual AccountProfile? AccountProfiles { get; set; }
    public virtual ICollection<Wallet?>? Wallets { get; set; }


    [Phone]
    public override string? PhoneNumber { get; set; } // Changed to string to accommodate phone number formats

    // Secondary Address
    public string? SecondAddress { get; set; }
    public string? SecondStreetAddress { get; set; }
    public string? SecondCity { get; set; }
    public string? SecondProvince { get; set; }
    public int? SecondPostalCode { get; set; } // Made nullable to handle optional fields


    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    // Employer Information

    [Display(Name = "Employer First Name")]
    public string? EmployerFirstName { get; set; }


    [Display(Name = "Employer Last Name")]
    public string? EmployerLastName { get; set; }


    [EmailAddress]
    [Display(Name = "Employer Email Address")]
    public string? EmailAddress { get; set; }


    [Display(Name = "Employer Address")]
    public string? EmployerAddress { get; set; }


    [Display(Name = "Employer Street Address")]
    public string? EmployerStreetAddress { get; set; }


    [Display(Name = "Employer City")]
    public string? EmployerCity { get; set; }


    [Display(Name = "Employer Province")]
    public string? EmployerProvince { get; set; }


    [Display(Name = "Employer Postal Code")]
    public string? EmployerPostalCode { get; set; }

    // Financial Information

    [Display(Name = "Annual Income")]
    public int AnnualIncome { get; set; }


    [Display(Name = "Rent Payment")]
    public decimal RentPayment { get; set; }


    public decimal Loans { get; set; }


    [Display(Name = "Loan Purpose")]
    public LoanPurpose? LoanPurpose { get; set; }

    // File Uploads
    [NotMapped]
    [Display(Name = "Upload ID File")]
    [DataType(DataType.Upload)]
    [FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg,pdf")]
    public IFormFile? IdFile { get; set; }

    public string? MemberIdPath { get; set; }
    public string? MemberIdFileName { get; set; }

    [NotMapped]
    [Display(Name = "Upload Bank Statement")]
    [DataType(DataType.Upload)]
    [FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg,pdf")]
    public IFormFile? BankStatementFile { get; set; }

    public string? MemberBankStatementPath { get; set; }
    public string? MemberBankStatementFileName { get; set; }

	public virtual ICollection<Account>? Accounts { get; set; }
	public virtual ICollection<Message>? Messages { get; set; }
}
