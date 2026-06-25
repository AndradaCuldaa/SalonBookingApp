using System;

namespace SalonBookingApp.Models
{
    public class WorkDaySchedule
    {
        public int? DbId { get; set; }
        public DateTime Date { get; set; }
        public string DayName => Date.ToString("ddd dd MMM", System.Globalization.CultureInfo.CurrentCulture);
        public bool IsOff { get; set; }

        public TimeSpan StartTime1 { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan EndTime1 { get; set; } = new TimeSpan(13, 0, 0);
        public TimeSpan StartTime2 { get; set; } = new TimeSpan(14, 0, 0);
        public TimeSpan EndTime2 { get; set; } = new TimeSpan(18, 0, 0);

        public string TotalHours => IsOff ? "Nu lucrează" : $"{CalculateHours()}h";

        private int CalculateHours()
        {
            if (IsOff) return 0;
            return (int)((EndTime1 - StartTime1).TotalHours + (EndTime2 - StartTime2).TotalHours);
        }
    }
}