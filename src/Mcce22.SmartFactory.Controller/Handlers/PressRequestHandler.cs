using Mcce22.SmartFactory.Controller.Entities;
using Mcce22.SmartFactory.Controller.Models;

namespace Mcce22.SmartFactory.Controller.Handlers
{
    public class PressRequestHandler : RequestHandlerBase
    {
        private const string DEVICE_S14 = "s14";
        private const string DEVICE_S15 = "s15";
        private const string DEVICE_Q11 = "q11";

        protected override string Topic => Topics.PRESS;

        public PressRequestHandler(string endpointAddress)
            : base(endpointAddress)
        {
        }

        protected override async Task OnHandleRequest(RequestModel model, DeviceState deviceState)
        {
            switch (model.DeviceId)
            {
                case DEVICE_S14:
                    await HandleS14Request(model, deviceState);
                    break;
                case DEVICE_S15:
                    await HandleS15Request(model, deviceState);
                    break;
            }
        }

        private async Task HandleS14Request(RequestModel model, DeviceState deviceState)
        {
            if (deviceState.S14 == model.Active)
            {
                // Same state => do nothing
                return;
            }

            deviceState.S14 = model.Active;

            if (deviceState.S14 && deviceState.S15)
            {
                // Both, S14 and S15 are pressed => active Q11

                deviceState.Q11 = true;

                await PublishMessage(DEVICE_Q11, true);
            }
            else if (deviceState.Q11)
            {
                // S14 has been released and Q11 is active => deactivate Q11

                deviceState.Q11 = false;

                await PublishMessage(DEVICE_Q11, false);
            }
        }

        private async Task HandleS15Request(RequestModel model, DeviceState deviceState)
        {
            if (deviceState.S15 == model.Active)
            {
                // Same state => do nothing
                return;
            }

            deviceState.S15 = model.Active;

            if (deviceState.S14 && deviceState.S15)
            {
                // Both, S14 and S15 are pressed => active Q11

                deviceState.Q11 = true;

                await PublishMessage(DEVICE_Q11, true);
            }
            else if (deviceState.Q11)
            {
                // S14 has been released and Q11 is active => deactivate Q11

                deviceState.Q11 = false;

                await PublishMessage(DEVICE_Q11, false);
            }
        }
    }
}
