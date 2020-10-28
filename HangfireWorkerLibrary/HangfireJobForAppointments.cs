using AppointmentLibrary;
using Dapper;
using HangfireWorker.SQLDatabase;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HangfireWorker
{
    public class HangfireJobForAppointments
    {
        private readonly IDatabase cache;
        private readonly ISqlDatabaseConnection sqlDatabase;

        public HangfireJobForAppointments(ISqlDatabaseConnection sqlDatabase)
        {
            this.sqlDatabase = sqlDatabase;
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
            cache = redis.GetDatabase();
        }

        public void RedistributionJob(Appointment appointment)
        {
            RedisValue[] appointments = cache.SortedSetRangeByScore("SortedSet" + appointment.DoctorId, start: appointment.StartTime + 1);

            var endTimeOfLastAppointment = Convert.ToInt32(appointment.RealEndTime);

            foreach (var item in appointments)
            {
                var key = RedisStore.GetRedisKey(item);
                var redisValue = cache.StringGet(key);
                var deserializeAppointment = JsonConvert.DeserializeObject<Appointment>(redisValue);

                if (endTimeOfLastAppointment > deserializeAppointment.StartTime)
                {
                    deserializeAppointment.EndTime = deserializeAppointment.EndTime + (endTimeOfLastAppointment - deserializeAppointment.StartTime);
                    deserializeAppointment.StartTime = endTimeOfLastAppointment;
                    endTimeOfLastAppointment = deserializeAppointment.EndTime;
                    cache.StringSetAsync(key, JsonConvert.SerializeObject(deserializeAppointment));                    
                }                
            }
        }

        public void PersistDataToDatabaseJob(Appointment appointment)
        {
            string sQuery = "PersistAppointment_StoredProcedure";
            DynamicParameters param = new DynamicParameters();
            param.Add("@AppointmentId", appointment.AppointmentId);
            param.Add("@DoctorId", appointment.DoctorId);
            param.Add("@Patient", appointment.Patient);
            param.Add("@StartTime", appointment.StartTime);
            param.Add("@EndTime", appointment.EndTime);
            param.Add("@RealEndTime", appointment.RealEndTime);
            param.Add("@AppointmentStatus", appointment.AppointmentStatus);

            using (IDbConnection db = this.sqlDatabase.CreateConnection())
            {
                db.QueryAsync<Appointment>(sQuery, param, commandType: CommandType.StoredProcedure);
            }
        }

    }
}
