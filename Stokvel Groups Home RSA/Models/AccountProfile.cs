
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class AccountProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountProfileId { get; set; }

    public string? Id { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }
    public int GroupsJoined { get; set; }
    public int GroupsLeft { get; set; }
    public int EmergencyCancel { get; set; }
    public MemberStatuses? StatusRank { get; set; }
    public int MembershipRank { get; set; }

    public decimal TotalAmountDeposited { get; set; }

    public decimal TotalPenaltyFee { get; set; }

    public int GroupWarnings { get; set; }
}
