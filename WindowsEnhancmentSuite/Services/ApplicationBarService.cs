using Gma.System.MouseKeyHook;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using WindowsEnhancementSuite.Forms;

namespace WindowsEnhancementSuite.Services
{
    public class ApplicationBarService
    {
        private readonly IKeyboardMouseEvents hooks;
        private readonly Rect hoverArea;
        private readonly ApplicationBarForm barForm;

        public ApplicationBarService()
        {
            hooks = Hook.GlobalEvents();
            hooks.MouseMove += mouseMoveHook;

            // Create MouseOver-Checkarea for
            // the left side of the primary screen
            var screenBounds = Screen.PrimaryScreen.WorkingArea;
            hoverArea = new Rect(screenBounds.Left - 2, screenBounds.Top, 5, screenBounds.Bottom);

            // Create and prepare the BarForm
            barForm = new ApplicationBarForm();
            barForm.Location = new System.Drawing.Point((int)hoverArea.X, (int)hoverArea.Y);
            barForm.Width = 200; // todo: make it adjustable
            barForm.Height = (int)Math.Floor(hoverArea.Bottom - hoverArea.Top);
        }

        ~ApplicationBarService()
        {
            hooks.MouseMove -= mouseMoveHook;
            hooks.Dispose();
        }

        private void mouseMoveHook(object sender, MouseEventArgs e)
        {
            if (hoverArea.Contains(e.X, e.Y))
            {
                if (!barForm.IsDisposed && !barForm.Visible)
                {
                    barForm.Show();
                    barForm.BringToFront();
                }
            }
        }
    }
}
