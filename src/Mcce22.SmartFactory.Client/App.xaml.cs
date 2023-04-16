using System;
using System.IO;
using System.Reflection;
using System.Windows;
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

            var doorViewModel = new FactoryViewModel(appSettings);
            var mainViewModel = new MainViewModel(doorViewModel);

            MainWindow = new MainWindow(mainViewModel);

            MainWindow.Show();
        }
    }
}
