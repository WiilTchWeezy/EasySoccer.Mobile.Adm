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

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        private TimeSpan _startHour;
        public TimeSpan StartHour
        {
            get { return _startHour; }
            set { SetProperty(ref _startHour, value); }
        }

        private TimeSpan _endHour;
        public TimeSpan EndHour
        {
            get { return _endHour; }
            set { SetProperty(ref _endHour, value); }
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

        private Guid _reservationId;

        private List<SoccerPitchResponse> SoccerPitches = new List<SoccerPitchResponse>();
        private List<PlansResponse> Plans = new List<PlansResponse>();
        public ObservableCollection<string> SoccerPitchesName = new ObservableCollection<string>();
        public ObservableCollection<string> PlansName = new ObservableCollection<string>();
        public ReservationEditAddViewModel()
        {

        }

        private async void LoadPlansAsync()
        {
            try
            {
                if (SelectedSoccerPitch.HasValue && SoccerPitches.Count > SelectedSoccerPitch.Value)
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
                        TimeSpan startHour;
                        TimeSpan endHour;
                        if (TimeSpan.TryParse(companyHoursResponse.Data.First(), out startHour))
                            StartHour = startHour;
                        if (TimeSpan.TryParse(companyHoursResponse.Data.Last(), out endHour))
                            EndHour = endHour;
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
