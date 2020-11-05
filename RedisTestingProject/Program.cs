using StackExchange.Redis;
using System;
using AppointmentLibrary;
using Newtonsoft.Json;

namespace RedisTestingProject
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
            IDatabase cache = redis.GetDatabase();

            Appointment appointment;

            int startTime = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            for (int d = 1; d <= 100; d++)
            {
                for (int a = 1; a <= 15; a++)
                {
                    if (a != 7)
                    {
                        appointment = new Appointment();

                        appointment.AppointmentId = "Pregled" + a.ToString();
                        appointment.DoctorId = "Doktor" + d.ToString();
                        appointment.Patient = "Pacijent" + a.ToString() + " - " + appointment.DoctorId;
                        appointment.StartTime = startTime;
                        appointment.EndTime = startTime + 180;

                        var key = RedisStore.GetRedisKey(appointment.DoctorId, appointment.AppointmentId);

                        cache.StringSet(key, JsonConvert.SerializeObject(appointment));
                        cache.SortedSetAddAsync("SortedSet" + appointment.DoctorId, appointment.AppointmentId, appointment.StartTime);
                        cache.SetAdd("DoctorsList", appointment.DoctorId);

                        startTime = appointment.EndTime;
                    }
                    else
                    {
                        startTime += 180;
                    }
                }
            }
        }
    }
}
