using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public abstract partial class ActivatorDevice : DeviceBase
    {
        [ObservableProperty]
        private bool _active;
        
        protected ActivatorDevice(IMqttService mqttService)
            : base(mqttService)
        {
        }

        [RelayCommand]
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
