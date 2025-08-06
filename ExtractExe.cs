using System.IO;
using System.Reflection;

namespace Altera
{
    internal class ExtractExe
    {
        public static void ExtractResFile(string resFileName, string outputFile)
        {
            BufferedStream inStream = null;
            FileStream outStream = null;
            try
            {
                var asm = Assembly.GetExecutingAssembly(); //读取嵌入式资源
                inStream = new BufferedStream(asm.GetManifestResourceStream(resFileName));
                outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);

                var buffer = new byte[1024];
                int length;

                while ((length = inStream.Read(buffer, 0, buffer.Length)) > 0) outStream.Write(buffer, 0, length);
                outStream.Flush();
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
        }
    }
}