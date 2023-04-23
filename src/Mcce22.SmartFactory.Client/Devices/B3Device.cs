using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class B3Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.B3;

        public override string Topic => Topics.DOOR;

        public B3Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.Q4:
                    if (e.Message.Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
            }
        }
    }
}
