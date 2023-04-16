using System.Windows;
using System.Windows.Controls;

namespace Mcce22.SmartFactory.Client.Controls
{
    internal class FactoryButton : Button
    {
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(FactoryButton), new PropertyMetadata(false));
    }
}
