using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.Infra.DTO;
using EasySoccer.Mobile.Adm.Infra.Enums;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class ModalSelectViewModel : BindableBase, INavigationAware
    {
        private ModalSelectItem _selectedItem;
        public ModalSelectItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set { SetProperty(ref _headerText, value); }
        }

        public ObservableCollection<ModalSelectItem> Itens { get; set; }
        public List<ModalSelectItem> _itens { get; set; }
        private int? _stateId;
        private ModalSelectEnum _currentModalType;
        private string _pageToNavigate;
        private bool _useApiSearch = false;
        public DelegateCommand OnSelectItemCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        private INavigationService _navigationService;

        public ModalSelectViewModel(INavigationService navigationService)
        {
            Itens = new ObservableCollection<ModalSelectItem>();
            _itens = new List<ModalSelectItem>();
            OnSelectItemCommand = new DelegateCommand(OnItemSelect);
            AddCommand = new DelegateCommand(Add);
            SearchCommand = new DelegateCommand(() => { this.Search(this.SearchText); });
            _navigationService = navigationService;
        }

        private void Add()
        {
            _navigationService.NavigateAsync(_pageToNavigate);
        }

        private void OnItemSelect()
        {
            var navigationParameters = new NavigationParameters();
            switch (_currentModalType)
            {
                case ModalSelectEnum.State:
                    navigationParameters.Add("StateId", SelectedItem.Id);
                    navigationParameters.Add("StateName", SelectedItem.Text);
                    break;
                case ModalSelectEnum.City:
                    navigationParameters.Add("CityId", SelectedItem.Id);
                    navigationParameters.Add("CityName", SelectedItem.Text);
                    break;
                case ModalSelectEnum.PersonCompany:
                    navigationParameters.Add("PersonId", SelectedItem.Identifier);
                    navigationParameters.Add("PersonName", SelectedItem.Text);
                    break;
                default:
                    break;
            }
            _navigationService.GoBackAsync(navigationParameters);
        }

        public void Search(string text)
        {
            Itens.Clear();
            if (_useApiSearch) 
            {
                if (_currentModalType == ModalSelectEnum.PersonCompany)
                    this.LoadPersonCompanyAsync(text, true);
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                {
                    foreach (var item in _itens)
                    {
                        Itens.Add(item);
                    }
                }
                else
                {
                    var filteredItens = _itens.Where(x => x.Text.ToLower().Contains(text.ToLower())).OrderBy(x => x.Text).ToList();
                    foreach (var item in filteredItens)
                    {
                        Itens.Add(item);
                    }
                }
            }
        }

        private void LoadData(ModalSelectEnum modalSelect)
        {
            _currentModalType = modalSelect;
            switch (modalSelect)
            {
                case ModalSelectEnum.State:
                    this.LoadStatesAsync();
                    break;
                case ModalSelectEnum.City:
                    if (_stateId.HasValue)
                    {
                        this.LoadCitiesAsync(_stateId.Value);
                    }
                    break;
                case ModalSelectEnum.PersonCompany:
                    this.LoadPersonCompanyAsync();
                    break;
                default:
                    break;
            }
        }

        private async void LoadStatesAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetStatesAsync();
                if (response != null)
                {
                    foreach (var item in response)
                    {

                        Itens.Add(new ModalSelectItem { Text = item.Name, Id = item.Id });
                        _itens.Add(new ModalSelectItem { Text = item.Name, Id = item.Id });
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadCitiesAsync(int stateId)
        {
            try
            {
                var response = await ApiClient.Instance.GetCitiesAsync(stateId);
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        Itens.Add(new ModalSelectItem { Text = item.Name, Id = item.Id });
                        _itens.Add(new ModalSelectItem { Text = item.Name, Id = item.Id });
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadPersonCompanyAsync(string name = "", bool clearItens = false)
        {
            try
            {
                var personCompanyResponse = await ApiClient.Instance.GetPersonCompanyAsync(name, string.Empty, string.Empty, 1, 50);
                if (personCompanyResponse != null && personCompanyResponse.Data != null && personCompanyResponse.Data.Count > 0)
                {
                    if (clearItens)
                    {
                        Itens.Clear();
                        _itens.Clear();
                    }
                    foreach (var item in personCompanyResponse.Data)
                    {
                        Itens.Add(new ModalSelectItem { Text = item.Name, Identifier = item.Id });
                        _itens.Add(new ModalSelectItem { Text = item.Name, Identifier = item.Id });
                    }
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
            if (parameters.ContainsKey("StateId"))
            {
                _stateId = parameters.GetValue<int>("StateId");
            }
            if (parameters.ContainsKey("ModalSelectType"))
            {
                var modalSelect = parameters.GetValue<ModalSelectEnum>("ModalSelectType");
                LoadData(modalSelect);
            }
            if (parameters.ContainsKey("PageToNavigate"))
            {
                _pageToNavigate = parameters.GetValue<string>("PageToNavigate");
            }
            if (parameters.ContainsKey("UseApiSearch"))
            {
                _useApiSearch = parameters.GetValue<bool>("UseApiSearch");
            }
            if (parameters.ContainsKey("HeaderText"))
            {
                HeaderText = parameters.GetValue<string>("HeaderText");
            }
            if(_currentModalType == ModalSelectEnum.PersonCompany)
            {
                this.LoadPersonCompanyAsync(string.Empty, true);
            }
        }
    }
}
