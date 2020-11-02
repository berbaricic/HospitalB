using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQEventBus
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

    }
}
