using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace SampleConfigModel
{
    public class TypeConverters
    {
        public class NestedConfigModelTypeConverter : ExpandableObjectConverter//TypeConverter
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

        public class EnumTypeConverter : EnumConverter
        {
            private Type enumType;

            public EnumTypeConverter(Type type) : base(type)
            {
                enumType = type;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
            {
                return destType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                             object value, Type destType)
            {
                FieldInfo fi = enumType.GetField(Enum.GetName(enumType, value));
                DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                            typeof(DescriptionAttribute));
                if (dna != null)
                    return dna.Description;
                else
                    return value.ToString();
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
            {
                return srcType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
                                               object value)
            {
                foreach (FieldInfo fi in enumType.GetFields())
                {
                    DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                                typeof(DescriptionAttribute));
                    if ((dna != null) && ((string)value == dna.Description))
                        return Enum.Parse(enumType, fi.Name);
                }
                return Enum.Parse(enumType, (string)value);
            }
        }

    }
}