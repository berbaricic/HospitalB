using System.Collections.Generic;
using AppointmentLibrary;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IDatabase cache;

        public AppointmentController(IDatabase cache)
        {
            this.cache = cache;
        }

        // GET: /appointment/doctor/doktor1
        [HttpGet("appointment/doctor/{doctorId}")]
        public ActionResult<IEnumerable<Appointment>> GetAllAppointment(string doctorId)
        {
            List<Appointment> appointments = new List<Appointment>();
            RedisValue[] allAppointments = cache.SortedSetRangeByScore("SortedSet" + doctorId);
            foreach (var item in allAppointments)
            {
                var key = RedisStore.GetRedisKey(item);
                var redisValue = cache.StringGet(key);
                var appointment = JsonConvert.DeserializeObject<Appointment>(redisValue);
                appointments.Add(appointment);
            }
            return appointments;
        }

        // GET: /appointment/pregled1
        [HttpGet("appointment/{id}")]
        public ActionResult<Appointment> Get(string id)
        {
            var key = RedisStore.GetRedisKey(id);
            //save object in Key-Value pairs and SortedSet
            var redisValue = cache.StringGet(key);
            var appointment = JsonConvert.DeserializeObject<Appointment>(redisValue);

            return appointment;
        }

        // POST: /appointment
        [HttpPost("appointment")]
        public void Post([FromBody]Appointment appointment)
        {
            var key = RedisStore.GetRedisKey(appointment.AppointmentId);
            //save object in Key-Value pairs and SortedSet
            cache.StringSetAsync(key, JsonConvert.SerializeObject(appointment));
            cache.SortedSetAddAsync("SortedSet" + appointment.DoctorId, appointment.AppointmentId, appointment.StartTime);
        }

        // PUT: /appointment/pregled1
        [HttpPut("appointment/{id}")]
        public void Put(string id, [FromBody]Appointment appointment)
        {
            var key = RedisStore.GetRedisKey(id);
            //save object in Key-Value pairs and SortedSet
            cache.StringSetAsync(key, JsonConvert.SerializeObject(appointment));
            cache.SortedSetAddAsync("SortedSet" + appointment.DoctorId, appointment.AppointmentId, appointment.StartTime);
        }

        // DELETE: /appointment/pregled1/doctor/doktor1
        [HttpDelete("appointment/{id}/doctor/{doctorId}")]
        public void Delete(string id, string doctorId)
        {
            var key = RedisStore.GetRedisKey(id);
            cache.KeyDelete(key);

            cache.SortedSetRemove("SortedSet" + doctorId, id);
        }

        [HttpDelete("doctor/{doctorId}")]
        public void Delete(string doctorId)
        {
            List<Appointment> appointments = new List<Appointment>();
            RedisValue[] allAppointments = cache.SortedSetRangeByScore("SortedSet" + doctorId);
            foreach (var item in allAppointments)
            {
                var key = RedisStore.GetRedisKey(item);
                cache.KeyDelete(key);
                cache.SortedSetRemove("SortedSet" + doctorId, item);
            }

 
        }
    }
}
