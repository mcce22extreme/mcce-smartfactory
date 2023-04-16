using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Mcce22.SmartFactory.Controller.Entities;
using Mcce22.SmartFactory.Controller.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartFactory.Controller.Handlers
{
    public class PlatformRequestHandler : RequestHandlerBase
    {
        private const string DEVICE_S1 = "s1";
        private const string DEVICE_S21 = "s21";
        private const string DEVICE_S2 = "S2";
        private const string DEVICE_B1 = "b1";
        private const string DEVICE_B2 = "b2";
        private const string DEVICE_Q1 = "q1";
        private const string DEVICE_Q2 = "q2";
        private const string DEVICE_F1 = "f1";


        public PlatformRequestHandler(string endpointAddress)
            : base(endpointAddress)
        {
        }

        protected override async Task OnHandleRequest(RequestModel model)
        {
            switch (model.DeviceId)
            {
                case DEVICE_S1:
                    await HandleS1Request(model);
                    break;
                case DEVICE_S21:
                    await Handle21Request(model);
                    break;
                case DEVICE_S2:
                    await Handle2Request(model);
                    break;
                case DEVICE_B1:
                    await HandleB4Request(model);
                    break;
                case DEVICE_B2:
                    await HandleB5Request(model);
                    break;
                case DEVICE_F1:
                    await HandleF1Request(model);
                    break;
            }
        }

        private async Task HandleS1Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.PLATFORM) ?? new DeviceState { Topic = Topics.PLATFORM };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            if (model.Active)
            {
                if (deviceState.S1)
                {
                    // Already active => do nothing
                    return;
                }

                deviceState.S1 = true;
                deviceState.Q1 = true;

                await PublishMessage(DEVICE_Q1, true);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private Task HandleF1Request(RequestModel model)
        {
            throw new NotImplementedException();
        }

        private Task HandleB5Request(RequestModel model)
        {
            throw new NotImplementedException();
        }

        private Task HandleB4Request(RequestModel model)
        {
            throw new NotImplementedException();
        }

        private Task Handle2Request(RequestModel model)
        {
            throw new NotImplementedException();
        }

        private Task Handle21Request(RequestModel model)
        {
            throw new NotImplementedException();
        }


    }
}
