using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Views;

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

			txtTime.Text = items [position].time.ToString ("H:mm");

			txtDescription.Text = items [position].Description;
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
	}
}

