using System.Threading.Tasks;
using Commom;
using EventBus;
using Microsoft.Extensions.Logging;

namespace ChildService.IntegrationEvent
{
    public class MainIntegrationEventHandler : IIntegrationEventHandler<MainIntegrationEvent>
    {
        private readonly ILogger<MainIntegrationEventHandler> _logger;

        public MainIntegrationEventHandler(ILogger<MainIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(MainIntegrationEvent @event)
        {
            _logger.LogInformation($" Accepted event: {@event.Value} ");
            return Task.CompletedTask;
        }
    }
}