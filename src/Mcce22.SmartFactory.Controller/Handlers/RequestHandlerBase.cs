using System.Text;
using Amazon.DynamoDBv2;
using Amazon.IotData;
using Mcce22.SmartFactory.Controller.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartFactory.Controller.Handlers
{
    public abstract class RequestHandlerBase : IRequestHandler
    {
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);

        protected abstract string Topic { get; }

        protected AmazonIotDataClient DataClient { get; }

        protected AmazonDynamoDBClient DynamoDBClient { get; }

        public RequestHandlerBase(string endpointAddress)
        {
            DataClient = new AmazonIotDataClient(endpointAddress);
            DynamoDBClient = new AmazonDynamoDBClient();
        }

        public async Task HandleRequest(RequestModel model)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                await OnHandleRequest(model);
            }
            finally { _semaphoreSlim.Release(); }
        }

        protected abstract Task OnHandleRequest(RequestModel model);

        protected async Task PublishMessage(string deviceId, bool active)
        {
            var json = JsonConvert.SerializeObject(new RequestModel
            {
                DeviceId = deviceId,
                Topic = Topic,
                Active = active
            });

            await DataClient.PublishAsync(new Amazon.IotData.Model.PublishRequest
            {
                Topic = Topic,
                Payload = new MemoryStream(Encoding.UTF8.GetBytes(json))
            });
        }
    }
}
