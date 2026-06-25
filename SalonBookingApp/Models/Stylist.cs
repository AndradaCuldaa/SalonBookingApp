using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SalonBookingApp.Models
{
    [Table("Stylist")]
    public class Stylist : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("FirstName")]
        public string FirstName { get; set; }

        [Column("LastName")]
        public string LastName { get; set; }

        [Column("Specialization")]
        public string Specialization { get; set; }

        [Column("SpecializationEn")]
        public string SpecializationEn { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Password")]
        public string Password { get; set; }

        [Column("ImagePath")]
        public string ImagePath { get; set; }

        [Column("IsStylist")]
        public bool IsStylist { get; set; } = true;

        [Column("cancel_limit")]
        public string CancelLimit { get; set; }

        [Column("booking_limit")]
        public string BookingLimit { get; set; }

        [Column("max_bookings")]
        public string MaxBookings { get; set; }

        [Column("future_limit")]
        public string FutureLimit { get; set; }

        [JsonIgnore]
        public string StarsDisplay { get; set; }

        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [JsonIgnore]
        public string DisplaySpecialization =>
            System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ro"
            ? Specialization
            : SpecializationEn;

        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}