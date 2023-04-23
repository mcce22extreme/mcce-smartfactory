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

<p align="center">
  <img src="images/factory_gate_diagram.png" height="256">
</p>

The picture above shows an example of the information flow between the simulated sensors and AWS IoT Core. Each sensor acts independently on the received messages and updates its status. The Message Queuing Telemetry Transport Protocol, MQTT for short, is used for communication between the individual components. The following topics were defined for controlling the individual factory systems:

* mcce22-smart-factory/door
* mcce22-smart-factory/lifter
* mcce22-smart-factory/press

In the simulator, the 3 systems can be controlled and each sensor simulates its action status changes. The communication between the individual sensor instances takes place exclusively via AWS IoT Core by exchanging messages. These messages contain the current status of the IoT device. Other sensors or IoT devices can then react to these status changes if they need to.

<p align="center">
  <img src="images/simulator.gif" height="256">
</p>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built with <a name="builtwith"></a>

- [Visual Studio](https://visualstudio.microsoft.com/de/vs/community/)
- [Net Core](https://dotnet.microsoft.com/)
- [Windows Presentation Foundation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)
- [AWS IoT Core](https://aws.amazon.com/de/iot-core/)
- [MahApps.Metro](https://mahapps.com/)
- [Castle Windsor](http://www.castleproject.org/)
- [MQTTnet](https://github.com/dotnet/MQTTnet)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started <a name="gettingstarted"></a>

1. Checkout the project
2. Create required IoT Things in AWS IoT Core
3. Run the simulator

## Challenges <a name="challenges"></a>

<p align="right">(<a href="#readme-top">back to top</a>)</p>