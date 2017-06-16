using System.Windows.Forms;

namespace WindowsEnhancementSuite.Forms
{
    public partial class ApplicationBarForm : Form
    {
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
    }
}
