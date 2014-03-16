using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using HudsonBeacon.MvvmHelper;

namespace HudsonBeacon.ViewModels
{
    public class NotifyIconViewModel : ViewModelBase
    {
        public ICommand ShowWindowCommand { get; set; }
        public ICommand HideWindowCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }

        public NotifyIconViewModel()
        {
            ShowWindowCommand = new RelayCommand(ShowWindow, CanShowWindow);
            HideWindowCommand = new RelayCommand(HideWindow, CanHideWindow);
            ExitApplicationCommand = new RelayCommand(ExitApplication);
        }

        #region Command Implementations
        private void ShowWindow(object obj)
        {
            Application.Current.MainWindow = new Shell();
            Application.Current.MainWindow.Show();
        }

        private bool CanShowWindow(object o)
        {
            return Application.Current.MainWindow == null;
        }

        private void HideWindow(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        private bool CanHideWindow(object obj)
        {
            return Application.Current.MainWindow != null;
        }

        private void ExitApplication(object obj)
        {
            Application.Current.Shutdown();
        }
        #endregion
    }
}
