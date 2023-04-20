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

        protected override string Topic => Topics.PLATFORM;

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
                    await HandleS21Request(model);
                    break;
                case DEVICE_S2:
                    await HandleS2Request(model);
                    break;
                case DEVICE_B1:
                    await HandleB1Request(model);
                    break;
                case DEVICE_B2:
                    await HandleB2Request(model);
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

            if (deviceState.F1 && model.Active)
            {
                // F1 has been triggerd => deactivate S21 again
                await PublishMessage(DEVICE_S1, false);
                return;
            }

            if (model.Active)
            {
                if (deviceState.S1)
                {
                    // Already active => do nothing
                    return;
                }

                if (deviceState.S2)
                {
                    deviceState.S2 = false;
                    deviceState.Q2 = false;

                    await PublishMessage(DEVICE_S2, false);
                    await PublishMessage(DEVICE_Q2, false);
                }

                if (deviceState.S21)
                {
                    deviceState.S21 = false;

                    await PublishMessage(DEVICE_S21, false);
                }

                deviceState.S1 = true;
                deviceState.Q1 = true;

                await PublishMessage(DEVICE_Q1, true);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleS2Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.PLATFORM) ?? new DeviceState { Topic = Topics.PLATFORM };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            if (model.Active)
            {
                if (deviceState.S2)
                {
                    // Already active => do nothing
                    return;
                }

                if (deviceState.S1)
                {
                    deviceState.S1 = false;
                    deviceState.Q1 = false;

                    await PublishMessage(DEVICE_S1, false);
                    await PublishMessage(DEVICE_Q1, false);
                }

                if (deviceState.S21)
                {
                    deviceState.S21 = false;

                    await PublishMessage(DEVICE_S21, false);
                }

                deviceState.S2 = true;
                deviceState.Q2 = true;

                await PublishMessage(DEVICE_Q2, true);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleS21Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.PLATFORM) ?? new DeviceState { Topic = Topics.PLATFORM };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            if (deviceState.F1 && model.Active)
            {
                // F1 has been triggerd => deactivate S21 again
                await PublishMessage(DEVICE_S21, false);
                return;
            }

            deviceState.S21 = model.Active;

            if (deviceState.S21)
            {
                if (deviceState.Q1)
                {
                    deviceState.Q1 = false;
                    deviceState.S1 = false;

                    await PublishMessage(DEVICE_Q1, false);
                    await PublishMessage(DEVICE_S1, false);
                }
                else if (deviceState.Q2)
                {
                    deviceState.Q2 = false;
                    deviceState.S2 = false;

                    await PublishMessage(DEVICE_Q2, false);
                    await PublishMessage(DEVICE_S2, false);
                }
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleF1Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.PLATFORM) ?? new DeviceState { Topic = Topics.PLATFORM };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            if (deviceState.F1 == model.Active)
            {
                // Already same state => do nothing
                return;
            }

            deviceState.F1 = model.Active;

            if (deviceState.F1)
            {
                if (deviceState.Q1)
                {
                    // deactive Q1+S1
                    deviceState.Q1 = false;
                    deviceState.S1 = false;                    

                    await PublishMessage(DEVICE_Q1, false);
                    await PublishMessage(DEVICE_S1, false);
                }

                if (!deviceState.Q2)
                {
                    // activate Q2+S2
                    deviceState.Q2 = true;
                    deviceState.S2 = true;

                    await PublishMessage(DEVICE_Q2, true);
                    await PublishMessage(DEVICE_S2, true);
                }
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleB1Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.PLATFORM) ?? new DeviceState { Topic = Topics.PLATFORM };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            deviceState.B1 = model.Active;

            if (deviceState.B1)
            {
                // B1 has been triggered => lifter reached top => stop Q1
                deviceState.Q3 = false;
                deviceState.S1 = false;

                await PublishMessage(DEVICE_Q1, false);
                await PublishMessage(DEVICE_S1, false);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleB2Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.PLATFORM) ?? new DeviceState { Topic = Topics.PLATFORM };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            deviceState.B2 = model.Active;

            if (deviceState.B2)
            {
                // B2 has been triggered => lifter reached bottom => stop Q2

                deviceState.Q2 = false;
                deviceState.S2 = false;

                await PublishMessage(DEVICE_Q2, false);
                await PublishMessage(DEVICE_S2, false);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }
    }
}
