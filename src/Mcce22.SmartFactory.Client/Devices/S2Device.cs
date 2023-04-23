using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S2Device : ActivatorDevice
    {
        private bool _b2Active;

        private bool _f1Active;

        public override string DeviceName => DeviceNames.S2;

        public override string Topic => Topics.LIFTER;

        public S2Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override void Activate()
        {
            if (!_b2Active && !Active && !_f1Active)
            {
                base.Activate();
            }
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S1:
                case DeviceNames.S21:
                    if (e.Message.Active && Active)
                    {
                        Active = false;
                    }
                    break;
                case DeviceNames.Q1:
                case DeviceNames.Q2:
                    if (!e.Message.Active && Active && (!_f1Active || _b2Active))
                    {
                        Active = false;

                        await PublishMessage(DeviceName, false);
                    }
                    break;
                case DeviceNames.B2:
                    _b2Active = e.Message.Active;
                    break;
                case DeviceNames.F1:
                    _f1Active = e.Message.Active;
                    if (_f1Active && !Active)
                    {
                        Active = true;
                    }
                    break;
            }
        }

        public override void Reset()
        {
            _b2Active = false;
            _f1Active = false;
            Active = false;
        }
    }
}
