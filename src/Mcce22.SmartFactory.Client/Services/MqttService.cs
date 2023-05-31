using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mcce22.SmartFactory.Client.Requests;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Oocx.ReadX509CertificateFromPem;

namespace Mcce22.SmartFactory.Client.Services
{
    public interface IMqttService
    {
        event EventHandler<MessageReceivedArgs> MessageReceived;

        Task Connect();

        Task PublishMessage(MessageModel message);
    }

    public class MqttService : IMqttService
    {
        private static readonly MqttFactory Factory = new MqttFactory();

        private readonly IMqttClient _mqttClient;
        private readonly AppSettings _appSettings;

        public event EventHandler<MessageReceivedArgs> MessageReceived;

        public MqttService(AppSettings appSettings)
        {
            _mqttClient = Factory.CreateMqttClient();
            _appSettings = appSettings;
        }        

        public async Task Connect()
        {
            if (!_mqttClient.IsConnected)
            {
                var broker = _appSettings.EndpointAddress;
                var port = _appSettings.EndpointPort;

                var deviceCertPEMString = await  File.ReadAllTextAsync(@$"{_appSettings.CertFolder}\certificate.pem.crt");
                var devicePrivateCertPEMString = await File.ReadAllTextAsync(@$"{_appSettings.CertFolder}\private.pem.key");
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
                await _mqttClient.SubscribeAsync(Topics.LIFTER);
                await _mqttClient.SubscribeAsync(Topics.PRESS);
                await _mqttClient.SubscribeAsync(Topics.LIGHT);
            }
        }

        private Task OnMqttMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            var payload = arg.ApplicationMessage.ConvertPayloadToString();

            var message = JsonConvert.DeserializeObject<MessageModel>(payload);

            MessageReceived?.Invoke(this, new MessageReceivedArgs(message));

            return Task.CompletedTask;
        }

        public async Task PublishMessage(MessageModel message)
        {
            if (_mqttClient.IsConnected)
            {
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic(message.Topic)
                    .WithPayload(JsonConvert.SerializeObject(message))
                    .Build();

                await _mqttClient.PublishAsync(msg, CancellationToken.None);
            }
        }
    }

    public class MessageReceivedArgs : EventArgs
    {
        public MessageModel Message { get; }

        public MessageReceivedArgs(MessageModel message)
        {
            Message = message;
        }        
    }
}
