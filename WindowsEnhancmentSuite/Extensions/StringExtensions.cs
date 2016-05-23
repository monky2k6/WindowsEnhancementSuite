using System.IO;
using System.Text;
using WindowsEnhancementSuite.Helper;

namespace WindowsEnhancementSuite.Extensions
{
    public static class StringExtensions
    {
        public static T ToStream<T>(this string input) where T : Stream, new()
        {
            var stream = new T();
            var sw = new StreamWriter(stream);
            sw.Write(input);
            sw.Flush();

            stream.Position = 0;

            return stream;
        }

        public static string ConditionalAttach(this string input, bool condition, string attachment)
        {
            if (condition) return input + attachment;
            return input;
        }

        public static string GetMd5Hash(this string input)
        {
            var bytes = MD5Helper.ComputeMD5Hash(Encoding.Default.GetBytes(input));
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
