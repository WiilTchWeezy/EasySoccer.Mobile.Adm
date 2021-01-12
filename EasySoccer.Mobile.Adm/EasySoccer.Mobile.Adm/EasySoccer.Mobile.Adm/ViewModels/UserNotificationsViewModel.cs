using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.ApiResponses.NotificationResponse;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class UserNotificationsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<CompanyUserNotificationResponse> Notifications { get; set; }

        public DelegateCommand ItemSelectedCommand { get; set; }

        private INavigationService _navigationService;
        public UserNotificationsViewModel(INavigationService navigationService)
        {
            Notifications = new ObservableCollection<CompanyUserNotificationResponse>();
            ItemSelectedCommand = new DelegateCommand(ItemSelected);
            _navigationService = navigationService;
        }

        private CompanyUserNotificationResponse _selectedItem;
        public CompanyUserNotificationResponse SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var userNotification = await ApiClient.Instance.GetNotificationsAsync();
                if(userNotification != null)
                {
                    Notifications.Clear();
                    foreach (var item in userNotification)
                    {
                        Notifications.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void ItemSelected()
        {
            switch (SelectedItem.NotificationType)
            {
                case API.ApiResponses.Enums.NotificationTypeEnum.Standard:
                    break;
                case API.ApiResponses.Enums.NotificationTypeEnum.FinancialRenewal:
                    _navigationService.NavigateAsync("Payment");
                    break;
                case API.ApiResponses.Enums.NotificationTypeEnum.NewReservation:
                    if (string.IsNullOrEmpty(SelectedItem.Data) == false)
                    {
                        var navigationParameters = new NavigationParameters();
                        var notificationData = JsonConvert.DeserializeObject<NotificationReservationInfo>(SelectedItem.Data);
                        navigationParameters.Add("ReservationId", notificationData.Id);
                        _navigationService.NavigateAsync("ReservationInfo", navigationParameters);
                    }
                    break;
                default:
                    break;
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
