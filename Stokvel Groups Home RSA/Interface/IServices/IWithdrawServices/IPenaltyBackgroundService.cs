namespace Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices
{
    public class PenaltyBackgroundService : BackgroundService
    {
        private readonly IWithdrawRequestService _withdrawRequestService;

        public PenaltyBackgroundService(IWithdrawRequestService withdrawRequestService)
        {
            _withdrawRequestService = withdrawRequestService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
              await _withdrawRequestService.ApplyPenaltiesAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);  // Run daily
            }
        }
    }

}
