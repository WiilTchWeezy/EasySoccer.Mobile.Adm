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
    public class PlansViewModel : BindableBase, INavigationAware
    {

        public ObservableCollection<PlansResponse> Plans { get; set; }
        public PlansViewModel()
        {
            Plans = new ObservableCollection<PlansResponse>();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetPlansAsync();
                if (response != null && response.Count > 0)
                {
                    Plans.Clear();
                    foreach (var item in response)
                    {
                        Plans.Add(item);
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
