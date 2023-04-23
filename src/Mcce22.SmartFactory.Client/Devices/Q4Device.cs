using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class Q4Device : SensorDevice
    {
        private bool _b3Active;

        private bool _b4Active;

        public override string DeviceName => DeviceNames.Q4;

        public override string Topic => Topics.DOOR;

        public Q4Device(IMqttService mqttService) : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S3:
                case DeviceNames.S22:
                    if (_b3Active)
                    {
                        await ToggleActivation(e.Message.Active);
                    }
                    break;
                case DeviceNames.B3:
                    _b3Active = e.Message.Active;
                    break;
                case DeviceNames.B4:
                    _b4Active = e.Message.Active;
                    if (_b4Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
                case DeviceNames.B5:
                    if (e.Message.Active && Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
            }
        }

        public override void Reset()
        {
            Active = false;
            _b3Active = false;
            _b4Active = true;
        }
    }
}
