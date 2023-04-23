using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S14Device : ActivatorDevice
    {
        public override string DeviceName => DeviceNames.S14;

        public override string Topic => Topics.PRESS;

        public S14Device(IMqttService mqttService)
            : base(mqttService)
        {
        }
    }
}
