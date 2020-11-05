using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Common;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class PlanInfoViewModel : BindableBase, INavigationAware
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private decimal _value;
        public decimal Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private bool _isEdit = false;
        private int _planId = 0;

        public DelegateCommand SaveCommand { get; set; }
        private INavigationService _navigationService;
        public PlanInfoViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SaveCommand = new DelegateCommand(UpdateInfoAsync);
        }

        private async void UpdateInfoAsync()
        {
            try
            {
                if (_isEdit)
                {
                    var updateResponse = await ApiClient.Instance.PatchSoccerPitchPlanAsync(new API.ApiRequest.SoccerPitchPlanRequest
                    {
                        Description = Description,
                        id = _planId,
                        Name = Name,
                        Value = Value
                    });
                    if (updateResponse != null)
                    {
                        UserDialogs.Instance.Alert("Dados atualizados com sucesso");
                        await _navigationService.GoBackAsync();
                    }
                }
                else
                {
                    var createResponse = await ApiClient.Instance.PostSoccerPitchPlanAsync(new API.ApiRequest.SoccerPitchPlanRequest
                    {
                        Description = Description,
                        Name = Name,
                        Value = Value
                    });
                    if (createResponse != null)
                    {
                        UserDialogs.Instance.Alert("Dados inseridos com sucesso");
                        await _navigationService.GoBackAsync();
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
            if (parameters.ContainsKey("selectedPlan"))
            {
                var planJson = parameters.GetValue<string>("selectedPlan");
                var plan = JsonConvert.DeserializeObject<PlansResponse>(planJson);
                if (plan != null)
                {
                    _isEdit = true;
                    Name = plan.Name;
                    Description = plan.Description;
                    Value = plan.Value;
                    _planId = plan.Id;
                }
            }
        }


    }
}
