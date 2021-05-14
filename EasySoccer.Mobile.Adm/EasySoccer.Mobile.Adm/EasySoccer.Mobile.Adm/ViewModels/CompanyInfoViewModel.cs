using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.Infra;
using EasySoccer.Mobile.Adm.Infra.Constants;
using EasySoccer.Mobile.Adm.Infra.Enums;
using EasySoccer.Mobile.Adm.Infra.Services;
using EasySoccer.Mobile.Adm.Infra.Services.DTO;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class CompanyInfoViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand SelectedImageCommand { get; set; }
        public DelegateCommand SearchPlacesCommand { get; set; }
        public DelegateCommand CurrentLocationCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand ActiveCommand { get; set; }
        public DelegateCommand NavigateToSchedulesCommand { get; set; }
        public DelegateCommand SelectStateCommand { get; set; }
        public DelegateCommand SelectCityCommand { get; set; }
        public ObservableCollection<string> HourStart { get; set; }
        public ObservableCollection<string> HourEnd { get; set; }
        public ObservableCollection<string> Days { get; set; }

        private IGooglePlacesService _googlePlacesService;
        private CompanyInfoResponse _companyInfoResponse = null;
        private INavigationService _navigationService;
        Action<PlaceDetail> onIntentResult;

        private double _lat;
        private double _long;
        public CompanyInfoViewModel(IGooglePlacesService googlePlacesService, INavigationService navigationService)
        {
            _googlePlacesService = googlePlacesService;
            _navigationService = navigationService;
            SelectedImageCommand = new DelegateCommand(SelectImage);
            SearchPlacesCommand = new DelegateCommand(SearchGoogleMaps);
            CurrentLocationCommand = new DelegateCommand(CurrentLocation);
            SaveCommand = new DelegateCommand(SaveAsync);
            ActiveCommand = new DelegateCommand(ActiveCompany);
            NavigateToSchedulesCommand = new DelegateCommand(NavigateToSchedule);
            HourStart = new ObservableCollection<string>();
            HourEnd = new ObservableCollection<string>();
            Days = new ObservableCollection<string>();
            SelectStateCommand = new DelegateCommand(SelectState);
            SelectCityCommand = new DelegateCommand(SelectCity);
            LoadDaysAndHours();
        }

        private void LoadDaysAndHours()
        {
            foreach (var item in Day.DaysNames)
            {
                Days.Add(item);
            }
            foreach (var item in Hour.HoursDescriptions)
            {
                HourStart.Add(item);
                HourEnd.Add(item);
            }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _cnpj;
        public string CNPJ
        {
            get { return _cnpj; }
            set { SetProperty(ref _cnpj, value); }
        }

        private string _completeAddress;
        public string CompleteAddress
        {
            get { return _completeAddress; }
            set { SetProperty(ref _completeAddress, value); }
        }

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        private double _latitude;
        public double Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    ActiveCompany();
                    if (value)
                        StatusText = "Ativo";
                    else
                        StatusText = "Inativo";
                }
            }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        private int? _selectedDay = null;
        public int? SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                if (SetProperty(ref _selectedDay, value))
                {
                    if (value.HasValue)
                        UpdateCompanyScheduleInfo(value.Value);
                }
            }
        }

        private int? _selectHourStart = null;
        public int? SelectHourStart
        {
            get { return _selectHourStart; }
            set
            {
                if (SetProperty(ref _selectHourStart, value))
                {
                    var companyScheduleRequest = GetCompanyScheduleRequest();
                    if (companyScheduleRequest != null && value.HasValue)
                    {
                        companyScheduleRequest.StartHour = value.Value;
                    }
                }
            }
        }

        private int? _selectHourEnd = null;
        public int? SelectHourEnd
        {
            get { return _selectHourEnd; }
            set
            {
                if (SetProperty(ref _selectHourEnd, value))
                {
                    var companyScheduleRequest = GetCompanyScheduleRequest();
                    if (companyScheduleRequest != null && value.HasValue)
                    {
                        companyScheduleRequest.FinalHour = value.Value;
                    }
                }
            }
        }

        private bool _workOnThisDay;
        public bool WorkOnThisDay
        {
            get { return _workOnThisDay; }
            set
            {
                if (SetProperty(ref _workOnThisDay, value))
                {
                    var companyScheduleRequest = GetCompanyScheduleRequest();
                    if (companyScheduleRequest != null)
                    {
                        companyScheduleRequest.WorkOnThisDay = value;
                    }
                }
            }
        }

        private int? _idCity;
        public int? IdCity
        {
            get { return _idCity; }
            set { SetProperty(ref _idCity, value); }
        }

        private int? _idState;
        public int? IdState
        {
            get { return _idState; }
            set { SetProperty(ref _idState, value); }
        }

        private string _stateName;
        public string StateName
        {
            get { return _stateName; }
            set { SetProperty(ref _stateName, value); }
        }

        private string _cityName;
        public string CityName
        {
            get { return _cityName; }
            set { SetProperty(ref _cityName, value); }
        }

        private string _planName;
        public string PlanName
        {
            get { return _planName; }
            set { SetProperty(ref _planName, value); }
        }

        private string _planCreatedDate;
        public string PlanCreatedDate
        {
            get { return _planCreatedDate; }
            set { SetProperty(ref _planCreatedDate, value); }
        }

        private string _planExpiresDates;
        public string PlanExpiresDates
        {
            get { return _planExpiresDates; }
            set { SetProperty(ref _planExpiresDates, value); }
        }

        private string _insertReservationConfirmedText;
        public string InsertReservationConfirmedText
        {
            get { return _insertReservationConfirmedText; }
            set { SetProperty(ref _insertReservationConfirmedText, value); }
        }

        private bool _insertReservationConfirmed;
        public bool InsertReservationConfirmed
        {
            get { return _insertReservationConfirmed; }
            set
            {
                if (SetProperty(ref _insertReservationConfirmed, value))
                {
                    if (value)
                        InsertReservationConfirmedText = "Sim";
                    else
                        InsertReservationConfirmedText = "Não";
                }
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var companyInfoResponse = await ApiClient.Instance.GetCompanyInfoAsync();
                if (companyInfoResponse != null)
                {
                    _companyInfoResponse = companyInfoResponse;
                    Image = Application.Instance.GetImage(companyInfoResponse.Logo, Infra.Enums.BlobContainerEnum.Company);
                    Name = companyInfoResponse.Name;
                    Description = companyInfoResponse.Description;
                    CNPJ = companyInfoResponse.CNPJ;
                    CompleteAddress = companyInfoResponse.CompleteAddress;
                    Longitude = companyInfoResponse.Longitude;
                    Latitude = companyInfoResponse.Latitude;
                    _lat = companyInfoResponse.Latitude;
                    _long = companyInfoResponse.Latitude;
                    IsActive = companyInfoResponse.Active;
                    IdState = companyInfoResponse.IdState;
                    IdCity = companyInfoResponse.IdCity;
                    CityName = companyInfoResponse.City;
                    StateName = companyInfoResponse.State;
                    InsertReservationConfirmed = companyInfoResponse.InsertReservationConfirmed;
                    if (IsActive)
                        StatusText = "Ativo";
                    else
                        StatusText = "Inativo";
                    if (InsertReservationConfirmed)
                        InsertReservationConfirmedText = "Sim";
                    else
                        InsertReservationConfirmedText = "Não";
                    SelectedDay = 0;
                    if (companyInfoResponse != null && companyInfoResponse.FinancialInfo != null)
                    {
                        switch (companyInfoResponse.FinancialInfo.FinancialPlan)
                        {
                            case 0:
                                PlanName = "Plano selecionado: Grátis";
                                break;
                            case 1:
                                PlanName = "Plano selecionado: Mensal";
                                break;
                            case 2:
                                PlanName = "Plano selecionado: Semestral";
                                break;
                            case 3:
                                PlanName = "Plano selecionado: Anual";
                                break;
                            default:
                                break;
                        }
                        PlanCreatedDate = $"Criado em : {companyInfoResponse.FinancialInfo.CreatedDate.ToString("dd/MM/yyyy")}";
                        PlanExpiresDates = $"Válido até : {companyInfoResponse.FinancialInfo.ExpiresDate.ToString("dd/MM/yyyy")}";
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void UpdateCompanyScheduleInfo(int day)
        {
            var companySchedule = _companyInfoResponse.CompanySchedules.Where(x => x.Day == day).FirstOrDefault();
            if (companySchedule != null)
            {
                SelectHourStart = (int)companySchedule.StartHour;
                SelectHourEnd = (int)companySchedule.FinalHour;
                WorkOnThisDay = companySchedule.WorkOnThisDay;
            }
        }

        private CompanyDaySchedule GetCompanyScheduleRequest()
        {
            return _companyInfoResponse.CompanySchedules.Where(x => x.Day == SelectedDay).FirstOrDefault();
        }

        private async void SelectImage()
        {
            try
            {
                var mediaResponse = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Selecione uma imagem" });
                if (mediaResponse != null)
                {
                    var stream = await mediaResponse.OpenReadAsync();
                    byte[] bytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    if (bytes != null)
                    {
                        string base64 = Convert.ToBase64String(bytes);
                        if (string.IsNullOrEmpty(base64) == false)
                        {
                            var apiResponse = await ApiClient.Instance.PostCompanyImageAsync(new API.ApiRequest.CompanyImageRequest { ImageBase64 = base64 });
                            LoadDataAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void SearchGoogleMaps()
        {
            onIntentResult = (placeDetail) =>
            {
                _lat = placeDetail.Latitude;
                _long = placeDetail.Longitude;
                this.Longitude = placeDetail.Longitude;
                this.Latitude = placeDetail.Latitude;
            };
            _googlePlacesService.DisplayIntent(onIntentResult);
        }

        private async void CurrentLocation()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                _lat = location.Latitude;
                _long = location.Longitude;
                this.Longitude = location.Longitude;
                this.Latitude = location.Latitude;
            }
        }

        private async void SaveAsync()
        {
            try
            {
                await ApiClient.Instance.PatchCompanyAsync(new API.ApiRequest.PatchCompanyInfoRequest
                {
                    CNPJ = this.CNPJ,
                    CompleteAddress = this.CompleteAddress,
                    Description = this.Description,
                    Latitude = _lat,
                    Longitude = _long,
                    Name = this.Name,
                    IdCity = this.IdCity,
                    InsertReservationConfirmed = InsertReservationConfirmed,
                    CompanySchedules = _companyInfoResponse.CompanySchedules,
                });
                UserDialogs.Instance.Alert("Dados atualizados com sucesso.");

            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void ActiveCompany()
        {
            try
            {
                if (_companyInfoResponse != null && _companyInfoResponse.Active != IsActive)
                    await ApiClient.Instance.ActiveAsync(IsActive);
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void SelectState()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ModalSelectType", ModalSelectEnum.State);
            _navigationService.NavigateAsync("ModalSelect", navigationParameters, useModalNavigation: true);
        }
        private void SelectCity()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ModalSelectType", ModalSelectEnum.City);
            navigationParameters.Add("StateId", IdState);
            _navigationService.NavigateAsync("ModalSelect", navigationParameters, useModalNavigation: true);
        }

        private async void NavigateToSchedule()
        {
            await _navigationService.NavigateAsync("CompanySchedules");
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() != Prism.Navigation.NavigationMode.Back)
                this.LoadDataAsync();
            if (parameters.ContainsKey("StateName"))
                StateName = parameters.GetValue<string>("StateName");
            if (parameters.ContainsKey("CityName"))
                CityName = parameters.GetValue<string>("CityName");
            if (parameters.ContainsKey("StateId"))
                IdState = parameters.GetValue<int>("StateId");
            if (parameters.ContainsKey("CityId"))
                IdCity = parameters.GetValue<int>("CityId");
        }
    }
}
