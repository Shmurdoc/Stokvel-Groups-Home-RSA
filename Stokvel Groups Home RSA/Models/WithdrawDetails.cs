using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class WithdrawDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DetailedId { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public string? AccoutNumber { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
    public decimal CreditAmount { get; set; }
    public int TaxID { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? CreditedDate { get; set; }
    public int PaymentId { get; set; }


}
