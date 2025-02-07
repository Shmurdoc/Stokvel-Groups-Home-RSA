using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Interface.IServices.IHomeService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
using Stokvel_Groups_Home_RSA.Interface.Messages;
using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo
{
    public interface IUnitOfWork : IDisposable
    {

        // repository section

        //user-area
        IAccountProfileRepository AccountProfileRepository { get; }
        IAccountsRepository AccountsRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        IGroupsRepository GroupsRepository { get; }
        IMessageRepository MessageRepository { get; }

        //finance
        //IBankDetailsRepository BankDetailsRepository { get; }
        IDepositRepository DepositRepository { get; }
        IPreDepositRepository PreDepositRepository { get; }
        IInvoicesRepository InvoicesRepository { get; }
        IInputFoldersRepository InputFoldersRepository { get; }
        //IPaymentLogsRepository PaymentLogsRepository { get; }
        IPenaltyFeeRepository PenaltyFeeRepository { get; }
        //IPrepaymentsRepository PrepaymentsRepository { get; }
        IWalletRepository? WalletRepository { get; }
        //IWithdrawRepository WithdrawRepository { get; }

       
        Task SaveChangesAsync();
        Task<List<EntityEntry>> TrackAsync();
        void Dispose();
    }
}
