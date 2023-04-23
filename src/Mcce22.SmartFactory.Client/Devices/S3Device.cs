using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S3Device : ActivatorDevice
    {
        private bool _b5Active;

        private bool _s22Active;

        public override string DeviceName => DeviceNames.S3;

        public override string Topic => Topics.DOOR;

        public S3Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override void Activate()
        {
            if (!_s22Active && !_b5Active)
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
                case DeviceNames.S22:
                    _s22Active = e.Message.Active;
                    break;
                case DeviceNames.S3:
                    Active = e.Message.Active;
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
            _s22Active = false;
        }
    }
}
