using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class Account
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountId { get; set; }

    public string? Id { get; set; }

    public int GroupId { get; set; }

    public Group? Group { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }


    public string? GroupVerifyKey { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime AccountCreated { get; set; }

    public int AccountQueue { get; set; }

    public DateTime AccountQueueStart { get; set; }

    public DateTime AccountQueueEnd { get; set; }
    public bool Blocked { get; set; }
    public bool Accepted { get; set; }

    public PreDeposit? PreDeposit { get; set; }








    public virtual ICollection<Invoice>? Invoices { get; set; } = null;
}
