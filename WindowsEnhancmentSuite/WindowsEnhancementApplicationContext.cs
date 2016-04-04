using System;
using System.Windows.Forms;
using WindowsEnhancementSuite.Forms;
using WindowsEnhancementSuite.Properties;
using WindowsEnhancementSuite.Services;
using Open.WinKeyboardHook;

namespace WindowsEnhancementSuite
{
    public class WindowsEnhancementApplicationContext : ApplicationContext
    {
        private NotifyIcon notifyIcon;
        private readonly FileAndImageSaveService fileAndImageSaveService;
        private readonly FileCreateService fileCreateService;
        private readonly FileAndImageViewService fileAndImageViewService;
        private readonly ExplorerBrowserService explorerBrowserService;

        private readonly KeyboardInterceptor keyboadHook;

        public WindowsEnhancementApplicationContext()
        {
            // Hotkeys initialisieren
            this.keyboadHook = new KeyboardInterceptor();
            this.keyboadHook.KeyDown += keyboadHookOnKeyDown;

            this.keyboadHook.StartCapturing();

            // Services initialisieren
            this.fileAndImageSaveService = new FileAndImageSaveService();
            this.fileCreateService = new FileCreateService();
            this.fileAndImageViewService = new FileAndImageViewService();
            this.explorerBrowserService = new ExplorerBrowserService();

            // TrayIcon etc. initalisieren
            this.initializeComponents();
            
            // Events hinzufügen
            Application.ApplicationExit += this.applicationOnApplicationExit;
        }

        private void keyboadHookOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Handled) return;

            if (Settings.Default.CliboardHotkey > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.CliboardHotkey)
                {
                    keyEventArgs.Handled = this.fileAndImageSaveService.SaveClipboardInFile();
                    return;
                }
            }

            if (Settings.Default.TextfileHotkey > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.TextfileHotkey)
                {
                    keyEventArgs.Handled = this.fileCreateService.CreateAndOpenTextfile();
                    return;
                }
            }

            if (Settings.Default.ShowHotkey > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.ShowHotkey)
                {
                    keyEventArgs.Handled = this.fileAndImageViewService.ShowClipboardContent();
                    return;
                }
            }

            if (Settings.Default.ShowBrowserHistory > 0)
            {
                if (keyEventArgs.KeyData == (Keys)Settings.Default.ShowBrowserHistory)
                {
                    keyEventArgs.Handled = this.explorerBrowserService.ShowExplorerHistory();
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
