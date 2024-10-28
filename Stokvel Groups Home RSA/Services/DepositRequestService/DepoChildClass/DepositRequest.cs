using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass
{
    public class DepositRequest : IDepositRequestServices
    {
        private readonly IDepositSet? _depositSet;

        public DepositRequest(IDepositSet? depositSet)
        {
            _depositSet = depositSet;
        }


        public async Task DepositAsync(Deposit deposit, string description, int accountId, string? userId)
        {
            await _depositSet?.DepositToAccountAsync(deposit, description, accountId, userId);
        }
    }
}
