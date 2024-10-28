using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Repositories.Finance;
using Stokvel_Groups_Home_RSA.Repositories.UserArea;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;

namespace Stokvel_Groups_Home_RSA.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private ApplicationDbContext _context;

        //repositories
        public IAccountProfileRepository AccountProfileRepository { get; private set; }
        public IAccountsRepository AccountsRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        //public IBankDetailsRepository BankDetailsRepository { get; private set; }

        public IDepositRepository DepositRepository { get; private set; }
        public IAccountUserPersonalRepository AccountUserPersonalRepository { get; private set; }

        public IPreDepositRepository PreDepositRepository { get; private set; }
        public IGroupsRepository GroupsRepository { get; private set; }
        public IInvoicesRepository InvoicesRepository { get; private set; }

        //public IMessageRepository MessageRepository { get; private set; }
        //public IPaymentLogsRepository PaymentLogsRepository { get; private set; }
        //public IPrepaymentsRepository PrepaymentsRepository { get; private set; }
        //public IPenaltyFeeRepository PenaltyFeeRepository { get; private set; }
        //public IWithdrawRepository WithdrawRepository { get; private set; }
        public IWalletRepository WalletRepository { get; private set; }


        //service area
        public IPreDepositRequestServices PreDepositRequestServices { get; private set; }
        public IDepositRequestServices DepositRequestServices { get; private set; }
        public IDepositRequestServices PreDepositDepositRequestServices { get; private set; }
        public IDepositRequestServices WalletDepositRequestServices { get; private set; }
        public IDepositSet DepositSet { get; private set; }
        public IWalletRequestServices WalletRequestServices { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {

            _context = context;
            ////repository
            AccountProfileRepository = new AccountProfileRepository(context);
            AccountsRepository = new AccountsRepository(context);
            ApplicationUserRepository = new ApplicationUserRepository(context);
            DepositRepository = new DepositRepository(context);

            PreDepositRepository = new PreDepositRepository(context);

            //GroupMembersRepository = new GroupMembersRepository(context);
            GroupsRepository = new GroupsRepository(context);
            InvoicesRepository = new InvoicesRepository(context);
            //MemberInvoiceRepository = new MemberInvoiceRepository(context);
            //MessageRepository = new MessagesRepository(context);
            //PaymentMethodsRepository = new PaymentMethodsRepository(context);
            //PaymentStatusRepository = new PaymentStatusRepository(context);
            //PenaltyFeeRepository = new PenaltyFeeRepository(context);
            //PrepaymentsRepository = new PrepaymentsRepository(context);
            //WithdrawRepository = new WithdrawRepository(context);
            WalletRepository = new WalletRepository(context);

            //Deposit Service 
            DepositRequestServices = new DepositRequest(DepositSet);
            PreDepositDepositRequestServices = new PreDepositDepositRequest(PreDepositRequestServices, DepositSet, this);
            WalletDepositRequestServices = new WalletDepositRequest(this, DepositSet, WalletRequestServices);
        }

        public IRepository<AccountProfile> AccountProfile { get; private set; }
        public IRepository<ApplicationUser> ApplicationUser { get; private set; }
        public IRepository<Account> Accounts { get; private set; }
        public IRepository<Group> Groups { get; private set; }
        public IRepository<PreDeposit> PreDeposit { get; private set; }
        public IRepository<Wallet> Wallet { get; private set; }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
