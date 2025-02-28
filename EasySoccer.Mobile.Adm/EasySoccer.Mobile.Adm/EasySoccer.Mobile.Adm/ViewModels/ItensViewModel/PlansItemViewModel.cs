﻿using EasySoccer.Mobile.Adm.API.ApiResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EasySoccer.Mobile.Adm.ViewModels.ItensViewModel
{
    public class PlansItemViewModel : PlansResponse, INotifyPropertyChanged
    {

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected"));
                }
            }
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get { return _isDefault; }
            set
            {
                if (_isDefault != value)
                {
                    _isDefault = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDefault"));
                }
            }
        }

        public PlansItemViewModel(PlansResponse item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Value = item.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
