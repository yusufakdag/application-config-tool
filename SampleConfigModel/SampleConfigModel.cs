using Luhmann.Kiosk.Tools.ConfigTool.Interfaces;
using System.ComponentModel;
using static SampleConfigModel.TypeConverters;

namespace SampleConfigModel
{
    public class SampleConfigModel : IConfigModel
    {
        public enum TestEnum
        {
            [Description("Test Enum 1")]
            TestEnumValue1 = 0,

            [Description("Test Enum 2")]
            TestEnumValue2 = 2,
        }

        [DisplayName("String Parameter")]
        public string StringParameter { get; set; }

        [DisplayName("Integer Parameter")]
        public int IntParameter { get; set; }

        [DisplayName("Boolean Parameter")]
        [Description("This is a boolean parameter")]
        public bool BoolParameter { get; set; }

        [DisplayName("Enum Parameter")]
        [Description("This is enum parameter")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public TestEnum EnumParameter { get; set; }

        [DisplayName("Nested Class")]
        [Description("Subclass Settings")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(NestedConfigModelTypeConverter))]
        public NestedConfigModel NestedConfig { get; set; }

        public object CreateDefaults()
        {
            SampleConfigModel c = new SampleConfigModel()
            {
                StringParameter = "Hello World!",
                BoolParameter = true,
                EnumParameter = TestEnum.TestEnumValue2,
                IntParameter = 123,
                NestedConfig = new NestedConfigModel()
                {
                    String1 = "I'm a String",
                    String2 = "I'm also a String",
                    Int1 = 321,
                },
            };

            return c;
        }

        public object CreateLuhmannWrap(object Object)
        {
            return Luhmann.Kiosk.Tools.ConfigTool.Utils.LuhmannConfigWrapper.Wrap(Object);
        }

        public object DeserializeSelf(string Json)
        {
            object res = null;
            try
            {
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<SampleConfigModel>(Json);
            }
            catch (System.Exception)
            {
            }

            return res;
        }
    }

    public class NestedConfigModel
    {
        [DisplayName("String 1")]
        public string String1 { get; set; }

        [DisplayName("String 2-ReadOnly")]
        [ReadOnly(true)]
        public string String2 { get; set; }

        [DisplayName("Int 1")]
        public int Int1 { get; set; }
    }
}