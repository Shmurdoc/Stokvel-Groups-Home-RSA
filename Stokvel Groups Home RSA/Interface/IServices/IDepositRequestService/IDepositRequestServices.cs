using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService
{
    public interface IDepositRequestServices
    {
        //DepositToAccount? PreDepoMembers(int accountId);
        //decimal PreDepoTarget(int groupId);
        Task DepositAsync(Deposit deposit, string description, int accountId, string? userId);

    }

}
