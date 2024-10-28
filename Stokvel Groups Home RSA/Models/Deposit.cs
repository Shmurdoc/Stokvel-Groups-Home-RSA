using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class Deposit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DepositId { get; set; }



    public decimal DepositAmount { get; set; }

    public DateTime DepositDate { get; set; }




    [StringLength(50)]
    public string? DepositReference { get; set; }


    public virtual ICollection<Invoice>? Invoices { get; set; }
    public virtual ICollection<DepositLog>? PaymentLogs { get; set; }

    public virtual ICollection<BankDetails>? BankDetails { get; set; }


}
