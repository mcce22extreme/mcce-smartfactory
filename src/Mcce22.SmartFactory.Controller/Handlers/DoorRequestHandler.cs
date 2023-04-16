using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Mcce22.SmartFactory.Controller.Entities;
using Mcce22.SmartFactory.Controller.Models;
using Newtonsoft.Json;

namespace Mcce22.SmartFactory.Controller.Handlers
{
    public class DoorRequestHandler : RequestHandlerBase
    {
        private const string DEVICE_S3 = "s3";
        private const string DEVICE_S22 = "s22";
        private const string DEVICE_B3 = "b3";
        private const string DEVICE_B4 = "b4";
        private const string DEVICE_B5 = "b5";
        private const string DEVICE_Q3 = "q3";
        private const string DEVICE_Q4 = "q4";

        public DoorRequestHandler(string endpointAddress)
            : base(endpointAddress)
        {
        }

        protected override async Task OnHandleRequest(RequestModel model)
        {
            switch (model.DeviceId)
            {
                case DEVICE_S3:
                case DEVICE_S22:
                    await HandleS3_S22Request(model);
                    break;
                case DEVICE_B3:
                    await HandleB3Request(model);
                    break;
                case DEVICE_B4:
                    await HandleB4Request(model);
                    break;
                case DEVICE_B5:
                    await HandleB5Request(model);
                    break;
            }
        }

        private async Task HandleB5Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.DOOR) ?? new DeviceState { Topic = Topics.DOOR };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            deviceState.B5 = model.Active;

            if (model.Active && !(deviceState.B3 || deviceState.B4))
            {
                // B5 has been triggered => stop Q4 and start Q3 (open door)
                if (deviceState.Q4)
                {
                    deviceState.Q4 = false;

                    await PublishMessage(DEVICE_Q4, false);
                }

                if (!deviceState.Q3)
                {
                    deviceState.Q3 = true;

                    await PublishMessage(DEVICE_Q3, true);
                }
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);

        }

        private async Task HandleB3Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.DOOR) ?? new DeviceState { Topic = Topics.DOOR };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            deviceState.B3 = model.Active;

            if (deviceState.B3)
            {
                // B3 has been triggered => door is open => stop Q3
                deviceState.Q3 = false;

                await PublishMessage(DEVICE_Q3, false);
                await PublishMessage(DEVICE_S3, false);
                await PublishMessage(DEVICE_S22, false);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleB4Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            var deviceState = await context.LoadAsync<DeviceState>(Topics.DOOR) ?? new DeviceState { Topic = Topics.DOOR };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            deviceState.B4 = model.Active;

            if (deviceState.B4)
            {
                // B4 has been triggered => door is closed => stop Q4

                deviceState.Q4 = false;

                await PublishMessage(DEVICE_Q4, false);
                await PublishMessage(DEVICE_S3, false);
                await PublishMessage(DEVICE_S22, false);
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }

        private async Task HandleS3_S22Request(RequestModel model)
        {
            using var context = new DynamoDBContext(DynamoDBClient);

            // Load device states
            var deviceState = await context.LoadAsync<DeviceState>(Topics.DOOR) ?? new DeviceState { Topic = Topics.DOOR };

            LambdaLogger.Log($"Device States before: {JsonConvert.SerializeObject(deviceState)}");

            if (model.Active)
            {
                if (deviceState.S3 || deviceState.S22)
                {
                    // Already active => do nothing
                    return;
                }

                deviceState.S3 = deviceState.S22 = true;

                // Check if door is open
                if (deviceState.B3)
                {
                    // Check if B5 is active
                    if (deviceState.B5)
                    {
                        // Action not allowed
                        deviceState.S3 = deviceState.S22 = false;
                        await PublishMessage(DEVICE_S3, false);
                        await PublishMessage(DEVICE_S22, false);
                    }
                    else
                    {
                        // Activate Q4
                        deviceState.Q4 = true;

                        await PublishMessage(DEVICE_Q4, true);
                    }
                }

                // Check if door is closed
                else if (deviceState.B4)
                {
                    // Activate Q3
                    deviceState.Q3 = true;

                    await PublishMessage(DEVICE_Q3, true);
                }

                // Check if we dont have a state for b3 and b4 (init)
                else if (!deviceState.B3 && !deviceState.B4)
                {
                    // Activate B4 since initial state of door is closed
                    deviceState.B4 = true;

                    // Activate Q3
                    deviceState.Q3 = true;

                    await PublishMessage(DEVICE_Q3, true);
                }
            }
            else
            {
                deviceState.S3 = deviceState.S22 = false;
            }

            LambdaLogger.Log($"Device States after: {JsonConvert.SerializeObject(deviceState)}");

            await context.SaveAsync(deviceState);
        }
    }
}
