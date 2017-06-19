using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsEnhancementSuite.Forms
{
    public partial class ApplicationBarForm : Form
    {
        public const byte DRAG_SPACE = 10;

        public ApplicationBarForm()
        {
            InitializeComponent();
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;

                const int WS_EX_NOACTIVATE = 0x08000000;
                const int WS_EX_TOOLWINDOW = 0x00000080;
                createParams.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;

                return createParams;
            }
        }
        
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_NCHITTEST = 0x84;
            if (m.Msg == WM_NCHITTEST)
            {
                var mousePos = new Point(m.LParam.ToInt32());
                mousePos = this.PointToClient(mousePos);

                if (mousePos.X >= this.Width - DRAG_SPACE)
                    m.Result = new IntPtr((int)HitTest.Right);
            }
        }

        protected enum HitTest
        {
            Caption = -2,
            Transparent = -1,
            Nowhere = 0,
            Client = 1,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17,
            Border = 18
        }
    }
}
