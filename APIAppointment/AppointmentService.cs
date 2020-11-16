using APIAppointment.Models;
using AppointmentLibrary;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIAppointment
{
    public class AppointmentService
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AppointmentService(IHospitalBDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _appointments = database.GetCollection<Appointment>(settings.AppointmentsCollectionName);
        }

        public void Create(Appointment appointment)
        {
            _appointments.InsertOne(appointment);
        }

        public void Update(string id, Appointment appointment)
        {
            _appointments.ReplaceOne(app => app.AppointmentId == id, appointment);
        }

        public void Remove(Appointment appointment)
        {
            _appointments.DeleteOne(app => app.AppointmentId == appointment.AppointmentId);
        }

        public Appointment Get(string id) =>
            _appointments.Find<Appointment>(app => app.AppointmentId == id).FirstOrDefault();

        public List<Appointment> Get() =>
            _appointments.Find(app => true).ToList();
    }
}
