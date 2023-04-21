using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Requests;
using Mcce22.SmartFactory.Client.Services;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public class LifterViewModel : FactoryPlantBase
    {
        private const string DEVICE_S1 = "s1";
        private const string DEVICE_S21 = "s21";
        private const string DEVICE_S2 = "S2";
        private const string DEVICE_B1 = "b1";
        private const string DEVICE_B2 = "b2";
        private const string DEVICE_Q1 = "q1";
        private const string DEVICE_Q2 = "q2";
        private const string DEVICE_F1 = "f1";

        protected override string Topic => Topics.LIFTER;

        private bool _s1Active;
        public bool S1Active
        {
            get { return _s1Active; }
            set { SetProperty(ref _s1Active, value); }
        }

        private bool _s21Active;
        public bool S21Active
        {
            get { return _s21Active; }
            set { SetProperty(ref _s21Active, value); }
        }

        private bool _s2Active;
        public bool S2Active
        {
            get { return _s2Active; }
            set { SetProperty(ref _s2Active, value); }
        }

        private bool _b1Active;
        public bool B1Active
        {
            get { return _b1Active; }
            set { SetProperty(ref _b1Active, value); }
        }

        private bool _b2Active = true;
        public bool B2Active
        {
            get { return _b2Active; }
            set { SetProperty(ref _b2Active, value); }
        }

        private bool _q1Active;
        public bool Q1Active
        {
            get { return _q1Active; }
            set { SetProperty(ref _q1Active, value); }
        }

        private bool _q2Active;
        public bool Q2Active
        {
            get { return _q2Active; }
            set { SetProperty(ref _q2Active, value); }
        }

        private bool _f1Active = true;
        public bool F1Active
        {
            get { return _f1Active; }
            set { SetProperty(ref _f1Active, value); }
        }

        public RelayCommand S1ActivatedCommand { get; }

        public RelayCommand S21ActivatedCommand { get; }

        public RelayCommand S2ActivatedCommand { get; }

        public RelayCommand F1ActivatedCommand { get; }

        public LifterViewModel(IMqttService mqttService)
            : base(mqttService)
        {
            S1ActivatedCommand = new RelayCommand(async () => await PublishMessage(DEVICE_S1, true));
            S21ActivatedCommand = new RelayCommand(async () => await PublishMessage(DEVICE_S21, true));
            S2ActivatedCommand = new RelayCommand(async () => await PublishMessage(DEVICE_S2, true));
            F1ActivatedCommand = new RelayCommand(async () => await PublishMessage(DEVICE_F1, true));
        }

        public override void Initialize()
        {
            S1Active = false;
            S21Active = false;
            S2Active = false;
            B1Active = false;
            B2Active = true;
            Q1Active = false;
            Q2Active = false;
            F1Active = false;
        }

        public override async Task HandleRequest(MessageModel request)
        {
            switch (request.DeviceId)
            {
                case DEVICE_S1:
                    S1Active = request.Active;
                    break;
                case DEVICE_S21:
                    S21Active = request.Active;
                    break;
                case DEVICE_S2:
                    S2Active = request.Active;
                    break;
                case DEVICE_Q1:
                    Q1Active = request.Active;
                    if (Q1Active)
                    {
                        await PublishMessage(DEVICE_B2, false);
                    }
                    break;
                case DEVICE_Q2:
                    Q2Active = request.Active;
                    if (Q2Active)
                    {
                        await PublishMessage(DEVICE_B1, false);
                    }
                    break;
                case DEVICE_B1:
                    B1Active = request.Active;
                    break;
                case DEVICE_B2:
                    B2Active = request.Active;
                    break;
                case DEVICE_F1:
                    F1Active = request.Active;
                    break;
            }
        }       

        public async void LifterUpCompleted()
        {
            await PublishMessage(DEVICE_B1, true);
        }

        public async void LifterDownCompleted()
        {
            await PublishMessage(DEVICE_B2, true);
        }
    }
}
