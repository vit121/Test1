using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using ScheduleDataGenerator.Model;
using Android.Content.Res;

namespace Test1
{
	public class AppointmentAdapter : BaseAdapter<AppointmentItem>
	{

		private List<AppointmentItem> items;
		private Context context;
		private List<CheckBox> clickableCheckboxes;

		private Button saveButton;

		public AppointmentAdapter (Context context, List<AppointmentItem> items, Button saveButton)
		{
			this.items = items;
			this.context = context;
			this.saveButton = saveButton;
			clickableCheckboxes = new List<CheckBox> ();
		}

		public override int Count  
		{
			get 
			{
				return items.Count;
			}
		}

		public override long GetItemId(int position) 
		{
			return position;
		}

		public override AppointmentItem this[int position]
		{
			get 
			{
				return items [position];
			}
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			if (row == null) {
				row = LayoutInflater.From (context).Inflate (Resource.Layout.AppointmentView, null, false);
			}

			TextView txtTime = row.FindViewById<TextView> (Resource.Id.txtTime),
					 txtDescription = row.FindViewById<TextView> (Resource.Id.txtDescription);

			CheckBox cbOccupied = row.FindViewById<CheckBox> (Resource.Id.cbOccupied);

			txtTime.Text = items [position].Time.ToString ("H.mm");
			AppointmentItem appointment = items[position];

			if (appointment.Occupied) {
				txtDescription.Text = context.Resources.GetString (Resource.String.description_occupied);
			}
			else
				txtDescription.Text = GetDescriptionByHour(appointment.Time.Hour);

			cbOccupied.Checked = false;

			if (!items [position].Occupied) {
				cbOccupied.Click += CheckBoxClick;
				clickableCheckboxes.Add (cbOccupied);
			} 
			else
				cbOccupied.Enabled = false;

			return row;
		}

		// Click Handler
		private void CheckBoxClick(object sender, EventArgs e) 
		{
			if (sender is CheckBox) {
				CheckBox checkBox = sender as CheckBox;
				if (checkBox.Checked) 
				{
					saveButton.Clickable = true;

					foreach (CheckBox chb in clickableCheckboxes) {
						if (chb != checkBox) {
							chb.Checked = false;
						} else {
							chb.Checked = true;
						}
					}
				} 
				else {
					saveButton.Clickable = false;
				}
			}
		}

		private string GetDescriptionByHour(int hour)
		{
			string[] descriptions = context.Resources.GetStringArray (Resource.Array.appointment_decriptions); 

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
				result = context.Resources.GetString (Resource.String.default_description) + " ";
				break;
			}
			return result;
		}	
	}
}

