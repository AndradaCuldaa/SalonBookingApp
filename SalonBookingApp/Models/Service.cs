using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SalonBookingApp.Models
{
    public class Service
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }

        [Ignore]
        public string DisplayNameWithPrice => $"{Name} - {Price} RON";


    }
}
