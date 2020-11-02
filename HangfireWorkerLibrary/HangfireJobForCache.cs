using AppointmentLibrary;
using Hangfire;
using Newtonsoft.Json;
using RabbitMQEventBus;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangfireWorker
{
    public class HangfireJobForCache
    {
        private readonly IDatabase cache;
        private readonly IEventBus eventBus;

        public HangfireJobForCache(IEventBus eventBus)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
            cache = redis.GetDatabase();
            this.eventBus = eventBus;
        }
        [DisableConcurrentExecution(timeoutInSeconds: 15)]
        public void RedistributionJob(Appointment appointment)
        {
            int delay;
            RedisValue[] appointments = cache.SortedSetRangeByScore("SortedSet" + appointment.DoctorId, start: appointment.StartTime + 1);

            var endTimeOfLastAppointment = Convert.ToInt32(appointment.RealEndTime);

            foreach (var item in appointments)
            {
                var key = RedisStore.GetRedisKey(item);
                var redisValue = cache.StringGet(key);
                var deserializeAppointment = JsonConvert.DeserializeObject<Appointment>(redisValue);

                if (endTimeOfLastAppointment > deserializeAppointment.StartTime)
                {
                    delay = endTimeOfLastAppointment - deserializeAppointment.StartTime;
                    deserializeAppointment.EndTime = deserializeAppointment.EndTime + delay;
                    deserializeAppointment.StartTime = endTimeOfLastAppointment;
                    endTimeOfLastAppointment = deserializeAppointment.EndTime;
                    cache.StringSetAsync(key, JsonConvert.SerializeObject(deserializeAppointment));

                    IntegrationEvent appointmentDelayEvent = new AppointmentDelayIntegrationEvent(delay);
                    eventBus.Publish(appointmentDelayEvent);
                }
            }
        }
    }
}
