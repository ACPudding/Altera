using System.Text;

namespace Altera
{
    public static class FNV1a
    {
        public const uint FnvOffsetBasis32 = 2166136261u;
        public const ulong FnvOffsetBasis64 = 14695981039346656037UL;
        private const uint FnvPrime32 = 16777619u;
        private const ulong FnvPrime64 = 1099511628211UL;

        public static uint Hash32(byte[] bytes, int offset, int len, uint hash = 2166136261u)
        {
            for (var i = offset; i < len; i++) hash = (hash ^ bytes[i]) * 16777619u;
            return hash;
        }

        public static ulong Hash64(byte[] bytes, int offset, int len, ulong hash = 14695981039346656037UL)
        {
            for (var i = offset; i < len; i++) hash = (hash ^ bytes[i]) * 1099511628211UL;
            return hash;
        }

        public static uint Hash32(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Hash32(bytes, 0, bytes.Length);
        }
    }
}