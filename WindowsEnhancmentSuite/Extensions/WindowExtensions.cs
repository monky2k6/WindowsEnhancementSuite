using System.Drawing;
using System.Windows;

namespace WindowsEnhancementSuite.Extensions
{
    public static class WindowExtensions
    {
        public static Rectangle Bounds(this Window window)
        {
            var rectangle = new Rectangle
            {
                X = (int)window.Left,
                Y = (int)window.Top,
                Width = (int)window.Width,
                Height = (int)window.Height
            };

            return rectangle;
        }
    }
}
