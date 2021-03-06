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

using ScheduleDataGenerator;
using ScheduleDataGenerator.Model;

namespace Test1.Droidt
{
	[Activity (MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]


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

		List<ScheduleItem> schedule;
		bool loadingSchedules = false;
		ListView appointmentsListView;
		LinearLayout datesLayout;
		View activeView;
		DayOfWeek activeDayOfWeek = DayOfWeek.Wednesday;
		private const int LOAD_NUMBER = 15;

		DataStorage dataStorage;

		protected override void OnCreate (Bundle bundle)
		{
			dataStorage = new DataStorage ();

			Window.AddFlags (WindowManagerFlags.Fullscreen);
			Window.ClearFlags (WindowManagerFlags.ForceNotFullscreen);
			this.Window.SetFlags (WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			FillHostSummary (dataStorage.GetHost());

			appointmentsListView = FindViewById<ListView> (Resource.Id.appointmentsListView);
			EndlessScrollView scrollView = FindViewById<EndlessScrollView> (Resource.Id.scroll);
			datesLayout = FindViewById<LinearLayout> (Resource.Id.datesLayout);

			schedule = dataStorage.GenerateSchedule (DateTime.Now.AddDays(0), DateTime.Now.AddDays (LOAD_NUMBER));

			scrollView.ScrollReachedRight += (sender, args) => {
				LoadMoreRight (LOAD_NUMBER);
			};
				
			for (var i = 0; i < schedule.Count; i++) {
				ScheduleItem item = schedule [i];

				View view = InflateScheduleItem (this, item);

				view.Click += (sender, e) => {
					DateClick (sender, item.Appointments, item.Day.DayOfWeek);
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
			
		private int counter = 0;

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

						if (activeView == null && counter <= 10) {
							ScrollTo(index, scrollView);
							counter++;
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
					List<ScheduleItem> nextDays = dataStorage.GenerateSchedule(lastItem.Day.Date.AddDays(1), lastItem.Day.Date.AddDays(numberToLoad));

					for (int i = 0; i < nextDays.Count; i++) {
						ScheduleItem item = nextDays [i];

						View view = InflateScheduleItem(this, item);
						view.Click += (sender, e) => {
							DateClick(sender, item.Appointments, item.Day.DayOfWeek);
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
			View clickedView = sender as View;
			if (clickedView != activeView) {
				AppointmentAdapter adapter = new AppointmentAdapter (this, appointments, FindViewById<Button> (Resource.Id.saveButton));
				appointmentsListView.Adapter = adapter;

				SetColor (clickedView, activeView, activeDayOfWeek);

				activeView = clickedView;			
				activeDayOfWeek = newDayOfWeek;
			}
		}

		private View InflateScheduleItem(Context context, ScheduleItem item)
		{
			View result = LayoutInflater.From (context).Inflate (Resource.Layout.DatesListView, null, false);
			TextView txtDate = result.FindViewById<TextView> (Resource.Id.txtDate),
			txtDayOfWeek = result.FindViewById<TextView> (Resource.Id.txtDayOfWeek);
			if (item.Day.DayOfWeek == DayOfWeek.Saturday || item.Day.DayOfWeek == DayOfWeek.Sunday) {
				txtDate.SetTextAppearance (this, Resource.Style.largeWeekendDate);
				txtDayOfWeek.SetTextAppearance (this, Resource.Style.smallWeekendDate);
			}
			txtDate.Text = item.Day.ToString ("dd");
			txtDayOfWeek.Text = item.Day.ToString ("ddd");

			return result;
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

		private void FillHostSummary(HostItem host)
		{
			View hostView = FindViewById<LinearLayout> (Resource.Id.hostSummary);
			TextView txtFullName = hostView.FindViewById<TextView> (Resource.Id.name);
			txtFullName.Text = host.FirstName + " " + host.LastName;

			TextView txtDescription = hostView.FindViewById<TextView> (Resource.Id.description);
			txtDescription.Text = host.Description;
		}
	}
}