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
    public class ReservationsFilterViewModel : BindableBase, INavigationAware
    {
        public List<SoccerPitchResponse> SoccerPitchs { get; set; }
        public List<PlansResponse> Plans { get; set; }
        public List<StatusResponse> Status { get; set; }
        public ObservableCollection<string> PlansNames { get; set; }
        public ObservableCollection<string> SoccerPitchsNames { get; set; }
        public ObservableCollection<string> StatusNames { get; set; }
        public ReservationsFilterViewModel()
        {
            SoccerPitchs = new List<SoccerPitchResponse>();
            SoccerPitchsNames = new ObservableCollection<string>();
            Plans = new List<PlansResponse>();
            PlansNames = new ObservableCollection<string>();
            Status = new List<StatusResponse>();
            StatusNames = new ObservableCollection<string>();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadData();
        }

        private void LoadData()
        {
            LoadSoccerPitchAsync();
            LoadPlansAsync();
            LoadStatusAsync();
        }

        private async void LoadSoccerPitchAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetSoccerPitchsAsync();
                SoccerPitchs.Clear();
                SoccerPitchsNames.Clear();
                if (response != null && response.Count > 0)
                {
                    foreach (var item in response)
                    {
                        SoccerPitchs.Add(item);
                        SoccerPitchsNames.Add(item.Name);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadPlansAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetPlansAsync();
                if (response != null && response.Count > 0)
                {
                    Plans.Clear();
                    PlansNames.Clear();
                    foreach (var item in response)
                    {
                        Plans.Add(item);
                        PlansNames.Add(item.Name);
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadStatusAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetStatusAsync();
                if(response != null && response.Count > 0)
                {
                    Status.Clear();
                    StatusNames.Clear();
                    foreach (var item in response)
                    {
                        Status.Add(item);
                        StatusNames.Add(item.Text);
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
