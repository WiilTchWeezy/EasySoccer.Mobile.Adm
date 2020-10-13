using Prism;
using Prism.Ioc;
using EasySoccer.Mobile.Adm.ViewModels;
using EasySoccer.Mobile.Adm.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using EasySoccer.Mobile.Adm.API.Session;

namespace EasySoccer.Mobile.Adm
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            if (CurrentUser.Instance.IsLoggedIn)
                await NavigationService.NavigateAsync("MainPage/NavigationPage/CompanyReservations");
            else
                await NavigationService.NavigateAsync("Login");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<Login, LoginViewModel>();
            containerRegistry.RegisterForNavigation<SignUp, SignUpViewModel>();
            containerRegistry.RegisterForNavigation<CompanyReservations, CompanyReservationsViewModel>();
            containerRegistry.RegisterForNavigation<CompanyInfo, CompanyInfoViewModel>();
            containerRegistry.RegisterForNavigation<ModalSelect, ModalSelectViewModel>();
            containerRegistry.RegisterForNavigation<UserInfo, UserInfoViewModel>();
            containerRegistry.RegisterForNavigation<ChangePassword, ChangePasswordViewModel>();
        }
    }
}
