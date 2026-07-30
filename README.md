# Luhmann Kiosk Config Tool

A generic, extensible configuration tool for Windows Desktop applications (e.g., Kiosk applications). It uses reflection and `System.ComponentModel` to automatically generate a rich configuration UI (Property Grid) based on your custom configuration models.

## How It Works

The **Config Tool** reads a `.dll` file containing a configuration model, uses reflection to load the properties, and presents them in a `PropertyGrid`. Users can modify these properties and save the output as a `.luhmann` file, which is essentially a JSON wrapper around the configuration data.

## Model Structure

To create a configuration model that the tool can understand, your model class must implement the `IConfigModel` interface:

```csharp
using Luhmann.Kiosk.Tools.ConfigTool.Interfaces;
using System.ComponentModel;

public class MyConfigModel : IConfigModel
{
    [Category("Network")]
    [DisplayName("Server URL")]
    [Description("The URL of the API server.")]
    [DefaultValue("https://api.example.com")]
    public string ServerUrl { get; set; }
    
    public object CreateDefaults()
    {
        // Return a new instance of your model with default values
        return new MyConfigModel() { ServerUrl = "https://api.example.com" };
    }

    public object CreateLuhmannWrap(object Object)
    {
        // Wrap your config using the utility method
        return Luhmann.Kiosk.Tools.ConfigTool.Utils.LuhmannConfigWrapper.Wrap(Object);
    }

    public object DeserializeSelf(string Json)
    {
        // Deserialize your model from the provided JSON string
        return Newtonsoft.Json.JsonConvert.DeserializeObject<MyConfigModel>(Json);
    }
}
```

### UI Customization

The tool leverages standard `System.ComponentModel` attributes to build the UI. You can apply multiple attributes to a single property to finely control its behavior and appearance in the Property Grid:
- `[Category("Network Settings")]`: Groups related properties together in the grid.
- `[DisplayName("My Setting")]`: Controls the name displayed in the grid instead of the property name.
- `[Description("This is what my setting does")]`: Displays help text at the bottom of the grid when the property is selected.
- `[DefaultValue("https://...")]`: Indicates the default value of the property (often used to bold non-default values).
- `[Browsable(false)]`: Hides the property from the grid entirely.
- `[ReadOnly(true)]`: Prevents the user from modifying the property.
- `[TypeConverter(typeof(EnumTypeConverter))]`: Use custom type converters for complex objects or enums to control how they are displayed and edited.
- `[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]`: Assign custom UI editors (like file pickers) for a property.
- `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]`: Useful for nested configuration objects to ensure their properties are expanded.

### Custom Type Converters Example

You can implement custom `TypeConverter` classes to control how specific types are displayed or edited. For example, converting boolean values to custom strings like "Evet" (Yes) and "Hayır" (No), or making complex nested classes expandable in the grid:

```csharp
using System.ComponentModel;
using System.Globalization;

// A custom boolean converter to display "Evet" / "Hayır" instead of True / False
public class YesNoBooleanConverter : BooleanConverter
{
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is bool b)
            return b ? "Evet" : "Hayır";
        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s)
        {
            if (string.Equals(s, "Evet", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(s, "Hayır", StringComparison.OrdinalIgnoreCase)) return false;
        }
        return base.ConvertFrom(context, culture, value);
    }
}

// A custom converter to make nested classes expandable in the PropertyGrid
// To use this, your nested class must implement a marker interface (e.g., IExpandable)
public class ExpandableTypeConverter : ExpandableObjectConverter
{
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
    {
        if (destType == typeof(string) && value is IExpandable)
        {
            return ""; // Hides the class name string in the grid row
        }
        return base.ConvertTo(context, culture, value, destType);
    }
}
public interface IExpandable { }
```

### Complex Model Example

Here is a more comprehensive example demonstrating how multiple attributes and custom type converters are applied to a configuration model:

```csharp
public class KioskDefinitionModel : IExpandable
{
    [Category("Cihaz Bilgileri")]
    [DisplayName("Cihaz ID")]
    [ReadOnly(true)]
    public string DeviceID { get; set; }

    [Category("Zamanlamalar")]
    [DisplayName("Ödeme Bekleme Süresi (sn)")]
    [Description("Ödeme başlatmak için ekrana dokunduktan sonra ilk parayı atması için verilen süredir")]
    public int TimerWaitForPayment { get; set; }

    [DisplayName("Ücret iadesine izin ver")]
    [Description("Eğer verilen sürede sepet çıkartılmazsa ücret iadesine izin verir.")]
    [TypeConverter(typeof(YesNoBooleanConverter))]
    public bool AllowRefund { get; set; }
}

public class ConfigModel : IConfigModel
{
    [Category("Kiosk")]
    [DisplayName("Kiosk Ayarları")]
    [Description("Cihazın özelliklerinin değiştirilmesini sağlar.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [TypeConverter(typeof(ExpandableTypeConverter))]
    [ReadOnly(true)]
    public KioskDefinitionModel KioskSettings { get; set; }
    
    // ... interface implementations ...
}
```

## Config File Structure (`.luhmann`)

The configuration is saved in a `.luhmann` file. This file contains a JSON object that serves as a wrapper (`LuhmannConfig`) for your actual configuration data.

```json
{
  "TypeName": "SampleConfigModel.SampleConfigModel",
  "AssemblyName": "Sample Model.dll",
  "AssemblyPath": "C:\\path\\to\\Sample Model.dll",
  "AssemblyVersion": "1.0.0.0",
  "ConfigToolVer": "1.0.0.0",
  "ConfigData": {
    "StringParameter": "Hello World!",
    "IntParameter": 123,
    "BoolParameter": true,
    "EnumParameter": 2,
    "NestedConfig": {
      "String1": "I'm a String",
      "String2": "I'm also a String",
      "Int1": 321
    }
  }
}
```

- `TypeName`, `AssemblyName`, `AssemblyPath`, `AssemblyVersion`: Metadata used by the tool to locate and load the correct assembly when opening the file.
- `ConfigToolVer`: The version of the Config Tool used to generate the file.
- `ConfigData`: The actual serialized payload of your custom configuration model.
