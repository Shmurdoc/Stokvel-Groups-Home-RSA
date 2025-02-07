using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IHomeService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Interface.Messages;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Repositories.Finance;
using Stokvel_Groups_Home_RSA.Repositories.UserArea;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;
using Stokvel_Groups_Home_RSA.Services.GroupServices;
using Stokvel_Groups_Home_RSA.Services.HomeService;
using Stokvel_Groups_Home_RSA.Services.InputFolders;
using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;

namespace Stokvel_Groups_Home_RSA.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Repositories
        public IAccountProfileRepository AccountProfileRepository { get; private set; }
        public IAccountsRepository AccountsRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        public IDepositRepository DepositRepository { get; private set; }
        public IPreDepositRepository PreDepositRepository { get; private set; }
        public IGroupsRepository GroupsRepository { get; private set; }
        public IInvoicesRepository InvoicesRepository { get; private set; }
        public IInputFoldersRepository InputFoldersRepository { get; private set; }
        public IMessageRepository MessageRepository { get; private set; }
        public IPenaltyFeeRepository PenaltyFeeRepository { get; private set; }
        public IWithdrawRepository WithdrawRepository { get; private set; }
        public IWalletRepository WalletRepository { get; private set; }

       

        public PreDepositInfo PreDepositInfo { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));

            // Initialize repositories
            AccountProfileRepository = new AccountProfileRepository(_context);
            AccountsRepository = new AccountsRepository(_context);
            ApplicationUserRepository = new ApplicationUserRepository(_context);
            DepositRepository = new DepositRepository(_context);
            PreDepositRepository = new PreDepositRepository(_context);
            GroupsRepository = new GroupsRepository(_context);
            InvoicesRepository = new InvoicesRepository(_context);
            InputFoldersRepository = new InputFoldersRepository(_context, _webHostEnvironment);
            MessageRepository = new MessagesRepository(_context);
            PenaltyFeeRepository = new PenaltyFeeRepository(_context);
            WithdrawRepository = new WithdrawRepository(_context);
            WalletRepository = new WalletRepository(_context);

        }

        public IRepository<ApplicationUser> UserRepository { get; private set; }
        public IRepository<AccountProfile> AccountProfile { get; private set; }
        public IRepository<Account> Accounts { get; private set; }
        public IRepository<Group> Groups { get; private set; }
        public IRepository<PreDeposit> PreDeposit { get; private set; }
        public IRepository<Wallet> Wallet { get; private set; }
        public IRepository<Message> Message { get; private set; }

        public async Task<List<EntityEntry>> TrackAsync()
        {
            var changes = _context.ChangeTracker.Entries()
                                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted)
                                .ToList();
            return changes;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
