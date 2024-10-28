using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo
{
    public interface IUnitOfWork : IDisposable
    {

        // repository section

        //user-area
        IAccountUserPersonalRepository AccountUserPersonalRepository { get; }
        IAccountProfileRepository AccountProfileRepository { get; }
        IAccountsRepository AccountsRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        IGroupsRepository GroupsRepository { get; }
        //IMessageRepository MessageRepository { get; }

        //  //finance
        //IBankDetailsRepository BankDetailsRepository { get; }
        IDepositRepository DepositRepository { get; }
        IDepositRequestServices DepositRequestServices { get; }
        IPreDepositRepository PreDepositRepository { get; }
        IInvoicesRepository InvoicesRepository { get; }
        //IPaymentLogsRepository PaymentLogsRepository { get; }
        //IPenaltyFeeRepository PenaltyFeeRepository { get; }
        //IPrepaymentsRepository PrepaymentsRepository { get; }
        IWalletRepository? WalletRepository { get; }
        //IWithdrawRepository WithdrawRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
