using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S21Device : ActivatorDevice
    {
        private bool _f1Active;

        public override string DeviceName => DeviceNames.S21;

        public override string Topic => Topics.LIFTER;

        public S21Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override void Activate()
        {
            if (!Active && !_f1Active)
            {
                base.Activate();
            }
        }

        protected override void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S1:
                case DeviceNames.S2:
                    Active = false;
                    break;
                case DeviceNames.F1:
                    _f1Active = e.Message.Active;
                    break;
            }
        }

        public override void Reset()
        {
            _f1Active = false;
            Active = false;
        }
    }
}
