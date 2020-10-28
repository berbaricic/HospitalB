using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQEventBus
{
    public class AppointmentDelayIntegrationEvent : IntegrationEvent
    {
        public int Delay { get; set; }
        public AppointmentDelayIntegrationEvent(int delay)
        {
            Delay = delay;
        }
    }
}
