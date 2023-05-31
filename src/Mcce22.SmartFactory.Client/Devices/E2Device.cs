using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class E2Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.E2;

        public override string Topic => Topics.LIGHT;

        public E2Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S9:
                case DeviceNames.S10:
                case DeviceNames.S11:
                case DeviceNames.S28:
                    await ToggleActivation(e.Message.Active);
                    break;
            }
        }
    }
}
