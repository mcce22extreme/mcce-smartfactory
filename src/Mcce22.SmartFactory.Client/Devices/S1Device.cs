using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S1Device : ActivatorDevice
    {
        private bool _b1Active;

        private bool _f1Active;

        public override string DeviceName => DeviceNames.S1;

        public override string Topic => Topics.LIFTER;

        public S1Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override void Activate()
        {
            if (!_b1Active && !Active && !_f1Active)
            {
                base.Activate();
            }
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S2:
                case DeviceNames.S21:
                    if (e.Message.Active && Active)
                    {
                        Active = false;

                        await PublishMessage(DeviceName, false);
                    }
                    break;
                case DeviceNames.Q1:
                case DeviceNames.Q2:
                    if (!e.Message.Active && Active)
                    {
                        Active = false;

                        await PublishMessage(DeviceName, false);
                    }
                    break;
                case DeviceNames.B1:
                    _b1Active = e.Message.Active;
                    break;
                case DeviceNames.F1:
                    _f1Active = e.Message.Active;
                    break;
            }
        }

        public override void Reset()
        {
            _b1Active = false;
            _f1Active = false;
            Active = false;
        }
    }
}
