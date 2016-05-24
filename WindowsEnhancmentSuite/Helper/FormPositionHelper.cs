using System.Drawing;
using System.Windows.Forms;

namespace WindowsEnhancementSuite.Helper
{
    public static class FormPositionHelper
    {
        public static Rectangle GetActiveScreenBounds()
        {
            var screen = Screen.FromPoint(Cursor.Position);
            return screen.Bounds;
        }
    }
}
