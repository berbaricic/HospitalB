using AppointmentLibrary;
using Dapper;
using Hangfire;
using HangfireWorker.SQLDatabase;
using Newtonsoft.Json;
using RabbitMQEventBus;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HangfireWorker
{
    public class HangfireJobForCache
    {
        private readonly IDatabase cache;

        public HangfireJobForCache(IDatabase cache)
        {
            this.cache = cache;
        }

        public void RedistributionJob()
        {
            int timestampCheck;
            int delayOfFirstAppointment;
            int delayToSend;

            RedisValue[] doctors = cache.SetMembers("DoctorsList");
            timestampCheck = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            foreach (var doctor in doctors)
            {
                RedisValue[] appointments = cache.SortedSetRangeByScore("SortedSet" + doctor);
                var first = appointments.First();

                var keyFirst = RedisStore.GetRedisKey(doctor, first);
                var redisValueFirst = cache.StringGet(keyFirst);
                var firstAppointment = JsonConvert.DeserializeObject<Appointment>(redisValueFirst);

                delayOfFirstAppointment = timestampCheck - firstAppointment.EndTime;
                
                //ako je kašnjenje veće od 10 minuta --> za test ide 1 minuta
                if (delayOfFirstAppointment > 60)
                {
                    RedisValue[] appointmentsWithoutFirst = cache.SortedSetRangeByScore("SortedSet" + doctor, start: firstAppointment.StartTime + 1);

                    foreach (var appointment in appointmentsWithoutFirst)
                    {
                        var keyAnother = RedisStore.GetRedisKey(doctor, appointment);
                        var redisValueAnother = cache.StringGet(keyAnother);
                        var deserializeAppointment = JsonConvert.DeserializeObject<Appointment>(redisValueAnother);

                        if (timestampCheck > deserializeAppointment.StartTime)
                        {
                            delayToSend = timestampCheck - deserializeAppointment.StartTime;
                            deserializeAppointment.StartTime = timestampCheck;
                            deserializeAppointment.EndTime = deserializeAppointment.EndTime + delayToSend;
                            timestampCheck = deserializeAppointment.EndTime;

                            cache.SortedSetAdd("SortedSet" + doctor, appointment, deserializeAppointment.StartTime);
                            cache.StringSetAsync(keyAnother, JsonConvert.SerializeObject(deserializeAppointment));

                            BackgroundJob.Enqueue<HangfireJobEventSender>(worker => worker.SendEvent(delayToSend));
                        }
                    }
                }
            }
        }
    }
}
