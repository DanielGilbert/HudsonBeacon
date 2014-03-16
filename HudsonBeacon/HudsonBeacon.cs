using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Media.Media3D;
using BlinkStick.Hid;
using HudsonBeacon.Implementations;
using HudsonBeacon.Interfaces;
using HudsonBeacon.Helpers;
using HudsonBeacon.Patterns;

namespace HudsonBeacon
{
    enum FadeModeEnum
    {
        FadeIn,
        FadeOut
    }

    public class Beacon
    {
        private IProjectSource _projectSource;
        private List<IProject> _projects;
        private ProjectStateEnum _projectState;

        private Timer _updateDisplayWorker;
        private Timer _fetchDataWorker;

        private AbstractBlinkstickHid _blinkstickDevice;
        private FadeModeEnum _fadeMode;
        private RgbColor _color;
        private RgbColor _oldColor;
        private bool _isPulse;
        private bool _displayLong;
        private int _targetPulses = 1;
        private double _animationSpeed = 0.0025;

        private double _maxLightness = 0.05;
        private int _timeoutBetweenPulses = 10000;

        public string ProjectListSource { get; set; }

        public string SourceUri
        {
            get;
            set;
        }

        public int FailurePulseIntervall
        {
            get;
            set;
        }

        public int SuccessPulseIntervall
        {
            get;
            set;
        }

        public int FetchIntervall { get; set; }

        public int Lightness { get; set; }

        public Beacon(IProjectSource projectSource)
        {
            if (projectSource == null)
                throw new ArgumentNullException("projectSource");

            _projectSource = projectSource;

        }

        public bool Init()
        {
            _projectState = ProjectStateEnum.Success;
            _isPulse = false;
            _displayLong = false;
            _color = new RgbColor { R = 0, G = 0, B = 0 };
            _oldColor = new RgbColor { R = 0, G = 0, B = 0 };

            if (FailurePulseIntervall == 0)
                FailurePulseIntervall = 1;

            if (SuccessPulseIntervall == 0)
                SuccessPulseIntervall = 1;

            if (FetchIntervall == 0)
                FetchIntervall = 30;

            if (Lightness == 0)
                Lightness = 100;

            //Not that perfect, but hey... works, bitches!
            foreach (AbstractBlinkstickHid device in BlinkstickDeviceFinder.FindDevices())
            {
                _blinkstickDevice = device;
            }

            if (_blinkstickDevice == null) return false;

            if (!_blinkstickDevice.OpenDevice()) return false;

            SendColor(0, 0, 0);

            _fetchDataWorker = new Timer(OnDataFetchRequested);
            _fetchDataWorker.Change(Timeout.Infinite, Timeout.Infinite);

            _updateDisplayWorker = new Timer(OnDisplayUpdateRequested);
            _updateDisplayWorker.Change(Timeout.Infinite, Timeout.Infinite);

            return true;
        }

        private void OnDataFetchRequested(object state)
        {
            try
            {
                List<IProject> projects = _projectSource.GetProjectList(ProjectListSource);

                var failedProjects = from p in projects where p.ProjectState == ProjectStateEnum.Failed select p;

                _projectState = failedProjects.Any() ? ProjectStateEnum.Failed : ProjectStateEnum.Success;

                switch (_projectState)
                {
                    case ProjectStateEnum.Failed:
                        DisplayFailure();
                        break;

                    case ProjectStateEnum.Success:
                        DisplaySuccess();
                        break;
                }
            }
            catch
            {
                DisplayNoData();
            }

            _fetchDataWorker.Change(FetchIntervall*60000, Timeout.Infinite);
        }

        private void OnDisplayUpdateRequested(object state)
        {
            PulseColor();
            _updateDisplayWorker.Change(_timeoutBetweenPulses, Timeout.Infinite);
        }

        public void Reset()
        {
            _updateDisplayWorker.Change(Timeout.Infinite, Timeout.Infinite);
            _fetchDataWorker.Change(Timeout.Infinite, Timeout.Infinite);

            SendColor(0,0,0);
        }

        private void DisplayFailure()
        {
            _isPulse = true;
            _displayLong = false;

            _animationSpeed = 0.00005 * Lightness;
            _maxLightness = 0.005 * Lightness;

            _targetPulses = 4;
            _timeoutBetweenPulses = FailurePulseIntervall * 1000;

            lock (_color)
            {
                _color = new RgbColor { B = 0, G = 0, R = 255 };
            }
        }

        private void DisplaySuccess()
        {
            _isPulse = true;
            _displayLong = false;
            _targetPulses = 1;
            _animationSpeed = 0.00005 * Lightness;
            _maxLightness = 0.005 * Lightness;


            _timeoutBetweenPulses = SuccessPulseIntervall * 60000;

            lock (_color)
            {
                _color = new RgbColor { B = 0, G = 255, R = 0 };
            }
        }

        private void DisplayNoData()
        {
            _isPulse = true;
            _displayLong = false;

            _animationSpeed = 0.00005 * Lightness;
            _maxLightness = 0.005 * Lightness;

            _targetPulses = 4;
            _timeoutBetweenPulses = FailurePulseIntervall * 1000;
            ;

            lock (_color)
            {
                _color = new RgbColor { B = 255, G = 0, R = 0 };
            }
        }

        private void PulseColor()
        {
            HSLColor currentColor = new HSLColor(_color);
            double targetLightness = currentColor.Lightness;

            if (targetLightness > _maxLightness)
                targetLightness = _maxLightness;

            currentColor.Lightness = 0;

            _fadeMode = FadeModeEnum.FadeIn;

            int pulseCount = 0;

            while (true)
            {
                if (_fadeMode == FadeModeEnum.FadeIn)
                {
                    if (currentColor.Lightness >= targetLightness)
                    {
                        //fade out
                        _fadeMode = FadeModeEnum.FadeOut;
                    }
                    else
                    {
                        currentColor.Lightness += _animationSpeed;
                        Thread.Sleep(5);

                        SendColor(currentColor.Color);
                    }
                }
                else
                {
                    if (currentColor.Lightness <= 0)
                    {
                        _fadeMode = FadeModeEnum.FadeIn;

                        pulseCount++;



                        if (pulseCount >= _targetPulses)
                        {
                            break;
                        }
                    }
                    else
                    {
                        currentColor.Lightness -= _animationSpeed;
                        Thread.Sleep(5);

                        SendColor(currentColor.Color);
                    }
                }
            }
        }

        public void SendColor(byte r, byte g, byte b)
        {
            if (_blinkstickDevice.Connected)
            {
                _blinkstickDevice.SetLedColor(r, g, b);
            }
        }

        public void SendColor(RgbColor newColor)
        {
            if (_blinkstickDevice.Connected)
            {
                _blinkstickDevice.SetLedColor(newColor.R, newColor.G, newColor.B);
            }
        }

        public void Shutdown()
        {
            Reset();

            if (_blinkstickDevice != null)
                _blinkstickDevice.CloseDevice();
        }

        public void UpdateSettings(string uri, int lightness, int fetchDataIntervall, int successPulseIntervall,
            int failurePulseIntervall)
        {
            Reset();

            ProjectListSource = uri;
            Lightness = lightness;
            FetchIntervall = fetchDataIntervall;
            SuccessPulseIntervall = successPulseIntervall;
            FailurePulseIntervall = failurePulseIntervall;

            _fetchDataWorker.Change(0, Timeout.Infinite);
            _updateDisplayWorker.Change(0, Timeout.Infinite);
        }
    }
}
