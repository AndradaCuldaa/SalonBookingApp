using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace SalonBookingApp.Models
{
    [Table("Review")]
    public class Review : BaseModel
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("StylistID")]
        public int StylistID { get; set; }

        [Column("ClientID")]
        public int ClientID { get; set; }

        [Column("ClientName")]
        public string ClientName { get; set; }

        [Column("RatingDisplay")]
        public string RatingDisplay { get; set; }

        [Column("Comment")]
        public string Comment { get; set; }

        [Column("DateAdded")]
        public DateTime DateAdded { get; set; }

        [Column("ServiceName")]
        public string ServiceName { get; set; }
    }
}