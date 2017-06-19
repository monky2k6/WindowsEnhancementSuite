using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using WindowsEnhancementSuite.Bases;
using WindowsEnhancementSuite.Forms;

namespace WindowsEnhancementSuite.Services
{
    public class ApplicationBarService : IDisposable
    {
        public static DraggableBaseForm DragForm;
        
        private readonly IKeyboardMouseEvents hooks;
        private readonly Rect hoverArea;
        private readonly ApplicationBarForm barForm;
        private readonly List<DraggableBaseForm> barForms;

        private Rectangle barFormHoverArea;
        private bool isBarResizing;

        public ApplicationBarService()
        {
            // Add global hooks
            hooks = Hook.GlobalEvents();
            hooks.MouseMove += mouseMoveHook;
            hooks.MouseUp += mouseUpHook;

            // Create MouseOver-Checkarea for
            // the left side of the primary screen
            var screenBounds = Screen.PrimaryScreen.WorkingArea;
            hoverArea = new Rect(screenBounds.Left - 2, screenBounds.Top, 5, screenBounds.Bottom);

            // Initiate collections
            barForms = new List<DraggableBaseForm>();

            // Create and prepare the BarForm
            barForm = new ApplicationBarForm();
            barForm.Location = new System.Drawing.Point((int)screenBounds.Left, (int)screenBounds.Top);
            barForm.Width = 250; // todo: make it adjustable
            barForm.Height = (screenBounds.Bottom - screenBounds.Top);
            barForm.MinimumSize = new System.Drawing.Size(250, barForm.Height);
            barForm.MaximumSize = new System.Drawing.Size(screenBounds.Width / 2, barForm.Height);

            barForm.ResizeBegin += barForm_ResizeBegin;
            barForm.ResizeEnd += barForm_ResizeEnd;
            barForm.VisibleChanged += barForm_VisibleChanged;
            barForm.Hide();

            // Calculte MouseHoverArea
            this.barForm_ResizeEnd(barForm, EventArgs.Empty);
        }

        public void Dispose()
        {
            lock (barForms)
            {
                foreach (var f in barForms)
                {
                    f.Close();
                    f.Dispose();
                }
                barForms.Clear();
            }

            hooks.MouseMove -= mouseMoveHook;
            hooks.MouseUp -= mouseUpHook;
            hooks.Dispose();
        }

        public void RemoveForm(DraggableBaseForm form)
        {
            form.AttachService = null;
            barForms.Remove(form);
            this.sortForms();
        }

        private void doFormDrop()
        {
            DragForm.AttachService = this;
            
            DragForm.ShowIcon = false;
            DragForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            DragForm.TopMost = true;
            DragForm.OriginalSize = new Tuple<int, int>(DragForm.Width, DragForm.Height);

            barForms.Add(DragForm);
            DragForm = null;

            this.sortForms();
        }

        private void doFormDetach()
        {
            DragForm.AttachService = null;
            DragForm.FormBorderStyle = FormBorderStyle.Sizable;
            DragForm.Icon = DragForm.FormIcon;
            DragForm.ShowIcon = true;
            DragForm.Icon = DragForm.FormIcon;

            barForms.Remove(DragForm);
            this.sortForms();
        }

        private void sortForms()
        {
            int formHeight = 10;
            foreach (var f in barForms.OrderBy(e => e.Top))
            {
                if (f.IsResizing)
                    f.ResizeValues = new Rectangle(0, formHeight, barForm.Width - ApplicationBarForm.DRAG_SPACE, f.Height);
                else
                {
                    f.Top = formHeight;
                    f.Left = 0;
                    f.Width = barForm.Width - ApplicationBarForm.DRAG_SPACE;
                }
                formHeight += f.Height + 5;
            }
        }

        private void barForm_ResizeBegin(object sender, EventArgs e)
        {
            this.isBarResizing = true;
        }

        private void barForm_ResizeEnd(object sender, EventArgs e)
        {
            this.barFormHoverArea = barForm.Bounds;
            this.barFormHoverArea.Inflate(1, 1);
            this.sortForms();

            this.isBarResizing = false;
        }

        private void barForm_VisibleChanged(object sender, EventArgs e)
        {
            foreach (var f in barForms)
            {
                if (DragForm == f) continue;
                f.Visible = barForm.Visible;
                if (f.Visible)
                    f.BringToFront();
            }
        }

        private void mouseMoveHook(object sender, MouseEventArgs e)
        {
            if (!barForm.IsDisposed && !barForm.Visible)
                if (hoverArea.Contains(e.X, e.Y))
                    barForm.Show();

            if (barForm.Visible)
                if (!this.isBarResizing)
                    if (Screen.GetWorkingArea(e.Location).Contains(e.Location))
                        if (!this.barFormHoverArea.Contains(e.X, e.Y))
                            barForm.Hide();
        }

        private void mouseUpHook(object sender, MouseEventArgs e)
        {
            if (DragForm != null)
                if (e.Button == MouseButtons.Left)
                    if (barForm.Visible)
                    {
                        if (barForm.Bounds.Contains(e.X, e.Y))
                            if (DragForm.IsAttached)
                                sortForms();
                            else
                                doFormDrop();
                    }
                    else if (DragForm.IsAttached)
                        doFormDetach();
        }
    }
}
