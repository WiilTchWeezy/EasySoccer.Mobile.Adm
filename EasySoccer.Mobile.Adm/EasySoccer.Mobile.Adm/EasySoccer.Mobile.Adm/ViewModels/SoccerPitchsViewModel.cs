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
    public class SoccerPitchsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<SoccerPitchResponse> SoccerPitchs { get; set; }

        public DelegateCommand AddSoccerPitchCommand { get; set; }
        public DelegateCommand<SoccerPitchResponse> EditSoccerPitchCommand { get; set; }
        private INavigationService _navigationService;
        public SoccerPitchsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SoccerPitchs = new ObservableCollection<SoccerPitchResponse>();
            AddSoccerPitchCommand = new DelegateCommand(OpenSoccerPitchInfoToAdd);
        }

        private void OpenSoccerPitchInfoToAdd()
        {
            _navigationService.NavigateAsync("SoccerPitchInfo");
        }

        private void OpenSoccerPitchInfoToEdit(SoccerPitchResponse soccerPitch)
        {
            var navigationParamters = new NavigationParameters();
            navigationParamters.Add("soccerPitch", JsonConvert.SerializeObject(soccerPitch));
            _navigationService.NavigateAsync("SoccerPitchInfo", navigationParamters);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetSoccerPitchsAsync();
                if (response != null && response.Data != null && response.Data.Any())
                {
                    SoccerPitchs.Clear();
                    foreach (var item in response.Data)
                    {
                        item.EditSoccerPitchCommand = new DelegateCommand<SoccerPitchResponse>(OpenSoccerPitchInfoToEdit);
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
