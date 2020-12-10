using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.Session;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        public DelegateCommand SignUpCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }

        private INavigationService _navigationService;
        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SignUpCommand = new DelegateCommand(NavigateSignUp);
            LoginCommand = new DelegateCommand(LoginAsync);
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private void NavigateSignUp()
        {
            _navigationService.NavigateAsync("SignUp");
        }

        private async void LoginAsync()
        {
            try
            {
                var response = await ApiClient.Instance.LoginAsync(Email, Password);
                if (response != null)
                {
                    CurrentUser.Instance.Login(response.Token, response.ExpireDate);
                    var fcmToken = Preferences.Get("FcmToken", string.Empty);
                    if (Preferences.ContainsKey("FcmToken") && string.IsNullOrEmpty(fcmToken) == false)
                    {
                        var token = await ApiClient.Instance.InserTokenAsync(new API.ApiRequest.InserTokenRequest { Token = fcmToken });
                    }
                    await _navigationService.NavigateAsync("/MainPage/NavigationPage/CompanyReservations");
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }
    }
}
