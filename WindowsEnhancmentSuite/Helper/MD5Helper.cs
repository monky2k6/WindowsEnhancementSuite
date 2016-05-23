using System.Security.Cryptography;

namespace WindowsEnhancementSuite.Helper
{
    public static class MD5Helper
    {
        public static byte[] ComputeMD5Hash(byte[] input)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(input);
            }
        }
    }
}
