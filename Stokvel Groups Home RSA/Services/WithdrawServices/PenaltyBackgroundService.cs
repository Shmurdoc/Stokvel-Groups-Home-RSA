using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices
{
    public class PenaltyBackgroundService : BackgroundService
    {
        private readonly IWithdrawRequestService _withdrawRequestService;
        private readonly ILogger<PenaltyBackgroundService> _logger;
        private readonly TimeSpan _delay;

        public PenaltyBackgroundService(IWithdrawRequestService withdrawRequestService, ILogger<PenaltyBackgroundService> logger)
        {
            _withdrawRequestService = withdrawRequestService;
            _logger = logger;
            // Configure delay interval (could be injected from configuration)
            _delay = TimeSpan.FromDays(1); // Default: Run daily
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PenaltyBackgroundService starting.");

            // Run the task until cancellation is requested
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Executing ApplyPenaltiesAsync...");

                    // Call the service method to apply penalties
                    await _withdrawRequestService.ApplyPenaltiesAsync();

                    _logger.LogInformation("Penalties applied successfully.");
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the task execution
                    _logger.LogError(ex, "An error occurred while applying penalties.");
                }

                // Wait for the next execution cycle or cancellation
                try
                {
                    await Task.Delay(_delay, stoppingToken);  // Run based on the configured delay
                }
                catch (TaskCanceledException)
                {
                    // This will be triggered when the task is canceled
                    _logger.LogInformation("PenaltyBackgroundService has been canceled.");
                    break;
                }
            }

            _logger.LogInformation("PenaltyBackgroundService has stopped.");
        }
    }
}
