using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Requests;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public class PressViewModel : FactoryPlantBase
    {
        private const string DEVICE_S14 = "s14";
        private const string DEVICE_S15 = "s15";
        private const string DEVICE_Q11 = "q11";

        protected override string Topic => Topics.PRESS;

        private bool _s14Active;
        public bool S14Active
        {
            get { return _s14Active; }
            set { SetProperty(ref _s14Active, value); }
        }

        private bool _s15Active;
        public bool S15Active
        {
            get { return _s15Active; }
            set { SetProperty(ref _s15Active, value); }
        }

        private bool _q11Active;
        public bool Q11Active
        {
            get { return _q11Active; }
            set { SetProperty(ref _q11Active, value); }
        }

        public RelayCommand S14ActivatedCommand { get; }

        public RelayCommand S15ActivatedCommand { get; }

        public PressViewModel(IMqttService mqttService)
            : base(mqttService)
        {
            S14ActivatedCommand = new RelayCommand(async () => await PublishMessage(DEVICE_S14, !S14Active));
            S15ActivatedCommand = new RelayCommand(async () => await PublishMessage(DEVICE_S15, !S15Active));
        }

        public override void Initialize()
        {
            S14Active = false;
            S15Active = false;
            Q11Active = false;
        }

        public override Task HandleRequest(MessageModel request)
        {
            switch (request.DeviceId)
            {
                case DEVICE_S14:
                    S14Active = request.Active;
                    break;
                case DEVICE_S15:
                    S15Active = request.Active;
                    break;                
                case DEVICE_Q11:
                    Q11Active = request.Active;
                    break;
            }

            return Task.CompletedTask;
        }

        public async void PressUpCompleted()
        {
            await PublishMessage(DEVICE_Q11, true);
        }
    }
}
