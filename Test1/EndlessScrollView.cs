using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

namespace Test1
{
	public class EndlessScrollView : HorizontalScrollView
	{ 
		public delegate void ScrollReachedEndEventHandler(object sender, EventArgs e);
		public event ScrollReachedEndEventHandler ScrollReachedRight,
												  ScrollReachedLeft;

		public EndlessScrollView (Context context) : base (context) { }

		public EndlessScrollView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

		public EndlessScrollView(Context context, IAttributeSet attrs) : base(context, attrs) { }
			
		protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
		{
			base.OnScrollChanged (l, t, oldl, oldt); 

			// Grab the last child placed in the ScrollView, we need it to determinate the bottom position.
			View lastChildView = GetChildAt (ChildCount - 1);

			// Calculate the scrolldiff
			var lastDiff = (lastChildView.Right - (Width + ScrollX));

			// if diff is zero, then the bottom has been reached
			if (lastDiff == 0 && ScrollReachedRight != null) {
				ScrollReachedRight.Invoke (this, new EventArgs ());
			} 
			else if (ScrollX == 0 && ScrollReachedLeft != null) {
				ScrollReachedLeft.Invoke (this, new EventArgs ());
			}


		}
	}
}


