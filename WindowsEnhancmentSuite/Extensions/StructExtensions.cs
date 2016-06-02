using System.Linq;

namespace WindowsEnhancementSuite.Extensions
{
    public static class StructExtensions
    {
        public static bool In<T>(this T value, params T[] values) where T : struct
        {
            return values.Contains(value);
        }
    }
}
