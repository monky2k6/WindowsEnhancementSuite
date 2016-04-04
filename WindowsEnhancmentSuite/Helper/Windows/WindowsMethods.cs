using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
                        return Utils.DecodeUrl(new Uri(currentExplorer.LocationURL));
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

        public static IntPtr GetFormMenuHandle(IntPtr formHandle)
        {
            return GetSystemMenu(formHandle, false);
        }

        public static void AddFormMenuSeparator(IntPtr menuHandle)
        {
            AddFormMenuItem(menuHandle, 0, "-");
        }

        public static void AddFormMenuItem(IntPtr menuHandle, int id, string caption)
        {
            if (id == 0 && caption == "-")
            {
                AppendMenu(menuHandle, MF_SEPARATOR, 0, String.Empty);
                return;
            }

            AppendMenu(menuHandle, MF_STRING, id, caption);
        }

        public static void SetFormMenuCheckBox(IntPtr menuHandle, int id, bool check)
        {
            CheckMenuItem(menuHandle, id, check ? MF_CHECKED : MF_UNCHECKED);
        }

        public static bool CheckMenuEvent(Message message, int id)
        {
            return (message.Msg == WM_SYSCOMMAND && (int) message.WParam == id);
        }

        private static bool isOnDesktop(IntPtr handle)
        {
            var processes = Process.GetProcesses();
            return processes.All(proc => proc.MainWindowHandle != handle);
        }

        #region WindowsMethods
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;
        private const int MF_CHECKED = 0x8;
        private const int MF_UNCHECKED = 0x0;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool CheckMenuItem(IntPtr hMenu, int uIDCheckItem, int uCheck);
        #endregion
    }
}
