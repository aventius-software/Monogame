using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;

namespace AndroidAdMobExample;

/// <summary>
/// TODO: Need to update this eventually due to Xamarin support ending in the near future (at time of writing)...
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

        /* Start of AdMob code - don't forget to also look at 'AndroidManifest.xml' file too! */

        // First create a layout to hold the ad           
        var adLayoutView = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal // Should the layout be a column or a row?
        };

        // We want to position the ad horizontally in the center of the screen, and
        // at the bottom of the screen
        adLayoutView.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom);
        adLayoutView.SetBackgroundColor(Android.Graphics.Color.Transparent);

        // Create a banner ad view. Note the test id's, for more details
        // see this link https://developers.google.com/admob/android/test-ads   
        //
        // test admob app id            = ca-app-pub-3940256099942544~3347511713
        // test admob banner ad unit id = ca-app-pub-3940256099942544/9214589741            
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
        // instead now set the content to use this new view
        SetContentView(mainView);

        /* End of AdMob code */

        // Run game as normal
        _game.Run();
    }
}
