using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using Xamarin.Media;
using System.Threading.Tasks;

using Xamarin.Geolocation;
using FlyoutNavigation;

namespace GravitySampleApplication
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		GravitySampleApplicationViewController viewController;
		UIWindow _window;
		UINavigationController _nav;
		DialogViewController _rootVC;
		RootElement _rootElement;
		UIBarButtonItem _addButton;
	  
 		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			try
			{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new GravitySampleApplicationViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			_window = new UIWindow (UIScreen.MainScreen.Bounds);

			_rootElement = new RootElement ("Gravity Interview Demo App") {
				new Section ("Features") {
					new StyledStringElement ("Geo Location Example", delegate {
						GeoLocation ();
					})
				},
			
					new Section ("Camera Demos") {
					new StyledStringElement ("Take A Picture", delegate {
						TakeAPicture ();
					}),
					new StyledStringElement ("Pick a Picture", delegate {
						PicPhoto ();
						}),


 
			 
				},
					new Section ("Data Demos") {

						new StyledStringElement ("Load Item From Json", delegate {
							loadJson ();
						}),
					},
				new Section ("Settings") {
					new RootElement ("Options", new RadioGroup ("dessert", 2)) {

						new Section () {
					 
							new RadioElement ("Allow Camera", "dessert"),
						}
					}
				}
			};

			_rootVC = new DialogViewController (_rootElement);
			_nav = new UINavigationController (_rootVC);

			_addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add);
			_rootVC.NavigationItem.RightBarButtonItem = _addButton;


			_window.RootViewController = _nav;
			_window.MakeKeyAndVisible ();

		}
			
			catch(Exception ex)
			{
				Console.WriteLine ("FinishedLaunching Failed" + ex.ToString());

			}

			return true;
		}
		private void PicPhoto()
		{
			try {
			var picker = new MediaPicker();
			picker.PickPhotoAsync().ContinueWith (t => {
				MediaFile file = t.Result;
				Console.WriteLine (file.Path);
			}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			catch(Exception ex)

			{
				Console.WriteLine ("Photo Picker Failed" + ex.ToString());

			}
		}
		// in UIHelpers class
		public static Task<bool> ShowConfirm(string title, string message) {
			var tcs = new TaskCompletionSource<bool>();
			try
			{
			UIApplication.SharedApplication.InvokeOnMainThread(
				new NSAction(() => {
					UIAlertView alert = new UIAlertView(
						title, 
						message, 
						null, 
						NSBundle.MainBundle.LocalizedString("Cancel", "Cancel"),
						NSBundle.MainBundle.LocalizedString("OK", "OK")
					);
					alert.Clicked += (sender, buttonArgs) => tcs.SetResult(buttonArgs.ButtonIndex != alert.CancelButtonIndex);
					alert.Show();
				})
			);
			}

			catch(Exception ex)

			{
				Console.WriteLine (" ShowConfirm Failed" + ex.ToString());

			}

			return tcs.Task;
		}
		private void loadJson ()
		{



			try {
			
				var GravityElement = JsonElement.FromFile ("import.json");
				var duedate = GravityElement ["task-duedate"] as DateElement;
				var description = GravityElement ["task-description"]as EntryElement;


				ShowConfirm("Json Data Loaded","Description" + description.ToString() + "Task Due Date=" + duedate.DateValue.ToShortDateString());

			}

			catch(Exception ex)

			{
				Console.WriteLine (" jsonTest Failed" + ex.ToString());

			}


		}
		private async void GeoLocation()

		{
			 
			try {

			
			var locator = new Geolocator { DesiredAccuracy = 50 };
//            new Geolocator (this) { ... }; on Android
			locator.GetPositionAsync (timeout: 10000).ContinueWith (t => {
					ShowConfirm("Latitude=",t.Result.Latitude.ToString() + "Longitude="+ t.Result.Longitude.ToString());
				 
				}, TaskScheduler.FromCurrentSynchronizationContext());

		
			}

			catch(Exception ex) {
				Console.WriteLine ("GeoLocation Failed" + ex.ToString());

			}
		}

	
		// this uses the Media Picker of xamrian mobile to capture an image and save it 
		//in the media libary
		private async void TakeAPicture()
		{

			var picker = new MediaPicker ();
	 		try {

				if (!picker.IsCameraAvailable)
					Console.WriteLine ("No camera!");
				else {
					Console.WriteLine ("Taking photo...");
					await picker.TakePhotoAsync (new StoreCameraMediaOptions {
						Name = "test.jpg",
						Directory = "MediaPickerSample"
					}).ContinueWith (t => {
						if (t.IsCanceled) {
							Console.WriteLine ("User canceled");
							return;
						}
						Console.WriteLine ("Photo Done...");
					}, TaskScheduler.FromCurrentSynchronizationContext ());
				}

			} catch (Exception ex) {

			 //handle_exception (ex);
				Console.WriteLine ("Take A picture  Failed" + ex.ToString());

			}
		}

	}
}
