using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SalonBookingApp.Models
{
    public class Stylist
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; } 

        
        [Ignore]
        public string FullName => $"{FirstName} {LastName} ";

        
    }
}
