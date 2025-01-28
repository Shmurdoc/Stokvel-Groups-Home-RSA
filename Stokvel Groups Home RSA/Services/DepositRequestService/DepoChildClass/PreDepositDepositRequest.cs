

using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass
{
    public class PreDepositDepositRequest : IDepositRequestServices
    {
        private readonly IPreDepositRequestServices _preDepositRequestServices;
        private readonly IDepositSet _depositSet;
        private IUnitOfWork _unitOfWork;

        public PreDepositDepositRequest(IPreDepositRequestServices preDepositRequestServices, IDepositSet depositSet, IUnitOfWork unitOfWork)
        {
            _preDepositRequestServices = preDepositRequestServices;
            _depositSet = depositSet;
            _unitOfWork = unitOfWork;
        }

        public async Task DepositAsync(Deposit? deposit, string? description, int accountId, string? userId, string? dropdownValue)
        {
            if (deposit == null)
            {
                throw new ArgumentNullException(nameof(deposit));
            }

            await _depositSet.DepositToAccountAsync(deposit, description, accountId, userId, dropdownValue);
            await _preDepositRequestServices.UpdatePreDepositAsync(deposit, accountId);
        }
    }

}
