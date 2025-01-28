using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class Invoice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InvoiceId { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }

    public int DepositId { get; set; }

    public Deposit? Deposit { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? InvoiceDate { get; set; }

    public string? Description { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual ICollection<PenaltyFee>? PenaltyFees { get; set; }
    public virtual ICollection<WithdrawDetails>? WithdrawDetails { get; set; }
}
