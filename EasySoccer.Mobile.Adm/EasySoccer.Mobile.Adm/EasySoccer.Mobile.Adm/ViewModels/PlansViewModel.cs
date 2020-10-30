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
    public class PlansViewModel : BindableBase, INavigationAware
    {

        public ObservableCollection<PlansResponse> Plans { get; set; }

        private INavigationService _navigationService;

        public DelegateCommand AddPlanCommand { get; set; }
        public PlansViewModel(INavigationService navigationService)
        {
            Plans = new ObservableCollection<PlansResponse>();
            _navigationService = navigationService;
            AddPlanCommand = new DelegateCommand(OpenPlanInfoToAdd);
        }

        private void OpenPlanInfoToAdd()
        {
            try
            {
                _navigationService.NavigateAsync("PlanInfo");
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void OpenPlanInfoEdit(PlansResponse plan)
        {
            try
            {
                var jsonPlan = JsonConvert.SerializeObject(plan);
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add("selectedPlan", jsonPlan);
                _navigationService.NavigateAsync("PlanInfo", navigationParameters);
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
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
                        item.EditPlanCommand = new DelegateCommand<PlansResponse>(OpenPlanInfoEdit);
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
