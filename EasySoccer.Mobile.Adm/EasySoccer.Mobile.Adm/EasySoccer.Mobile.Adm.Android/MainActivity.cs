using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.Session;
using EasySoccer.Mobile.Adm.Droid.Services;
using EasySoccer.Mobile.Adm.Infra;
using EasySoccer.Mobile.Adm.Infra.Services;
using EasySoccer.Mobile.Adm.Infra.Services.DTO;
using Google.Places;
using Plugin.FirebasePushNotification;
using Prism;
using Prism.Common;
using Prism.Ioc;
using System;
using System.Collections.Generic;

namespace EasySoccer.Mobile.Adm.Droid
{
    [Activity(Theme = "@style/MainTheme",
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, 
          ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Action<PlaceDetail> _onIntentResult;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            UserDialogs.Init(this);
            if (!PlacesApi.IsInitialized)
            {
                PlacesApi.Initialize(this, "AIzaSyCi2hnhPwsHYMqTe-CNnAZOaw9Grtt3ESQ");
            }

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                FirebasePushNotificationManager.DefaultNotificationChannelId = "EasySoccerNotificationChannel";
                FirebasePushNotificationManager.DefaultNotificationChannelName = "Geral";
            }

#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
              FirebasePushNotificationManager.Initialize(this,false);
#endif

            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                var notificationService = new LocalNotificationService();
                string message = string.Empty;
                string title = string.Empty;
                if (p.Data.ContainsKey("message"))
                    p.Data.TryGetValue("message", out message);
                if (p.Data.ContainsKey("title"))
                    p.Data.TryGetValue("title", out title);
                if (string.IsNullOrEmpty(message) == false && string.IsNullOrEmpty(title) == false)
                {
                    notificationService.SendLocalNotification(this, title, message);
                }
            };

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
                if (Xamarin.Essentials.Preferences.ContainsKey("FcmToken") && string.IsNullOrEmpty(Xamarin.Essentials.Preferences.Get("FcmToken", string.Empty)) == false)
                {
                    Xamarin.Essentials.Preferences.Remove("FcmToken");
                }
                Xamarin.Essentials.Preferences.Set("FcmToken", p.Token);
                if (CurrentUser.Instance.IsLoggedIn)
                {
                    try
                    {
                        ApiClient.Instance.InserTokenAsync(new API.ApiRequest.InserTokenRequest { Token = p.Token }).GetAwaiter().GetResult();
                    }
                    catch (Exception)
                    {

                    }
                }
            };


            LoadApplication(new App(new AndroidInitializer()));

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void DisplayIntent(Action<PlaceDetail> onIntentResult)
        {
            _onIntentResult = onIntentResult;
            List<Place.Field> fields = new List<Place.Field>();

            fields.Add(Place.Field.Id);
            fields.Add(Place.Field.Name);
            fields.Add(Place.Field.LatLng);
            fields.Add(Place.Field.Address);

            Intent intent = new Autocomplete.IntentBuilder(AutocompleteActivityMode.Overlay, fields)
                .SetCountry("BR")
                .Build(this);

            StartActivityForResult(intent, 0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                var place = Autocomplete.GetPlaceFromIntent(data);
                var placeDetail = new PlaceDetail
                {
                    Address = place.Address,
                    Id = place.Id,
                    Name = place.Name,
                    Latitude = place.LatLng.Latitude,
                    Longitude = place.LatLng.Longitude
                };
                _onIntentResult(placeDetail);
            }
            catch (Exception)
            {
                return;
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IGooglePlacesService, GooglePlacesService>();
        }
    }
}

