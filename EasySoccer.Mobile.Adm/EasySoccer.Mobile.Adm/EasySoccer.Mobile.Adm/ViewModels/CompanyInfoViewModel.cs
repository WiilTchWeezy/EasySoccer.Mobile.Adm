﻿using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.Infra;
using EasySoccer.Mobile.Adm.Infra.Services;
using EasySoccer.Mobile.Adm.Infra.Services.DTO;
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
        public DelegateCommand SearchPlacesCommand { get; set; }
        public DelegateCommand CurrentLocationCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private IGooglePlacesService _googlePlacesService;

        Action<PlaceDetail> onIntentResult;
        public CompanyInfoViewModel(IGooglePlacesService googlePlacesService)
        {
            SelectedImageCommand = new DelegateCommand(SelectImage);
            _googlePlacesService = googlePlacesService;
            SearchPlacesCommand = new DelegateCommand(SearchGoogleMaps);
            CurrentLocationCommand = new DelegateCommand(CurrentLocation);
            SaveCommand = new DelegateCommand(SaveAsync);
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

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        private double _latitude;
        public double Latitude
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
            try
            {
                var mediaResponse = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Selecione uma imagem" });
                if (mediaResponse != null)
                {
                    var stream = await mediaResponse.OpenReadAsync();
                    byte[] bytes = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    if (bytes != null)
                    {
                        string base64 = Convert.ToBase64String(bytes);
                        if (string.IsNullOrEmpty(base64) == false)
                        {
                            var apiResponse = await ApiClient.Instance.PostCompanyImageAsync(new API.ApiRequest.CompanyImageRequest { ImageBase64 = base64 });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void SearchGoogleMaps()
        {
            onIntentResult = (placeDetail) =>
            {
                this.Longitude = placeDetail.Longitude;
                this.Latitude = placeDetail.Latitude;
            };
            _googlePlacesService.DisplayIntent(onIntentResult);
        }

        private async void CurrentLocation()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                this.Longitude = location.Longitude;
                this.Latitude = location.Latitude;
            }
        }

        private async void SaveAsync()
        {
            try
            {
                var response = await ApiClient.Instance.PatchCompanyAsync(new API.ApiRequest.PatchCompanyInfoRequest
                {
                    CNPJ = this.CNPJ,
                    CompleteAddress = this.CompleteAddress,
                    Description = this.Description,
                    Latitude = this.Latitude,
                    Longitude = this.Longitude,
                    Name = this.Name
                });
                if (response != null)
                {
                    UserDialogs.Instance.Alert("Dados atualizados com sucesso.");
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
