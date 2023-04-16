using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mcce22.SmartFactory.Client.Requests;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public abstract class FactoryPlantBase : ObservableObject
    {
        private readonly IMqttClient _mqttClient;

        protected abstract string Topic { get; }

        public FactoryPlantBase(IMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        protected async Task PublishMessage(string deviceId, bool active)
        {
            if (_mqttClient.IsConnected)
            {
                var request = new RequestModel
                {
                    DeviceId = deviceId,
                    Topic = Topic,
                    Active = active
                };

                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(Topic)
                    .WithPayload(JsonConvert.SerializeObject(request))
                    .Build();

                await _mqttClient.PublishAsync(msg, CancellationToken.None);
            }
        }

        public abstract void Initialize();

        public abstract Task HandleRequest(RequestModel request);
    }
}
