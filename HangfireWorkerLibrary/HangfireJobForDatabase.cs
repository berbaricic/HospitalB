using AppointmentLibrary;
using Dapper;
using HangfireWorker.SQLDatabase;
using Newtonsoft.Json;
using RabbitMQEventBus;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HangfireWorker
{
    public class HangfireJobForDatabase
    {
        private readonly ISqlDatabaseConnection sqlDatabase;

        public HangfireJobForDatabase(ISqlDatabaseConnection sqlDatabase)
        {
            this.sqlDatabase = sqlDatabase;
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
