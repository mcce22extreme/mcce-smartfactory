using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class F1Device : ActivatorDevice
    {
        public override string DeviceName => DeviceNames.F1;

        public override string Topic => Topics.LIFTER;

        public F1Device(IMqttService mqttService)
            : base(mqttService)
        {
        }
    }
}
