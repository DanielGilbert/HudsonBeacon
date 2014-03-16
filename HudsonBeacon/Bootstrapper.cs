using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using HudsonBeacon.Helpers;
using HudsonBeacon.Implementations;
using HudsonBeacon.Patterns;

namespace HudsonBeacon
{
    public class Bootstrapper
    {
        private TaskbarIcon _notifyIcon;
        private Beacon _beacon;
        private Settings _settings;

        internal void Init()
        {
            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = (TaskbarIcon)Application.Current.FindResource("NotifyIcon");

            //Create Settings class
            _settings = Settings.Load();

            //Create the Beacon itself
            _beacon = new Beacon(new HudsonProjectSource());
            if (!_beacon.Init()) return;


            Mediator.Instance.Register(DoReloadSettings, MediatorMessages.ReloadSettings);

            //Get ProjectListSource, if any.
            _beacon.UpdateSettings(_settings.Uri, 
                                    _settings.Lightness, 
                                    _settings.FetchMinuteIntervall, 
                                    _settings.SuccessPulseIntervall,
                                    _settings.FailurePulseIntervall);
        }

        private void DoReloadSettings(object obj)
        {
            _settings = Settings.Load();

            _beacon.UpdateSettings(_settings.Uri,
                        _settings.Lightness,
                        _settings.FetchMinuteIntervall,
                        _settings.SuccessPulseIntervall,
                        _settings.FailurePulseIntervall);
        }

        internal void Shutdown()
        {
            _notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            _beacon.Shutdown();
        }
    }
}
