using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class Q11Device : SensorDevice
    {
        private bool _s14Active;

        private bool _s15Active;

        public override string DeviceName => DeviceNames.Q11;

        public override string Topic => Topics.PRESS;

        public Q11Device(IMqttService mqttService)
            : base(mqttService)
        {
        }

        protected override async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.S14:
                    _s14Active = e.Message.Active;
                    if (_s14Active && _s15Active && !Active)
                    {
                        await ToggleActivation(true);
                    }
                    else if (Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
                case DeviceNames.S15:
                    _s15Active = e.Message.Active;
                    if (_s14Active && _s15Active && !Active)
                    {
                        await ToggleActivation(true);
                    }
                    else if (Active)
                    {
                        await ToggleActivation(false);
                    }
                    break;
            }
        }
    }
}
