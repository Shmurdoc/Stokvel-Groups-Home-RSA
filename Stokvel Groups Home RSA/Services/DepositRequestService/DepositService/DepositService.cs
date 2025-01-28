using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;
using System.Text.RegularExpressions;

namespace Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositService;

public class DepositService : IDepositService
{
    public IDepositRequestServices? DepositRequestServices { get; private set; }
    private readonly IUnitOfWork? _unitOfWork;
    private readonly IDepositSet? _depositSet;
    private readonly IPreDepositRequestServices _preDepositRequestServices;
    private readonly IWalletRequestServices _walletRequestServices;

    public DepositService(IDepositSet depositSet, IUnitOfWork unitOfWork, IPreDepositRequestServices preDepositRequestServices, IWalletRequestServices walletRequestServices)
    {
        _unitOfWork = unitOfWork;
        _depositSet = depositSet;
        _preDepositRequestServices = preDepositRequestServices;
        _walletRequestServices = walletRequestServices;
    }

    public async Task DepositRequestAsync(Deposit deposit, string? description, int accountId,string? userId, string? dropdownValue)
    {
        DepositRequestServices = new DepositRequest(_depositSet);
        await DepositRequestServices.DepositAsync(deposit, description, accountId, userId, dropdownValue);
    }

    public async Task PreDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue)
    {
        DepositRequestServices = new PreDepositDepositRequest(_preDepositRequestServices, _depositSet, _unitOfWork);
        await DepositRequestServices.DepositAsync(deposit, description, accountId, userId, dropdownValue);
    }

    public async Task WalletDepositRequestAsync(Deposit deposit, string? description, int accountId, string? userId, string? dropdownValue)
    {
        DepositRequestServices = new WalletDepositRequest(_unitOfWork, _depositSet, _walletRequestServices);
        await DepositRequestServices.DepositAsync(deposit, description, accountId, userId, dropdownValue);
    }

}
