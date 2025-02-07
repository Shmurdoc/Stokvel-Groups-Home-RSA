using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountId { get; set; }

    [StringLength(450)] // Assuming this matches the length of ApplicationUser Id (usually a GUID or string)
    public string? Id { get; set; }

    [Required(ErrorMessage = "GroupId is required.")]
    public int GroupId { get; set; }

    [ForeignKey("GroupId")]
    public Group? Group { get; set; }

    [ForeignKey("Id")]
    public ApplicationUser? ApplicationUser { get; set; }

    [StringLength(100, ErrorMessage = "GroupVerifyKey cannot exceed 100 characters.")]
    public string? GroupVerifyKey { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Required]
    [Display(Name = "Account Created")]
    public DateTime AccountCreated { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "AccountQueue must be a non-negative number.")]
    [Display(Name = "Account Queue")]
    public int AccountQueue { get; set; }

    [Display(Name = "Account Queue Start")]
    [DataType(DataType.Date)]
    [Required]
    public DateTime AccountQueueStart { get; set; }

    [Display(Name = "Account Queue End")]
    [DataType(DataType.Date)]
    [Required]
    public DateTime AccountQueueEnd { get; set; }

    [StringLength(50, ErrorMessage = "Account number cannot exceed 50 characters.")]
    [Display(Name = "Account Number")]
    public string? AccoutNumber { get; set; }

    [StringLength(20, ErrorMessage = "Payment method cannot exceed 20 characters.")]
    [Display(Name = "Payment Method")]
    public string? PaymentMethod { get; set; }

    [Display(Name = "Is Blocked")]
    public bool Blocked { get; set; }

    [Display(Name = "Is Accepted")]
    public bool Accepted { get; set; }

    
    public PreDeposit? PreDeposit { get; set; }

    
    public virtual ICollection<Invoice>? Invoices { get; set; } = new List<Invoice>();
}
