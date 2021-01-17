using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiRequest;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Validations;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class PaymentViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navigationService;
        private PaymentValidator _validator;

        public PaymentViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PlansName = new ObservableCollection<string>();
            PlansInstallments = new ObservableCollection<string>();
            Plans = new List<PlansInfoResponse>();
            OpenLinkCommand = new DelegateCommand(OpenLink);
            PayCommand = new DelegateCommand(Pay);
            _validator = new PaymentValidator();
        }


        public List<PlansInfoResponse> Plans { get; set; }
        public ObservableCollection<string> PlansName { get; set; }
        public DelegateCommand OpenLinkCommand { get; set; }
        public DelegateCommand PayCommand { get; set; }
        public ObservableCollection<string> PlansInstallments { get; set; }

        private async void OpenLink()
        {
            await Browser.OpenAsync("http://www.easysoccer.com.br/", BrowserLaunchMode.SystemPreferred);
        }

        private async void Pay()
        {
            var request = new PaymentRequest()
            {
                SelectedPlan = SelectedPlan.HasValue ? SelectedPlan.Value : -1,
                SelectedInstallments = SelectedInstallment.HasValue ? SelectedInstallment.Value : -1,
                CardExpiration = CardExpiration,
                CardNumber = CardNumber,
                FinancialBirthDay = FinancialBirthDay,
                FinancialDocument = FinancialDocument,
                FinancialName = FinancialName,
                SecurityCode = SecurityCode
            };
            var validationResponse = _validator.Validate(request);
            if (validationResponse.IsValid)
            {
                var alertConfig = new AlertConfig()
                {
                    Title = "Obrigado por realizar o pagamento!",
                    Message = "Você receberá uma notificação para confirmar seu pagamento. Muito Obrigado!",
                    OkText = "Fechar"
                };
                UserDialogs.Instance.Alert(alertConfig);
                await _navigationService.GoBackAsync();
            }
            else
            {
                var validationMessages = validationResponse.Errors.Select(x => x.ErrorMessage).Distinct().ToArray();
                var errorMessage = string.Join(" - ", validationMessages);
                var alertConfig = new AlertConfig()
                {
                    Title = "Alguns erros de validação!",
                    Message = errorMessage,
                    OkText = "Fechar"
                };
                UserDialogs.Instance.Alert(alertConfig);
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetPlansInfoAsync();
                if (response != null)
                {
                    Plans.Clear();
                    PlansName.Clear();
                    foreach (var item in response)
                    {
                        Plans.Add(item);
                        PlansName.Add($"{item.PlanName} - (R$ {item.Value:00},00)");
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void GetPlansInstallments(int? selectedPlanIndex)
        {
            if (selectedPlanIndex.HasValue)
            {
                var currentPlan = this.Plans.ElementAtOrDefault(selectedPlanIndex.Value);
                if (currentPlan != null)
                {
                    PlansInstallments.Clear();
                    for (int i = 1; i <= currentPlan.MaxInstallments; i++)
                    {
                        var installmentValue = currentPlan.Value / i;
                        PlansInstallments.Add($"{i} parcelas - (R$ {installmentValue:00},00)");
                    }
                }
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadDataAsync();
        }

        private string _financialName;
        public string FinancialName
        {
            get { return _financialName; }
            set { SetProperty(ref _financialName, value); }
        }

        private string _financialDocument;
        public string FinancialDocument
        {
            get { return _financialDocument; }
            set { SetProperty(ref _financialDocument, value); }
        }

        private string _financialBirthDay;
        public string FinancialBirthDay
        {
            get { return _financialBirthDay; }
            set { SetProperty(ref _financialBirthDay, value); }
        }

        private string _cardNumber;
        public string CardNumber
        {
            get { return _cardNumber; }
            set { SetProperty(ref _cardNumber, value); }
        }

        private string _cardExpiration;
        public string CardExpiration
        {
            get { return _cardExpiration; }
            set { SetProperty(ref _cardExpiration, value); }
        }

        private string _securityCode;
        public string SecurityCode
        {
            get { return _securityCode; }
            set { SetProperty(ref _securityCode, value); }
        }

        private int? _selectedPlan;
        public int? SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                if (SetProperty(ref _selectedPlan, value))
                    GetPlansInstallments(value);
            }
        }

        private int? _selectedInstallment;
        public int? SelectedInstallment
        {
            get { return _selectedInstallment; }
            set
            {
                if (SetProperty(ref _selectedInstallment, value))
                    GetPlansInstallments(value);
            }
        }
    }
}
