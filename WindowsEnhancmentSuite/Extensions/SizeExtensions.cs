using System.Drawing;

namespace WindowsEnhancementSuite.Extensions
{
    public static class SizeExtensions
    {
        public static Size Adjust(this Size size, int width, int height)
        {
            return new Size(size.Width + width, size.Height + height);
        }
    }
}
