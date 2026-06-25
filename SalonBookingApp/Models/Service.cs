using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SalonBookingApp.Models
{
    [Table("Service")]
    public class Service : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("ID", false)]
        public int ID { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("NameEn")]
        public string NameEn { get; set; }

        [Column("Category")]
        public string Category { get; set; }

        [Column("CategoryEn")]
        public string CategoryEn { get; set; }

        [Column("Price")]
        public decimal Price { get; set; }

        [Column("Duration")]
        public int Duration { get; set; }

        [JsonIgnore]
        public string DisplayNameWithPrice => $"{DisplayName} - {Price} RON";

        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                string limbaSalvata = Preferences.Get("SelectedLanguage", "ro-RO");
                return limbaSalvata.StartsWith("ro") ? Name : (string.IsNullOrEmpty(NameEn) ? Name : NameEn);
            }
        }

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