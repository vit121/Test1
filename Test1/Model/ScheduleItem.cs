using System;
using System.Collections.Generic;

namespace Test1
{
	public class ScheduleItem 
	{
		public DateTime day;
		public List<AppointmentItem> appointments;

		public ScheduleItem() {
			appointments = new List<AppointmentItem> ();	
		}
	}
}

