using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mcce22.SmartFactory.Client.Entities;
using Mcce22.SmartFactory.Client.Requests;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Newtonsoft.Json;
using Oocx.ReadX509CertificateFromPem;

namespace Mcce22.SmartFactory.Client.ViewModels
{
    public interface ISimulatorViewModel : INotifyPropertyChanged
    {
        DoorViewModel Door { get; }

        PlatformViewModel Platform { get; }

        event EventHandler FactoryReseted;
    }

    public class FactoryViewModel : ObservableObject, ISimulatorViewModel
    {
        private static readonly AmazonDynamoDBClient _dynamoDBClient = new AmazonDynamoDBClient();
        private static readonly MqttFactory Factory = new MqttFactory();

        private readonly IMqttClient _mqttClient;

        private readonly AppSettings _appSettings;

        public event EventHandler FactoryReseted;

        public DoorViewModel Door { get; }

        public PlatformViewModel Platform { get; }

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

        public FactoryViewModel(AppSettings appSettings)
        {
            _appSettings = appSettings;

            _mqttClient = Factory.CreateMqttClient();

            Door = new DoorViewModel(_mqttClient);
            Platform = new PlatformViewModel(_mqttClient);

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

                await Task.Delay(500);

                await InitializeFactory();
                await Connect();

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

            using var context = new DynamoDBContext(_dynamoDBClient);

            await context.DeleteAsync<DeviceState>(Topics.DOOR);

            FactoryReseted?.Invoke(this, EventArgs.Empty);
        }

        private async Task Connect()
        {
            if (!_mqttClient.IsConnected)
            {
                var broker = _appSettings.EndpointAddress;
                var port = _appSettings.EndpointPort;

                var deviceCertPEMString = await  File.ReadAllTextAsync(@$"{_appSettings.CertFolder}\door-certificate.pem.crt");
                var devicePrivateCertPEMString = await File.ReadAllTextAsync(@$"{_appSettings.CertFolder}\door-private.pem.key");
                var certificateAuthorityCertPEMString = await File.ReadAllTextAsync(@$"{_appSettings.CertFolder}\AmazonRootCA1.pem");

                //Converting from PEM to X509 certs in C# is hard
                //Load the CA certificate
                //https://gist.github.com/ChrisTowles/f8a5358a29aebcc23316605dd869e839
                var certBytes = Encoding.UTF8.GetBytes(certificateAuthorityCertPEMString);
                var signingcert = new X509Certificate2(certBytes);

                //Load the device certificate
                //Use Oocx.ReadX509CertificateFromPem to load cert from pem
                var reader = new CertificateFromPemReader();
                var deviceCertificate = reader.LoadCertificateWithPrivateKeyFromStrings(deviceCertPEMString, devicePrivateCertPEMString);

                // Certificate based authentication
                var certs = new List<X509Certificate>
                {
                    signingcert,
                    deviceCertificate
                };

                //Set things up for our MQTTNet client
                var tlsOptions = new MqttClientOptionsBuilderTlsParameters
                {
                    Certificates = certs,
                    SslProtocol = SslProtocols.Tls12,
                    UseTls = true,
                    AllowUntrustedCertificates = true,
                    IgnoreCertificateChainErrors = true,
                    IgnoreCertificateRevocationErrors = true
                };

                var options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port)
                .WithTls(tlsOptions)
                .Build();

                _mqttClient.ApplicationMessageReceivedAsync += OnMqttMessageReceived;

                await _mqttClient.ConnectAsync(options, CancellationToken.None);

                await _mqttClient.SubscribeAsync(Topics.DOOR);
            }
        }

        private async Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            var payload = args.ApplicationMessage.ConvertPayloadToString();

            MessageLog += $"{DateTime.Now}: {payload}" + Environment.NewLine;

            var request = JsonConvert.DeserializeObject<RequestModel>(payload);

            switch (request.Topic)
            {
                case Topics.DOOR:
                    await Door.HandleRequest(request);
                    break;
            }            
        }
    }
}
