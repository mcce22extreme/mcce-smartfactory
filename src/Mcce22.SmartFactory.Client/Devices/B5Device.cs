using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class B5Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.B5;

        public override string Topic => Topics.DOOR;

        public B5Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.F2:
                    await ToggleActivation(e.Message.Active);
                    break;
            }
        }
    }
}
