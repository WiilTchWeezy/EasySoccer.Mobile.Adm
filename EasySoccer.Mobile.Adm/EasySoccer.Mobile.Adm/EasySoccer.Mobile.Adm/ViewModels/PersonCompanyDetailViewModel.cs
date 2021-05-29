using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiRequest;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class PersonCompanyDetailViewModel : BindableBase, INavigationAware
    {
        private Guid _id;
        private bool _isEditting;

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

        private bool _showLastReservations;
        public bool ShowLastReservations
        {
            get { return _showLastReservations; }
            set { SetProperty(ref _showLastReservations, value); }
        }
        public ObservableCollection<ReservationsInfoResponse> Reservations { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        private INavigationService _navigationService;
        public PersonCompanyDetailViewModel(INavigationService navigationService)
        {
            Reservations = new ObservableCollection<ReservationsInfoResponse>();
            SaveCommand = new DelegateCommand(SaveAsync);
            _navigationService = navigationService;
        }

        private async void LoadDataAsync()
        {
            try
            {
                var personInfoResponse = await ApiClient.Instance.GetPersonCompanyInfoAsync(_id);
                if (personInfoResponse != null)
                {
                    Name = personInfoResponse.Name;
                    Email = personInfoResponse.Email;
                    Phone = personInfoResponse.Phone;
                    if (personInfoResponse.Reservations != null && personInfoResponse.Reservations.Count > 0)
                    {
                        foreach (var item in personInfoResponse.Reservations)
                        {
                            Reservations.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void SaveAsync()
        {
            try
            {
                var request = new PersonCompanyRequest
                {
                    Email = Email,
                    Name = Name,
                    Phone = Phone,
                    Id = _id
                };
                if (_isEditting)
                {
                    var response = await ApiClient.Instance.PatchPersonCompanyAsync(request);
                    if(response != null)
                    {
                        UserDialogs.Instance.Alert("Cliente inserido com sucesso!");
                        await _navigationService.GoBackAsync();
                    }
                }
                else
                {
                    var response = await ApiClient.Instance.PostPersonCompanyAsync(request);
                    if (response != null)
                    {
                        UserDialogs.Instance.Alert("Cliente atualizado com sucesso!");
                        await _navigationService.GoBackAsync();
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
            if (parameters.ContainsKey("Id"))
            {
                _id = parameters.GetValue<Guid>("Id");
                LoadDataAsync();
                _isEditting = true;
                ShowLastReservations = true;
            }
            else
            {
                _isEditting = false;
                ShowLastReservations = false;
            }
        }
    }
}
