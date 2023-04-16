using CommunityToolkit.Mvvm.ComponentModel;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public FactoryViewModel DoorSimulator { get; }

        private object _activeContent;
        public object ActiveContent
        {
            get { return _activeContent; }
            set { SetProperty(ref _activeContent, value); }
        }

        public MainViewModel(FactoryViewModel doorSimulator)
        {
            DoorSimulator = doorSimulator;

            ActiveContent = DoorSimulator;
        }
    }
}
