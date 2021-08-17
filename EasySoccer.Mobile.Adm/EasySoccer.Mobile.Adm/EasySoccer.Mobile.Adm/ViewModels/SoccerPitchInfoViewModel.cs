using Acr.UserDialogs;
using EasySoccer.Mobile.Adm.API;
using EasySoccer.Mobile.Adm.API.ApiRequest;
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
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Application = EasySoccer.Mobile.Adm.Infra.Application;

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

        private ImageSource _image;
        public ImageSource Image
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

        private bool _hasActive;
        public bool HasActive
        {
            get { return _hasActive; }
            set { SetProperty(ref _hasActive, value); }
        }

        public DelegateCommand SelectedImageCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }

        private INavigationService _navigationService;
        public SoccerPitchInfoViewModel(INavigationService navigationService)
        {
            SelectedImageCommand = new DelegateCommand(async () => { await SelectImage(); });
            SportTypes = new ObservableCollection<SportTypeResponse>();
            SportTypesName = new ObservableCollection<string>();
            Colors = new ObservableCollection<ColorsResponse>();
            ColorsName = new ObservableCollection<string>();
            Plans = new ObservableCollection<PlansItemViewModel>();
            SaveCommand = new DelegateCommand(async () => { await SaveAsync(); });
            _navigationService = navigationService;
        }

        private async Task SelectImage()
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
                            Image = ImageSource.FromStream(() => new MemoryStream(bytes));
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

        private async Task GetSportTypesAsync()
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

        private async Task GetColorsAsync()
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

        private async Task LoadPlansAsync()
        {
            try
            {
                var plansResponse = await ApiClient.Instance.GetPlansAsync();
                if (plansResponse != null && plansResponse.Data != null)
                {
                    Plans.Clear();
                    foreach (var item in plansResponse.Data)
                    {
                        var viewModelItem = new PlansItemViewModel(item);
                        viewModelItem.PropertyChanged += ViewModelItem_PropertyChanged;
                        Plans.Add(viewModelItem);
                    }
                    if (Plans.Count > 0)
                        PlansHeight = Plans.Count * 75;
                    if (_currentSoccerPitch != null && _currentSoccerPitch.Plans != null && _currentSoccerPitch.Plans.Any())
                    {
                        foreach (var item in _currentSoccerPitch.Plans)
                        {
                            var plan = Plans.Where(x => x.Id == item.Id).FirstOrDefault();
                            if (plan != null)
                            {
                                plan.Selected = true;
                                plan.IsDefault = item.IsDefault;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);
            }
        }

        private void ViewModelItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDefault")
            {
                foreach (var item in this.Plans)
                {
                    var itemViewModel = sender as PlansItemViewModel;
                    if (item.Id != itemViewModel.Id && itemViewModel.IsDefault)
                    {
                        if (item.IsDefault == true)
                            item.IsDefault = false;
                    }
                }
            }
        }

        private async Task LoadSoccerPitchDataAsync()
        {
            try
            {
                if (_currentSoccerPitch != null)
                {
                    var response = await ApiClient.Instance.GetSoccerPitchByIdAsync(_currentSoccerPitch.Id);
                    if (response != null)
                    {
                        Image = Application.Instance.GetImageSource(response.ImageName, Infra.Enums.BlobContainerEnum.SoccerPitch);
                        Name = response.Name;
                        Description = response.Description;
                        Interval = response.Interval;
                        NumberOfPlayers = response.NumberOfPlayers;
                        HasRoof = response.HasRoof;
                        SportTypeId = response.SportTypeId;
                        Color = response.Color;
                        _currentSoccerPitch.Plans = response.Plans;
                    }
                }
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Alert(e.Message);

            }
        }

        private async Task SaveAsync()
        {
            try
            {
                if (_isEditing)
                {
                    var patchResponse = await ApiClient.Instance.PatchSoccerPitchAsync(new SoccerPitchRequest
                    {
                        Id = _currentSoccerPitch.Id,
                        Active = this.HasActive,
                        Color = this.Colors[this.SelectedColor.HasValue ? this.SelectedColor.Value : 0].Value,
                        Description = this.Description,
                        HasRoof = this.HasRoof,
                        Interval = this.Interval.HasValue ? this.Interval.Value : 0,
                        Name = this.Name,
                        NumberOfPlayers = this.NumberOfPlayers.HasValue ? this.NumberOfPlayers.Value : 0,
                        SportTypeId = this.SportTypes[this.SelectedSportTypeId.HasValue ? this.SelectedSportTypeId.Value : 0].Id,
                        Plans = this.Plans.Where(x => x.Selected).Select(x => new SoccerPitchSoccerPitchPlanRequest { Id = x.Id, IsDefault = x.IsDefault }).ToArray()
                    });
                    if (patchResponse != null)
                    {
                        UserDialogs.Instance.Alert("Dados atualizados com sucesso!", "EasySoccer");
                        //_currentSoccerPitch.Plans = Plans.Where(x => x.Selected).Select(x => new PlansResponse { Id = x.Id, Description = x.Description, Name = x.Name, Value = x.Value, IsDefault = x.IsDefault }).ToList();
                    }
                }
                else
                {
                    var postResponse = await ApiClient.Instance.PostSoccerPitchAsync(new SoccerPitchRequest
                    {
                        Active = this.HasActive,
                        Color = this.Colors[this.SelectedColor.HasValue ? this.SelectedColor.Value : 0].Value,
                        Description = this.Description,
                        HasRoof = this.HasRoof,
                        Interval = this.Interval.HasValue ? this.Interval.Value : 0,
                        Name = this.Name,
                        NumberOfPlayers = this.NumberOfPlayers.HasValue ? this.NumberOfPlayers.Value : 0,
                        SportTypeId = this.SportTypes[this.SelectedSportTypeId.HasValue ? this.SelectedSportTypeId.Value : 0].Id,
                        Plans = this.Plans.Where(x => x.Selected).Select(x => new SoccerPitchSoccerPitchPlanRequest { Id = x.Id }).ToArray(),
                        ImageBase64 = _imageBase64
                    });
                    if (postResponse != null)
                    {
                        UserDialogs.Instance.Alert("Dados inseridos com sucesso!", "EasySoccer");
                        _navigationService.GoBackAsync();
                    }
                }
                await this.LoadSoccerPitchDataAsync();
                await this.LoadPlansAsync();
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
                    Image = Application.Instance.GetImageSource(_currentSoccerPitch.ImageName, Infra.Enums.BlobContainerEnum.SoccerPitch);
                    Interval = _currentSoccerPitch.Interval;
                    NumberOfPlayers = _currentSoccerPitch.NumberOfPlayers;
                    HasRoof = _currentSoccerPitch.HasRoof;
                    SportTypeId = _currentSoccerPitch.SportTypeId;
                    Color = _currentSoccerPitch.Color;
                    HasActive = _currentSoccerPitch.Active;
                }
                else
                {
                    Image = ImageSource.FromUri(new Uri("https://easysoccer.blob.core.windows.net/soccerpitch/default.png"));
                }
            }
            if(_isEditing == false)
                Image = ImageSource.FromUri(new Uri("https://easysoccer.blob.core.windows.net/soccerpitch/default.png"));

            this.GetSportTypesAsync();
            this.GetColorsAsync();
            this.LoadPlansAsync();
        }
    }
}
