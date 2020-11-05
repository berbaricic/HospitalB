using Hangfire;
using RabbitMQEventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangfireWorker
{
    public class HangfireJobEventSender
    {
        private readonly IEventBus eventBus;

        public HangfireJobEventSender(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }
        public void SendEvent(int delay)
        {
            IntegrationEvent appointmentDelayEvent = new AppointmentDelayIntegrationEvent(delay);
            eventBus.Publish(appointmentDelayEvent);
        }
    }
}
