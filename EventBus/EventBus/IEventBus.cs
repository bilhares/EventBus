using System;
namespace EventBus
{
    public interface IEventBus
    {
        void Publish(IIntegrationEvent @event, string exchangeName);

        void Subscribe<TH, TE>(string exchangeName, string subscriberName)
            where TH : IIntegrationEventHandler<TE>
            where TE : IIntegrationEvent;
    }
}
