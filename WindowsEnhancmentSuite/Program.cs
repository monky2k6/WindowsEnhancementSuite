using System;
using System.Windows.Forms;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.Services;

namespace WindowsEnhancementSuite
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var uacAssistService = new UacAssistService(args);
            if (uacAssistService.Process()) return;

            // Enforce Single Instance
            if (Utils.IsProcessRunning()) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WindowsEnhancementApplicationContext());
        }
    }
}
