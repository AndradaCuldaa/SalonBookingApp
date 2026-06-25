using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;

namespace SalonBookingApp.Models
{
    [Table("Client")]
    public class Client : BaseModel
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Phone")]
        public string Phone { get; set; }

        [Column("Password")]
        public string Password { get; set; }

        [Column("IsAdmin")]
        public bool IsAdmin { get; set; }

        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}