using CommunityToolkit.Mvvm.ComponentModel;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public FactoryViewModel DoorSimulator { get; }

        [ObservableProperty]
        private object _activeContent;

        public MainViewModel(FactoryViewModel doorSimulator)
        {
            DoorSimulator = doorSimulator;

            ActiveContent = DoorSimulator;
        }
    }
}
