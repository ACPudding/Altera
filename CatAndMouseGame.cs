using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;

public class CatAndMouseGame
{
    private static readonly string salt = "pN6ds2Bg";
    private static byte[] BattleIV = new byte[32];
    private static byte[] BattleKey = new byte[32];
    protected static byte[] stageTop = new byte[32];

    protected static byte[] stageData = new byte[32];

    protected static byte[] baseTop = new byte[32];

    protected static byte[] baseData = new byte[32];

    static CatAndMouseGame()
    {
        var bytes = Encoding.UTF8.GetBytes("kzdMtpmzqCHAfx00saU1gIhTjYCuOD1JstqtisXsGYqRVcqrHRydj3k6vJCySu3g");
        var bytes2 = Encoding.UTF8.GetBytes("PFBs0eIuunoxKkCcLbqDVerU1rShhS276SAL3A8tFLUfGvtz3F3FFeKELIk3Nvi4");
        for (var i = 0; i < bytes2.Length / 4; i++)
            if (i % 2 == 0)
            {
                baseData[i / 2 * 4] = bytes2[i * 4];
                baseData[i / 2 * 4 + 1] = bytes2[i * 4 + 1];
                baseData[i / 2 * 4 + 2] = bytes2[i * 4 + 2];
                baseData[i / 2 * 4 + 3] = bytes2[i * 4 + 3];
            }
            else
            {
                baseTop[i / 2 * 4] = bytes2[i * 4];
                baseTop[i / 2 * 4 + 1] = bytes2[i * 4 + 1];
                baseTop[i / 2 * 4 + 2] = bytes2[i * 4 + 2];
                baseTop[i / 2 * 4 + 3] = bytes2[i * 4 + 3];
            }

        for (var i = 0; i < bytes.Length; i++)
            if (i % 2 == 0)
                stageData[i / 2] = bytes[i];
            else
                stageTop[i / 2] = bytes[i];
    }

    public static string GetMD5String(string input)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(input + salt);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
                sb.Append(t.ToString("x2"));

            return sb.ToString();
        }
    }

    public static string GetShaName(string name)
    {
        SHA1 sha = new SHA1CryptoServiceProvider();
        var utf8Encoding = new UTF8Encoding();
        var bytes = utf8Encoding.GetBytes(name);
        var array = sha.ComputeHash(bytes);
        var stringBuilder = new StringBuilder();
        foreach (var b in array) stringBuilder.AppendFormat("{0,0:x2}", b ^ 170);
        stringBuilder.Append(".bin");
        return stringBuilder.ToString();
    }

    public static void CN()
    {
        var bytes = Encoding.UTF8.GetBytes("d3b13d9093cc6b457fd89766bafa1626ee2ef76626d49ce0d424f4156231ce56");
        var bytes2 = Encoding.UTF8.GetBytes("5ec7ce0fddc50bca9f82b8338b9135c69e0e9e169648df69054dcb96553598e6");
        for (var i = 0; i < bytes2.Length; i++)
            if (i % 2 == 0)
                baseData[i / 2] = bytes2[i];
            else
                baseTop[i / 2] = bytes2[i];
        for (var j = 0; j < bytes.Length / 4; j++)
            if (j % 2 == 0)
            {
                stageData[j / 2 * 4] = bytes[j * 4];
                stageData[j / 2 * 4 + 1] = bytes[j * 4 + 1];
                stageData[j / 2 * 4 + 2] = bytes[j * 4 + 2];
                stageData[j / 2 * 4 + 3] = bytes[j * 4 + 3];
            }
            else
            {
                stageTop[j / 2 * 4] = bytes[j * 4];
                stageTop[j / 2 * 4 + 1] = bytes[j * 4 + 1];
                stageTop[j / 2 * 4 + 2] = bytes[j * 4 + 2];
                stageTop[j / 2 * 4 + 3] = bytes[j * 4 + 3];
            }
    }

    public static void EN()
    {
        var bytes = Encoding.UTF8.GetBytes("xaVPXPtrkXlUZsJRa3Eu1o1kSDYtjlwhoRQI2MHq2Q4szmpVvDcbmpi7UIZF9Rle");
        var bytes2 = Encoding.UTF8.GetBytes("FEq45VzsnHv8ynuLIGGF9qRA2tJ6vJ61FkG6KliUnD77cN7pvveVAH5gcPeLEzOR");
        for (var i = 0; i < bytes2.Length / 4; i++)
            if (i % 2 == 0)
            {
                baseData[i / 2 * 4] = bytes2[i * 4];
                baseData[i / 2 * 4 + 1] = bytes2[i * 4 + 1];
                baseData[i / 2 * 4 + 2] = bytes2[i * 4 + 2];
                baseData[i / 2 * 4 + 3] = bytes2[i * 4 + 3];
            }
            else
            {
                baseTop[i / 2 * 4] = bytes2[i * 4];
                baseTop[i / 2 * 4 + 1] = bytes2[i * 4 + 1];
                baseTop[i / 2 * 4 + 2] = bytes2[i * 4 + 2];
                baseTop[i / 2 * 4 + 3] = bytes2[i * 4 + 3];
            }

        for (var i = 0; i < bytes.Length; i++)
            if (i % 2 == 0)
                stageData[i / 2] = bytes[i];
            else
                stageTop[i / 2] = bytes[i];
    }

    public static string getShaName(string name)
    {
        SHA1 sha = new SHA1CryptoServiceProvider();
        var utf8Encoding = new UTF8Encoding();
        var bytes = utf8Encoding.GetBytes(name);
        var array = sha.ComputeHash(bytes);
        var stringBuilder = new StringBuilder();
        foreach (var b in array) stringBuilder.AppendFormat("{0,0:x2}", b ^ 170);
        stringBuilder.Append(".bin");
        return stringBuilder.ToString();
    }

    public static string CatGame3(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        for (var i = 0; i < bytes.Length; i++) bytes[i] = (byte) ~bytes[i];
        return CatHome(bytes, stageData, stageTop, true);
    }

    public static string MouseGame3(string str)
    {
        var data = Convert.FromBase64String(str);
        var array = MouseHomeMain(data, stageData, stageTop, true);
        if (array == null) return null;
        for (var i = 0; i < array.Length; i++) array[i] = (byte) ~array[i];
        return Encoding.UTF8.GetString(array).TrimEnd(new char[1]);
    }

    public static byte[] CatGame4(byte[] data)
    {
        for (var i = 0; i < data.Length; i += 2)
        {
            if (i + 1 >= data.Length) break;
            var b = data[i];
            var b2 = data[i + 1];
            data[i] = (byte) (b2 ^ 206);
            data[i + 1] = (byte) (b ^ 210);
        }

        return CatHomeMain(data, baseData, baseTop);
    }

    public static byte[] MouseGame4(byte[] data)
    {
        var array = MouseHomeMain(data, baseData, baseTop);
        if (array == null)
        {
            Console.WriteLine("MouseHomeMain failed");
            return null;
        }

        for (var i = 0; i < array.Length; i += 2)
        {
            if (i + 1 >= array.Length) break;
            var b = array[i];
            var b2 = array[i + 1];
            array[i] = (byte) (b2 ^ 210);
            array[i + 1] = (byte) (b ^ 206);
        }

        return array;
    }

    public static string CatGame8(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        for (var i = 0; i < bytes.Length; i++) bytes[i] = (byte) ~bytes[i];
        return CatHomeZ2(bytes, stageData, stageTop, true);
    }

    public static string MouseGame8(string str)
    {
        var data = Convert.FromBase64String(str);
        var array = MouseHomeMainZ2(data, stageData, stageTop, true);
        if (array == null)
        {
            array = MouseHomeMain(data, stageData, stageTop, true);
            if (array == null) return null;
        }

        for (var i = 0; i < array.Length; i++) array[i] = (byte) ~array[i];
        return Encoding.UTF8.GetString(array).TrimEnd(new char[1]);
    }

    public static byte[] CatHomeMain(byte[] data, byte[] home, byte[] info, bool isCompress = false)
    {
        MemoryStream memoryStream = null;
        CryptoStream cryptoStream = null;
        byte[] result;
        try
        {
            var transform = new RijndaelManaged
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256
            }.CreateEncryptor(home, info);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            if (isCompress)
            {
                var bzip2OutputStream = new BZip2OutputStream(cryptoStream, 1);
                bzip2OutputStream.Write(data, 0, data.Length);
                bzip2OutputStream.Close();
            }
            else
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
            }

            result = memoryStream.ToArray();
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

    public static byte[] CatHomeMainZ2(byte[] data, byte[] home, byte[] info, bool isCompress = false)
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
            }.CreateEncryptor(home, info);
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
            if (isCompress)
            {
                var gzipOutputStream = new GZipOutputStream(cryptoStream);
                gzipOutputStream.Write(data, 0, data.Length);
                gzipOutputStream.Close();
            }
            else
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
            }

            result = memoryStream.ToArray();
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

    public static byte[] MouseHomeMainZ2(byte[] data, byte[] home, byte[] info, bool isCompress = false)
    {
        MemoryStream memoryStream = null;
        CryptoStream cryptoStream = null;
        byte[] result;
        try
        {
            var transform = new RijndaelManaged
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256
            }.CreateDecryptor(home, info);
            var array = new byte[data.Length];
            memoryStream = new MemoryStream(data);
            cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
            cryptoStream.Read(array, 0, array.Length);
            if (isCompress)
            {
                if (array[0] == 66 && array[1] == 90) return null;
                var memoryStream2 = new MemoryStream();
                var memoryStream3 = new MemoryStream(array);
                var gzipInputStream = new GZipInputStream(memoryStream3);
                var array2 = new byte[16384];
                int count;
                while ((count = gzipInputStream.Read(array2, 0, array2.Length)) > 0)
                    memoryStream2.Write(array2, 0, count);
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

    public static byte[] MouseHomeMain(byte[] data, byte[] home, byte[] info, bool isCompress = false)
    {
        byte[] result;
        try
        {
            using (var cryptoTransform = new RijndaelManaged
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 256,
                BlockSize = 256
            }.CreateDecryptor(home, info))
            {
                using (var dataDecryptor = new DataDecryptor(cryptoTransform, data, isCompress))
                {
                    dataDecryptor.ApplyWrite();
                    result = dataDecryptor.ToByteArray();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            result = null;
        }

        return result;
    }

    public static string CatHome(byte[] data, byte[] home, byte[] info, bool isCompress = false)
    {
        var array = CatHomeMain(data, home, info, isCompress);
        return array != null ? Convert.ToBase64String(array) : null;
    }

    public static string CatHomeZ2(byte[] data, byte[] home, byte[] info, bool isCompress = false)
    {
        var array = CatHomeMainZ2(data, home, info, isCompress);
        return array != null ? Convert.ToBase64String(array) : null;
    }

    private class DataDecryptor : IDisposable
    {
        private readonly byte[] data;

        private readonly bool isCompress;

        private BZip2InputStream bzipStream;

        private CryptoStream cryptoStream;

        private bool isDisposed;

        private MemoryStream memoryStream;

        private MemoryStream memoryStreamBZip;

        public DataDecryptor(ICryptoTransform decryptor, byte[] data, bool isCompress)
        {
            this.data = data;
            this.isCompress = isCompress;
            memoryStream = new MemoryStream(data.Length);
            cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ApplyWrite()
        {
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
            if (!isCompress) return;
            memoryStream.Seek(0L, SeekOrigin.Begin);
            memoryStreamBZip = new MemoryStream();
            bzipStream = new BZip2InputStream(memoryStream);
            var array = new byte[16384];
            int count;
            while ((count = bzipStream.Read(array, 0, array.Length)) > 0) memoryStreamBZip.Write(array, 0, count);
        }

        public byte[] ToByteArray()
        {
            if (!isCompress && memoryStream != null)
                return (long) memoryStream.Capacity != memoryStream.Length
                    ? memoryStream.ToArray()
                    : memoryStream.GetBuffer();
            if (memoryStreamBZip != null)
                return (long) memoryStreamBZip.Capacity != memoryStreamBZip.Length
                    ? memoryStreamBZip.ToArray()
                    : memoryStreamBZip.GetBuffer();
            Console.Write("memoryStream is null !");
            return (byte[]) Enumerable.Empty<byte>();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposed) return;
            if (isDisposing)
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }

                if (cryptoStream != null)
                {
                    cryptoStream.Dispose();
                    cryptoStream = null;
                }

                if (memoryStreamBZip != null)
                {
                    memoryStreamBZip.Dispose();
                    memoryStreamBZip = null;
                }

                if (bzipStream != null)
                {
                    bzipStream.Dispose();
                    bzipStream = null;
                }
            }

            isDisposed = true;
        }
    }
}