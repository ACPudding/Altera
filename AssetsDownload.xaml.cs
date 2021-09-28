using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using MessageBox = HandyControl.Controls.MessageBox;

namespace Altera
{
    public partial class AssetsDownload : Window
    {
        private static readonly string path = Directory.GetCurrentDirectory();
        private static readonly DirectoryInfo folder = new DirectoryInfo(path + @"\Android\");
        private static readonly DirectoryInfo gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
        private static readonly string AssetStorageFilePath = gamedata.FullName + "AssetStorage.txt";
        private static readonly string AssetStorageLastFilePath = gamedata.FullName + "AssetStorage_last.txt";
        private static readonly DirectoryInfo AssetsFolder = new DirectoryInfo(folder + @"\assets\bin\Data\");

        public AssetsDownload()
        {
            InitializeComponent();
        }

        private void DownloadOn(object sender, RoutedEventArgs e)
        {
            var Starter = new Task(DownloadAssets);
            Starter.Start();
        }

        private void DownloadAssets()
        {
            Dispatcher.Invoke(() => { Download_Status.Items.Clear(); });
            Dispatcher.Invoke(() => { Download_Progress.Value = 0.0; });
            Dispatcher.Invoke(() => { Start.IsEnabled = false; });
            if (!AssetsFolder.Exists)
            {
                AssetsFolder.Create();
            }
            else if (!File.Exists(AssetStorageFilePath))
            {
                MessageBox.Error("未找到AssetStorage.txt文件,请通过主界面的数据更新选项卡进行下载.", "错误");
                Dispatcher.Invoke(() => { Start.IsEnabled = true; });
                Close();
            }

            var AssetTask = new Task(DownloadAssetsSub);
            var AudioTask = new Task(DownloadAudioSub);
            var MovieTask = new Task(DownloadMovieSub);
            var SpecialTask = new Task(DownloadHighAcc);
            Dispatcher.Invoke(() =>
            {
                if (Mode1.IsChecked != true)
                {
                    AssetTask.Start();
                    if (isDownloadAudio.IsChecked == true) AudioTask.Start();
                    if (isDownloadMovie.IsChecked == true) MovieTask.Start();
                    GC.Collect();
                }
                else
                {
                    SpecialTask.Start();
                    GC.Collect();
                }
            });
        }

        private void DownloadHighAcc()
        {
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASLineCount = ASLine.Length;
            var ProgressBarValueAdd = 0;
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            Dispatcher.Invoke(() =>
            {
                Download_Status.Items.Insert(0, "正在检测需要下载的文件...该模式下方进度条不可用. ");
                Download_Status.Items.Insert(0, "等下方列表长时间没有动静后即可关闭该窗口.");
            });
            if (!File.Exists(AssetStorageLastFilePath)) File.Copy(AssetStorageFilePath, AssetStorageLastFilePath);
            var ASOldLine = File.ReadAllLines(AssetStorageLastFilePath);
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            for (var i = 0; i < ASLineCount; i++)
            {
                var tmp = ASLine[i].Split(',');
                if (tmp.Length != 5) continue;
                var downloadName = "";
                Parallel.ForEach(ASOldLine, paralleloptions, ASOldItem =>
                {
                    var tmpold = ASOldItem.Split(',');
                    if (tmpold.Length != 5) return;
                    if (tmpold[4] != tmp[4]) return;
                    if (tmpold[2] == tmp[2] && tmpold[3] == tmp[3]) return;
                    if (tmp[4].Contains("Audio") || tmp[4].Contains("Movie"))
                    {
                        downloadName = tmp[4].Replace('/', '_');
                        Dispatcher.Invoke(() => { Download_Status.Items.Insert(0, "差异: " + tmp[4]); });
                        var downloadfile = downloadName;
                        var writePath = AssetsFolder.FullName + tmp[4].Replace("/", "\\");
                        var writeDirectory = Path.GetDirectoryName(writePath);
                        if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                        File.Delete(writePath);
                        var SubTask = new Task(() =>
                        {
                            DownloadAssetsSpecialSub(assetBundleFolder, downloadfile, writePath, tmp[4],
                                ProgressBarValueAdd);
                        });
                        SubTask.Start();
                    }
                    else
                    {
                        var tmpname = tmp[4].Replace('/', '@') + ".unity3d";
                        downloadName = CatAndMouseGame.GetShaName(tmpname);
                        Dispatcher.Invoke(() => { Download_Status.Items.Insert(0, "差异: " + tmpname); });
                        var downloadfile = downloadName;
                        var writePath = AssetsFolder.FullName + tmpname.Replace('@', '\\').Replace("/", "\\");
                        var writeDirectory = Path.GetDirectoryName(writePath);
                        if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                        File.Delete(writePath);
                        var SubTask = new Task(() =>
                        {
                            DownloadAssetsSpecialSub(assetBundleFolder, downloadfile, writePath, tmpname,
                                ProgressBarValueAdd);
                        });
                        SubTask.Start();
                    }
                });
                GC.Collect();
            }

            Dispatcher.Invoke(() => { Start.IsEnabled = true; });
            GC.Collect();
        }

        private void DownloadAssetsSpecialSub(string assetBundleFolder, string filename, string writePath, string names,
            int ProgressBarValueAdd)
        {
            try
            {
                var raw = HttpRequest
                    .Get($"https://cdn.data.fate-go.jp/AssetStorages/{assetBundleFolder}Android/{filename}")
                    .ToBinary();
                var output = writePath.Contains("unity3d") ? CatAndMouseGame.MouseGame4(raw) : raw;
                using (var fs = new FileStream(writePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(output, 0, output.Length);
                }

                Dispatcher.Invoke(() =>
                {
                    Download_Status.Items.Insert(0, "下载: " + names);
                    Download_Progress.Value += ProgressBarValueAdd;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    Download_Status.Items.Insert(0, "下载错误: " + names);
                    Download_Status.Items.Insert(0, ex);
                });
            }
        }

        private void DownloadAssetsSub()
        {
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASLineCount = ASLine.Length;
            var ProgressBarValueAdd = Convert.ToInt32(50000 / ASLineCount);
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            var assetList = JArray.Parse(File.ReadAllText(gamedata.FullName + "AssetName.json"));
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            Parallel.ForEach(assetList, paralleloptions, asset =>
            {
                Dispatcher.Invoke(() =>
                {
                    var filename = asset["fileName"].ToString();
                    var assetName = asset["assetName"].ToString();
                    if (!assetName.EndsWith(".unity3d")) return;
                    var writePath = AssetsFolder.FullName;
                    var names = assetName.Split('@');
                    if (names.Length > 1)
                    {
                        writePath += string.Join(@"\", names);
                        var writeDirectory = Path.GetDirectoryName(writePath);
                        if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                    }
                    else
                    {
                        writePath = AssetsFolder.FullName + assetName;
                    }

                    if (File.Exists(writePath))
                    {
                        if (Mode2.IsChecked == true)
                        {
                            Download_Status.Items.Insert(0, "跳过: " + $"{string.Join(@"\", names)}");
                            Download_Progress.Value += ProgressBarValueAdd;
                            return;
                        }

                        File.Delete(writePath);
                    }

                    var SubTask = new Task(() =>
                    {
                        DownloadAssetSub1(assetBundleFolder, filename, writePath, names, ProgressBarValueAdd);
                    });
                    SubTask.Start();
                });
            });
            Dispatcher.Invoke(() => { Start.IsEnabled = true; });
            GC.Collect();
        }

        private void DownloadAudioSub()
        {
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASLineCount = ASLine.Length;
            var ProgressBarValueAdd = Convert.ToInt32(50000 / ASLineCount);
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            var audioList = JArray.Parse(File.ReadAllText(gamedata.FullName + "AudioName.json"));
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            Dispatcher.Invoke(() =>
            {
                if (isDownloadAudio.IsChecked == true)
                    Parallel.ForEach(audioList, paralleloptions, audio =>
                    {
                        var audioName = audio["audioName"].ToString();
                        var writePath = AssetsFolder.FullName;
                        var names = audioName.Split('@');
                        if (names.Length > 1)
                        {
                            writePath += string.Join(@"\", names);
                            var writeDirectory = Path.GetDirectoryName(writePath);
                            if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                        }
                        else
                        {
                            writePath = AssetsFolder.FullName + audioName;
                        }

                        if (File.Exists(writePath))
                        {
                            if (Mode2.IsChecked == true)
                            {
                                Download_Status.Items.Insert(0, "跳过: " + $"{string.Join(@"\", names)}");
                                Download_Progress.Value += ProgressBarValueAdd;
                                return;
                            }

                            File.Delete(writePath);
                        }

                        var realAudioDownloadName = audioName.Replace("@", "_");
                        var SubTask2 = new Task(() =>
                        {
                            DownloadAssetSub2(assetBundleFolder, realAudioDownloadName, writePath, names,
                                ProgressBarValueAdd);
                        });
                        SubTask2.Start();
                    });
            });
            GC.Collect();
        }

        private void DownloadMovieSub()
        {
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASLineCount = ASLine.Length;
            var ProgressBarValueAdd = Convert.ToInt32(50000 / ASLineCount);
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            var movieList = JArray.Parse(File.ReadAllText(gamedata.FullName + "MovieName.json"));
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            Dispatcher.Invoke(() =>
            {
                if (isDownloadMovie.IsChecked == true)
                    Parallel.ForEach(movieList, paralleloptions, movie =>
                    {
                        var movieName = movie["movieName"].ToString();
                        var writePath = AssetsFolder.FullName;
                        var names = movieName.Split('@');
                        if (names.Length > 1)
                        {
                            writePath += string.Join(@"\", names);
                            var writeDirectory = Path.GetDirectoryName(writePath);
                            if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                        }
                        else
                        {
                            writePath = AssetsFolder.FullName + movieName;
                        }

                        if (File.Exists(writePath))
                        {
                            if (Mode2.IsChecked == true)
                            {
                                Download_Status.Items.Insert(0, "跳过: " + $"{string.Join(@"\", names)}");
                                Download_Progress.Value += ProgressBarValueAdd;
                                return;
                            }

                            File.Delete(writePath);
                        }

                        var realAudioDownloadName = movieName.Replace("@", "_");
                        var SubTask2 = new Task(() =>
                        {
                            DownloadAssetSub2(assetBundleFolder, realAudioDownloadName, writePath, names,
                                ProgressBarValueAdd);
                        });
                        SubTask2.Start();
                    });
            });
            GC.Collect();
        }

        private void DownloadAssetSub1(string assetBundleFolder, string filename, string writePath, string[] names,
            int ProgressBarValueAdd)
        {
            try
            {
                var raw = HttpRequest
                    .Get($"https://cdn.data.fate-go.jp/AssetStorages/{assetBundleFolder}Android/{filename}")
                    .ToBinary();
                var output = CatAndMouseGame.MouseGame4(raw);
                using (var fs = new FileStream(writePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(output, 0, output.Length);
                }

                Dispatcher.Invoke(() =>
                {
                    Download_Status.Items.Insert(0, "下载: " + $"{string.Join(@"\", names)}");
                    Download_Progress.Value += ProgressBarValueAdd;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    Download_Status.Items.Insert(0, "下载错误: " + $"{string.Join(@"\", names)}");
                    Download_Progress.Value += ProgressBarValueAdd;
                    Download_Status.Items.Insert(0, ex);
                });
            }
        }

        private void DownloadAssetSub2(string assetBundleFolder, string filename, string writePath, string[] names,
            int ProgressBarValueAdd)
        {
            try
            {
                var raw = HttpRequest
                    .Get($"https://cdn.data.fate-go.jp/AssetStorages/{assetBundleFolder}Android/{filename}")
                    .ToBinary();
                var output = raw;
                using (var fs = new FileStream(writePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(output, 0, output.Length);
                }

                Dispatcher.Invoke(() =>
                {
                    Download_Status.Items.Insert(0, "下载: " + $"{string.Join(@"\", names)}");
                    Download_Progress.Value += ProgressBarValueAdd;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    Download_Status.Items.Insert(0, "下载错误: " + $"{string.Join(@"\", names)}");
                    Download_Status.Items.Insert(0, ex);
                });
            }
        }
    }
}