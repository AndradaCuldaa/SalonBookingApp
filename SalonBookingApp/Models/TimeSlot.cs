using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SalonBookingApp.Models
{
    public class TimeSlot : INotifyPropertyChanged
    {
        public DateTime FullDateTime { get; set; }

        public string TimeDisplay => FullDateTime.ToString("HH:mm");

        public string PriceDisplay { get; set; }

        public bool IsAvailable { get; set; }

        public Color TextColor => IsAvailable ? Colors.Black : Colors.LightGray;

        private bool _isSelected;
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
