using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class PenaltyFee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PenaltyFeeId { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public DateTime PenaltyDate { get; set; }
    public decimal PenaltyAmount { get; set; }
    public string? PenaltyLevel { get; set; }

}
