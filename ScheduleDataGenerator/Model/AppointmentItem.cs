using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleDataGenerator.Model
{
    public class AppointmentItem
    {
        public DateTime Time { get; set; }
        public bool Occupied { get; set; }
    }
}
