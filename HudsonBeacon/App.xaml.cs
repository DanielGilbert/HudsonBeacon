using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace HudsonBeacon
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootstrapper ;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _bootstrapper = new Bootstrapper();
            _bootstrapper.Init();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bootstrapper.Shutdown();
            base.OnExit(e);
        }
    }
}
