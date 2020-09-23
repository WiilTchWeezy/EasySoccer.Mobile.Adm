using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.API.Session;
using EasySoccer.Mobile.Adm.Infra;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class CompanyReservationsViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<CompanySchedulesResponse> CompanySchedules { get; set; }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { SetProperty(ref _selectedDate, value); }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        private string _completeAddress;
        public string CompleteAddress
        {
            get { return _completeAddress; }
            set { SetProperty(ref _completeAddress, value); }
        }
        public CompanyReservationsViewModel()
        {
            CompanySchedules = new ObservableCollection<CompanySchedulesResponse>();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var companySchedules = await ApiClient.Instance.GetCompanySchedulesAsync(SelectedDate);
                if(companySchedules != null)
                {
                    foreach (var item in companySchedules)
                    {
                        CompanySchedules.Add(item);
                    }
                }
                var companyInfo = await ApiClient.Instance.GetCompanyInfoAsync();
                if(companyInfo != null)
                {
                    Image = Application.Instance.GetImage(companyInfo.Logo, Infra.Enums.BlobContainerEnum.Company);
                    Name = companyInfo.Name;
                    City = companyInfo.City;
                    CompleteAddress = companyInfo.CompleteAddress;
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message, "Easysoccer");
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
