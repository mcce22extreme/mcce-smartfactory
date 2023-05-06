using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S9Device : ActivatorDevice
    {
        public override string DeviceName => DeviceNames.S9;

        public override string Topic => Topics.LIGHT;

        public S9Device(IMqttService mqttService)
            : base(mqttService)
        {            
        }

        protected override void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            switch (e.Message.DeviceId)
            {
                case DeviceNames.E2:
                    Active = e.Message.Active;
                    break;
            }
        }
    }
}
