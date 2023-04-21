<p id="readme-top" align="center">
  <img src="images/smart_factory_logo.png" height="128">
  <h1  align="center">Smart Factory</h1>
</p>

The Smart Factory is an IoT project, which we realized in the IoT Frameworks lecture as part of our master’s degree in Cloud Computer Engineering at the Burgenland University of Applied Sciences.

## Table of Contents

- [Motivation](#motivation)
- [Architectur](#architectur)
- [Built with](#builtwith)
- [Getting started](#gettingstarted)
- [Challenges](#challenges)

## Motivation <a name="motivation"></a>

In the smart factory there are numerous plants that are operated by various IoT Devices. These IoT devices are managed and controlled using the AWS IoT Framework IoT Core. In order to illustrate the functions of this framework, the following 3 systems were simulated in the demonstrator and controlled via IoT Core:

<p align="center">
  <img src="images/smart_factory.png" height="256">
</p>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Architectur <a name="architectur"></a>

The Smart Factory consists of 2 parts. The visualization is done in a desktop application that was implemented using the Windows Presentation Foundation. When implementing this simulator, care was taken to ensure that each activator only sends messages to AWS IoT Core, but does not trigger any local action itself. The factory gate, the elevator or the hydraulic press are soly controlled by commands, which are received from AWS IoT Core.

The brain of the smart factory is an AWS Lambda Function, which receives the messages from AWS IoT Core, processes them and then sends commands to AWS IoT Core back. The simulation then reacts to the commands and triggers the corresponding action.

<p align="center">
  <img src="images/factory_gate_diagram.png" height="256">
</p>

The diagram above shows an example of the information flow between the simulator, AWS IoT Core and the AWS Lambda Function. The reported statuses of the individual actuators and sensors are temporarily saved by the AWS Lambda Function in an AWS DynamoDb.

The Message Queuing Telemetry Transport Protocol, MQTT for short, is used for communication between the individual components. The following topics were defined for controlling the individual factory systems:

* mcce22-smart-factory/door
* mcce22-smart-factory/lifter
* mcce22-smart-factory/press

In the simulator, the 3 systems can be controlled using the controls. The information transacted in the simulator is first sent to AWS IoT Core. By defining message rules, IoT Core then forwards the received messages to the Factory Controller Lambda function, which then processes the received messages and, if necessary, reacts with new messages to AWS IoT Core.

<p align="center">
  <img src="images/simulator.gif" height="256">
</p>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built with <a name="builtwith"></a>

- [Visual Studio](https://visualstudio.microsoft.com/de/vs/community/)
- [Net Core](https://dotnet.microsoft.com/)
- [Windows Presentation Foundation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)
- [AWS IoT Core](https://aws.amazon.com/de/iot-core/)
- [AWS Lambda](https://aws.amazon.com/de/lambda/)
- [AWS DynamoDB](https://aws.amazon.com/de/dynamodb/)
- [AWS CloudFormation](https://aws.amazon.com/cloudformation/)
- [MahApps.Metro](https://mahapps.com/)
- [Castle Windsor](http://www.castleproject.org/)
- [MQTTnet](https://github.com/dotnet/MQTTnet)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started <a name="gettingstarted"></a>

1. Checkout the project
2. Deploy the required AWS environment to your AWS account
3. Run the simulator

## Challenges <a name="challenges"></a>

<p align="right">(<a href="#readme-top">back to top</a>)</p>