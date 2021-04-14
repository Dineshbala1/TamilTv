using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using FFImageLoading;
using FormsToolkit.Droid;
using MonkeyCache.FileStore;
using TamilSerial.Contracts;
using TamilSerial.ViewModels.Base;

namespace TamilSerial.Droid
{
    [Activity(Label = "Tamil TV", Icon = "@mipmap/icon", Theme = "@style/SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.splash_layout);

            var imageView = FindViewById<ImageView>(Resource.Id.imageView1);

            ImageService.Instance.LoadCompiledResource("splash_load").Into(imageView);

            Barrel.ApplicationId = "TamilSerial";
            Toolkit.Init();

            ViewModelLocator.Resolve<ICachedBigbossService>();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await Task.Delay(2500);

            await Task.Run(() => { StartActivity(typeof(MainActivity)); });
        }
    }
}
