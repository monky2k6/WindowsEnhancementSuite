using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsEnhancementSuite.Forms;
using WindowsEnhancementSuite.Services;

namespace WindowsEnhancementSuite.Bases
{
    public class DraggableBaseForm : Form
    {
        public Tuple<int, int> OriginalSize { get; set; }
        public Rectangle? ResizeValues { get; set; }
        public ApplicationBarService AttachService { get; set; }
        public Icon FormIcon { get; protected set; }
        public bool IsAttached
        {
            get { return (this.AttachService != null); }
        }
        public bool IsResizing { get; protected set; }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            this.IsResizing = true;
            ApplicationBarService.DragForm = this;
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            this.IsResizing = false;
            ApplicationBarService.DragForm = null;

            if (this.ResizeValues.HasValue && this.IsAttached)
            {
                this.Top = ResizeValues.Value.Top;
                this.Left = ResizeValues.Value.Left;
                this.Width = ResizeValues.Value.Width;

                this.ResizeValues = null;
            }

            if (this.OriginalSize != null && !this.IsAttached)
            {
                this.Width = OriginalSize.Item1;
                this.Height = OriginalSize.Item2;

                this.OriginalSize = null;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel) return;
            if (this.IsAttached) this.AttachService.RemoveForm(this);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.FormIcon = this.Icon;
        }
    }
}
