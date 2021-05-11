using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API.Events;
using EasySoccer.Mobile.Adm.API.Session;
using EasySoccer.Mobile.Adm.Infra;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand<string> NavigatePageCommand { get; set; }
        public DelegateCommand<string> OpenLinkCommand { get; set; }
        private INavigationService _navigationService;
        private IEventAggregator _eventAggregator;
        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            NavigatePageCommand = new DelegateCommand<string>(NavigatePage);
            OpenLinkCommand = new DelegateCommand<string>(OpenLink);
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<UserLoggedInEvent>().Subscribe(UserHasLoggedIn);
            UserLoggedIn = CurrentUser.Instance.IsLoggedIn;
            CurrentUser.Instance.SetEventAggregator(_eventAggregator);
        }

        private void NavigatePage(string page)
        {
            if (page == "Logout")
            {
                var confirmConfig = new ConfirmConfig()
                {
                    CancelText = "Cancelar",
                    Message = "Deseja realmente sair?",
                    OkText = "Sair",
                    Title = "EasySoccer",
                    OnAction = (selection) =>
                    {
                        if (selection)
                            Application.Instance.LogOff(_navigationService);
                    }
                };
                UserDialogs.Instance.Confirm(confirmConfig);
            }
            else if (page == "Login")
            {
                _navigationService.NavigateAsync("Login", useModalNavigation: true);
            }
            else
                _navigationService.NavigateAsync("NavigationPage/" + page);
        }

        private async void OpenLink(string url)
        {
            var confirmConfig = new ConfirmConfig()
            {
                CancelText = "Fale conosco",
                Message = "Como podemos te ajudar ?",
                OkText = "Manual da Plataforma",
                Title = "EasySoccer"
            };
            var response = await UserDialogs.Instance.ConfirmAsync(confirmConfig);
            if (response)
                await Browser.OpenAsync("https://www.easysoccer.com.br/documents/ManualEasySoccerv1.0.pdf", BrowserLaunchMode.SystemPreferred);
            else
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
        }

        private void UserHasLoggedIn(bool payLoad)
        {
            this.UserLoggedIn = payLoad;
        }

        private bool _userLoggedIn;
        public bool UserLoggedIn
        {
            get { return _userLoggedIn; }
            set { SetProperty(ref _userLoggedIn, value); }
        }
    }
}
