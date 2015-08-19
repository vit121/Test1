using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using System;

using ScheduleDataGenerator;
using ScheduleDataGenerator.Model;

namespace Test1_IOS
{
	public class DatesScrollHoziontalController : UIViewController
	{
		UIScrollView scrollView;
		List<UIButton> buttons;
		DataStorage dataStorage;
		private const int LOAD_NUMBER = 35;
		List<ScheduleItem> schedule;
        DateTime activeDate;

		public DatesScrollHoziontalController ()
		{
			buttons = new List<UIButton> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			nfloat h = 25.0f;
			nfloat w = 50.0f;
			nfloat padding = 10.0f;

			dataStorage = new DataStorage ();

			schedule = dataStorage.GenerateSchedule (DateTime.Now.AddDays(0), DateTime.Now.AddDays (LOAD_NUMBER));
			activeDate = DateTime.Now.Date;


			scrollView = new UIScrollView {
				Frame = new CGRect (0, 150, View.Frame.Width, h + 2 * padding),
				ContentSize = new CGSize ((w + padding) * LOAD_NUMBER, h),
				BackgroundColor = UIColor.White,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth
			};

		      for (int i=0; i<schedule.Count; i++) {
		     	var button = UIButton.FromType (UIButtonType.RoundedRect);
				ScheduleItem item = schedule [i];
				button.SetTitle (item.Day.ToString("dd ddd"), UIControlState.Normal);
				button.Frame = new CGRect (padding * (i + 1) + (i * w), padding, w, h);
				scrollView.AddSubview (button);
				buttons.Add (button);

				button.SetTitleColor(UIColor.Blue, UIControlState.Normal);
			   if (item.Day.DayOfWeek == DayOfWeek.Saturday || item.Day.DayOfWeek == DayOfWeek.Sunday) 
				{
					button.SetTitleColor(UIColor.Red, UIControlState.Normal);
				}
				if (item.Day.Day == DateTime.Now.Day) 
				{
					button.SetTitleColor(UIColor.Green, UIControlState.Normal);
				}
			}
			View.AddSubview (scrollView);
		}

		private int FindTodayIndex()
		{
			int result = -1;
			if (schedule != null)
			{
				for (int i = 0; i < schedule.Count; i++) {
					if (schedule [i].Day.Date == DateTime.Now.Date) {
						result = i;
						break;
					}
				}
			}
			return result;
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

