namespace Stokvel_Groups_Home_RSA.Models.GroupedTables
{
    public class DepositToAccount
    {
        public PreDeposit? PreDeposit { get; set; }
        public Account? Account { get; set; }
        public List<Invoice?>? Invoice { get; set; }
        public List<Deposit?>? Deposit { get; set; }
    }
}
