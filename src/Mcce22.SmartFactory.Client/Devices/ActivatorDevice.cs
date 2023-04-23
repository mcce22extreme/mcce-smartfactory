using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public abstract class ActivatorDevice : DeviceBase
    {
        private bool _active;
        public bool Active
        {
            get { return _active; }
            protected set { SetProperty(ref _active, value); }
        }

        public RelayCommand ActivateCommand { get; }

        protected ActivatorDevice(IMqttService mqttService)
            : base(mqttService)
        {
            ActivateCommand = new RelayCommand(Activate);
        }

        protected virtual async void Activate()
        {
            Active = !Active;

            await PublishMessage(DeviceName, Active);
        }

        public override void Reset()
        {
            Active = false;
        }
    }
}
