using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class B2Device : SensorDevice
    {
        public override string DeviceName => DeviceNames.B2;

        public override string Topic => Topics.LIFTER;

        public B2Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.Q1:
                    if (e.Message.Active && Active)
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
