namespace Luhmann.Kiosk.Tools.ConfigTool.Interfaces
{
    public interface IConfigModel
    {
        object CreateDefaults();
        object DeserializeSelf(string Json);
    }
}