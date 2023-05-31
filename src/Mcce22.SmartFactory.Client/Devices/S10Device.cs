using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S10Device : ActivatorDevice
    {
        public override string DeviceName => DeviceNames.S10;

        public override string Topic => Topics.LIGHT;

        public S10Device(IMqttService mqttService)
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
