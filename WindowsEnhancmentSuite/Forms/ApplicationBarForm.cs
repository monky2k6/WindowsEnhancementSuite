using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsEnhancementSuite.Bases;

namespace WindowsEnhancementSuite.Forms
{
    public partial class ApplicationBarForm : Form
    {
        private List<Control> childControls;
        public ApplicationBarForm()
        {
            InitializeComponent();
            childControls = new List<Control>();
        }

        private void ApplicationBarForm_MouseLeave(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void ApplicationBarForm_DragOver(object sender, DragEventArgs e)
        {
        }

        private void ApplicationBarForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DraggableBaseForm.DRAG_FORMAT))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ApplicationBarForm_DragDrop(object sender, DragEventArgs e)
        {
            var dragForm = (Form)e.Data.GetData(DraggableBaseForm.DRAG_FORMAT, false);
            var dragControl = (Control)e.Data.GetData(DraggableBaseForm.DRAG_FORMAT, true);

            dragControl.Parent = this;
            dragForm.Close();

            // Positioning the Control (todo: preview in DragOver)
            childControls.Add(dragControl);

            int childHeight = 3;
            foreach (var c in childControls)
            {
                c.Left = 0;
                c.Top = childHeight;
                c.Width = this.Width;
                childHeight += c.Height + 3;
            }
        }
    }
}
