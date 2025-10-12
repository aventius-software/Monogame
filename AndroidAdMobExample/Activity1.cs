using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;

namespace AndroidAdMobExample;

/// <summary>
/// A very simple example of how to integrate AdMob banner ads into a MonoGame Android project. This is
/// just the usual 'Activity1.cs' file that is created when you create a new MonoGame Android project, but
/// with the AdMob specific code added in. This example uses a banner ad, but you could easily adapt it 
/// to use interstitial ads, rewarded ads, etc. See the AdMob documentation for more details.
/// 
/// This example should show a banner ad at the top of the screen, and then the MonoGame game 'blue' back
/// ground below it.
/// 
/// One thing to note is that when you are running in debug mode or release mode. Take a look at the .csproj 
/// file as this shows how to include different 'AndroidManifest.xml' files per configuration, and thus
/// set the correct 'AndroidManifest.xml' file to use. This is so you can use different AdMob App Id's for
/// debugging and production. You should ALWAYS use AdMobs/Google's test id's when debugging/building!
/// 
/// NOTE: You might need to install some of the Visual Studio Android workload components to get this to 
/// build. Such as the emulator, SDK tools, etc. (which in itself can be a pain to get working!)
/// 
/// TODO: Need to update this eventually due to Xamarin support ending
/// https://learn.microsoft.com/en-gb/dotnet/maui/migration/?view=net-maui-8.0
/// </summary>
[Activity(
    Label = "@string/app_name",
    MainLauncher = true,
    Icon = "@drawable/icon",
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ScreenOrientation = ScreenOrientation.FullUser,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
)]
public class Activity1 : AndroidGameActivity
{
    private Game1 _game;
    private View _view;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        _game = new Game1();
        _view = _game.Services.GetService(typeof(View)) as View;

        /*           
         * Start of AdMob specific code
         * 
         * Don't forget to also look at the two 'AndroidManifest.xml' files. They are 
         * in the 'Configurations' folder. The .csproj file also needs to be updated to 
         * ensure that it loads the correct 'AndroidManifest.xml' per configuration. This 
         * is so you can use different AdMob App Id's for testing and production. Since
         * you could get banned from AdMob for using your production AdMob App Id while 
         * testing, since it could generate invalid traffic.
         *           
         */

        // First create a layout to hold the ad           
        var adLayoutView = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal // Should the layout be a column or a row?
        };

        // In this example, we want to position the ad horizontally in the
        // center, and then at the top of the screen, so we set the flags appropriately
        adLayoutView.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Top);
        adLayoutView.SetBackgroundColor(Android.Graphics.Color.Transparent);

        // Now we can create a 'banner' style ad view. When debugging/building your app you
        // should ALWAYS use AdMobs/Google's test id's - otherwise you could get your AdMob
        // account/app blocked for invalid views/impressions. When publishing your app live
        // though you need to swap the test id's for your REAL id values you got from
        // AdMob. For more details, see here https://developers.google.com/admob/android/test-ads   
        //
        //  - Test admob app id            = ca-app-pub-3940256099942544~3347511713
        //  - Test admob banner ad unit id = ca-app-pub-3940256099942544/9214589741            
        var bannerAdView = new AdView(this)
        {
            AdUnitId = "ca-app-pub-3940256099942544/9214589741", // Test ad unit id for a banner ad
            AdSize = AdSize.Banner // We want a banner ad
        };

        // Build the banner ad view
        bannerAdView.LoadAd(new AdRequest.Builder().Build());

        // We want to always show the ad
        adLayoutView.AddView(bannerAdView);

        // Create another layout to hold the usual game view, plus our ad layout view
        var mainView = new FrameLayout(this);
        mainView.AddView(_view);
        mainView.AddView(adLayoutView);

        // Finally, instead of setting the content view to use '_view' we
        // instead now set the content to use this new view instead!
        SetContentView(mainView);

        /* 
         * End of AdMob specific code 
         * 
         */

        // Run game as normal
        _game.Run();
    }
}
