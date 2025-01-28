using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Models;


namespace Stokvel_Groups_Home_RSA.Repositories.UserArea;

public class AccountsRepository : Repository<Account>, IAccountsRepository
{
    private readonly ApplicationDbContext _context;
    public AccountsRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public bool AccountExists(string userId, string VerifyKey)
    {
        return _context.Accounts.Any(x => x.Id == userId && x.GroupVerifyKey == VerifyKey);
    }
}




