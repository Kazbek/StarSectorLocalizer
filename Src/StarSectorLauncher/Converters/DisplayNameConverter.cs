using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace StarSectorLauncher.Converters
{
    public class DisplayNameConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetDisplayName((Enum)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private static string GetDisplayName(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attr)
                    {
                        return attr.Name;
                    }
                }
            }
            return value.ToString();
        }
    }
}
