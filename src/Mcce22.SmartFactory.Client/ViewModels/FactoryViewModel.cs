using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Entities;
using Mcce22.SmartFactory.Client.Services;
using Newtonsoft.Json;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public interface ISimulatorViewModel : INotifyPropertyChanged
    {
        DoorViewModel Door { get; }

        LifterViewModel Lifter { get; }

        PressViewModel Press { get; }

        event EventHandler FactoryReseted;
    }

    public class FactoryViewModel : ObservableObject, ISimulatorViewModel
    {
        private static readonly AmazonDynamoDBClient _dynamoDBClient = new AmazonDynamoDBClient();

        private readonly IMqttService _mqttService;

        public event EventHandler FactoryReseted;

        public DoorViewModel Door { get; }

        public LifterViewModel Lifter { get; }

        public PressViewModel Press { get; }

        private string _messageLog;
        public string MessageLog
        {
            get { return _messageLog; }
            set { SetProperty(ref _messageLog, value); }
        }      

        private bool _factoryStarted;
        public bool FactoryStarted
        {
            get { return _factoryStarted; }
            set
            {
                if (SetProperty(ref _factoryStarted, value))
                {
                    StartFactoryCommand.NotifyCanExecuteChanged();
                    ResetFactoryCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _startingFactory;
        public bool FactoryStarting
        {
            get { return _startingFactory; }
            set
            {
                if (SetProperty(ref _startingFactory, value))
                {
                    StartFactoryCommand.NotifyCanExecuteChanged();
                    ResetFactoryCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public RelayCommand StartFactoryCommand { get; }

        public RelayCommand ResetFactoryCommand { get; }

        public RelayCommand ClearMessageLogCommand { get; }

        public FactoryViewModel(IMqttService mqttService)
        {
            _mqttService = mqttService;
            _mqttService.MessageReceived += OnMqttMessageReceived;

            Door = new DoorViewModel(mqttService);
            Lifter = new LifterViewModel(mqttService);
            Press = new PressViewModel(mqttService);

            StartFactoryCommand = new RelayCommand(StartFactory, CanStartFactory);
            ResetFactoryCommand = new RelayCommand(ResetFactory, CanResetFactory);
            ClearMessageLogCommand = new RelayCommand(ClearMessageLog);            
        }

        private void ClearMessageLog()
        {
            MessageLog = string.Empty;
        }

        private bool CanStartFactory()
        {
            return !FactoryStarting && !FactoryStarted;
        }

        private async void StartFactory()
        {
            if (CanStartFactory())
            {
                FactoryStarting = true;

                await Task.Delay(300);

                await InitializeFactory();

                await _mqttService.Connect();

                FactoryStarted = true;
                FactoryStarting = false;
            }
        }

        private bool CanResetFactory()
        {
            return FactoryStarted || FactoryStarting;
        }

        private async void ResetFactory()
        {
            if (CanResetFactory())
            {
                await InitializeFactory();
            }
        }

        private async Task InitializeFactory()
        {
            Door.Initialize();
            Lifter.Initialize();
            Press.Initialize();

            using var context = new DynamoDBContext(_dynamoDBClient);

            await context.DeleteAsync<DeviceState>(Topics.DOOR);
            await context.DeleteAsync<DeviceState>(Topics.LIFTER);
            await context.DeleteAsync<DeviceState>(Topics.PRESS);

            FactoryReseted?.Invoke(this, EventArgs.Empty);
        }

        private async void OnMqttMessageReceived(object sender, MessageReceivedArgs e)
        {
            MessageLog += $"{DateTime.Now}: {JsonConvert.SerializeObject(e.Message)}" + Environment.NewLine;

            switch (e.Message.Topic)
            {
                case Topics.DOOR:
                    await Door.HandleRequest(e.Message);
                    break;
                case Topics.LIFTER:
                    await Lifter.HandleRequest(e.Message);
                    break;
                case Topics.PRESS:
                    await Press.HandleRequest(e.Message);
                    break;
            }
        }
    }
}
