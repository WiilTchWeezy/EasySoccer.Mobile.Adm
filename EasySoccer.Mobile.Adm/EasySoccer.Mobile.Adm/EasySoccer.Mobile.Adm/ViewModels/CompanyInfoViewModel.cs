using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.Infra;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Essentials;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class CompanyInfoViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand SelectedImageCommand { get; set; }
        public CompanyInfoViewModel()
        {
            SelectedImageCommand = new DelegateCommand(SelectImage);
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

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _cnpj;
        public string CNPJ
        {
            get { return _cnpj; }
            set { SetProperty(ref _cnpj, value); }
        }

        private string _completeAddress;
        public string CompleteAddress
        {
            get { return _completeAddress; }
            set { SetProperty(ref _completeAddress, value); }
        }

        private decimal _longitude;
        public decimal Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        private decimal _latitude;
        public decimal Latitude
        {
            get { return _latitude; }
            set { SetProperty(ref _latitude, value); }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var companyInfoResponse = await ApiClient.Instance.GetCompanyInfoAsync();
                if (companyInfoResponse != null)
                {
                    Image = Application.Instance.GetImage(companyInfoResponse.Logo, Infra.Enums.BlobContainerEnum.Company);
                    Name = companyInfoResponse.Name;
                    Description = companyInfoResponse.Description;
                    CNPJ = companyInfoResponse.CNPJ;
                    CompleteAddress = companyInfoResponse.CompleteAddress;
                    Longitude = companyInfoResponse.Longitude;
                    Latitude = companyInfoResponse.Latitude;
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void SelectImage()
        {
            var response = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Selecione uma imagem" });
            if (response != null)
            {
                var stream = await response.OpenReadAsync();
                byte[] bytes = null;
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                if (bytes != null)
                {
                    string base64 = Convert.ToBase64String(bytes);
                }
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
