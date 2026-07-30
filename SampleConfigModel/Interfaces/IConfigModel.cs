using System;
using System.Diagnostics;
using System.Reflection;

namespace Luhmann.Kiosk.Tools.ConfigTool.Interfaces
{
    public interface IConfigModel
    {
        object CreateDefaults();

        object DeserializeSelf(string Json);

        object CreateLuhmannWrap(object Object);
    }
}

namespace Luhmann.Kiosk.Tools.ConfigTool.Utils
{
    public class LuhmannConfig
    {
        public string TypeName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyPath { get; set; }
        public string AssemblyVersion { get; set; }
        public string ConfigToolVer { get; set; }
        public object ConfigData { get; set; }
    }

    public static class LuhmannConfigWrapper
    {
        private static Assembly dllFile { get; set; }
        private static Type WorkingType { get; set; }

        public static LuhmannConfig Wrap(object Object)
        {
            dllFile = Assembly.GetExecutingAssembly();
            StackTrace stackTrace = new StackTrace();

            var methodInfo = stackTrace.GetFrame(1).GetMethod();

            WorkingType = methodInfo.DeclaringType;

            LuhmannConfig workingConfig = new LuhmannConfig()
            {
                AssemblyName = dllFile.ManifestModule.Name,
                AssemblyPath = dllFile.ManifestModule.FullyQualifiedName,
                AssemblyVersion = dllFile.GetName().Version.ToString(),
                TypeName = WorkingType.FullName,
                ConfigToolVer = "0",
                ConfigData = Object,
            };

            return workingConfig;
        }
    }
}