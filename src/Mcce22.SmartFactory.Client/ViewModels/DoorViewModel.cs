using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Requests;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public class DoorViewModel : FactoryPlantBase
    {
        private const string DEVICE_S3 = "s3"; // physical button for opening/closing door
        private const string DEVICE_S22 = "s22"; // radio switch for opening/closing door
        private const string DEVICE_B3 = "b3"; // triggers when the door is fully opened 
        private const string DEVICE_B4 = "b4"; // triggers when the door is fully closed
        private const string DEVICE_B5 = "b5"; // photoelectric barrier
        private const string DEVICE_Q3 = "q3"; // motor that opens door
        private const string DEVICE_Q4 = "q4"; // motor that closes door

        protected override string Topic => Topics.DOOR;

        private bool _s3Active;
        public bool S3Active
        {
            get { return _s3Active; }
            set { SetProperty(ref _s3Active, value); }
        }

        private bool _s22Active;
        public bool S22Active
        {
            get { return _s22Active; }
            set { SetProperty(ref _s22Active, value); }
        }

        private bool _b3Active;
        public bool B3Active
        {
            get { return _b3Active; }
            set { SetProperty(ref _b3Active, value); }
        }

        private bool _b4Active = true;
        public bool B4Active
        {
            get { return _b4Active; }
            set { SetProperty(ref _b4Active, value); }
        }

        private bool _b5Active;
        public bool B5Active
        {
            get { return _b5Active; }
            set { SetProperty(ref _b5Active, value); }
        }

        private bool _q3Active;
        public bool Q3Active
        {
            get { return _q3Active; }
            set { SetProperty(ref _q3Active, value); }
        }

        private bool _q4Active;
        public bool Q4Active
        {
            get { return _q4Active; }
            set { SetProperty(ref _q4Active, value); }
        }

        private bool _emergencyOpen;
        public bool EmergencyOpen
        {
            get { return _emergencyOpen; }
            set { SetProperty(ref _emergencyOpen, value); }
        }

        public RelayCommand S3ActivatedCommand { get; }

        public RelayCommand S22ActivatedCommand { get; }

        public RelayCommand B5ActivatedCommand { get; }

        public DoorViewModel(IMqttService mqttService)
            : base(mqttService)
        {
            S3ActivatedCommand = new RelayCommand(S3Activated);
            S22ActivatedCommand = new RelayCommand(S22Activated);
            B5ActivatedCommand = new RelayCommand(B5Activated);
        }

        private async void S3Activated()
        {
            await PublishMessage(DEVICE_S3, true);
        }

        private async void S22Activated()
        {
            await PublishMessage(DEVICE_S22, true);
        }

        private async void B5Activated()
        {
            await PublishMessage(DEVICE_B5, !B5Active);
        }

        public override async Task HandleRequest(MessageModel request)
        {
            switch (request.DeviceId)
            {
                case DEVICE_S3:
                    S3Active = request.Active;
                    break;
                case DEVICE_S22:
                    S22Active = request.Active;
                    break;
                case DEVICE_Q3:
                    Q3Active = request.Active;
                    if (Q3Active)
                    {
                        await PublishMessage(DEVICE_B4, false);
                    }
                    break;
                case DEVICE_Q4:
                    Q4Active = request.Active;
                    if (Q4Active)
                    {
                        await PublishMessage(DEVICE_B3, false);
                    }
                    break;
                case DEVICE_B3:
                    B3Active = request.Active;
                    break;
                case DEVICE_B4:
                    B4Active = request.Active;
                    break;
                case DEVICE_B5:
                    B5Active = request.Active;

                    if (B5Active)
                    {
                        EmergencyOpen = !Q3Active && !B3Active && !B4Active;
                    }
                    else
                    {
                        EmergencyOpen = false;
                    }
                    break;
            }
        }

        public async void DoorIsOpened()
        {
            await PublishMessage(DEVICE_B3, true);
        }

        public async void DoorIsClosed()
        {
            await PublishMessage(DEVICE_B4, true);
        }

        public override void Initialize()
        {
            S3Active = false;
            S22Active = false;
            B3Active = false;
            B3Active = false;
            B4Active = true;
            B5Active = false;
            Q3Active = false;
            Q4Active = false;
        }
    }
}
