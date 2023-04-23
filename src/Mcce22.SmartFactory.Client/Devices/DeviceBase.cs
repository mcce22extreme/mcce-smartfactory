using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartFactory.Client.Requests;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public interface IDevice : INotifyPropertyChanged
    {
        string DeviceName { get; }

        string Topic { get; }

        void Reset();
    }

    public abstract class DeviceBase : ObservableObject, IDevice
    {
        private readonly IMqttService _mqttService;

        public abstract string DeviceName { get; }

        public abstract string Topic { get; }

        public DeviceBase(IMqttService mqttService)
        {
            _mqttService = mqttService;
            _mqttService.MessageReceived += OnMessageReceived;
        }

        protected virtual void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
        }

        protected async Task PublishMessage(string deviceName, bool active)
        {
            await _mqttService.PublishMessage(new MessageModel
            {
                DeviceId = deviceName,
                Topic = Topic,
                Active = active
            });
        }

        public abstract void Reset();
    }
}
