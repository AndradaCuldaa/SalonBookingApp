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

        public string FirstName { get; set; } // Prenume
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Username { get; set; } // Pentru autentificare
        public string Password { get; set; } // Pentru securitate
        public bool IsAdmin { get; set; }

        [Ignore] // Această proprietate nu se salvează în DB, se calculează pe loc
        public string FullName => $"{FirstName} {LastName}";
    }
}
