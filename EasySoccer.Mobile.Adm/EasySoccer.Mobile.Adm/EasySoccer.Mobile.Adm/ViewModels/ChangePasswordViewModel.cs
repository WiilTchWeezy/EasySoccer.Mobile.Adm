using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class ChangePasswordViewModel : BindableBase
    {
        private string _currentPassword;
        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { SetProperty(ref _currentPassword, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { SetProperty(ref _confirmPassword, value); }
        }

        public DelegateCommand SaveCommand { get; set; }
        private INavigationService _navigationService;
        public ChangePasswordViewModel(INavigationService navigationService)
        {
            SaveCommand = new DelegateCommand(Save);
            _navigationService = navigationService;
        }

        private async void Save()
        {
            try
            {
                if (Password.Equals(ConfirmPassword))
                {
                    if (await ApiClient.Instance.ChangePasswordAsync(new API.ApiRequest.ChangePasswordRequest { NewPassword = Password, OldPassword = CurrentPassword }))
                    {
                        UserDialogs.Instance.Alert("Senha alterada com sucesso!");
                        await _navigationService.GoBackAsync();
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Senha não alterada, verifique as informações!");
                    }
                }
                else
                {
                    UserDialogs.Instance.Alert("As senhas diferem, confirme a senha corretamente!");
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert("Senha não alterada, verifique as informações!");
            }
        }
    }
}
