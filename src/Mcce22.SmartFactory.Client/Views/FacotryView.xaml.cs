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
            var factory = DataContext as ISimulatorViewModel;

            factory.FactoryReseted += OnFactoryReseted;
            factory.Lifter.PropertyChanged += OnLifterPropertyChanged;
            factory.Press.PropertyChanged += OnPressPropertyChanged;
        }

        private void OnLifterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var factory = DataContext as ISimulatorViewModel;

                switch (e.PropertyName)
                {
                    case nameof(LifterViewModel.S21Active):
                        if (factory.Lifter.S21Active && !factory.Lifter.F1Active)
                        {
                            _lifterUpStoryboard.Pause(Lifter);
                            _lifterDownStoryboard.Pause(Lifter);

                            _platformUpStoryboard.Pause(Platform);
                            _platformDownStoryboard.Pause(Platform);
                        }
                        break;
                    case nameof(LifterViewModel.Q1Active):
                        if (factory.Lifter.Q1Active)
                        {
                            _lifterDownStoryboard.Pause(Lifter);
                            _lifterUpStoryboard.Begin(Lifter, true);

                            _platformDownStoryboard.Pause(Platform);
                            _platformUpStoryboard.Begin(Platform, true);
                        }
                        break;
                    case nameof(LifterViewModel.Q2Active):
                        if (factory.Lifter.Q2Active)
                        {
                            _lifterUpStoryboard.Pause(Lifter);
                            _lifterDownStoryboard.Begin(Lifter, true);

                            _platformUpStoryboard.Pause(Platform);
                            _platformDownStoryboard.Begin(Platform, true);
                        }
                        break;
                }
            });            
        }

        private void OnPressPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var factory = DataContext as ISimulatorViewModel;

                switch (e.PropertyName)
                {
                    case nameof(PressViewModel.Q11Active):
                        if (factory.Press.Q11Active)
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
            factory?.Lifter?.LifterUpCompleted();
        }

        private void OnLifterDownCompleted(object sender, EventArgs e)
        {
            var factory = DataContext as ISimulatorViewModel;
            factory?.Lifter?.LifterDownCompleted();
        }
    }
}
