using System;
using System.ComponentModel;
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

        private readonly Storyboard _lifterUpStoryboard;
        private readonly Storyboard _lifterDownStoryboard;
        private readonly Storyboard _platformUpStoryboard;
        private readonly Storyboard _platformDownStoryboard;

        public FactoryView()
        {
            InitializeComponent();

            _lifterUpStoryboard = FindResource("LifterUpStoryboard") as Storyboard;
            _lifterDownStoryboard = FindResource("LifterDownStoryboard") as Storyboard;
            _platformUpStoryboard = FindResource("PlatformUpStoryboard") as Storyboard;
            _platformDownStoryboard = FindResource("PlatformDownStoryboard") as Storyboard;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var simulator = DataContext as ISimulatorViewModel;

            simulator.FactoryReseted += OnFactoryReseted;
            simulator.Platform.PropertyChanged += OnPlatformPropertyChanged;
        }

        private void OnPlatformPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var factory = DataContext as ISimulatorViewModel;

                if (e.PropertyName == nameof(PlatformViewModel.S21Active) && factory.Platform.S21Active && !factory.Platform.F1Active)
                {
                    _lifterUpStoryboard.Pause(Lifter);
                    _lifterDownStoryboard.Pause(Lifter);

                    _platformUpStoryboard.Pause(Platform);
                    _platformDownStoryboard.Pause(Platform);
                }
                else if (e.PropertyName == nameof(PlatformViewModel.Q1Active))
                {
                    if (factory.Platform.Q1Active)
                    {
                        _lifterDownStoryboard.Pause(Lifter);
                        _lifterUpStoryboard.Begin(Lifter, true);

                        _platformDownStoryboard.Pause(Platform);
                        _platformUpStoryboard.Begin(Platform, true);
                    }
                }
                else if (e.PropertyName == nameof(PlatformViewModel.Q2Active))
                {
                    if (factory.Platform.Q2Active)
                    {
                        _lifterUpStoryboard.Pause(Lifter);
                        _lifterDownStoryboard.Begin(Lifter, true);

                        _platformUpStoryboard.Pause(Platform);
                        _platformDownStoryboard.Begin(Platform, true);
                    }
                }
            });
        }

        private void OnFactoryReseted(object sender, EventArgs e)
        {
            Door.SetValue(HeightProperty, (double)DEFAULT_DOOR_HEIGHT);
            Lifter.SetValue(HeightProperty, (double)DEFAULT_LIFTER_HEIGHT);
            Lifter.SetValue(Canvas.TopProperty, (double)DEFAULT_LIFTER_CANVAS_TOP);
            Platform.SetValue(Canvas.TopProperty, (double)DEFAULT_PLATFORM_CANVAS_TOP);

            var openDoorStoryboard = Door.FindResource("OpenDoorStoryboard") as Storyboard;
            openDoorStoryboard?.Seek(TimeSpan.Zero);

            var closeDoorStoryboard = Door.FindResource("CloseDoorStoryboard") as Storyboard;
            closeDoorStoryboard?.Seek(TimeSpan.Zero);

            _lifterUpStoryboard.Stop(Lifter);
            _lifterUpStoryboard.Seek(TimeSpan.Zero);

            _lifterDownStoryboard.Stop(Lifter);
            _lifterDownStoryboard.Seek(TimeSpan.Zero);

            _platformUpStoryboard.Stop(Platform);
            _platformUpStoryboard.Seek(TimeSpan.Zero);

            _platformDownStoryboard.Stop(Platform);
            _platformDownStoryboard.Seek(TimeSpan.Zero);
        }

        private void OnOpenDoorCompleted(object sender, EventArgs e)
        {
            Door.Height = 0;
            var factory = DataContext as ISimulatorViewModel;
            factory?.Door?.DoorIsOpened();
        }

        private void OnCloseDoorCompleted(object sender, EventArgs e)
        {
            Door.Height = DEFAULT_DOOR_HEIGHT;
            var factory = DataContext as ISimulatorViewModel;
            factory?.Door?.DoorIsClosed();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void OnLifterUpCompleted(object sender, EventArgs e)
        {
            var factory = DataContext as ISimulatorViewModel;
            factory?.Platform?.LifterUpCompleted();
        }

        private void OnLifterDownCompleted(object sender, EventArgs e)
        {
            var factory = DataContext as ISimulatorViewModel;
            factory?.Platform?.LifterDownCompleted();
        }
    }
}
