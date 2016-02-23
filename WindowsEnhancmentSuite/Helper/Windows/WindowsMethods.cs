using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using SHDocVw;

namespace WindowsEnhancementSuite.Helper.Windows
{
    public static class WindowsMethods
    {
        public static string GetCurrentExplorerPath()
        {
            var currentHandle = GetForegroundWindow();

            var currentExplorer = new ShellWindows().Cast<InternetExplorer>().FirstOrDefault(e => (IntPtr)e.HWND == currentHandle);
            if (currentExplorer != null)
            {
                if (currentExplorer.FullName != null)
                {
                    if (Path.GetFileNameWithoutExtension(currentExplorer.FullName).ToLower().Equals("explorer"))
                    {
                        string path = new Uri(currentExplorer.LocationURL).LocalPath;
                        return HttpUtility.UrlDecode(path, Encoding.Default);
                    }
                }
            }

            if (isOnDesktop(currentHandle))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            return "";
        }

        public static void BringToFront(int handle)
        {
            SetForegroundWindow((IntPtr)handle);
        }

        private static bool isOnDesktop(IntPtr handle)
        {
            var processes = Process.GetProcesses();
            return processes.All(proc => proc.MainWindowHandle != handle);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
