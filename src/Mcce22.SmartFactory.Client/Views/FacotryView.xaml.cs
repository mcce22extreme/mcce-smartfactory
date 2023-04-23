using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Mcce22.SmartFactory.Client.ViewModels;

namespace Mcce22.SmartFactory.Client.Views
{
    /// <summary>
    /// Interaction logic for DoorSimulatorView.xaml
    /// </summary>
    public partial class FactoryView : UserControl
    {
        private const int DEFAULT_DOOR_HEIGHT = 130;
        private const int DEFAULT_LIFTER_HEIGHT = 8;
        private const int DEFAULT_LIFTER_CANVAS_TOP = 565;
        private const int DEFAULT_PLATFORM_CANVAS_TOP = 485;
        private const int DEFAULT_SHAFT_HEIGHT = 25;
        private const int DEFAULT_PRESS_CANVAS_TOP = 182;

        private readonly Storyboard _lifterUpStoryboard;
        private readonly Storyboard _lifterDownStoryboard;
        private readonly Storyboard _platformUpStoryboard;
        private readonly Storyboard _platformDownStoryboard;
        private readonly Storyboard _pressUpStoryboard;
        private readonly Storyboard _pressDownStoryboard;
        private readonly Storyboard _shaftDownStoryboard;
        private readonly Storyboard _shaftUpStoryboard;

        private FactoryViewModel Factory { get { return DataContext as FactoryViewModel; } }

        public FactoryView()
        {
            InitializeComponent();

            _lifterUpStoryboard = FindResource("LifterUpStoryboard") as Storyboard;
            _lifterDownStoryboard = FindResource("LifterDownStoryboard") as Storyboard;
            _platformUpStoryboard = FindResource("PlatformUpStoryboard") as Storyboard;
            _platformDownStoryboard = FindResource("PlatformDownStoryboard") as Storyboard;
            _pressUpStoryboard = FindResource("PressUpStoryboard") as Storyboard;
            _pressDownStoryboard = FindResource("PressDownStoryboard") as Storyboard;
            _shaftDownStoryboard = FindResource("ShaftDownStoryboard") as Storyboard;
            _shaftUpStoryboard = FindResource("ShaftUpStoryboard") as Storyboard;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Factory.FactoryReseted += OnFactoryReseted;
            Factory.DeviceChanged += OnDeviceChanged;
        }

        private void OnDeviceChanged(object sender, DeviceChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                switch (e.Device.DeviceName)
                {
                    case DeviceNames.S21:
                        if (Factory.S21.Active && !Factory.F1.Active)
                        {
                            _lifterUpStoryboard.Pause(Lifter);
                            _lifterDownStoryboard.Pause(Lifter);

                            _platformUpStoryboard.Pause(Platform);
                            _platformDownStoryboard.Pause(Platform);
                        }
                        break;
                    case DeviceNames.Q1:
                        if (Factory.Q1.Active)
                        {
                            _lifterDownStoryboard.Pause(Lifter);
                            _lifterUpStoryboard.Begin(Lifter, true);

                            _platformDownStoryboard.Pause(Platform);
                            _platformUpStoryboard.Begin(Platform, true);
                        }
                        break;
                    case DeviceNames.Q2:
                        if (Factory.Q2.Active)
                        {
                            _lifterUpStoryboard.Pause(Lifter);
                            _lifterDownStoryboard.Begin(Lifter, true);

                            _platformUpStoryboard.Pause(Platform);
                            _platformDownStoryboard.Begin(Platform, true);
                        }
                        break;
                    case DeviceNames.Q11:
                        if (Factory.Q11.Active)
                        {
                            _shaftUpStoryboard.Pause();
                            _shaftDownStoryboard.Begin(Shaft, true);

                            _pressUpStoryboard.Pause();
                            _pressDownStoryboard.Begin(Press, true);
                        }
                        else
                        {
                            _shaftDownStoryboard.Pause();
                            _shaftUpStoryboard.Begin(Shaft, true);

                            _pressDownStoryboard.Pause();
                            _pressUpStoryboard.Begin(Press, true);
                        }
                        break;
                }
            });
        }

        private void OnFactoryReseted(object sender, EventArgs e)
        {
            // Reset Door
            Door.SetValue(HeightProperty, (double)DEFAULT_DOOR_HEIGHT);

            var openDoorStoryboard = Door.FindResource("OpenDoorStoryboard") as Storyboard;
            openDoorStoryboard?.Seek(TimeSpan.Zero);

            var closeDoorStoryboard = Door.FindResource("CloseDoorStoryboard") as Storyboard;
            closeDoorStoryboard?.Seek(TimeSpan.Zero);

            // Reset Lifter
            Lifter.SetValue(HeightProperty, (double)DEFAULT_LIFTER_HEIGHT);
            Lifter.SetValue(Canvas.TopProperty, (double)DEFAULT_LIFTER_CANVAS_TOP);
            Platform.SetValue(Canvas.TopProperty, (double)DEFAULT_PLATFORM_CANVAS_TOP);

            _lifterUpStoryboard.Stop(Lifter);
            _lifterUpStoryboard.Seek(TimeSpan.Zero);

            _lifterDownStoryboard.Stop(Lifter);
            _lifterDownStoryboard.Seek(TimeSpan.Zero);

            _platformUpStoryboard.Stop(Platform);
            _platformUpStoryboard.Seek(TimeSpan.Zero);

            _platformDownStoryboard.Stop(Platform);
            _platformDownStoryboard.Seek(TimeSpan.Zero);

            // Reset Press
            Shaft.SetValue(HeightProperty, (double)DEFAULT_SHAFT_HEIGHT);
            Press.SetValue(Canvas.TopProperty, (double)DEFAULT_PRESS_CANVAS_TOP);

            _shaftUpStoryboard.Stop(Shaft);
            _shaftUpStoryboard.Seek(TimeSpan.Zero);

            _shaftDownStoryboard.Stop(Shaft);
            _shaftDownStoryboard.Seek(TimeSpan.Zero);

            _pressDownStoryboard.Stop(Press);
            _pressDownStoryboard.Seek(TimeSpan.Zero);

            _pressUpStoryboard.Stop(Press);
            _pressUpStoryboard.Seek(TimeSpan.Zero);
        }

        private async void OnOpenDoorCompleted(object sender, EventArgs e)
        {
            Door.Height = 0;
            await Factory.B3.ToggleActivation(true);

        }

        private async void OnCloseDoorCompleted(object sender, EventArgs e)
        {
            Door.Height = DEFAULT_DOOR_HEIGHT;
            await Factory.B4.ToggleActivation(true);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private async void OnLifterUpCompleted(object sender, EventArgs e)
        {
            await Factory.B1.ToggleActivation(true);
        }

        private async void OnLifterDownCompleted(object sender, EventArgs e)
        {
            await Factory.B2.ToggleActivation(true);
        }
    }
}
