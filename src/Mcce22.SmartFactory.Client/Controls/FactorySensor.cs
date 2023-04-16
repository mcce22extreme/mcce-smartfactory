using System.Windows;
using System.Windows.Controls;

namespace Mcce22.SmartFactory.Client.Controls
{
    public class FactorySensor : ContentControl
    {
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(FactorySensor), new PropertyMetadata(false));
    }
}
