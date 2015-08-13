using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleDataGenerator.Model
{
    public class ScheduleItem
    {
        public DateTime Day { get; set; }
        public List<AppointmentItem> Appointments { get; set; }

        public ScheduleItem()
        {
            Appointments = new List<AppointmentItem>();
        }
    }
}
