using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class PreDeposit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PreDepositId { get; set; }
    public DateTime PreDepositDate { get; set; }
    public decimal Amount { get; set; }
    public int AccountId { get; set; }
    public virtual Account? Account { get; set; }

}
