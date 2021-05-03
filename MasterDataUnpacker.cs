using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace Altera
{
    public class MasterDataUnpacker
    {
        private static StringBuilder sb;

        protected static byte[] ownerTop = new byte[32];
        protected static byte[] ownerData = new byte[32];
        protected static byte[] InfoTop = new byte[32];
        protected static byte[] infoData = new byte[32];
        private readonly MemoryStream writeStream = new MemoryStream(2000000);
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

        public void UnpackByte(Stream s, Stream ws)
        {
            var num = s.ReadByte();
            ws.WriteByte((byte) num);
            if (num < 0) throw new FormatException();
            if (num > 127)
            {
                if (num <= 143)
                {
                    UnpackMapByte(num, s, ws);
                }
                else if (num <= 159)
                {
                    UnpackArrayByte(num, s, ws);
                }
                else if (num <= 191)
                {
                    UnpackBinary(num, s, ws);
                }
                else if (num >= 224)
                {
                }
            }

            switch (num)
            {
                case 196:
                case 197:
                case 198:
                case 217:
                case 218:
                case 219:
                    UnpackBinary(num, s, ws);
                    break;
                case 202:
                    s.Read(tmp0, 0, 4);
                    ws.Write(tmp0, 0, 4);
                    break;
                case 203:
                    s.Read(tmp0, 0, 8);
                    ws.Write(tmp0, 0, 8);
                    break;
                case 204:
                    ws.WriteByte((byte) s.ReadByte());
                    break;
                case 205:
                    s.Read(tmp0, 0, 2);
                    ws.Write(tmp0, 0, 2);
                    break;
                case 206:
                    s.Read(tmp0, 0, 4);
                    ws.Write(tmp0, 0, 4);
                    break;
                case 207:
                    if (s.Read(tmp0, 0, 8) != 8) throw new FormatException();
                    ws.Write(tmp0, 0, 8);
                    break;
                case 208:
                    ws.WriteByte((byte) s.ReadByte());
                    break;
                case 209:
                    if (s.Read(tmp0, 0, 2) != 2) throw new FormatException();
                    ws.Write(tmp0, 0, 2);
                    break;
                case 210:
                    if (s.Read(tmp0, 0, 4) != 4) throw new FormatException();
                    ws.Write(tmp0, 0, 4);
                    break;
                case 211:
                    if (s.Read(tmp0, 0, 8) != 8) throw new FormatException();
                    ws.Write(tmp0, 0, 8);
                    break;
                case 220:
                case 221:
                    UnpackArrayByte(num, s, ws);
                    break;
                case 222:
                case 223:
                    UnpackMapByte(num, s, ws);
                    break;
            }
        }

        public void UnpackBinary(int b, Stream s, Stream ws)
        {
            if (b <= 191)
            {
                var num = b & 31;
                if (num == 0) return;
                var array = new byte[num];
                s.Read(array, 0, num);
                ws.Write(array, 0, num);
            }
            else
            {
                switch (b)
                {
                    case 196:
                        break;
                    case 197:
                        goto IL_9D;
                    case 198:
                        goto IL_EE;
                    default:
                        switch (b)
                        {
                            case 217:
                                break;
                            case 218:
                                goto IL_9D;
                            case 219:
                                goto IL_EE;
                            default:
                                return;
                        }

                        break;
                }

                var num2 = s.ReadByte();
                var array2 = new byte[num2];
                s.Read(array2, 0, num2);
                ws.WriteByte((byte) num2);
                ws.Write(array2, 0, num2);
                return;
                IL_9D:
                var array3 = new byte[2];
                s.Read(array3, 0, 2);
                var array4 = new byte[(long) ((array3[0] << 8) | array3[1])];
                s.Read(array4, 0, array4.Length);
                ws.Write(array3, 0, 2);
                ws.Write(array4, 0, array4.Length);
                return;
                IL_EE:
                var array5 = new byte[4];
                s.Read(array5, 0, 4);
                var array6 = new byte[((long) array5[0] << 24) | ((long) array5[1] << 16) | ((long) array5[2] << 8) |
                                      array5[3]];
                s.Read(array6, 0, array6.Length);
                ws.Write(array5, 0, 4);
                ws.Write(array6, 0, array6.Length);
            }
        }

        public void UnpackArrayByte(int b, Stream s, Stream ws)
        {
            var num = 0L;
            if (b <= 159)
            {
                num = b & 15;
            }
            else if (b != 220)
            {
                if (b == 221)
                {
                    s.Read(tmp0, 0, 4);
                    num = ((long) tmp0[0] << 24) | ((long) tmp0[1] << 16) | ((long) tmp0[2] << 8) | tmp0[3];
                    ws.Write(tmp0, 0, 4);
                }
            }
            else
            {
                s.Read(tmp0, 0, 2);
                num = (tmp0[0] << 8) | tmp0[1];
                ws.Write(tmp0, 0, 2);
            }

            var num2 = 0;
            while (num2 < num)
            {
                UnpackByte(s, ws);
                num2++;
            }
        }

        public void UnpackMapByte(int b, Stream s, Stream ws)
        {
            var num = 0L;
            if (b <= 143)
            {
                num = b & 15;
            }
            else if (b != 222)
            {
                if (b == 223)
                {
                    s.Read(tmp0, 0, 4);
                    num = ((long) tmp0[0] << 24) | ((long) tmp0[1] << 16) | ((long) tmp0[2] << 8) | tmp0[3];
                    ws.Write(tmp0, 0, 4);
                }
            }
            else
            {
                s.Read(tmp0, 0, 2);
                num = (tmp0[0] << 8) | tmp0[1];
                ws.Write(tmp0, 0, 2);
            }

            var num2 = 0;
            while (num2 < num)
            {
                UnpackByte(s, ws);
                UnpackByte(s, ws);
                num2++;
            }
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

        private Dictionary<string, byte[]> UnpackMap(Stream s, long len)
        {
            var dictionary = new Dictionary<string, byte[]>((int) len);
            for (var num = 0L; num < len; num += 1L)
            {
                var text = Unpack(s) as string;
                writeStream.Position = 0L;
                writeStream.SetLength(0L);
                UnpackByte(s, writeStream);
                if (text != null) dictionary.Add(text, writeStream.ToArray());
            }

            return dictionary;
        }

        public static byte[] MouseHomeSub(byte[] data, byte[] home, byte[] info, bool isCompress = false)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            byte[] result;
            try
            {
                var cryptoTransform = new RijndaelManaged
                {
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC,
                    KeySize = 256,
                    BlockSize = 256
                }.CreateDecryptor(home, info);
                var array = new byte[data.Length];
                memoryStream = new MemoryStream(data);
                cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
                cryptoStream.Read(array, 0, array.Length);
                if (isCompress)
                {
                    var memoryStream2 = new MemoryStream();
                    var memoryStream3 = new MemoryStream(array);
                    var gzipInputStream = new GZipInputStream(memoryStream3);
                    var array2 = new byte[16384];
                    int num;
                    while ((num = gzipInputStream.Read(array2, 0, array2.Length)) > 0)
                        memoryStream2.Write(array2, 0, num);
                    gzipInputStream.Close();
                    array = memoryStream2.ToArray();
                    memoryStream3.Close();
                    memoryStream2.Close();
                }

                result = array;
            }
            catch (Exception)
            {
                result = null;
            }
            finally
            {
                if (memoryStream != null) memoryStream.Close();
                if (cryptoStream != null) cryptoStream.Close();
            }

            return result;
        }

        public static object MouseHomeMaster(byte[] data, byte[] home, byte[] info, bool isCompress = false)
        {
            var buf = MouseHomeSub(data, home, info, isCompress);
            var MasterDataUnpacker = new MasterDataUnpacker();
            return MasterDataUnpacker.Unpack(buf);
        }

        public static object MouseGame2Unpacker(byte[] data, bool isCompress = false)
        {
            Array.Copy(data, 0, ownerTop, 0, 32);
            var array = new byte[data.Length - 32];
            Array.Copy(data, 32, array, 0, data.Length - 32);
            ownerData = Encoding.UTF8.GetBytes("pX6q6xK2UymhFKcaGHHUlfXqfTsWF0uH");
            return MouseHomeMaster(array, ownerData, ownerTop, true);
        }

        public static object MouseHomeMsgPack(byte[] data, byte[] home, byte[] info, bool isCompress = false)
        {
            var miniMessagePacker = new MiniMessagePacker();
            var buf = MouseHomeSub(data, home, info, isCompress);
            return miniMessagePacker.Unpack(buf);
        }

        public static byte[] Object2Bytes(object obj)
        {
            byte[] buff;
            using (var ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }

            return buff;
        }

        public static object MouseInfoMsgPack(byte[] data)
        {
            var array = new byte[data.Length - 32];
            infoData = Encoding.UTF8.GetBytes("W0Juh4cFJSYPkebJB9WpswNF51oa6Gm7");
            Array.Copy(data, 0, InfoTop, 0, 32);
            Array.Copy(data, 32, array, 0, data.Length - 32);
            return MouseHomeMsgPack(array, infoData, InfoTop, true);
        }

        public static object MouseGame2MsgPack(byte[] data, bool isCompress = false)
        {
            Array.Copy(data, 0, ownerTop, 0, 32);
            var array = new byte[data.Length - 32];
            Array.Copy(data, 32, array, 0, data.Length - 32);
            ownerData = Encoding.UTF8.GetBytes("pX6q6xK2UymhFKcaGHHUlfXqfTsWF0uH");
            return MouseHomeMsgPack(array, ownerData, ownerTop, true);
        }
    }
}