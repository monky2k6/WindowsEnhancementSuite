using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsEnhancementSuite.Forms;
using WindowsEnhancementSuite.Services;

namespace WindowsEnhancementSuite.Bases
{
    public class DraggableBaseForm : Form
    {
        public Rectangle? ResizeValues { get; set; }
        public ApplicationBarForm AttachForm { get; set; }
        public bool IsAttached
        {
            get { return (this.AttachForm != null); }
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
        }
    }
}
