using System;
using System.Collections.Generic;
using AppointmentLibrary;
using Hangfire;
using HangfireWorker;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace APIAppointment.Controllers
{
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IDatabase cache;
        private readonly AppointmentService appointmentService;

        public AppointmentController(IDatabase cache, AppointmentService appointmentService)
        {
            this.cache = cache;
            this.appointmentService = appointmentService;
        }

        // GET: /appointment/doctor/doktor1
        [HttpGet("appointment/doctor/{doctorId}")]
        public ActionResult<IEnumerable<Appointment>> GetAllAppointment(string doctorId)
        {
            List<Appointment> appointments = new List<Appointment>();
            RedisValue[] allAppointments = cache.SortedSetRangeByScore("SortedSet" + doctorId);
            foreach (var appointment in allAppointments)
            {
                var key = RedisStore.GetRedisKey(doctorId, appointment);
                var redisValue = cache.StringGet(key);
                var desAppointment = JsonConvert.DeserializeObject<Appointment>(redisValue);
                appointments.Add(desAppointment);
            }
            return appointments;
        }

        // GET: doctor/doktor1/appointment/pregled1
        [HttpGet("doctor/{doctorId}/appointment/{id}")]
        public ActionResult<Appointment> Get(string doctorId, string id)
        {
            var key = RedisStore.GetRedisKey(doctorId, id);
            //save object in Key-Value pairs and SortedSet
            var redisValue = cache.StringGet(key);
            var appointment = JsonConvert.DeserializeObject<Appointment>(redisValue);

            return appointment;
        }

        // POST: /appointment
        [HttpPost("appointment")]
        public void Post([FromBody]Appointment appointment)
        {
            var key = RedisStore.GetRedisKey(appointment.DoctorId, appointment.AppointmentId);

            cache.StringSet(key, JsonConvert.SerializeObject(appointment));
            cache.SortedSetAddAsync("SortedSet" + appointment.DoctorId, appointment.AppointmentId, appointment.StartTime);
            cache.SetAdd("DoctorsList", appointment.DoctorId);
        }

        // PUT: doctor/doktor1/appointment/pregled1
        [HttpPut("doctor/{doctorId}/appointment/{id}")]
        public void Put(string doctorId, string id, [FromBody]Appointment appointment)
        {
            var key = RedisStore.GetRedisKey(doctorId, id);

            cache.StringSet(key, JsonConvert.SerializeObject(appointment));
            cache.SortedSetAddAsync("SortedSet" + appointment.DoctorId, appointment.AppointmentId, appointment.StartTime);
            cache.SetAdd("DoctorsList", appointment.DoctorId);

            if (appointment.RealEndTime != null && appointment.AppointmentStatus == "DONE")
            {
                //brisanje termina iz cache-a
                cache.KeyDelete(key);
                cache.SortedSetRemove("SortedSet" + appointment.DoctorId, id);
                if (!cache.KeyExists("SortedSet:" + appointment.DoctorId))
                {
                    cache.SetRemove("DoctorsList", appointment.DoctorId);
                }
                //perzistencija termina u bazu nakon što su obavljeni
                BackgroundJob.Enqueue<HangfireJobForDatabase>(worker => worker.PersistDataToDatabaseJob(appointment));
                //perzistencija termina u NoSQL bazu
                appointmentService.Create(appointment);
            }
        }

        // DELETE: doctor/doktor1/appointment/pregled1
        [HttpDelete("doctor/{doctorId}/appointment/{id}")]
        public void Delete(string doctorId, string id)
        {
            var key = RedisStore.GetRedisKey(doctorId, id);
            cache.KeyDelete(key);
            cache.SortedSetRemove("SortedSet" + doctorId, id);
        }

        //DELETE: doctor/doktor1
        [HttpDelete("doctor/{doctorId}")]
        public void Delete(string doctorId)
        {
            List<Appointment> appointments = new List<Appointment>();
            RedisValue[] allAppointments = cache.SortedSetRangeByScore("SortedSet" + doctorId);
            foreach (var appointment in allAppointments)
            {
                var key = RedisStore.GetRedisKey(doctorId, appointment);
                cache.SortedSetRemove("SortedSet" + doctorId, appointment);
                cache.SetRemove("DoctorsList", doctorId);
                cache.KeyDelete(key);
            }


        }
    }
}
