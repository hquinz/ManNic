using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace HQ4P.Tools.ManNic.View.Tools
{

    public class ControlKeyEventArgs
    {
        public Control ControlObject { get; set; }
        public KeyEventArgs EventArgument { get; set; }
    }

    //MarkupExtension,
    public class KeyEventConverter :  IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new ControlKeyEventArgs()
            {
                ControlObject = values[0] as Control,
                EventArgument = values[1] as KeyEventArgs
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /*
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        */
    }
}