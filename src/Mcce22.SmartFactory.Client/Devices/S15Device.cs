using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class S15Device : ActivatorDevice
    {
        public override string DeviceName => DeviceNames.S15;

        public override string Topic => Topics.PRESS;

        public S15Device(IMqttService mqttService)
            : base(mqttService)
        {
        }
    }
}
