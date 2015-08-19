using ScheduleDataGenerator;
using ScheduleDataGenerator.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MakeAnAppointmentConsole
{
	public class Program
	{
		static DataStorage dataStorage;
		static List<ScheduleItem> schedule;
		static DateTime activeDate;

		static void Main(string[] args)
		{
			dataStorage = new DataStorage();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(dataStorage.GetHost().FirstName + " " + dataStorage.GetHost().LastName);
			Console.ResetColor();
			Console.WriteLine(dataStorage.GetHost().Description);
			Console.WriteLine();

			schedule = dataStorage.GenerateSchedule(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(5));
			activeDate = DateTime.Now.Date;

			ShowSchedule();
			int todayIndex = schedule.FindIndex(p => p.Day.Date == DateTime.Now.Date);
			ShowAppointmentsFor(todayIndex);


			Console.WriteLine("Enter command 'app [index]' to see appointments.");
			Console.WriteLine("Enter command 'more' to load next 5 days.");
			Console.WriteLine("Enter command 'exit' to quit.");
			string command = string.Empty;
			while (command != "exit")
			{
				// 'app 3' or 'app 4'
				if (command.StartsWith("app "))
				{
					try
					{
						string[] words = command.Split(' ');
						int relativeIndex = int.Parse(words[1]);
						int index = -1;

						index = relativeIndex + todayIndex;

						if (index >= 0)
						{
							activeDate = schedule[index].Day.Date;
							ShowSchedule();

							ShowAppointmentsFor(index);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				else if (command == "more")
				{
					List<ScheduleItem> moreItems = dataStorage.GenerateSchedule(schedule[schedule.Count - 1].Day.AddDays(1), schedule[schedule.Count - 1].Day.AddDays(6));
					schedule.AddRange(moreItems);
					ShowSchedule();
				}

				command = Console.ReadLine();
			}
		}

		private static void ShowSchedule()
		{
			int todayIndex = schedule.FindIndex(p => p.Day.Date == DateTime.Now.Date);

			for (int i = 0; i < schedule.Count; i++)
			{
				if (schedule[i].Day.DayOfWeek == DayOfWeek.Saturday || schedule[i].Day.DayOfWeek == DayOfWeek.Sunday)
					Console.ForegroundColor = ConsoleColor.Red;

				if (schedule[i].Day == activeDate)
					Console.ForegroundColor = ConsoleColor.Green;

				int relativeIndex = i - todayIndex;

				Console.Write(relativeIndex + ": " + schedule[i].Day.ToString("dd ddd   "));

				Console.ResetColor();
			}
			Console.WriteLine();
		}

		private static void ShowAppointmentsFor(int index)
		{
			try
			{
				ScheduleItem scheduleItem = schedule[index];

				for (int i = 0; i < scheduleItem.Appointments.Count; i++)
				{
					AppointmentItem appointment = scheduleItem.Appointments[i];
					string description = appointment.Occupied ? "Время приема занято" : GetDescriptionByHour(appointment.Time.Hour);

					Console.WriteLine(appointment.Time.ToString("H.mm   ") + description);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static string GetDescriptionByHour(int hour)
		{
			string result = "Not occupied";
			switch (hour)
			{
			case 9:
				result = "Утро вечера мудреннее";
				break;
			case 10:
				result = "Сделал дело - гуляй смело";
				break;
			case 13:
				result = "Дорога к обеду ложка";
				break;
			case 17:
				result = "Вечерний прием";
				break;
			case 18:
				result = "Время приема ограничено";
				break;
			default:
				result = "Свободный прием";
				break;

			}
			return result;
		}
	}
}
