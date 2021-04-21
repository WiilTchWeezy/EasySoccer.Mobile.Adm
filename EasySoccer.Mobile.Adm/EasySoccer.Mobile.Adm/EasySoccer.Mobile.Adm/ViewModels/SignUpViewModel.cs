using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiRequest;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Validations;
using FluentValidation;
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
    public class SignUpViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navigationService;
        private CompanyValidator _validator;
        public SignUpViewModel(INavigationService navigationService)
        {
            PlansName = new ObservableCollection<string>();
            PlansInstallments = new ObservableCollection<string>();
            Plans = new List<PlansInfoResponse>();
            OpenLinkCommand = new DelegateCommand(OpenLink);
            SaveCommand = new DelegateCommand(PostCompanyInput);
            _navigationService = navigationService;
            _validator = new CompanyValidator();
            OpenTermsCommand = new DelegateCommand(OpenTerms);
            OpenPolicyCommand = new DelegateCommand(OpenPolicy);
        }

        public List<PlansInfoResponse> Plans { get; set; }
        public ObservableCollection<string> PlansName { get; set; }
        public DelegateCommand OpenLinkCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand OpenTermsCommand { get; set; }
        public DelegateCommand OpenPolicyCommand { get; set; }

        public ObservableCollection<string> PlansInstallments { get; set; }
        private CompanyFormInputRequest _request = new CompanyFormInputRequest();

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

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value); }
        }

        private string _cnpj;
        public string CNPJ
        {
            get { return _cnpj; }
            set { SetProperty(ref _cnpj, value); }
        }

        private string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set { SetProperty(ref _companyName, value); }
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

        private async void OpenLink()
        {
            await Browser.OpenAsync("http://www.easysoccer.com.br/", BrowserLaunchMode.SystemPreferred);
        }

        private async void OpenTerms()
        {
            await Browser.OpenAsync("https://www.easysoccer.com.br/documents/terms.html", BrowserLaunchMode.SystemPreferred);
        }

        private async void OpenPolicy()
        {
            await Browser.OpenAsync("https://www.easysoccer.com.br/documents/privacypolicy.html", BrowserLaunchMode.SystemPreferred);
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

        private async void PostCompanyInput()
        {
            try
            {
                if (SelectedPlan.HasValue && SelectedInstallment.HasValue)
                {
                    var currentPlan = this.Plans.ElementAtOrDefault(SelectedPlan.Value);
                    _request.SelectedPlan = currentPlan.PlanId;
                    _request.SelectedInstallments = SelectedInstallment.Value + 1;
                }
                _request.UserName = UserName;
                _request.UserEmail = UserEmail;
                _request.CompanyName = CompanyName;
                _request.CompanyDocument = CNPJ;

                _request.SecurityCode = SecurityCode;
                _request.FinancialName = FinancialName;
                _request.FinancialDocument = FinancialDocument;
                _request.FinancialBirthDay = FinancialBirthDay;
                _request.CardNumber = CardNumber;
                _request.CardExpiration = CardExpiration;

                var validationResponse = _validator.Validate(_request);
                if (validationResponse.IsValid)
                {
                    var response = await ApiClient.Instance.PostCompanyFormInputAsync(_request);
                    var alertConfig = new AlertConfig()
                    {
                        Title = "Obrigado por se cadastrar!",
                        Message = "Você receberá um e-mail com suas credenciais. [Não se esqueça de checar a caixa de Spam] Muito Obrigado!",
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

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            LoadDataAsync();
        }
    }
}
