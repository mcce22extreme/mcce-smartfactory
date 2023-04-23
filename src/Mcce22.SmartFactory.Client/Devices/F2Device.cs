using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public class F2Device : ActivatorDevice
    {
        public override string DeviceName => DeviceNames.F2;

        public override string Topic => Topics.DOOR;

        public F2Device(IMqttService mqttService)
            : base(mqttService)
        {
        }
    }
}
