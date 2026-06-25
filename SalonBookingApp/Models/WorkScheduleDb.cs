using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace SalonBookingApp.Models
{
    [Table("work_schedules")]
    public class WorkScheduleDb : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("stylist_id")]
        public int StylistId { get; set; }

        [Column("schedule_date")]
        public DateTime ScheduleDate { get; set; }

        [Column("is_off")]
        public bool IsOff { get; set; }

        [Column("start_time_1")]
        public TimeSpan? StartTime1 { get; set; }

        [Column("end_time_1")]
        public TimeSpan? EndTime1 { get; set; }

        [Column("start_time_2")]
        public TimeSpan? StartTime2 { get; set; }

        [Column("end_time_2")]
        public TimeSpan? EndTime2 { get; set; }
    }
}