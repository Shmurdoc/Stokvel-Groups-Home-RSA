namespace Stokvel_Groups_Home_RSA.Models.GroupedTables
{
    public class AccountPreDeposit
    {
        public AccountProfile? AccountProfile { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public Account? Account { get; set; }
        public PreDeposit? PreDeposit { get; set; }

    }
}
