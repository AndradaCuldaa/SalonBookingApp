using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SalonBookingApp.Models
{
    public class Client
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Username { get; set; } 
        public string Password { get; set; } 
        public bool IsAdmin { get; set; }

        [Ignore] 
        public string FullName => $"{FirstName} {LastName}";
    }
}
