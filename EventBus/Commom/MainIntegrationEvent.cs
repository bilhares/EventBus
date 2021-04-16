using System;
using EventBus;

namespace Commom
{
    public class MainIntegrationEvent : IIntegrationEvent
    {
        public string Value { get; set; }
    }
}
