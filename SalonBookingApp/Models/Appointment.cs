using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace SalonBookingApp.Models
{
    [Table("Appointment")]
    public class Appointment : BaseModel
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("AppointmentDate")]
        public DateTime AppointmentDate { get; set; }

        [Column("ClientID")]
        public int ClientID { get; set; }

        [Column("StylistID")]
        public int StylistID { get; set; }

        [Column("ServiceID")]
        public int ServiceID { get; set; }

        [JsonIgnore]
        public Client Client { get; set; }

        [JsonIgnore]
        public Stylist Stylist { get; set; }

        [JsonIgnore]
        public Service Service { get; set; }

        [JsonIgnore]
        public string ClientNameDisplay { get; set; }
    }
}