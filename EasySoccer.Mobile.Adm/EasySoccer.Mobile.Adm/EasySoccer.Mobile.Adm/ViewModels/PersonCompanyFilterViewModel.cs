using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class PersonCompanyFilterViewModel : BindableBase
    {
        private INavigationService _navigationService;
        public DelegateCommand ApplyCommand { get; set; }
        public PersonCompanyFilterViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ApplyCommand = new DelegateCommand(ApplyFilters);
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set { SetProperty(ref _phone, value); }
        }

        private void ApplyFilters()
        {
            var navParams = new NavigationParameters();
            navParams.Add("Name", Name);
            navParams.Add("Phone", Phone);
            navParams.Add("Email", Email);
            _navigationService.GoBackAsync(navParams);
        }
    }
}
