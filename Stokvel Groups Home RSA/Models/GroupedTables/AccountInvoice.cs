namespace Stokvel_Groups_Home_RSA.Models.GroupedTables
{
    public class AccountInvoice
    {
        public List<Deposit?>? Deposit { get; set; }
        public List<Invoice?>? Invoice { get; set; }
        public List<Account?>? Account { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
