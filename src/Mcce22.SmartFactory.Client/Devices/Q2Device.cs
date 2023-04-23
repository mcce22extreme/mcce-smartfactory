using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class Q2Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.Q2;

        public override string Topic => Topics.LIFTER;

        public Q2Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S2:
                case DeviceNames.F1:
                    if (e.Message.Active && !Active)
                    {
                        await ToggleActivation(true);
                    }
                    else if (!e.Message.Active && Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
                case DeviceNames.S1:
                case DeviceNames.S21:
                case DeviceNames.B2:
                    if (e.Message.Active && Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
            }
        }
    }
}
