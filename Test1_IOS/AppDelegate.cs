using Foundation;
using UIKit;

using ScheduleDataGenerator;
using ScheduleDataGenerator.Model;

namespace Test1_IOS
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UIWindow window1;
		UIViewController vc;

		/*
		public override UIWindow Window {
			get;
			set;
		}
        */
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			window1 = new UIWindow (UIScreen.MainScreen.Bounds); 
			vc = new DatesScrollHoziontalController();
			window1.RootViewController = vc;
			window1.MakeKeyAndVisible();



			return true;
		}


	}
}


