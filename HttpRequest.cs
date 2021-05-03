using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace Altera
{
    internal class HttpRequest
    {
        public static string PhttpReq(string url, string parameters)
        {
            var hRequest = (HttpWebRequest) WebRequest.Create(url);
            hRequest.CookieContainer = new CookieContainer();

            hRequest.Accept = "gzip, identity";
            hRequest.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 6.0.1; MI 6 Build/V417IR)";
            hRequest.ServicePoint.Expect100Continue = false;
            hRequest.KeepAlive = true;
            hRequest.Method = "POST";

            hRequest.ContentType = "application/x-www-form-urlencoded";

            hRequest.ContentLength = parameters.Length;

            var dataParsed = Encoding.UTF8.GetBytes(parameters);
            hRequest.GetRequestStream().Write(dataParsed, 0, dataParsed.Length);


            hRequest.Timeout = 5 * 1000;

            var response = (HttpWebResponse) hRequest.GetResponse();

            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public static Stream GetXlsx()
        {
            var xlsxurl1 = "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/SvtInfo.xlsx";
            var xlsxurl2 =
                "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/SvtInfo.xlsx";
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(xlsxurl2);
            httpWebRequest.Method = "GET";
            try
            {
                var response = httpWebRequest.GetResponse();
                var stream = response.GetResponseStream();
                return stream;
            }
            catch (Exception)
            {
                httpWebRequest =
                    (HttpWebRequest) WebRequest.Create(xlsxurl1);
                httpWebRequest.Method = "GET";
                try
                {
                    var response2 = httpWebRequest.GetResponse();
                    var stream2 = response2.GetResponseStream();
                    return stream2;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("网络连接异常,请检查网络连接并重试.\r\n" + exception, "网络连接异常", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    throw;
                }
            }
        }

        public static string GetList(string ALink, string Blink)
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(ALink);
            httpWebRequest.Method = "GET";
            try
            {
                var response = httpWebRequest.GetResponse();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                httpWebRequest =
                    (HttpWebRequest) WebRequest.Create(Blink);
                httpWebRequest.Method = "GET";
                try
                {
                    var response2 = httpWebRequest.GetResponse();
                    var stream2 = response2.GetResponseStream();
                    var reader2 = new StreamReader(stream2, Encoding.UTF8);
                    return reader2.ReadToEnd();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("网络连接异常,请检查网络连接并重试.\r\n" + exception, "网络连接异常", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    throw;
                }
            }
        }

        public static string GetApplicationUpdateJson()
        {
            var api = "https://api.github.com/repos/ACPudding/Altera/releases/latest";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072; //TLS1.2=3702
            var result = "";
            var req = WebRequest.Create(api) as HttpWebRequest;
            HttpWebResponse res = null;
            if (req == null) return result;
            req.Method = "GET";
            req.ContentType = @"application/octet-stream";
            req.UserAgent =
                @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
            var postData = Encoding.GetEncoding("UTF-8").GetBytes("");
            if (postData.Length > 0)
            {
                req.ContentLength = postData.Length;
                req.Timeout = 15000;
                var outputStream = req.GetRequestStream();
                outputStream.Write(postData, 0, postData.Length);
                outputStream.Flush();
                outputStream.Close();
                try
                {
                    res = (HttpWebResponse) req.GetResponse();
                    var InputStream = res.GetResponseStream();
                    var encoding = Encoding.GetEncoding("UTF-8");
                    var sr = new StreamReader(InputStream, encoding);
                    result = sr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return result;
                }
            }
            else
            {
                try
                {
                    res = (HttpWebResponse) req.GetResponse();
                    var InputStream = res.GetResponseStream();
                    var encoding = Encoding.GetEncoding("UTF-8");
                    var sr = new StreamReader(InputStream, encoding);
                    result = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return result;
                }
            }

            return result;
        }

        public static string ReadableFilesize(double size)
        {
            string[] units = {"B", "KB", "MB", "GB", "TB", "PB"};
            var mod = 1024.0;
            var i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }

            return Math.Round(size) + units[i];
        }
    }
}