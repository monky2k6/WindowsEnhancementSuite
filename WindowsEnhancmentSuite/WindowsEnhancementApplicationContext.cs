using Open.WinKeyboardHook;
using System;
using System.Windows.Forms;
using WindowsEnhancementSuite.Forms;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite
{
    public class WindowsEnhancementApplicationContext : ApplicationContext
    {
        private NotifyIcon notifyIcon;
        private readonly FileAndImageSaver fileAndImageSaver;
        private readonly FileCreateHelper fileCreateHelper;
        private readonly FileAndImageViewer fileAndImageViewer;
        private readonly ExplorerBrowserHelper explorerBrowserHelper;

        private readonly KeyboardInterceptor keyboadHook;

        public WindowsEnhancementApplicationContext()
        {
            // Hotkeys initialisieren
            this.keyboadHook = new KeyboardInterceptor();
            this.keyboadHook.KeyDown += keyboadHookOnKeyDown;

            this.keyboadHook.StartCapturing();

            // Services initialisieren
            this.fileAndImageSaver = new FileAndImageSaver();
            this.fileCreateHelper = new FileCreateHelper();
            this.fileAndImageViewer = new FileAndImageViewer();
            this.explorerBrowserHelper = new ExplorerBrowserHelper();

            // TrayIcon etc. initalisieren
            this.initializeComponents();
            
            // Events hinzufügen
            Application.ApplicationExit += this.applicationOnApplicationExit;
        }

        private void keyboadHookOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (Settings.Default.CliboardHotkey > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.CliboardHotkey)
                {
                    keyEventArgs.Handled = this.fileAndImageSaver.SaveClipboardInFile();
                    return;
                }
            }

            if (Settings.Default.TextfileHotkey > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.TextfileHotkey)
                {
                    keyEventArgs.Handled = this.fileCreateHelper.CreateAndOpenTextfile();
                    return;
                }
            }

            if (Settings.Default.ShowHotkey > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.ShowHotkey)
                {
                    keyEventArgs.Handled = this.fileAndImageViewer.ShowClipboardContent();
                    return;
                }
            }

            if (Settings.Default.ShowBrowserHistory > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.ShowBrowserHistory)
                {
                    keyEventArgs.Handled = this.explorerBrowserHelper.ShowExplorerHistory();
                    return;
                }
            }
        }

        private void initializeComponents()
        {
            // Tray-Icon inkl. ContextMenu hinzufügen
            this.notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = @"Windows Enhancement Suite",
                Icon = Resources.WES,
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Settings", this.showSettings),
                    new MenuItem("-"),
                    new MenuItem("Exit", this.exitApplication)
                })
            };
        }

        private void applicationOnApplicationExit(object sender, EventArgs eventArgs)
        {
            // Alles entladen etc.            
            this.notifyIcon.Visible = false;
            this.keyboadHook.StopCapturing();
            Settings.Default.Save();
        }

        private void exitApplication(object sender, EventArgs eventArgs)
        {
            Application.Exit();
        }

        private void showSettings(object sender, EventArgs eventArgs)
        {
            this.keyboadHook.StopCapturing();
            new SettingsForm().ShowDialog();
            this.keyboadHook.StartCapturing();
        }
    }
}
