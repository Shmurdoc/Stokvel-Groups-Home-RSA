using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class Account
{

    
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        public string? Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group? Group { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser? ApplicationUser { get; set; }

        public string? GroupVerifyKey { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AccountCreated { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "AccountQueue must be a non-negative number.")]
        public int AccountQueue { get; set; }

        [Display(Name = "Account Queue Start")]
        public DateTime AccountQueueStart { get; set; }

        [Display(Name = "Account Queue End")]
        public DateTime AccountQueueEnd { get; set; }

        public string? AccoutNumber { get; set; }
        public string? PaymentMethod { get; set; }
    
        [Display(Name = "Is Blocked")]
        public bool Blocked { get; set; }

        [Display(Name = "Is Accepted")]
        public bool Accepted { get; set; }

        public PreDeposit? PreDeposit { get; set; }

        public virtual ICollection<Invoice>? Invoices { get; set; } = null;
    

}
