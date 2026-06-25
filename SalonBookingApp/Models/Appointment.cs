using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SalonBookingApp.Models
{
    public class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data și Ora Programării")]
        public DateTime AppointmentDate { get; set; }

       
        [ForeignKey(typeof(Client))]
        public int ClientID { get; set; }

        [ManyToOne]
        public Client Client { get; set; }

        
        [ForeignKey(typeof(Stylist))]
        public int StylistID { get; set; }

        [ManyToOne]
        public Stylist Stylist { get; set; }

        [ForeignKey(typeof(Service))]
        public int ServiceID { get; set; }  

        [ManyToOne]
        public Service Service { get; set; }
    }
}
