using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using Newtonsoft.Json;
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
    public class ReservationsFilterViewModel : BindableBase, INavigationAware
    {
        public List<SoccerPitchResponse> SoccerPitchs { get; set; }
        public List<PlansResponse> Plans { get; set; }
        public ObservableCollection<StatusResponse> Status { get; set; }
        public ObservableCollection<string> PlansNames { get; set; }
        public ObservableCollection<string> SoccerPitchsNames { get; set; }
        public ObservableCollection<string> StatusNames { get; set; }

        public DelegateCommand ApplyFilterCommand { get; set; }

        private INavigationService _navigationService;


        private int? _selectedPlan;
        public int? SelectedPlan
        {
            get { return _selectedPlan; }
            set { SetProperty(ref _selectedPlan, value); }
        }

        private int? _selectedSoccerPitch;
        public int? SelectedSoccerPitch
        {
            get { return _selectedSoccerPitch; }
            set { SetProperty(ref _selectedSoccerPitch, value); }
        }

        private DateTime _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private DateTime _finalDate = DateTime.Now;
        public DateTime FinalDate
        {
            get { return _finalDate; }
            set { SetProperty(ref _finalDate, value); }
        }

        private int _currentSoccerPitch = 0;
        private int _currentPlan = 0;
        private int[] _selectedStatus;
        public ReservationsFilterViewModel(INavigationService navigationService)
        {
            SoccerPitchs = new List<SoccerPitchResponse>();
            SoccerPitchsNames = new ObservableCollection<string>();
            Plans = new List<PlansResponse>();
            PlansNames = new ObservableCollection<string>();
            Status = new ObservableCollection<StatusResponse>();
            StatusNames = new ObservableCollection<string>();
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            _navigationService = navigationService;
        }

        private void ApplyFilter()
        {
            _navigationService.GoBackAsync(FillParameters());
        }

        private NavigationParameters FillParameters()
        {
            var navParams = new NavigationParameters();
            navParams.Add(nameof(StartDate), StartDate);
            navParams.Add(nameof(FinalDate), FinalDate);
            if (SelectedSoccerPitch.HasValue && SelectedSoccerPitch.Value <= SoccerPitchs.Count)
            {
                navParams.Add(nameof(SelectedSoccerPitch), SoccerPitchs[SelectedSoccerPitch.Value].Id);
            }
            if (SelectedPlan.HasValue && SelectedPlan.Value <= Plans.Count)
            {
                navParams.Add(nameof(SelectedPlan), Plans[SelectedPlan.Value].Id);
            }
            var selectedStatus = Status.Where(x => x.Selected).Select(x => x.Key).ToList();
            navParams.Add("SelectedStatus", JsonConvert.SerializeObject(selectedStatus));
            return navParams;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadData();
            if (parameters.ContainsKey("InitialDate"))
                StartDate = parameters.GetValue<DateTime>("InitialDate");
            if (parameters.ContainsKey("FinalDate"))
                FinalDate = parameters.GetValue<DateTime>("FinalDate");
            if (parameters.ContainsKey("SoccerPitchId"))
            {
                int soccerPitchId = 0;
                if (int.TryParse(parameters.GetValue<string>("SoccerPitchId"), out soccerPitchId))
                {
                    if (soccerPitchId != 0 && SoccerPitchs != null)
                    {
                        _currentSoccerPitch = soccerPitchId;
                        var selectedSoccerPitch = SoccerPitchs.Where(x => x.Id == soccerPitchId).FirstOrDefault();
                        if (selectedSoccerPitch != null)
                            SelectedSoccerPitch = SoccerPitchs.IndexOf(selectedSoccerPitch);
                    }
                }
            }
            if (parameters.ContainsKey("SoccerPitchPlanId"))
            {
                int planId = 0;
                if(int.TryParse(parameters.GetValue<string>("SoccerPitchPlanId"), out planId))
                {
                    if(planId != 0 && Plans != null)
                    {
                        _currentPlan = planId;
                        var selectedPlan = Plans.Where(x => x.Id == planId).FirstOrDefault();
                        if (selectedPlan != null)
                            SelectedPlan = Plans.IndexOf(selectedPlan);
                    }
                }
            }
            if(parameters.ContainsKey("SelectedStatus"))
            {
                var statusStr = parameters.GetValue<string>("SelectedStatus");
                if(string.IsNullOrEmpty(statusStr) == false)
                {
                    var selectedStatus = statusStr.Split(';');
                    List<int> status = new List<int>();
                    foreach (var item in selectedStatus)
                    {
                        int currentStatus = 0;
                        if(int.TryParse(item, out currentStatus))
                        {
                            if (currentStatus != 0)
                                status.Add(currentStatus);
                        }
                    }
                    _selectedStatus = status.ToArray();
                    if (Status != null)
                    {
                        foreach (var item in Status)
                        {
                            if (status.Contains(item.Key))
                                item.Selected = true;
                        }
                    }
                }
            }
        }

        private void LoadData()
        {
            LoadSoccerPitchAsync();
            LoadPlansAsync();
            LoadStatusAsync();
        }

        private async Task LoadSoccerPitchAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetSoccerPitchsAsync();
                SoccerPitchs.Clear();
                SoccerPitchsNames.Clear();
                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    foreach (var item in response.Data)
                    {
                        SoccerPitchs.Add(item);
                        SoccerPitchsNames.Add(item.Name);
                    }
                    if (_currentSoccerPitch != 0)
                    {
                        var selectedSoccerPitch = SoccerPitchs.Where(x => x.Id == _currentSoccerPitch).FirstOrDefault();
                        if (selectedSoccerPitch != null)
                            SelectedSoccerPitch = SoccerPitchs.IndexOf(selectedSoccerPitch);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async Task LoadPlansAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetPlansAsync();
                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    Plans.Clear();
                    PlansNames.Clear();
                    foreach (var item in response.Data)
                    {
                        Plans.Add(item);
                        PlansNames.Add(item.Name);
                    }
                    if (_currentPlan != 0)
                    {
                        var selectedPlan = Plans.Where(x => x.Id == _currentPlan).FirstOrDefault();
                        if (selectedPlan != null)
                            SelectedPlan = Plans.IndexOf(selectedPlan);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async Task LoadStatusAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetStatusAsync();
                if (response != null && response.Count > 0)
                {
                    Status.Clear();
                    StatusNames.Clear();
                    foreach (var item in response)
                    {
                        Status.Add(item);
                        StatusNames.Add(item.Text);
                    }
                    if (Status != null && _selectedStatus != null)
                    {
                        foreach (var item in Status)
                        {
                            if (_selectedStatus.Contains(item.Key))
                                item.Selected = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }
    }
}
