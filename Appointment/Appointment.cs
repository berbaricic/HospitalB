using System;
using System.Collections.Generic;
using System.Text;

namespace AppointmentLibrary
{
    public class Appointment
    {
        public string AppointmentId { get; set; }
        public string DoctorId { get; set; }
        public string Patient { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int? RealEndTime { get; set; }
        public string AppointmentStatus { get; set; }
    }
}
