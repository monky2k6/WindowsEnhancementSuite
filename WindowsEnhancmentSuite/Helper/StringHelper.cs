using System;

namespace WindowsEnhancementSuite.Helper
{
    public static class StringHelper
    {
        public static string GetFirstNonEmpty(params string[] values)
        {
            foreach (string s in values)
            {
                if (!String.IsNullOrEmpty(s)) return s;
            }

            return String.Empty;
        }
    }
}
