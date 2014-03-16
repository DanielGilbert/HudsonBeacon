using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using HudsonBeacon.MvvmHelper;
using HudsonBeacon.Helpers;
using HudsonBeacon.Patterns;

namespace HudsonBeacon.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Settings _settings;

        public ICommand SaveCommand { get; set; }

        public string ProjectSource
        {
            get
            {
                return _settings.Uri;
            }
            set
            {
                _settings.Uri = value;
                RaisePropertyChanged("ProjectSource");
            }
        }

        public string FetchMinuteIntervall
        {
            get
            {
                return _settings.FetchMinuteIntervall.ToString();
            }
            set
            {
                int result = -1;
                Int32.TryParse(value, out result);
                if (result >= 0)
                {
                    _settings.FetchMinuteIntervall = result;
                }
            }
        }

        public string FailurePulseIntervall
        {
            get
            {
                return _settings.FailurePulseIntervall.ToString();
            }
            set
            {
                int result = -1;
                Int32.TryParse(value, out result);
                if (result >= 0)
                {
                    _settings.FailurePulseIntervall = result;
                }
            }
        }

        public string SuccessPulseIntervall
        {
            get
            {
                return _settings.SuccessPulseIntervall.ToString();
            }
            set
            {
                int result = -1;
                Int32.TryParse(value, out result);
                if (result >= 0)
                {
                    _settings.SuccessPulseIntervall = result;
                }
            }
        }

        public int Lightness
        {
            get
            {
                return _settings.Lightness;
            }
            set
            {
                _settings.Lightness = value;
            }
        }

        public SettingsViewModel()
        {
            _settings = Settings.Load();

            SaveCommand = new RelayCommand(DoSave);
        }
        private void DoSave(object obj)
        {
            _settings.Save();

            Mediator.Instance.NotifyColleagues(MediatorMessages.ReloadSettings, null);
        }
    }
}
