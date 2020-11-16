using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIAppointment.Models
{
    public interface IHospitalBDatabaseSettings
    {
        string AppointmentsCollectionName { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
