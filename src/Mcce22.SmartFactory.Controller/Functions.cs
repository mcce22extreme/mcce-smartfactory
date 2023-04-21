using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Mcce22.SmartFactory.Controller.Handlers;
using Mcce22.SmartFactory.Controller.Models;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Mcce22.SmartFactory.Controller
{
    public class Functions
    {
        private readonly DoorRequestHandler _doorRequestHandler;
        private readonly LifterRequestHandler _lifterRequestHandler;
        private readonly PressRequestHandler _pressRequestHandler;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            var endpointAddress = "https://data-ats.iot.us-east-1.amazonaws.com/";

            _doorRequestHandler = new DoorRequestHandler(endpointAddress);
            _lifterRequestHandler = new LifterRequestHandler(endpointAddress);
            _pressRequestHandler = new PressRequestHandler(endpointAddress);
        }

        public APIGatewayProxyResponse HandleRequest(RequestModel model, ILambdaContext context)
        {                      
            context.Logger.LogInformation(JsonConvert.SerializeObject(model));

            LambdaLogger.Log("Test");

            switch (model.Topic)
            {
                case Topics.DOOR:
                    Task.WaitAll(_doorRequestHandler.HandleRequest(model));
                    break;
                case Topics.LIFTER:
                    Task.WaitAll(_lifterRequestHandler.HandleRequest(model));
                    break;
                case Topics.PRESS:
                    Task.WaitAll(_pressRequestHandler.HandleRequest(model));
                    break;
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = string.Empty,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }        
    }
}
