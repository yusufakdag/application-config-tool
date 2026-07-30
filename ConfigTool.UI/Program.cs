using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Luhmann.Kiosk.Tools.ConfigTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args != null && args.Length > 0)
            {
                string filepath = args[0].ToString();
                Application.Run(new frmConfig(filepath));
            }
            else
            {
                Application.Run(new frmConfig()); 
            }
        }
    
    }
}
