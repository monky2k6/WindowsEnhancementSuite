using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using WindowsEnhancementSuite.Bases;
using WindowsEnhancementSuite.Forms;
using WindowsEnhancementSuite.Properties;
using WindowsEnhancementSuite.Extensions;
using System.IO;
using System.Windows.Markup;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace WindowsEnhancementSuite.Services
{
    public class ApplicationBarService : IDisposable
    {
        public static DraggableBaseForm DragForm;
        
        private readonly IKeyboardMouseEvents hooks;
        private readonly Rect hoverArea;
        private readonly Window applicationBarWindow;
        private readonly List<DraggableBaseForm> barForms;
        private readonly DoubleAnimation slideIn;
        private readonly DoubleAnimation slideOut;
        private readonly DependencyProperty widthProperty;

        private Rectangle barFormHoverArea;
        private bool isBarResizing;

        public ApplicationBarService()
        {
            // Add global hooks
            hooks = Hook.GlobalEvents();
            hooks.MouseMove += mouseMoveHook;
            hooks.MouseUp += mouseUpHook;

            // Load WPF Window
            using (var stream = Resources.ApplicationBar.ToStream<MemoryStream>())
            {
                applicationBarWindow = XamlReader.Load(stream) as Window;
            }

            if (applicationBarWindow == null) return;
            ElementHost.EnableModelessKeyboardInterop(applicationBarWindow);
            applicationBarWindow.Topmost = true;

            // Create MouseOver-Checkarea for
            var screenBounds = Screen.PrimaryScreen.WorkingArea;
            hoverArea = new Rect(screenBounds.Left - 2, screenBounds.Top, 5, screenBounds.Bottom);

            // Create and prepare the BarForm
            applicationBarWindow.Show();
            applicationBarWindow.Hide();
            applicationBarWindow.Left = screenBounds.Left;
            applicationBarWindow.Top = screenBounds.Top;
            applicationBarWindow.Width = 250;
            applicationBarWindow.Height = screenBounds.Height;

            // Prepare animations
            slideIn = new DoubleAnimation(0f, 250f, new Duration(TimeSpan.FromSeconds(3)));
            widthProperty = DependencyProperty.Register("Width", typeof(double), applicationBarWindow.GetType());

            this.barFormHoverArea = applicationBarWindow.Bounds();
            this.barFormHoverArea.Inflate(1, 1);

            //// Create MouseOver-Checkarea for
            //// the left side of the primary screen
            //var screenBounds = Screen.PrimaryScreen.WorkingArea;
            //hoverArea = new Rect(screenBounds.Left - 2, screenBounds.Top, 5, screenBounds.Bottom);

            // Initiate collections
            barForms = new List<DraggableBaseForm>();

            // Create and prepare the BarForm
            //applicationBarWindow = new ApplicationBarForm();
            //applicationBarWindow.Location = new System.Drawing.Point((int)screenBounds.Left, (int)screenBounds.Top);
            //applicationBarWindow.Width = 250; // todo: make it adjustable
            //applicationBarWindow.Height = (screenBounds.Bottom - screenBounds.Top);
            //applicationBarWindow.MinimumSize = new System.Drawing.Size(250, applicationBarWindow.Height);
            //applicationBarWindow.MaximumSize = new System.Drawing.Size(screenBounds.Width / 2, applicationBarWindow.Height);

            //applicationBarWindow.ResizeBegin += barForm_ResizeBegin;
            //applicationBarWindow.ResizeEnd += barForm_ResizeEnd;
            //applicationBarWindow.VisibleChanged += barForm_VisibleChanged;
            //applicationBarWindow.Hide();

            // Calculte MouseHoverArea
            //this.barForm_ResizeEnd(applicationBarWindow, EventArgs.Empty);
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
            //int formHeight = 10;
            //foreach (var f in barForms.OrderBy(e => e.Top))
            //{
            //    if (f.IsResizing)
            //        f.ResizeValues = new Rectangle(0, formHeight, applicationBarWindow.Width - ApplicationBarForm.DRAG_SPACE, f.Height);
            //    else
            //    {
            //        f.Top = formHeight;
            //        f.Left = 0;
            //        f.Width = applicationBarWindow.Width - ApplicationBarForm.DRAG_SPACE;
            //    }
            //    formHeight += f.Height + 5;
            //}
        }

        private void doSlideIn()
        {
            applicationBarWindow.Show();
            applicationBarWindow.BeginAnimation(widthProperty, slideIn);
        }

        private void doSlideOut()
        {

        }

        private void barForm_ResizeBegin(object sender, EventArgs e)
        {
            this.isBarResizing = true;
        }

        private void barForm_ResizeEnd(object sender, EventArgs e)
        {
            //this.barFormHoverArea = applicationBarWindow.Bounds;
            //this.barFormHoverArea.Inflate(1, 1);
            //this.sortForms();

            //this.isBarResizing = false;
        }

        private void barForm_VisibleChanged(object sender, EventArgs e)
        {
            //foreach (var f in barForms)
            //{
            //    if (DragForm == f) continue;
            //    f.Visible = applicationBarWindow.IsVisible;
            //    if (f.Visible)
            //        f.BringToFront();
            //}
        }

        private void mouseMoveHook(object sender, MouseEventArgs e)
        {
            if (applicationBarWindow.IsLoaded && !applicationBarWindow.IsVisible)
                if (hoverArea.Contains(e.X, e.Y))
                    doSlideIn();
                    //applicationBarWindow.Show();

            if (applicationBarWindow.IsVisible)
                //if (!this.isBarResizing)
                    if (Screen.GetWorkingArea(e.Location).Contains(e.Location))
                        if (!this.barFormHoverArea.Contains(e.X, e.Y))
                            applicationBarWindow.Hide();
        }

        private void mouseUpHook(object sender, MouseEventArgs e)
        {
            if (DragForm != null)
                if (e.Button == MouseButtons.Left)
                    if (applicationBarWindow.IsVisible)
                    {
                        if (applicationBarWindow.Bounds().Contains(e.X, e.Y))
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
