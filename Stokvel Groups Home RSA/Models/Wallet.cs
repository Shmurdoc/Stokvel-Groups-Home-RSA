using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class Wallet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int WalletId { get; set; }
    public decimal Amount { get; set; }

    [ForeignKey("Id")]
    public string Id { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

}
