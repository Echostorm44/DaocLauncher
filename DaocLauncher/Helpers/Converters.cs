using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DaocLauncher.Helpers;

public class CleanVkEnumToString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value == null || value.GetType() != typeof(VirtualKeyCode))
        {
            return "";
        }
        string original = Enum.GetName(typeof(VirtualKeyCode), value) ?? "";
        return original.Replace("VK_", "");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

