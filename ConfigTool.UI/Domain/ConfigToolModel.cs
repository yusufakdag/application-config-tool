using System;
using System.ComponentModel;

namespace Luhmann.Kiosk.Tools.ConfigTool
{
    internal class LuhmannConfig
    {
        public string TypeName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyPath { get; set; }
        public string AssemblyVersion { get; set; }
        public string ConfigToolVer { get; set; }
        [TypeConverter(typeof(ConfigModelTypeConverter))]
        public object ConfigData { get; set; }
    }

    public class ConfigModelTypeConverter : ExpandableObjectConverter//TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(object))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                            System.Globalization.CultureInfo culture,
                            object value, Type destType)
        {
            if (destType == typeof(string) && value is object)
            {
                return "";
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }

}