using Postgrest.Attributes;
using Postgrest.Models;

namespace SalonBookingApp.Models
{
    [Table("StylistService")]
    public class StylistService : BaseModel
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("StylistID")]
        public int StylistID { get; set; }

        [Column("ServiceID")]
        public int ServiceID { get; set; }
    }
}