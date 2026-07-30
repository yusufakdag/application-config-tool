using Luhmann.Kiosk.Tools.ConfigTool.Interfaces;
using System.ComponentModel;

namespace SampleConfigModel
{
    internal class SampleConfigModelOther : IConfigModel
    {
        [DisplayName("String Parameter")]
        public string StringParameter { get; set; }

        [DisplayName("Integer Parameter")]
        public int IntParameter { get; set; }

        [DisplayName("BoolParameter")]
        [Description("This is a boolean parameter")]
        public bool BoolParameter { get; set; }

        public object CreateDefaults()
        {
            SampleConfigModelOther c = new SampleConfigModelOther()
            {
                StringParameter = "Hello World!",
                BoolParameter = true,
                IntParameter = 123,
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
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<SampleConfigModelOther>(Json);
            }
            catch (System.Exception)
            {
            }

            return res;
        }
    }
}