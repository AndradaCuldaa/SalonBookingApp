using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;


namespace SalonBookingApp.Models
{
    public class CalendarDay : INotifyPropertyChanged
    {
        public DateTime Date { get; set; }
        public string DayName => Date.ToString("ddd"); 
        public string DayNumber => Date.Day.ToString(); 

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
