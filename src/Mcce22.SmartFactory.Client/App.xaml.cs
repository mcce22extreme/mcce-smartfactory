using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Mcce22.SmartFactory.Client.Services;
using Mcce22.SmartFactory.Client.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Mcce22.SmartFactory.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .Build()
                .Get<AppSettings>();

            var container = new WindsorContainer();
            container.Register(Component.For<AppSettings>().Instance(appSettings));
            container.Register(Component.For<MainViewModel>());
            container.Register(Component.For<MainWindow>());
            container.Register(Component.For<FactoryViewModel>());
            container.Register(Component.For<IMqttService>()
                .ImplementedBy<MqttService>()
                .LifestyleSingleton());

            container.Install();

            MainWindow = container.Resolve<MainWindow>();
            MainWindow.Show();
        }
    }
}
