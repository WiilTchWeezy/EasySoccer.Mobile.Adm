using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.Infra;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class ReservationInfoViewModel : BindableBase, INavigationAware
    {
        private string _companyImage;
        public string CompanyImage
        {
            get { return _companyImage; }
            set { SetProperty(ref _companyImage, value); }
        }

        private string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set { SetProperty(ref _companyName, value); }
        }

        private string _companyAddress;
        public string CompanyAddress
        {
            get { return _companyAddress; }
            set { SetProperty(ref _companyAddress, value); }
        }

        private string _companyCity;
        public string CompanyCity
        {
            get { return _companyCity; }
            set { SetProperty(ref _companyCity, value); }
        }

        private string _soccerPitchImage;
        public string SoccerPitchImage
        {
            get { return _soccerPitchImage; }
            set { SetProperty(ref _soccerPitchImage, value); }
        }

        private string _soccerPitchName;
        public string SoccerPitchName
        {
            get { return _soccerPitchName; }
            set { SetProperty(ref _soccerPitchName, value); }
        }

        private string _soccerPitchSportType;
        public string SoccerPitchSportType
        {
            get { return _soccerPitchSportType; }
            set { SetProperty(ref _soccerPitchSportType, value); }
        }

        private string _selectedDate;
        public string SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        private string _hours;
        public string Hours
        {
            get { return _hours; }
            set { SetProperty(ref _hours, value); }
        }

        private string _soccerPitchPlanName;
        public string SoccerPitchPlanName
        {
            get { return _soccerPitchPlanName; }
            set { SetProperty(ref _soccerPitchPlanName, value); }
        }

        private string _soccerPitchPlanDescription;
        public string SoccerPitchPlanDescription
        {
            get { return _soccerPitchPlanDescription; }
            set { SetProperty(ref _soccerPitchPlanDescription, value); }
        }

        private string _personName;
        public string PersonName
        {
            get { return _personName; }
            set { SetProperty(ref _personName, value); }
        }

        private string _personPhone;
        public string PersonPhone
        {
            get { return _personPhone; }
            set { SetProperty(ref _personPhone, value); }
        }

        private string _statusDescription;
        public string StatusDescription
        {
            get { return _statusDescription; }
            set { SetProperty(ref _statusDescription, value); }
        }

        private string _statusColor;
        public string StatusColor
        {
            get { return _statusColor; }
            set { SetProperty(ref _statusColor, value); }
        }

        private bool _showConfirmationButton;
        public bool ShowConfirmationButton
        {
            get { return _showConfirmationButton; }
            set { SetProperty(ref _showConfirmationButton, value); }
        }

        private bool _showCanceledButton;
        public bool ShowCanceledButton
        {
            get { return _showCanceledButton; }
            set { SetProperty(ref _showCanceledButton, value); }
        }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand ConfimCommand { get; set; }

        private int _status = 0;
        private Guid _reservationId;
        public ReservationInfoViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
            ConfimCommand = new DelegateCommand(Confirm);
        }
        private void Cancel()
        {
            var confirmConfig = new ConfirmConfig()
            {
                Message = "Deseja realmente cancelar este agendamento?",
                OkText = "Sim",
                CancelText = "Não",
                Title = "Cancelar agendamento",
                OnAction = (confirm) =>
                {
                    if (confirm)
                    {
                        this.ChangeStatusAsync(2);
                    }
                }
            };
            UserDialogs.Instance.Confirm(confirmConfig);
        }

        private void Confirm()
        {
            var confirmConfig = new ConfirmConfig()
            {
                Message = "Deseja realmente confirmar este agendamento?",
                OkText = "Sim",
                CancelText = "Não",
                Title = "Confirmar agendamento",
                OnAction = (confirm) =>
                {
                    if (confirm)
                    {
                        this.ChangeStatusAsync(3);
                    }
                }
            };
            UserDialogs.Instance.Confirm(confirmConfig);
        }

        private async void ChangeStatusAsync(int status)
        {
            try
            {
                await ApiClient.Instance.PostChangeReservationStatus(new API.ApiRequest.ChangeStatusRequest { ReservationId = _reservationId, Status = status });
                UserDialogs.Instance.Alert("Status alterado com sucesso.");
                LoadDataAsync(_reservationId);
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadDataAsync(Guid reservationId)
        {
            try
            {
                var response = await ApiClient.Instance.GetReservationInfoAsync(reservationId);
                if (response != null)
                {
                    CompanyImage = Application.Instance.GetImage(response.CompanyImage, Infra.Enums.BlobContainerEnum.Company);
                    CompanyName = response.CompanyName;
                    CompanyAddress = response.CompanyAddress;
                    CompanyCity = response.CompanyCity;
                    SoccerPitchImage = Application.Instance.GetImage(response.SoccerPitchImage, Infra.Enums.BlobContainerEnum.SoccerPitch);
                    SoccerPitchSportType = response.SoccerPitchSportType;
                    SoccerPitchName = response.SoccerPitchName;
                    SelectedDate = response.SelectedDateStart.ToString("dd/MM/yyyy");
                    Hours = $"{response.SelectedDateStart:HH:mm} - {response.SelectedDateEnd:HH:mm}";
                    SoccerPitchPlanName = response.SoccerPitchPlanName;
                    SoccerPitchPlanDescription = response.SoccerPitchPlanDescription;
                    PersonName = response.PersonName;
                    PersonPhone = response.PersonPhone;
                    _status = response.Status;
                    _reservationId = response.Id;
                    StatusDescription = $"Status : {response.StatusDescription}";
                    UpdateStatusColor();
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void UpdateStatusColor()
        {
            switch (_status)
            {
                case 1:
                    ShowCanceledButton = true;
                    ShowConfirmationButton = true;
                    StatusColor = "#f0ad4e";
                    break;
                case 2:
                    ShowCanceledButton = false;
                    ShowConfirmationButton = false;
                    StatusColor = "#d9534f";
                    break;
                case 3:
                    ShowCanceledButton = true;
                    ShowConfirmationButton = false;
                    StatusColor = "#5cb85c";
                    break;
                case 4:
                    ShowCanceledButton = false;
                    ShowConfirmationButton = false;
                    StatusColor = "#0275d8";
                    break;
                default:
                    break;
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("ReservationId"))
            {
                var reservationId = parameters.GetValue<Guid>("ReservationId");
                _reservationId = reservationId;
                LoadDataAsync(reservationId);
            }
        }
    }
}
