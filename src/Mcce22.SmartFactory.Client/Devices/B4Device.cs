using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class B4Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.B4;

        public override string Topic => Topics.DOOR;

        public B4Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.Q3:
                    if (e.Message.Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
            }
        }

        public override void Reset()
        {
            Active = true;
        }
    }
}
