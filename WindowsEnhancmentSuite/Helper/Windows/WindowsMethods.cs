using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public static IEnumerable<KeyValuePair<string, IntPtr>> GetOpenWindows()
        {
            var shellWindow = GetShellWindow();
            var list = new ConcurrentBag<KeyValuePair<string, IntPtr>>();

            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                list.Add(new KeyValuePair<string, IntPtr>(builder.ToString(), hWnd));

                return true;
            }, 0);

            return list;
        }

        public static void ActivateWindow(IntPtr handle)
        {
            if (IsIconic(handle)) ShowWindow(handle, 9);
            SetForegroundWindow(handle);
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

        public static void DoDragDrop(IntPtr handle)
        {
            ReleaseCapture();
            SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
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
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool CheckMenuItem(IntPtr hMenu, int uIDCheckItem, int uCheck);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("USER32.DLL")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool ReleaseCapture();
        #endregion
    }
}
