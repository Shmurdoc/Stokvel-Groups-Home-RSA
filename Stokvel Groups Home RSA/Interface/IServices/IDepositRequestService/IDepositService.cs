using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService
{
    public interface IDepositService
    {
        Task DepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId);
        Task PreDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId);
        Task WalletDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId);
    }
}
