using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class UserInfoViewModel : BindableBase, INavigationAware
    {
        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand ChangePasswordCommand { get; set; }
        private INavigationService _navigationService;
        public UserInfoViewModel(INavigationService navigationService)
        {
            SaveCommand = new DelegateCommand(async () => { await SaveAsync(); });
            ChangePasswordCommand = new DelegateCommand(NavigateChangePassword);
            _navigationService = navigationService;
        }
        private async Task LoadDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetUserInfo();
                if (response != null)
                {
                    Email = response.Email;
                    Name = response.Name;
                    Phone = response.Phone;
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                var response = await ApiClient.Instance.PatchUserAsync(new API.ApiRequest.PatchUserInfoRequest
                {
                    Email = Email,
                    Name = Name,
                    Phone = Phone
                });
                UserDialogs.Instance.Alert("Dados atualizados com sucesso!");
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void NavigateChangePassword()
        {
            _navigationService.NavigateAsync("ChangePassword");
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() != NavigationMode.Back)
            {
                LoadDataAsync();
            }
        }
    }
}
