using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiResponses;
using EasySoccer.Mobile.Adm.Infra;
using EasySoccer.Mobile.Adm.ViewModels.ItensViewModel;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace EasySoccer.Mobile.Adm.ViewModels
{
    public class SoccerPitchInfoViewModel : BindableBase, INavigationAware
    {
        private SoccerPitchResponse _currentSoccerPitch;
        private bool _isEditing = false;
        private string _imageBase64 = string.Empty;
        public ObservableCollection<SportTypeResponse> SportTypes { get; set; }
        public ObservableCollection<string> SportTypesName { get; set; }
        public ObservableCollection<string> ColorsName { get; set; }
        public ObservableCollection<ColorsResponse> Colors { get; set; }
        public ObservableCollection<PlansItemViewModel> Plans { get; set; }

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

        private string _image;
        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private int? _interval;
        public int? Interval
        {
            get { return _interval; }
            set { SetProperty(ref _interval, value); }
        }

        private int? _numberOfPlayers;
        public int? NumberOfPlayers
        {
            get { return _numberOfPlayers; }
            set { SetProperty(ref _numberOfPlayers, value); }
        }

        private bool _hasRoof;
        public bool HasRoof
        {
            get { return _hasRoof; }
            set { SetProperty(ref _hasRoof, value); }
        }

        private int _sportTypeId;
        public int SportTypeId
        {
            get { return _sportTypeId; }
            set { SetProperty(ref _sportTypeId, value); }
        }

        private int? _selectedSportTypeId;
        public int? SelectedSportTypeId
        {
            get { return _selectedSportTypeId; }
            set { SetProperty(ref _selectedSportTypeId, value); }
        }

        private int? _selectedColor;
        public int? SelectedColor
        {
            get { return _selectedColor; }
            set { SetProperty(ref _selectedColor, value); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        private int _plansHeight = 100;
        public int PlansHeight
        {
            get { return _plansHeight; }
            set { SetProperty(ref _plansHeight, value); }
        }

        public DelegateCommand SelectedImageCommand { get; set; }
        public SoccerPitchInfoViewModel()
        {
            SelectedImageCommand = new DelegateCommand(SelectImage);
            SportTypes = new ObservableCollection<SportTypeResponse>();
            SportTypesName = new ObservableCollection<string>();
            Colors = new ObservableCollection<ColorsResponse>();
            ColorsName = new ObservableCollection<string>();
            Plans = new ObservableCollection<PlansItemViewModel>();
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
                            _imageBase64 = base64;
                            if (_isEditing)
                                await ApiClient.Instance.PostSoccerPitchImageAsync(new API.ApiRequest.SoccerPitchImageRequest { ImageBase64 = base64, SoccerPitchId = _currentSoccerPitch.Id });
                            LoadSoccerPitchDataAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void GetSportTypesAsync()
        {
            try
            {
                var sportTypesResponse = await ApiClient.Instance.GetSportTypesAsync();
                if (sportTypesResponse != null)
                {
                    SportTypes.Clear();
                    SportTypesName.Clear();
                    foreach (var item in sportTypesResponse)
                    {
                        SportTypes.Add(item);
                        SportTypesName.Add(item.Name);
                    }
                    if (SportTypeId > 0 && SportTypes != null && SportTypes.Any())
                    {
                        var currentSportType = SportTypes.Where(x => x.Id == SportTypeId).FirstOrDefault();
                        if (currentSportType != null)
                        {
                            this.SelectedSportTypeId = SportTypes.IndexOf(currentSportType);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void GetColorsAsync()
        {
            try
            {
                var colorsResponse = await ApiClient.Instance.GetColorsAsync();
                if (colorsResponse != null)
                {
                    Colors.Clear();
                    ColorsName.Clear();
                    foreach (var item in colorsResponse)
                    {
                        Colors.Add(item);
                        ColorsName.Add(item.Name);
                    }
                }
                if (string.IsNullOrEmpty(Color) == false)
                {
                    var currentColor = Colors.Where(x => x.Value == this.Color).FirstOrDefault();
                    if (currentColor != null)
                    {
                        this.SelectedColor = Colors.IndexOf(currentColor);
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
                var plansResponse = await ApiClient.Instance.GetPlansAsync();
                if (plansResponse != null)
                {
                    Plans.Clear();
                    foreach (var item in plansResponse)
                    {
                        Plans.Add(new PlansItemViewModel(item));
                    }
                    if (Plans.Count > 0)
                        PlansHeight = Plans.Count * 75;
                    if (_currentSoccerPitch.Plans != null && _currentSoccerPitch.Plans.Any())
                    {
                        foreach (var item in _currentSoccerPitch.Plans)
                        {
                            var plan = Plans.Where(x => x.Id == item.Id).FirstOrDefault();
                            if (plan != null)
                                plan.Selected = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private async void LoadSoccerPitchDataAsync()
        {
            try
            {
                var response = await ApiClient.Instance.GetSoccerPitchByIdAsync(_currentSoccerPitch.Id);
                if (response != null)
                {
                    Image = Application.Instance.GetImage(response.ImageName, Infra.Enums.BlobContainerEnum.SoccerPitch);
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
            if (parameters.ContainsKey("soccerPitch"))
            {
                var jsonSoccerPitch = parameters.GetValue<string>("soccerPitch");
                if (!string.IsNullOrEmpty(jsonSoccerPitch))
                {
                    _currentSoccerPitch = JsonConvert.DeserializeObject<SoccerPitchResponse>(jsonSoccerPitch);
                    _isEditing = true;
                    Name = _currentSoccerPitch.Name;
                    Description = _currentSoccerPitch.Description;
                    Image = Application.Instance.GetImage(_currentSoccerPitch.ImageName, Infra.Enums.BlobContainerEnum.SoccerPitch);
                    Interval = _currentSoccerPitch.Interval;
                    NumberOfPlayers = _currentSoccerPitch.NumberOfPlayers;
                    HasRoof = _currentSoccerPitch.HasRoof;
                    SportTypeId = _currentSoccerPitch.SportTypeId;
                    Color = _currentSoccerPitch.Color;
                }
            }
            this.GetSportTypesAsync();
            this.GetColorsAsync();
            this.LoadPlansAsync();
        }
    }
}
