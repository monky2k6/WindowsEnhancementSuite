using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsEnhancementSuite.Forms;
using WindowsEnhancementSuite.Helper;
using WindowsEnhancementSuite.Properties;

namespace WindowsEnhancementSuite.Extensions
{
    public static class FormExtensions
    {
        public static void AttachToolBar(this Form winForm, Action action, bool runAsSta = true)
        {
            var layeredWindow = new LayeredWindow
            {
                Size = new Size(10, 10),
                StartPosition = FormStartPosition.Manual,
                Owner = winForm
            };

            layeredWindow.SetBitmap(Resources.ClipboardButton);
            layeredWindow.Click += (sender, args) =>
            {
                if (runAsSta)
                {
                    ThreadHelper.RunAsStaThread(action.Invoke);
                }
                else
                {
                    action();
                }
            };

            winForm.Move += (sender, args) => layeredWindow.setLocation(winForm);

            winForm.Resize += (sender, args) =>
            {
                layeredWindow.setLocation(winForm);

                if (winForm.WindowState == FormWindowState.Maximized)
                {
                    layeredWindow.Location = new Point(layeredWindow.Location.X, 0);
                }
            };

            winForm.Shown += (sender, args) =>
            {
                layeredWindow.setLocation(winForm);
                layeredWindow.Show();
            };
        }

        private static void setLocation(this Form locationForm, Form baseForm)
        {
            locationForm.Location = new Point(baseForm.Location.X + baseForm.Width - locationForm.Width - 105, baseForm.Location.Y + 1);
        }
    }
}
