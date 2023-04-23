using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class B1Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.B1;

        public override string Topic => Topics.LIFTER;

        public B1Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.Q2:
                    if (e.Message.Active && Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
            }
        }
    }
}
