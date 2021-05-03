using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DereTore.Exchange.Audio.HCA;
using VGMToolbox.format;
using static System.String;

namespace Altera
{
    public static class FGOAudioDecoder
    {
        public static string UnpackCpkFiles(FileInfo filename, DirectoryInfo AudioFolder)
        {
            var cpk_name = filename.FullName;
            var cpk = new CPK(new Tools());
            cpk.ReadCPK(cpk_name);
            var oldFile = new BinaryReader(File.OpenRead(cpk_name));
            List<FileEntry> entries = null;
            entries = cpk.FileTable.Where(x => x.FileType == "FILE").ToList();
            if (entries.Count == 0) return "";
            var filefullname = "";
            foreach (var t in entries)
            {
                if (!IsNullOrEmpty((string) t.DirName)) Directory.CreateDirectory(t.DirName.ToString());

                oldFile.BaseStream.Seek((long) t.FileOffset, SeekOrigin.Begin);
                var isComp = Encoding.ASCII.GetString(oldFile.ReadBytes(8));
                oldFile.BaseStream.Seek((long) t.FileOffset, SeekOrigin.Begin);

                var chunk = oldFile.ReadBytes(int.Parse(t.FileSize.ToString()));
                if (isComp == "CRILAYLA")
                {
                    var size = int.Parse((t.ExtractSize ?? t.FileSize).ToString());
                    chunk = cpk.DecompressCRILAYLA(chunk, size);
                }

                File.WriteAllBytes(AudioFolder.FullName + @"\" + t.FileName, chunk);
                if (t.FileName.ToString().Contains(".acb"))
                    filefullname = AudioFolder.FullName + @"\" + t.FileName;
            }

            oldFile.Close();
            return filefullname;
        }

        public static void DecodeAcbFiles(FileInfo filename, DirectoryInfo AudioFolder)
        {
            var volume = 1F;
            var mode = 16;
            var loop = 0;
            var ciphKey1 = 0x92EBF464;
            uint ciphKey2 = 0x7E896318;
            var dir = AudioFolder;
            var dir2 = new DirectoryInfo(AudioFolder.FullName + @"\DecodedWavs\");
            var acbfile = filename;
            var fs = new FileStream(acbfile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var af = new CriAcbFile(fs, 0, false);
            af.ExtractAll();
            fs.Close();
            var destinationFolder = new DirectoryInfo(Path.Combine(acbfile.DirectoryName,
                "_vgmt_acb_ext_" + Path.GetFileNameWithoutExtension(acbfile.FullName)));
            var OutFolder =
                Path.Combine(Path.GetDirectoryName(acbfile.FullName.Replace(dir.FullName, dir2.FullName)),
                    Path.GetFileNameWithoutExtension(acbfile.FullName));
            Directory.CreateDirectory(OutFolder);

            Parallel.ForEach(destinationFolder.GetFiles("*.hca", SearchOption.AllDirectories), hcafile =>
            {
                using (var inputFileStream = File.Open(hcafile.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var outputFileStream =
                        File.Open(OutFolder + @"\" + hcafile.Name.Substring(0, hcafile.Name.Length - 4) + ".wav",
                            FileMode.Create, FileAccess.Write))
                    {
                        var decodeParams = DecodeParams.CreateDefault();
                        decodeParams.Key1 = ciphKey1;
                        decodeParams.Key2 = ciphKey2;
                        decodeParams.KeyModifier = 0;

                        var audioParams = AudioParams.CreateDefault();

                        audioParams.InfiniteLoop = AudioParams.Default.InfiniteLoop;
                        audioParams.SimulatedLoopCount = AudioParams.Default.SimulatedLoopCount;
                        audioParams.OutputWaveHeader = true;

                        using (var hcaStream = new HcaAudioStream(inputFileStream, decodeParams, audioParams))
                        {
                            var read = 1;
                            var dataBuffer = new byte[1024];

                            while (read > 0)
                            {
                                read = hcaStream.Read(dataBuffer, 0, dataBuffer.Length);

                                if (read > 0) outputFileStream.Write(dataBuffer, 0, read);
                            }
                        }
                    }
                }

                File.Delete(hcafile.FullName);
            });
            var awbfilename = acbfile.FullName.Substring(0, acbfile.FullName.Length - 4) + ".awb";
            File.Delete(acbfile.FullName);
            File.Delete(awbfilename);
            Directory.Delete(destinationFolder.FullName, true);
        }
    }
}