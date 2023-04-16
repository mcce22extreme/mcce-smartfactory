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

        public FactoryView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((ISimulatorViewModel)DataContext).FactoryReseted += OnFactoryReseted;
        }

        private void OnFactoryReseted(object sender, EventArgs e)
        {
            Door.Height = DEFAULT_DOOR_HEIGHT;

            var openDoorStoryboard = Door.FindResource("OpenDoorStoryboard") as Storyboard;
            var closeDoorStoryboard = Door.FindResource("CloseDoorStoryboard") as Storyboard;

            openDoorStoryboard?.Seek(TimeSpan.Zero);
            closeDoorStoryboard?.Seek(TimeSpan.Zero);
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
            var simulator = DataContext as ISimulatorViewModel;
            simulator?.Door?.DoorIsClosed();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void OnLifterUpCompleted(object sender, EventArgs e)
        {

        }

        private void OnLifterDownCompleted(object sender, EventArgs e)
        {

        }
    }
}
