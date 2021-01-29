using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiRequest;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Validations;
using EasySoccer.Mobile.Adm.Infra.Enums;
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
            SelectStateCommand = new DelegateCommand(SelectState);
            SelectCityCommand = new DelegateCommand(SelectCity);
        }


        public List<PlansInfoResponse> Plans { get; set; }
        public ObservableCollection<string> PlansName { get; set; }
        public DelegateCommand OpenLinkCommand { get; set; }
        public DelegateCommand PayCommand { get; set; }
        public DelegateCommand SelectStateCommand { get; set; }
        public DelegateCommand SelectCityCommand { get; set; }
        public ObservableCollection<string> PlansInstallments { get; set; }

        private async void OpenLink()
        {
            await Browser.OpenAsync("http://www.easysoccer.com.br/", BrowserLaunchMode.SystemPreferred);
        }

        private async void Pay()
        {
            try
            {
                var request = new PaymentRequest()
                {
                    SelectedPlan = SelectedPlan.HasValue ? SelectedPlan.Value : -1,
                    SelectedInstallments = SelectedInstallment.HasValue ? SelectedInstallment.Value : -1,
                    CardExpiration = CardExpiration.Replace("/", string.Empty),
                    CardNumber = CardNumber.Trim(),
                    FinancialBirthDay = FinancialBirthDay,
                    FinancialDocument = FinancialDocument.Replace(".", string.Empty).Replace("-", string.Empty),
                    FinancialName = FinancialName,
                    SecurityCode = SecurityCode,
                    CityId = IdCity.HasValue ? IdCity.Value : 0,
                    Complementary = Complementary,
                    Neighborhood = Neighborhood,
                    StateId = IdState.HasValue ? IdState.Value : 0,
                    Street = Street,
                    StreetNumber = StreetNumber,
                    ZipCode = ZipCode.Replace("-", string.Empty)
                };
                var validationResponse = _validator.Validate(request);
                if (validationResponse.IsValid)
                {
                    await ApiClient.Instance.PostPaymentAsync(request);
                    var alertConfig = new AlertConfig()
                    {
                        Title = "Obrigado por realizar o pagamento!",
                        Message = "Seu pagamento foi aprovado. Muito Obrigado!",
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
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
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

        private void SelectState()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ModalSelectType", ModalSelectEnum.State);
            _navigationService.NavigateAsync("ModalSelect", navigationParameters, useModalNavigation: true);
        }
        private void SelectCity()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("ModalSelectType", ModalSelectEnum.City);
            navigationParameters.Add("StateId", IdState);
            _navigationService.NavigateAsync("ModalSelect", navigationParameters, useModalNavigation: true);
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("StateId"))
                IdState = parameters.GetValue<int>("StateId");
            if (parameters.ContainsKey("CityId"))
                IdCity = parameters.GetValue<int>("CityId");
            if (parameters.ContainsKey("StateName"))
                StateName = parameters.GetValue<string>("StateName");
            if (parameters.ContainsKey("CityName"))
                CityName = parameters.GetValue<string>("CityName");
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
            set { SetProperty(ref _selectedInstallment, value); }
        }

        private int? _idCity;
        public int? IdCity
        {
            get { return _idCity; }
            set { SetProperty(ref _idCity, value); }
        }

        private int? _idState;
        public int? IdState
        {
            get { return _idState; }
            set { SetProperty(ref _idState, value); }
        }

        private string _stateName;
        public string StateName
        {
            get { return _stateName; }
            set { SetProperty(ref _stateName, value); }
        }

        private string _cityName;
        public string CityName
        {
            get { return _cityName; }
            set { SetProperty(ref _cityName, value); }
        }

        private string _zipCode;
        public string ZipCode
        {
            get { return _zipCode; }
            set { SetProperty(ref _zipCode, value); }
        }

        private string _streetNumber;
        public string StreetNumber
        {
            get { return _streetNumber; }
            set { SetProperty(ref _streetNumber, value); }
        }

        private string _street;
        public string Street
        {
            get { return _street; }
            set { SetProperty(ref _street, value); }
        }

        private string _complementary;
        public string Complementary
        {
            get { return _complementary; }
            set { SetProperty(ref _complementary, value); }
        }

        private string _neighborhood;
        public string Neighborhood
        {
            get { return _neighborhood; }
            set { SetProperty(ref _neighborhood, value); }
        }
    }
}
