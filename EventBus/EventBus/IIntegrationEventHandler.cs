using System;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IIntegrationEventHandler<in TE> where TE : IIntegrationEvent
    {
        Task HandleAsync(TE @event);
    }
}
