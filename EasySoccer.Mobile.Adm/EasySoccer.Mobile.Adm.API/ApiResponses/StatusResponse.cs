using Newtonsoft.Json;
using System.ComponentModel;

namespace EasySoccer.Mobile.Adm.API.ApiResponses
{
    public class StatusResponse : INotifyPropertyChanged
    {
        public int Key { get; set; }
        public string Text { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        private bool _selected;
        [JsonIgnore]
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
                }
            }
        }
    }
}
