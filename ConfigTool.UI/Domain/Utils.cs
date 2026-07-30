using System.Threading.Tasks;

namespace Luhmann.Kiosk.Tools.ConfigTool.Domain
{
    internal class Utils

    {
        public static string GetAppVersion()
        {
            string ver = "0.0.0.0";
            try
            {
                System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fieVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);
                ver = fieVersionInfo.FileVersion;
            }
            catch
            {
            }
            return ver;
        }

        public static void SafeDelay(int Miliseconds)
        {
            Task.Run(async () => await SafeDelayAsync(Miliseconds)).Wait();
        }

        public static async Task<bool> SafeDelayAsync(int Miliseconds)
        {
            var x = Task.Delay(Miliseconds).GetAwaiter();
            int i = 0;

            while (!x.IsCompleted)
            {
                await Task.Delay(1);
            }
            return true;
        }
    }
}