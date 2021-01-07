using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
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
        public UserNotificationsViewModel()
        {
            Notifications = new ObservableCollection<CompanyUserNotificationResponse>();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var userNotification = await ApiClient.Instance.GetNotificationsAsync();
                if(userNotification != null)
                {
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

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadDataAsync();
        }
    }
}
