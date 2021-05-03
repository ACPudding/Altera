﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altera
{
    public class MiniMessagePacker
    {
        private const int TmpStringHashCapacity = 2000;
        private const int InternPoolCapacity = 40000;
        private static StringBuilder sb;
        private static byte[] tmpStringHash = new byte[2000];
        private Encoding encoder = Encoding.UTF8;
        private readonly byte[] tmp0 = new byte[8];
        private readonly byte[] tmp1 = new byte[8];

        public object Unpack(byte[] buf, int offset, int size)
        {
            object result;
            using (var memoryStream = new MemoryStream(buf, offset, size))
            {
                result = Unpack(memoryStream);
            }

            return result;
        }

        public object Unpack(byte[] buf)
        {
            return Unpack(buf, 0, buf.Length);
        }

        public object Unpack(Stream s)
        {
            var num = s.ReadByte();
            if (num < 0) throw new FormatException();
            if (num <= 127) return (long) num;
            if (num <= 143) return UnpackMap(s, num & 15);
            if (num <= 159) return UnpackArray(s, num & 15);
            if (num <= 191) return UnpackString(s, num & 31);
            if (num >= 224) return (long) (sbyte) num;
            switch (num)
            {
                case 192:
                    return null;
                case 194:
                    return false;
                case 195:
                    return true;
                case 196:
                    return UnpackBinary(s, s.ReadByte());
                case 197:
                    return UnpackBinary(s, UnpackUint16(s));
                case 198:
                    return UnpackBinary(s, UnpackUint32(s));
                case 202:
                    s.Read(tmp0, 0, 4);
                    if (!BitConverter.IsLittleEndian) return (double) BitConverter.ToSingle(tmp0, 0);
                    tmp1[0] = tmp0[3];
                    tmp1[1] = tmp0[2];
                    tmp1[2] = tmp0[1];
                    tmp1[3] = tmp0[0];
                    return (double) BitConverter.ToSingle(tmp1, 0);

                case 203:
                    s.Read(tmp0, 0, 8);
                    if (!BitConverter.IsLittleEndian) return BitConverter.ToDouble(tmp0, 0);
                    tmp1[0] = tmp0[7];
                    tmp1[1] = tmp0[6];
                    tmp1[2] = tmp0[5];
                    tmp1[3] = tmp0[4];
                    tmp1[4] = tmp0[3];
                    tmp1[5] = tmp0[2];
                    tmp1[6] = tmp0[1];
                    tmp1[7] = tmp0[0];
                    return BitConverter.ToDouble(tmp1, 0);

                case 204:
                    return (long) s.ReadByte();
                case 205:
                    return UnpackUint16(s);
                case 206:
                    return UnpackUint32(s);
                case 207:
                    if (s.Read(tmp0, 0, 8) != 8) throw new FormatException();
                    return ((long) tmp0[0] << 56) | ((long) tmp0[1] << 48) | ((long) tmp0[2] << 40) |
                           (((long) tmp0[3] << 32) + ((long) tmp0[4] << 24)) | ((long) tmp0[5] << 16) |
                           ((long) tmp0[6] << 8) | tmp0[7];
                case 208:
                    return (long) (sbyte) s.ReadByte();
                case 209:
                    if (s.Read(tmp0, 0, 2) != 2) throw new FormatException();
                    return ((long) (sbyte) tmp0[0] << 8) | tmp0[1];
                case 210:
                    if (s.Read(tmp0, 0, 4) != 4) throw new FormatException();
                    return ((long) (sbyte) tmp0[0] << 24) | ((long) tmp0[1] << 16) | ((long) tmp0[2] << 8) | tmp0[3];
                case 211:
                    if (s.Read(tmp0, 0, 8) != 8) throw new FormatException();
                    return ((long) (sbyte) tmp0[0] << 56) | ((long) tmp0[1] << 48) | ((long) tmp0[2] << 40) |
                           (((long) tmp0[3] << 32) + ((long) tmp0[4] << 24)) | ((long) tmp0[5] << 16) |
                           ((long) tmp0[6] << 8) | tmp0[7];
                case 217:
                    return UnpackString(s, s.ReadByte());
                case 218:
                    return UnpackString(s, UnpackUint16(s));
                case 219:
                    return UnpackString(s, UnpackUint32(s));
                case 220:
                    return UnpackArray(s, UnpackUint16(s));
                case 221:
                    return UnpackArray(s, UnpackUint32(s));
                case 222:
                    return UnpackMap(s, UnpackUint16(s));
                case 223:
                    return UnpackMap(s, UnpackUint32(s));
            }

            return null;
        }

        private long UnpackUint16(Stream s)
        {
            if (s.Read(tmp0, 0, 2) != 2) throw new FormatException();
            return (tmp0[0] << 8) | tmp0[1];
        }

        private long UnpackUint32(Stream s)
        {
            if (s.Read(tmp0, 0, 4) != 4) throw new FormatException();
            return ((long) tmp0[0] << 24) | ((long) tmp0[1] << 16) | ((long) tmp0[2] << 8) | tmp0[3];
        }

        private string UnpackString(Stream s, long len)
        {
            if (sb == null)
            {
                sb = new StringBuilder((int) len);
            }
            else
            {
                sb.Length = 0;
                sb.EnsureCapacity((int) len);
            }

            var num = 0u;
            var num2 = 0u;
            var num3 = 0u;
            var num4 = 0;
            while (num4 < len)
            {
                var num5 = (uint) s.ReadByte();
                if (num2 == 0u)
                {
                    if (num5 < 128u)
                    {
                        sb.Append((char) num5);
                    }
                    else if ((num5 & 224u) == 192u)
                    {
                        num = num5 & 31u;
                        num3 = 1u;
                        num2 = 2u;
                    }
                    else if ((num5 & 240u) == 224u)
                    {
                        num = num5 & 15u;
                        num3 = 1u;
                        num2 = 3u;
                    }
                    else if ((num5 & 248u) == 240u)
                    {
                        num = num5 & 7u;
                        num3 = 1u;
                        num2 = 4u;
                    }
                    else if ((num5 & 252u) == 248u)
                    {
                        num = num5 & 3u;
                        num3 = 1u;
                        num2 = 5u;
                    }
                    else if ((num5 & 254u) == 252u)
                    {
                        num = num5 & 3u;
                        num3 = 1u;
                        num2 = 6u;
                    }
                }
                else if ((num5 & 192u) == 128u)
                {
                    num = (num << 6) | (num5 & 63u);
                    if ((num3 += 1u) >= num2)
                    {
                        if (num < 65536u)
                        {
                            sb.Append((char) num);
                        }
                        else if (num < 1114112u)
                        {
                            num -= 65536u;
                            sb.Append((char) ((num >> 10) + 55296u));
                            sb.Append((char) ((num & 1023u) + 56320u));
                        }

                        num2 = 0u;
                    }
                }

                num4++;
            }

            return sb.ToString();
        }

        private byte[] UnpackBinary(Stream s, long len)
        {
            var array = new byte[len];
            s.Read(array, 0, (int) len);
            return array;
        }

        private List<object> UnpackArray(Stream s, long len)
        {
            var list = new List<object>((int) len);
            for (var num = 0L; num < len; num += 1L) list.Add(Unpack(s));
            return list;
        }

        private Dictionary<string, object> UnpackMap(Stream s, long len)
        {
            var dictionary = new Dictionary<string, object>((int) len);
            for (var num = 0L; num < len; num += 1L)
            {
                var text = Unpack(s) as string;
                var obj = Unpack(s);
                if (text != null) dictionary.Add(text, obj);
            }

            return dictionary;
        }

        public void LL_Skip(Stream s)
        {
            var num = s.ReadByte();
            if (num < 0) throw new FormatException();
            if (num <= 127) return;
            if (num <= 143)
            {
                LL_SkipMap(s, num & 15);
                return;
            }

            if (num <= 159)
            {
                LL_SkipArray(s, num & 15);
                return;
            }

            if (num <= 191)
            {
                LL_Seek(s, num & 31);
                return;
            }

            if (num >= 224) return;
            switch (num)
            {
                case 192:
                    return;
                case 194:
                    return;
                case 195:
                    return;
                case 196:
                    LL_Seek(s, s.ReadByte());
                    return;
                case 197:
                    LL_Seek(s, UnpackUint16(s));
                    return;
                case 198:
                    LL_Seek(s, UnpackUint32(s));
                    return;
                case 202:
                    LL_Seek(s, 4L);
                    return;
                case 203:
                    LL_Seek(s, 8L);
                    return;
                case 204:
                    LL_Seek(s, 1L);
                    return;
                case 205:
                    LL_Seek(s, 2L);
                    return;
                case 206:
                    LL_Seek(s, 4L);
                    return;
                case 207:
                    LL_Seek(s, 8L);
                    return;
                case 208:
                    LL_Seek(s, 1L);
                    return;
                case 209:
                    LL_Seek(s, 2L);
                    return;
                case 210:
                    LL_Seek(s, 4L);
                    return;
                case 211:
                    LL_Seek(s, 8L);
                    return;
                case 217:
                    LL_Seek(s, s.ReadByte());
                    return;
                case 218:
                    LL_Seek(s, UnpackUint16(s));
                    return;
                case 219:
                    LL_Seek(s, UnpackUint32(s));
                    return;
                case 220:
                    LL_SkipArray(s, (int) UnpackUint16(s));
                    return;
                case 221:
                    LL_SkipArray(s, (int) UnpackUint32(s));
                    return;
                case 222:
                    LL_SkipMap(s, (int) UnpackUint16(s));
                    return;
                case 223:
                    LL_SkipMap(s, (int) UnpackUint32(s));
                    return;
            }

            throw new FormatException();
        }

        private void LL_SkipArray(Stream s, int len)
        {
            for (var i = 0; i < len; i++) LL_Skip(s);
        }

        private void LL_SkipMap(Stream s, int len)
        {
            for (var i = 0; i < len; i++)
            {
                LL_Skip(s);
                LL_Skip(s);
            }
        }

        private void LL_Seek(Stream s, long offset)
        {
            if (s.Length < s.Seek(offset, SeekOrigin.Current)) throw new FormatException();
        }

        internal List<object> Unpack(object v)
        {
            throw new NotImplementedException();
        }
    }
}