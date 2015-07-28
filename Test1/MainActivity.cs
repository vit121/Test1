using System;
using System.Collections.Generic; 
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Android.Graphics;
using Java.Lang;

namespace Test1
{
	[Activity (MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	//[Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]

	public class MainActivity : Activity
	{
		private void SetColor(View newActiveView, View oldActiveView, DayOfWeek oldActiveDayOfWeek) 
		{
			TextView newActiveTxtDate = newActiveView.FindViewById<TextView> (Resource.Id.txtDate),
					 newActiveTxtDayOfWeek = newActiveView.FindViewById<TextView> (Resource.Id.txtDayOfWeek);

			newActiveTxtDate.SetTextAppearance (this, Resource.Style.largeActiveDate);
			newActiveTxtDayOfWeek.SetTextAppearance (this, Resource.Style.smallActiveDate);


			if (oldActiveView != null)
			{
				TextView oldActiveTxtDate = oldActiveView.FindViewById<TextView> (Resource.Id.txtDate),
						 oldActiveTxtDayOfWeek = oldActiveView.FindViewById<TextView> (Resource.Id.txtDayOfWeek);

				if (oldActiveDayOfWeek == DayOfWeek.Saturday || oldActiveDayOfWeek == DayOfWeek.Sunday) 
				{
					oldActiveTxtDate.SetTextAppearance (this, Resource.Style.largeWeekendDate);
					oldActiveTxtDayOfWeek.SetTextAppearance (this, Resource.Style.smallWeekendDate);

				}
				else {
					oldActiveTxtDate.SetTextAppearance (this, Resource.Style.largeDate);
					oldActiveTxtDayOfWeek.SetTextAppearance (this, Resource.Style.smallDate);
				}
			}
		}

		private string GetDescriptionByHour(int hour)
		{
			string[] descriptions = Resources.GetStringArray (Resource.Array.appointment_decriptions); 

			string result = "";
			switch (hour) {
				case 9:
					result = descriptions [0];
					break;
			    case 10:
				    result = descriptions [1];
				    break;
				case 13:
					result = descriptions [2];
					break;
				case 17:
					result = descriptions [3];
			     	break;
			    case 18:
				    result = descriptions [4];
				    break;
				default:
					result = Resources.GetString (Resource.String.default_description) + " ";
				break;
			}
			return result;
		}	

		List<ScheduleItem> GenerateSchedule(DateTime from, DateTime to)  
		{

			Random rnd = new Random ();

			List<ScheduleItem> result = new List<ScheduleItem>();

			for (DateTime day = from; day <= to; day = day.AddDays(1))
			{
				ScheduleItem scheduleItem = new ScheduleItem ();
				scheduleItem.day = day.Date;

				if (scheduleItem.day.DayOfWeek != DayOfWeek.Saturday && scheduleItem.day.DayOfWeek != DayOfWeek.Sunday)
				{
					int startTime = rnd.Next (8, 11),
					endTime = rnd.Next (17, 19);

					for (int i = startTime; i < endTime; i++)
					{
						AppointmentItem appointment = new AppointmentItem();
						appointment.time = day.Date.AddHours(i);
						appointment.Occupied = rnd.Next (0, 100) > 70;

						if (appointment.Occupied) {
							appointment.Description = Resources.GetString (Resource.String.description_occupied);
						}
						else
							appointment.Description = GetDescriptionByHour(i);

						scheduleItem.appointments.Add(appointment);
					}
				}

				result.Add (scheduleItem);

			}

			return result;
		}


		List<ScheduleItem> schedule;
		bool loadingSchedules = false;
		ListView appointmentsListView;
		LinearLayout datesLayout;
		View activeView;
		DayOfWeek activeDayOfWeek = DayOfWeek.Wednesday;
		private const int LOAD_NUMBER = 15;

		protected override void OnCreate (Bundle bundle)
		{
			
			
			Window.AddFlags (WindowManagerFlags.Fullscreen);
			Window.ClearFlags (WindowManagerFlags.ForceNotFullscreen);
			this.Window.SetFlags (WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			appointmentsListView = FindViewById<ListView> (Resource.Id.appointmentsListView);
			EndlessScrollView scrollView = FindViewById<EndlessScrollView> (Resource.Id.scroll);
			datesLayout = FindViewById<LinearLayout> (Resource.Id.datesLayout);

			schedule = GenerateSchedule (DateTime.Now, DateTime.Now.AddDays (LOAD_NUMBER));

			scrollView.ScrollReachedRight += (sender, args) => {
				LoadMoreRight (LOAD_NUMBER);
			};
				
			for (var i = 0; i < schedule.Count; i++) {
				ScheduleItem item = schedule [i];

				View view = InflateScheduleItem (this, item);

				view.Click += (sender, e) => {
					DateClick (sender, item.appointments, item.day.DayOfWeek);
				};

				datesLayout.AddView (view);
			}

			int todayIndex = FindTodayIndex();
			// Can't scroll right away, it needs time to render element first
			ScrollTo (todayIndex, scrollView);

			Button saveButton = FindViewById<Button> (Resource.Id.saveButton);


			saveButton.Click += (object sender, EventArgs e) => {
			};
			saveButton.Clickable = false;

		}

		//=======================================================
		private int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int) ((pixelValue)/Resources.DisplayMetrics.Density);
			return dp;
		}
		//======================================================
		private void ScrollTo(int index, EndlessScrollView scrollView)
		{
			if (index >= 0) 
			{
				Handler handler = new Handler ();
				handler.PostDelayed (() => {
					View today = datesLayout.GetChildAt (index);
					if (today != null)
					{
						today.PerformClick ();
						int currentScroll = scrollView.ScrollX;

						int[] viewLocation = new int[2];
						today.GetLocationOnScreen(viewLocation);

						int scrollViewWidth = scrollView.Width / 2;
						int clickedOffetX = viewLocation[0] + currentScroll;

						if (clickedOffetX > scrollViewWidth) 
						{
							int scrollX = clickedOffetX - scrollViewWidth + today.Width / 2;
							scrollView.SmoothScrollTo(scrollX, 0);
						}
					}
				}, 100);
			}			
		}

		private void LoadMoreRight(int numberToLoad) 
		{
			if (!loadingSchedules) 
			{
				loadingSchedules = true;

				Handler loadingHandler = new Handler ();
				loadingHandler.PostDelayed (() => {
					ScheduleItem lastItem = schedule[schedule.Count - 1];
					List<ScheduleItem> nextDays = GenerateSchedule(lastItem.day.Date.AddDays(1), lastItem.day.Date.AddDays(numberToLoad));

					for (int i = 0; i < nextDays.Count; i++) {
						ScheduleItem item = nextDays [i];

						View view = InflateScheduleItem(this, item);
						view.Click += (sender, e) => {
							DateClick(sender, item.appointments, item.day.DayOfWeek);
						};

						datesLayout.AddView (view);
					}

					if (schedule == null) 
					{
						schedule = new List<ScheduleItem>();
					}

					schedule.AddRange(nextDays);

					loadingSchedules = false;
				}, 1000);
			}
		}

		private void DateClick(object sender, List<AppointmentItem> appointments, DayOfWeek newDayOfWeek)
		{
			AppointmentAdapter adapter = new AppointmentAdapter (this, appointments, FindViewById<Button> (Resource.Id.saveButton));
			appointmentsListView.Adapter = adapter;

			View clickedView = sender as View;

			SetColor(clickedView, activeView, activeDayOfWeek);

			activeView = clickedView;			
			activeDayOfWeek = newDayOfWeek;
		}

		private View InflateScheduleItem(Context context, ScheduleItem item)
		{
			View result = LayoutInflater.From (context).Inflate (Resource.Layout.DatesListView, null, false);
			TextView txtDate = result.FindViewById<TextView> (Resource.Id.txtDate),
			txtDayOfWeek = result.FindViewById<TextView> (Resource.Id.txtDayOfWeek);
			if (item.day.DayOfWeek == DayOfWeek.Saturday || item.day.DayOfWeek == DayOfWeek.Sunday) {
				txtDate.SetTextAppearance (this, Resource.Style.largeWeekendDate);
				txtDayOfWeek.SetTextAppearance (this, Resource.Style.smallWeekendDate);
			}
			txtDate.Text = item.day.ToString ("dd");
			txtDayOfWeek.Text = item.day.ToString ("ddd");

			return result;
		}

		private int FindTodayIndex()
		{
			int result = -1;
			if (schedule != null)
			{
				for (int i = 0; i < schedule.Count; i++) {
					if (schedule [i].day.Date == DateTime.Now.Date) {
						result = i;
						break;
					}
				}
			}
				
			return result;
		}


	}
}