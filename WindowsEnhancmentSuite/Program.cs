using System;
using System.Windows.Forms;
using WindowsEnhancementSuite.Services;

namespace WindowsEnhancementSuite
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var uacAssistService = new UacAssistService(args);
            if (uacAssistService.Process()) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WindowsEnhancementApplicationContext());
        }
    }
}
