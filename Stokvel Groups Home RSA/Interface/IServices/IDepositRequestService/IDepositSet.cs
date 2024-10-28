using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService
{
    public interface IDepositSet
    {
        Task DepositToAccountAsync(Deposit? deposit, string? description, int accountId, string? userId);
    }
}
