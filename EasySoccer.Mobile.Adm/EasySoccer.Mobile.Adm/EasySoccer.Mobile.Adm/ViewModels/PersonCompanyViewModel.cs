using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class PersonCompanyViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<PersonCompany> Persons { get; set; }
        public DelegateCommand ItemTresholdCommand { get; set; }
        public DelegateCommand ItemSelectedCommand { get; set; }
        public DelegateCommand FilterCommand { get; set; }
        public DelegateCommand AddPersonCompanyCommand { get; set; }

        private INavigationService _navigationService;
        public PersonCompanyViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Persons = new ObservableCollection<PersonCompany>();
            ItemTresholdCommand = new DelegateCommand(ItemTreshold);
            ItemSelectedCommand = new DelegateCommand(ItemSelected);
            FilterCommand = new DelegateCommand(OpenFilter);
            AddPersonCompanyCommand = new DelegateCommand(AddPersonCompany);
        }

        private void ItemTreshold()
        {
            if (Persons.Count > 0 && _hasMoreData && _isBusy == false)
            {
                LoadDataAsync(false);
            }
        }

        private void ItemSelected()
        {
            if (SelectedItem != null)
            {
                var navParams = new NavigationParameters();
                navParams.Add("Id", SelectedItem.Id);
                _navigationService.NavigateAsync("PersonCompanyDetail", navParams);
            }
        }

        private void AddPersonCompany()
        {
            _navigationService.NavigateAsync("PersonCompanyDetail");
        }

        private void OpenFilter()
        {
            _navigationService.NavigateAsync("PersonCompanyFilter");
        }
        private string _name;
        private string _phone;
        private string _email;

        private PersonCompany _selectedItem;
        public PersonCompany SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private int _page = 1;
        public int Page
        {
            get { return _page; }
            set { SetProperty(ref _page, value); }
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set { SetProperty(ref _pageSize, value); }
        }

        private bool _isBusy = false;
        private bool _hasMoreData = true;
        private async Task LoadDataAsync(bool clear = true)
        {
            try
            {
                _isBusy = true;
                var response = await ApiClient.Instance.GetPersonCompanyAsync(_name, _email, _phone, Page, PageSize);
                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    _isBusy = false;
                    Page++;
                    _hasMoreData = true;
                    if (clear)
                        Persons.Clear();
                    foreach (var item in response.Data)
                    {
                        Persons.Add(item);
                    }
                }
                else
                {
                    _isBusy = false;
                    _hasMoreData = false;
                    if (clear)
                        Persons.Clear();
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Page = 1;
            if (parameters.ContainsKey("Name"))
                _name = parameters.GetValue<string>("Name");
            if (parameters.ContainsKey("Phone"))
                _phone = parameters.GetValue<string>("Phone");
            if (parameters.ContainsKey("Email"))
                _email = parameters.GetValue<string>("Email");
            LoadDataAsync();
        }
    }
}
