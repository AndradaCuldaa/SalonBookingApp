using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using SQLiteNetExtensions.Attributes;

namespace SalonBookingApp.Models
{
    public class AppointmentService
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(Appointment))]
        public int AppointmentID { get; set; }

        [ForeignKey(typeof(Service))]
        public int ServiceID { get; set; }
    }
}
