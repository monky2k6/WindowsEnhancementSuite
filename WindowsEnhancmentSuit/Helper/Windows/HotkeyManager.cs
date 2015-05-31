using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsEnhancementSuit.Helper.Windows
{
    public sealed class HotkeyManager : NativeWindow, IDisposable
    {
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int WM_HOTKEY = 0x0312;

        private readonly Action keyAction;

        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public HotkeyManager(Keys hotkey, Action action, bool shiftModifier = false, bool controlModifier = false)
        {
            this.CreateHandle(new CreateParams());

            var modifier = (shiftModifier ? MOD_SHIFT : 0) + (controlModifier ? MOD_CONTROL : 0);
            if (RegisterHotKey(this.Handle, 1, (uint)modifier, (uint)hotkey))
            {
                this.keyAction = action;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                this.keyAction.Invoke();
            }
        }

        void IDisposable.Dispose()
        {
            UnregisterHotKey(this.Handle, 1);
            this.DestroyHandle();
        }
    }
}
