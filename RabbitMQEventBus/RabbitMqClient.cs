using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQEventBus
{
    public class RabbitMqClient : IEventBus
    {
        public RabbitMqClient()
        {

        }
        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
