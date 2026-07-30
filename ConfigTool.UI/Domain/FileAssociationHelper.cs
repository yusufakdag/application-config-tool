using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Luhmann.Kiosk.Tools.ConfigTool
{
    internal static class FileAssociationHelper
    {
       

        internal static void Associate(int IconIndex)
        {
            try
            {
                FileAssociationHelper.Associate(".luhmann", "Kiosk Config File", "Luhmann Kiosk Config File",
                    IconIndex, Assembly.GetExecutingAssembly().Location);
            }
            catch (Exception exception)
            {
                Exception _ex = exception;
                //Functions.CreateMessage("Permission Required", "Permission Required", string.Concat("This action requires \"Administrative Permission\" to application!\r\n", _ex.Message), eTaskDialogIcon.ShieldStop);
            }
        }

        private static void Associate(string extension, string progID, string description, int iconIndex, string application)
        {
            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
            if ((progID == null ? false : progID.Length > 0))
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID);
                try
                {
                    if (description != null)
                    {
                        key.SetValue("", description);
                    }
                    key.CreateSubKey("DefaultIcon").SetValue("", string.Concat(application, ",", iconIndex));
                    if (application != null)
                    {
                        key.CreateSubKey("Shell\\Open\\Command").SetValue("", string.Concat("\"", application, "\" \"%1\""));
                    }
                    SHChangeNotify(134217728, 0, IntPtr.Zero, IntPtr.Zero);
                    //Functions.CreateMessage("Operation Complete", "Operation Complete", ".unitypackage file assoication with .unitypackage Tools complete.", eTaskDialogIcon.Information2);
                }
                finally
                {
                    if (key != null)
                    {
                        ((IDisposable)key).Dispose();
                    }
                }
            }
        }

        internal static bool IsAssociated(string extension)
        {
            return Registry.ClassesRoot.OpenSubKey(extension, false) != null;
        }

        private static void Restore(string extension, string progID, string description, int iconIndex, string application)
        {
            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progID);
            if ((progID == null ? false : progID.Length > 0))
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(progID);
                try
                {
                    if (description != null)
                    {
                        key.SetValue("", description);
                    }
                    key.CreateSubKey("DefaultIcon").SetValue("", string.Concat(application, ",", iconIndex));
                    if (application != null)
                    {
                        key.CreateSubKey("Shell\\Open\\Command").SetValue("", string.Concat(application, "\" -openfile \"%1\""));
                    }
                    SHChangeNotify(134217728, 0, IntPtr.Zero, IntPtr.Zero);
                    //Functions.CreateMessage("Operation Complete", "Operation Complete", ".unitypackage extension restored to original settings.", eTaskDialogIcon.Information2);
                }
                finally
                {
                    if (key != null)
                    {
                        ((IDisposable)key).Dispose();
                    }
                }
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
