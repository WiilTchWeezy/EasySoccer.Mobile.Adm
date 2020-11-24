using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Infra.Exceptions;
using EasySoccer.Mobile.Adm.API.Session;
using EasySoccer.Mobile.Adm.Infra;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class CompanyReservationsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<CompanySchedulesResponse> CompanySchedules { get; set; }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    this.LoadDataAsync();
                }
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

        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        private string _completeAddress;
        public string CompleteAddress
        {
            get { return _completeAddress; }
            set { SetProperty(ref _completeAddress, value); }
        }
        private INavigationService _navigationService;
        public CompanyReservationsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CompanySchedules = new ObservableCollection<CompanySchedulesResponse>();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var companySchedules = await ApiClient.Instance.GetCompanySchedulesAsync(SelectedDate);
                if (companySchedules != null && companySchedules.Count > 0)
                {
                    CompanySchedules.Clear();
                    foreach (var item in companySchedules)
                    {
                        var schedule = new CompanySchedulesResponse { Hour = item.Hour, HourSpan = item.HourSpan, Events = new ObservableCollection<CompanySchedulesEventResponse>() };
                        foreach (var e in item.Events)
                        {
                            schedule.Events.Add(new CompanySchedulesEventResponse
                            {
                                GetReservationInfo = new DelegateCommand<Guid?>(OpenReservationInfo),
                                HasReservation = e.HasReservation,
                                PersonName = e.PersonName,
                                ScheduleHour = e.ScheduleHour,
                                SoccerPitch = e.SoccerPitch,
                                SoccerPitchId = e.SoccerPitchId,
                                SoccerPitchReservationId = e.SoccerPitchReservationId
                            });
                        }
                        CompanySchedules.Add(schedule);
                    }
                }
                var companyInfo = await ApiClient.Instance.GetCompanyInfoAsync();
                if (companyInfo != null)
                {
                    Image = Application.Instance.GetImage(companyInfo.Logo, Infra.Enums.BlobContainerEnum.Company);
                    Name = companyInfo.Name;
                    City = companyInfo.City;
                    CompleteAddress = companyInfo.CompleteAddress;
                }
            }
            catch (ApiUnauthorizedException unae)
            {
                UserDialogs.Instance.Alert(unae.Message, "Easysoccer");
                Application.Instance.LogOff(_navigationService);
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message, "Easysoccer");
            }
        }

        private void OpenReservationInfo(Guid? reservationId)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ReservationId", reservationId);
            _navigationService.NavigateAsync("ReservationInfo", navigationParameters);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            this.LoadDataAsync();
        }
    }
}
