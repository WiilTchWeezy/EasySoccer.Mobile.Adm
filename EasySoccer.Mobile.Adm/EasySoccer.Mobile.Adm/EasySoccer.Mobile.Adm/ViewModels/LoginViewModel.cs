using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        public DelegateCommand SignUpCommand { get; set; }

        private INavigationService _navigationService;
        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SignUpCommand = new DelegateCommand(NavigateSignUp);
        }

        private void NavigateSignUp()
        {
            _navigationService.NavigateAsync("SignUp");
        }
    }
}
