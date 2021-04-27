using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Session;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class ReservationEditAddViewModel : BindableBase, INavigationAware
    {
        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set { SetProperty(ref _isEditing, value); }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }
        private int? _selectedSoccerPitch;
        public int? SelectedSoccerPitch
        {
            get { return _selectedSoccerPitch; }
            set
            {
                if (SetProperty(ref _selectedSoccerPitch, value))
                    LoadPlansAsync();
            }
        }

        private string _startHour;
        public string StartHour
        {
            get { return _startHour; }
            set { SetProperty(ref _startHour, value); }
        }

        private string _startMinute;
        public string StartMinute
        {
            get { return _startMinute; }
            set { SetProperty(ref _startMinute, value); }
        }

        private string _endHour;
        public string EndHour
        {
            get { return _endHour; }
            set { SetProperty(ref _endHour, value); }
        }

        private string _endMinute;
        public string EndMinute
        {
            get { return _endMinute; }
            set { SetProperty(ref _endMinute, value); }
        }

        private int? _selectedPlanIndex;
        public int? SelectedPlanIndex
        {
            get { return _selectedPlanIndex; }
            set { SetProperty(ref _selectedPlanIndex, value); }
        }

        private Guid _reservationId;

        private List<SoccerPitchResponse> SoccerPitches = new List<SoccerPitchResponse>();
        private List<PlansResponse> Plans = new List<PlansResponse>();
        public ObservableCollection<string> SoccerPitchesName { get; set; }
        public ObservableCollection<string> PlansName { get; set; }

        public DelegateCommand SaveCommand { get; set; }
        private INavigationService _navigationService;
        public ReservationEditAddViewModel(INavigationService navigationService)
        {
            SoccerPitchesName = new ObservableCollection<string>();
            PlansName = new ObservableCollection<string>();
            _navigationService = navigationService;
            SaveCommand = new DelegateCommand(SaveAsync);
            SelectedDate = DateTime.Now;
        }

        private async void SaveAsync()
        {
            try
            {
                var request = new API.ApiRequest.SoccerPitchReservationRequest { };

                TimeSpan startHour, endHour;
                if (Validate(out startHour, out endHour))
                {
                    var currentSoccerPitch = SoccerPitches[SelectedSoccerPitch.Value];
                    var currentPlan = Plans[SelectedPlanIndex.Value];
                    if (currentSoccerPitch != null && currentPlan != null)
                    {
                        request.HourEnd = endHour;
                        request.HourStart = startHour;
                        request.SelectedDate = SelectedDate;
                        request.SoccerPitchId = currentSoccerPitch.Id;
                        request.SoccerPitchPlanId = currentPlan.Id;
                    }
                    var response = await ApiClient.Instance.PostReservationAsync(request);
                    if (response != null)
                    {
                        var navParams = new NavigationParameters();
                        navParams.Add("ReservationId", response.Id);
                        await _navigationService.GoBackAsync(navParams);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private bool Validate(out TimeSpan startHour, out TimeSpan endHour)
        {
            bool valid = true;
            if (SelectedSoccerPitch.HasValue == false || SoccerPitches.Count < SelectedSoccerPitch.Value)
            {
                UserDialogs.Instance.Alert("Selecione um quadra.");
                valid = false;
            }
            if (SelectedPlanIndex.HasValue == false || Plans.Count < SelectedPlanIndex.Value)
            {
                UserDialogs.Instance.Alert("Selecione um plano.");
                valid = false;
            }
            if (!TimeSpan.TryParse($"{StartHour}:{StartMinute}", out startHour))
            {
                UserDialogs.Instance.Alert("É necessário informar um horário válido.");
                valid = false;
            }
            if (!TimeSpan.TryParse($"{EndHour}:{EndMinute}", out endHour))
            {
                UserDialogs.Instance.Alert("É necessário informar um horário válido.");
                valid = false;
            }
            if (endHour < startHour)
            {
                UserDialogs.Instance.Alert("O horário final deve ser maior que o inicial.");
                valid = false;
            }
            return valid;
        }

        private async void LoadPlansAsync()
        {
            try
            {
                if (SelectedSoccerPitch.HasValue && SoccerPitches.Count > SelectedSoccerPitch.Value && SelectedSoccerPitch.Value >= 0)
                {
                    var currentSoccerPitch = SoccerPitches[SelectedSoccerPitch.Value];
                    if (currentSoccerPitch != null)
                    {
                        var plansResponse = await ApiClient.Instance.GetPlansBySoccerPitchIdAsync(currentSoccerPitch.Id);
                        if (plansResponse != null && plansResponse.Any())
                        {
                            foreach (var item in plansResponse)
                            {
                                Plans.Add(item);
                                PlansName.Add(item.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadSoccerPitchs()
        {
            try
            {
                var soccerPitchs = await ApiClient.Instance.GetSoccerPitchsAsync();
                if (soccerPitchs != null && soccerPitchs.Any())
                {
                    foreach (var item in soccerPitchs)
                    {
                        SoccerPitches.Add(item);
                        SoccerPitchesName.Add(item.Name);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadHoursAsync()
        {
            try
            {
                if (CurrentUser.Instance.CompanyId.HasValue)
                {
                    var companyHoursResponse = await ApiClient.Instance.GetCompanyHourStartEndAsync(CurrentUser.Instance.CompanyId.Value, (int)SelectedDate.DayOfWeek);
                    if (companyHoursResponse != null && companyHoursResponse.Data != null && companyHoursResponse.Data.Any())
                    {

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
            if (parameters.ContainsKey("ReservationId"))
            {
                IsEditing = true;
                _reservationId = parameters.GetValue<Guid>("ReservationId");
            }
            LoadHoursAsync();
            LoadSoccerPitchs();
        }
    }
}
