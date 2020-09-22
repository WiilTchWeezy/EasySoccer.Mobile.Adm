using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class CompanyReservationsViewModel : BindableBase, INavigationAware
    {
        public CompanyReservationsViewModel()
        {

        }

        private async void LoadDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetCompanySchedulesAsync(DateTime.Now);
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message, "Easysoccer");
            }
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
