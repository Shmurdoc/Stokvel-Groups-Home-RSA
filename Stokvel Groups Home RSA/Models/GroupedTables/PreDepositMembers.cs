namespace Stokvel_Groups_Home_RSA.Models.GroupedTables
{
    public class PreDepositMembers
    {
        public List<ApplicationUser?>? ApplicationUser { get; set; }
        public List<Account?>? Account { get; set; }
        public List<PreDeposit?>? PreDeposit { get; set; }
    }
}
