using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace SalonBookingApp.Models
{
    [Table("ClientPackage")]
    public class ClientPackage : BaseModel
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("ClientID")]
        public int ClientID { get; set; }

        [Column("PackageName")]
        public string PackageName { get; set; }

        [Column("PackageType")]
        public string PackageType { get; set; }

        [Column("PurchaseDate")]
        public DateTime PurchaseDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        [Column("TotalUses")]
        public int TotalUses { get; set; }

        [Column("RemainingUses")]
        public int RemainingUses { get; set; }

        [Column("RemainingService1")]
        public int RemainingService1 { get; set; }

        [Column("RemainingService2")]
        public int RemainingService2 { get; set; }

        [Column("RemainingService3")]
        public int RemainingService3 { get; set; }

        [Column("RemainingService4")]
        public int RemainingService4 { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public bool IsValid => IsActive && DateTime.Now <= ExpiryDate &&
                                (RemainingUses > 0 || RemainingService1 > 0 || RemainingService2 > 0 || RemainingService3 > 0 || RemainingService4 > 0);

        [JsonIgnore]
        public bool IsUnused
        {
            get
            {
                if (PackageType == "Sedinte") return RemainingUses == TotalUses;

                switch (PackageName)
                {
                    case "BASIC": return RemainingService1 == 1 && RemainingService2 == 1;
                    case "SILVER": return RemainingService1 == 1 && RemainingService2 == 1 && RemainingService3 == 1;
                    case "GOLD": return RemainingService1 == 1 && RemainingService2 == 2 && RemainingService3 == 2;
                    case "DIAMOND": return RemainingService1 == 1 && RemainingService2 == 1 && RemainingService3 == 2 && RemainingService4 == 1;
                    default: return false;
                }
            }
        }
    }
}