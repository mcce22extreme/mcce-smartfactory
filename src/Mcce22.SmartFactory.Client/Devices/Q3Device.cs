using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class Q3Device : SensorDevice
    {
        private bool _b3Active;

        private bool _b4Active;

        public override string DeviceName => DeviceNames.Q3;

        public override string Topic => Topics.DOOR;

        public Q3Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.B5:
                    if (!_b3Active && !_b4Active)
                    {
                        await ToggleActivation(true);
                    }
                    break;
                case DeviceNames.S3:
                case DeviceNames.S22:
                    if (_b4Active)
                    {
                        await ToggleActivation(e.Message.Active);
                    }
                    break;
                case DeviceNames.B3:
                    _b3Active = e.Message.Active;
                    if (_b3Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
                case DeviceNames.B4:
                    _b4Active = e.Message.Active;
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
