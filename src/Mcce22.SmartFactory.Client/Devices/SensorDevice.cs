using System.Threading.Tasks;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.Devices
{
    public abstract class SensorDevice : DeviceBase
    {
        private bool _active;
        public bool Active
        {
            get { return _active; }
            protected set { SetProperty(ref _active, value); }
        }

        protected SensorDevice(IMqttService mqttService)
            : base(mqttService)
        {
        }

        public async Task ToggleActivation(bool active)
        {
            if (Active != active)
            {
                Active = active;

                await PublishMessage(DeviceName, Active);
            }
        }

        public override void Reset()
        {
            Active = false;
        }
    }
}
