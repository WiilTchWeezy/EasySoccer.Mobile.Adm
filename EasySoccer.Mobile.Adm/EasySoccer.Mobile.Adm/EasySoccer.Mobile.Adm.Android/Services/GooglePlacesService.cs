using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using EasySoccer.Mobile.Adm.Droid.Services;
using EasySoccer.Mobile.Adm.Infra.Services;
using Google.Places;
using Xamarin.Forms;

[assembly:Dependency(typeof(GooglePlacesService))]
namespace EasySoccer.Mobile.Adm.Droid.Services
{
    public class GooglePlacesService : AppCompatActivity, IGooglePlacesService
    {
        public void DisplayIntent()
        {
            var main = Xamarin.Essentials.Platform.CurrentActivity as MainActivity;
            main.DisplayIntent();
        }
    }
}