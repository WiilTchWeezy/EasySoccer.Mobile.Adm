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

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class ReservationsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<ReservationResponse> Reservations { get; set; }
        public DelegateCommand ReservationFilterCommand { get; set; }
        public DelegateCommand ItemTresholdCommand { get; set; }
        private INavigationService _navigationService;
        private int[] _selectedStatus;
        private bool _hasMoreData = true;
        private bool _isBusy = false;

        public ReservationsViewModel(INavigationService navigationService)
        {
            Reservations = new ObservableCollection<ReservationResponse>();
            ReservationFilterCommand = new DelegateCommand(OpenFilter);
            ItemTresholdCommand = new DelegateCommand(ItemTreshold);
            _navigationService = navigationService;
        }

        private async void ItemTreshold()
        {
            if (Reservations.Count > 0 && _hasMoreData && _isBusy == false)
            {
                LoadDataAsync(_selectedStatus, false);
            }
        }

        private void OpenFilter()
        {
            var navParams = new NavigationParameters();
            navParams.Add(nameof(InitialDate), InitialDate);
            navParams.Add(nameof(FinalDate), FinalDate);
            navParams.Add(nameof(SoccerPitchId), SoccerPitchId);
            navParams.Add(nameof(SoccerPitchPlanId), SoccerPitchPlanId);
            navParams.Add(nameof(UserName), UserName);
            if (_selectedStatus != null)
            {
                var statusStr = string.Empty;
                statusStr = string.Join(";", _selectedStatus);
                navParams.Add("SelectedStatus", statusStr);
            }
            _navigationService.NavigateAsync("ReservationsFilter", navParams);
        }

        private DateTime? _initialDate;
        public DateTime? InitialDate
        {
            get { return _initialDate; }
            set { SetProperty(ref _initialDate, value); }
        }

        private DateTime? _finalDate;
        public DateTime? FinalDate
        {
            get { return _finalDate; }
            set { SetProperty(ref _finalDate, value); }
        }

        private long? _soccerPitchId;
        public long? SoccerPitchId
        {
            get { return _soccerPitchId; }
            set { SetProperty(ref _soccerPitchId, value); }
        }

        private int? _soccerPitchPlanId;
        public int? SoccerPitchPlanId
        {
            get { return _soccerPitchPlanId; }
            set { SetProperty(ref _soccerPitchPlanId, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private int? _status;
        public int? Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
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
        private async void LoadDataAsync(int[] status = null, bool clear = true)
        {
            try
            {
                _isBusy = true;
                var response = await ApiClient.Instance.GetReservationsAsync(InitialDate, FinalDate, SoccerPitchId, SoccerPitchPlanId, UserName, status, Page, PageSize);
                if (response != null && response.Data != null && response.Data.Count > 0)
                {
                    _isBusy = false;
                    Page++;
                    _hasMoreData = true;
                    if (clear)
                    {
                        Reservations.Clear();
                    }
                    foreach (var item in response.Data)
                    {
                        Reservations.Add(item);
                    }
                }
                else
                {
                    _isBusy = false;
                    _hasMoreData = false;
                }
            }
            catch (Exception e)
            {
                _isBusy = false;
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.ContainsKey("StartDate"))
                    InitialDate = parameters.GetValue<DateTime?>("StartDate");
                else
                    InitialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                if (parameters.ContainsKey("FinalDate"))
                    FinalDate = parameters.GetValue<DateTime?>("FinalDate");
                else
                    FinalDate = DateTime.Now;
                if (parameters.ContainsKey("SelectedSoccerPitch"))
                    SoccerPitchId = parameters.GetValue<long?>("SelectedSoccerPitch");
                else
                    SoccerPitchId = null;
                if (parameters.ContainsKey("SelectedPlan"))
                    SoccerPitchPlanId = parameters.GetValue<int?>("SelectedPlan");
                else
                    SoccerPitchPlanId = null;
                int[] status = null;
                if (parameters.ContainsKey("SelectedStatus"))
                {
                    var statusStr = parameters.GetValue<string>("SelectedStatus");
                    status = JsonConvert.DeserializeObject<int[]>(statusStr);
                    _selectedStatus = status;
                }
                else
                {
                    status = new int[] { 1, 3, 4 };
                    _selectedStatus = status;
                }
                Page = 1;
                LoadDataAsync(status);
            }
            catch (Exception E)
            {

            }
        }
    }
}
