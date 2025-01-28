using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IRepo.Finance;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet;

namespace Stokvel_Groups_Home_RSA.Services.WithdrawServices;

public class WithdrawServices : IWithdrawServices
{

    private readonly IUnitOfWork _unitOfWork;

    public WithdrawServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task PenaltiesAsync(List<Account> overdueMembers, decimal groupTargetAmount)
    {
        foreach (var member in overdueMembers)
        {
            // Apply penalty logic, assuming a fine of 50 per overdue month
            var penaltyAmount = await CalculateTotalAmountAsync(member, groupTargetAmount);

            var invoice = new Invoice
            {
                InvoiceDate = DateTime.Now,
                TotalAmount = -penaltyAmount,
                Description = "Penalty for overdue month",
            };

            await _unitOfWork.InvoicesRepository.Add(invoice);
            await _unitOfWork.SaveChangesAsync();

            var penalty = new PenaltyFee
            {
                InvoiceId = invoice.InvoiceId,
                PenaltyAmount = penaltyAmount,
                PenaltyDate = DateTime.Now,
                PenaltyLevel = "Overdue",
            };

            await _unitOfWork.PenaltyFeeRepository.Add(penalty);
            await _unitOfWork.SaveChangesAsync();
        }
    }


    public async Task<decimal> CalculateTotalAmountAsync(Account member, decimal groupTargetAmount)
    {
        var differance = groupTargetAmount - member.Invoices.Sum(x => x.TotalAmount);

        // Calculate the 18% penalty
        var penalty = differance * 0.18m;

        // Add the penalty to the difference
        var result = differance + penalty;

        return result;
    }

}