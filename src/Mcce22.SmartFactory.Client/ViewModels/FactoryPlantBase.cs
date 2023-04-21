using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartFactory.Client.Requests;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public abstract class FactoryPlantBase : ObservableObject
    {
        private readonly IMqttService _mqttService;

        protected abstract string Topic { get; }

        public FactoryPlantBase(IMqttService mqttService)
        {
            _mqttService = mqttService;
        }

        protected async Task PublishMessage(string deviceId, bool active)
        {
            await _mqttService.PublishMessage(new MessageModel
            {
                DeviceId = deviceId,
                Topic = Topic,
                Active = active
            });
        }

        public abstract void Initialize();

        public abstract Task HandleRequest(MessageModel request);
    }
}
