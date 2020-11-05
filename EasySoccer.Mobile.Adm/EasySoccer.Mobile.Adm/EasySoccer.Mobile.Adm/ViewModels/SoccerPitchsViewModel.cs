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
    public class SoccerPitchsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<SoccerPitchResponse> SoccerPitchs { get; set; }
        public SoccerPitchsViewModel()
        {
            SoccerPitchs = new ObservableCollection<SoccerPitchResponse>();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetSoccerPitchsAsync();
                if (response != null && response.Any())
                {
                    SoccerPitchs.Clear();
                    foreach (var item in response)
                    {
                        SoccerPitchs.Add(item);
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
            this.LoadDataAsync();
        }
    }
}
