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

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class ReservationsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<ReservationResponse> Reservations { get; set; }
        public DelegateCommand ReservationFilterCommand { get; set; }
        private INavigationService _navigationService;
        public ReservationsViewModel(INavigationService navigationService)
        {
            Reservations = new ObservableCollection<ReservationResponse>();
            ReservationFilterCommand = new DelegateCommand(OpenFilter);
            _navigationService = navigationService;
        }

        private void OpenFilter()
        {
            var navParams = new NavigationParameters();
            navParams.Add(nameof(InitialDate), InitialDate);
            navParams.Add(nameof(FinalDate), FinalDate);
            navParams.Add(nameof(SoccerPitchId), SoccerPitchId);
            navParams.Add(nameof(SoccerPitchPlanId), SoccerPitchPlanId);
            navParams.Add(nameof(UserName), UserName);
            navParams.Add(nameof(Status), Status);
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

        private int? _soccerPitchId;
        public int? SoccerPitchId
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
        private async void LoadDataAsync(bool clear = true)
        {
            try
            {
                var response = await ApiClient.Instance.GetReservationsAsync(InitialDate, FinalDate, SoccerPitchId, SoccerPitchPlanId, UserName, Status, Page, PageSize);
                if(response != null && response.Data != null && response.Data.Count > 0)
                {
                    if (clear)
                        Reservations.Clear();
                    foreach (var item in response.Data)
                    {
                        Reservations.Add(item);
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
            LoadDataAsync();
        }
    }
}
