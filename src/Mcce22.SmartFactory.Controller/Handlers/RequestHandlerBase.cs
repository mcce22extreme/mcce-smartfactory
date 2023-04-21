using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.IotData;
using Amazon.Lambda.Core;
using Mcce22.SmartFactory.Controller.Entities;
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

                using var context = new DynamoDBContext(DynamoDBClient);

                var deviceState = await context.LoadAsync<DeviceState>(Topic) ?? new DeviceState { Topic = Topic };

                LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

                await OnHandleRequest(model, deviceState);

                LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

                await context.SaveAsync(deviceState);
            }
            finally { _semaphoreSlim.Release(); }
        }

        protected abstract Task OnHandleRequest(RequestModel model, DeviceState deviceState);

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
