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
using System.Threading.Tasks;

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

        private bool _saveButtonIsVisible;
        public bool SaveButtonIsVisible
        {
            get { return _saveButtonIsVisible; }
            set { SetProperty(ref _saveButtonIsVisible, value); }
        }

        private string _statusDescription;
        public string StatusDescription
        {
            get { return _statusDescription; }
            set { SetProperty(ref _statusDescription, value); }
        }

        private string _statusColor;
        public string StatusColor
        {
            get { return _statusColor; }
            set { SetProperty(ref _statusColor, value); }
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
                    PostReservationResponse response;
                    if (IsEditing)
                    {
                        request.Id = _reservationId;
                        response = await ApiClient.Instance.PatchReservationAsync(request);
                    }
                    else
                        response = await ApiClient.Instance.PostReservationAsync(request);

                    if (response != null)
                    {
                        var text = IsEditing ? "alterados" : "inseridos";
                        UserDialogs.Instance.Alert($"Dados {text} com sucesso!");
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

        private async Task LoadPlansAsync()
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
                            Plans.Clear();
                            PlansName.Clear();
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

        private async Task LoadSoccerPitchs()
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

        private async void LoadReservationInfo()
        {
            try
            {
                var reservationInfo = await ApiClient.Instance.GetReservationInfoAsync(_reservationId);
                if (reservationInfo != null)
                {
                    await LoadSoccerPitchs();
                    if (SoccerPitches != null && SoccerPitches.Any())
                    {
                        var currentSoccerPitch = SoccerPitches.Where(z => z.Id == reservationInfo.SoccerPitchId).FirstOrDefault();
                        if (currentSoccerPitch != null)
                        {
                            SelectedSoccerPitch = SoccerPitches.IndexOf(currentSoccerPitch);
                            await LoadPlansAsync();
                        }
                    }
                    if (Plans != null && Plans.Any())
                    {
                        var currentPlan = Plans.Where(x => x.Id == reservationInfo.SoccerPitchPlanId).FirstOrDefault();
                        if (currentPlan != null)
                            SelectedPlanIndex = Plans.IndexOf(currentPlan);
                    }
                    StartHour = reservationInfo.SelectedDateStart.ToString("HH");
                    StartMinute = reservationInfo.SelectedDateStart.ToString("mm");
                    EndHour = reservationInfo.SelectedDateEnd.ToString("HH");
                    EndMinute = reservationInfo.SelectedDateEnd.ToString("mm");
                    SelectedDate = reservationInfo.SelectedDateStart;
                    StatusDescription = reservationInfo.StatusDescription;
                    StatusColor = reservationInfo.StatusColor;
                    if (IsEditing == false)
                        SaveButtonIsVisible = true;
                    else
                        SaveButtonIsVisible = reservationInfo.Status == 1 || reservationInfo.Status == 3;
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
                LoadReservationInfo();
            }
            else
            {
                SaveButtonIsVisible = true;
                LoadSoccerPitchs();
            }
        }
    }
}
