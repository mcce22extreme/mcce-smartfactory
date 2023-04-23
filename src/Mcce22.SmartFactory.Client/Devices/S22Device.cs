using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S22Device : ActivatorDevice
    {
        private bool _b5Active;

        private bool _s3Active;

        public override string DeviceName => DeviceNames.S22;

        public override string Topic => Topics.DOOR;

        public S22Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override void Activate()
        {
            if (!_s3Active && !_b5Active)
            {
                base.Activate();
            }
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.B5:
                    _b5Active = e.Message.Active;
                    break;
                case DeviceNames.S3:
                    _s3Active = e.Message.Active;
                    break;
                case DeviceNames.S22:
                    if (!_s3Active)
                    {
                        Active = e.Message.Active;
                    }
                    break;
                case DeviceNames.Q3:
                case DeviceNames.Q4:
                    if (!e.Message.Active && Active)
                    {
                        Active = false;

                        await PublishMessage(DeviceName, false);
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Reset()
        {
            Active = false;
            _b5Active = false;
            _s3Active = false;
        }
    }
}
