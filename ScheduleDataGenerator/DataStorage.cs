using ScheduleDataGenerator.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleDataGenerator
{
	public class DataStorage
	{
		private List<HostItem> hosts;

		public DataStorage()
		{
			hosts = new List<HostItem>();
			HostItem newHost = new HostItem()
			{
				FirstName = "Пафос",
				LastName = "Паллас",
				Description = "Отель на берегу моря. Все включено",
				Price = 12500
			};

			hosts.Add(newHost);
		}

		public HostItem GetHost()
		{
			if (hosts != null && hosts.Count > 0)
				return hosts[0];
			else
				return null;
		}

		public List<ScheduleItem> GenerateSchedule(DateTime from, DateTime to)
		{
			Random rnd = new Random();

			List<ScheduleItem> result = new List<ScheduleItem>();

			for (DateTime day = from; day <= to; day = day.AddDays(1))
			{
				ScheduleItem scheduleItem = new ScheduleItem();
				scheduleItem.Day = day.Date;

				if (scheduleItem.Day.DayOfWeek != DayOfWeek.Saturday && scheduleItem.Day.DayOfWeek != DayOfWeek.Sunday)
				{
					int startTime = rnd.Next(8, 11),
					endTime = rnd.Next(17, 19);

					for (int i = startTime; i < endTime; i++)
					{
						AppointmentItem appointment = new AppointmentItem();
						appointment.Time = day.Date.AddHours(i);
						appointment.Occupied = rnd.Next(0, 100) > 70;

						scheduleItem.Appointments.Add(appointment);
					}
				}

				result.Add(scheduleItem);

			}

			return result;
		}
	}
}
