using System;
using System.Collections.Generic;
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
        private static string[,] tmp;
        private static string[,] tmpold;
        private static object LockedList = new object();

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

            Dispatcher.Invoke(async () =>
            {
                if (Mode1.IsChecked != true)
                {
                    await Task.Run(DownloadAssetsSub).ConfigureAwait(false);
                    if (isDownloadAudio.IsChecked == true) await Task.Run(DownloadAudioSub).ConfigureAwait(false);
                    if (isDownloadMovie.IsChecked == true) await Task.Run(DownloadMovieSub).ConfigureAwait(false);
                    GC.Collect();
                }
                else
                {
                    await Task.Run(DownloadHighAcc).ConfigureAwait(false);
                    GC.Collect();
                }
            });
        }

        private async Task DHASub(int[] DownloadLine)
        {
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            var ProgressBarValueAdd = 50000 / DownloadLine.Length;
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            Parallel.ForEach(DownloadLine, paralleloptions, async DownloadItem =>
            {
                if (tmp[DownloadItem, 4].Contains("Audio") || tmp[DownloadItem, 4].Contains("Movie"))
                {
                    var downloadName = tmp[DownloadItem, 4].Replace('/', '_');
                    var downloadfile = downloadName;
                    var writePath = AssetsFolder.FullName + tmp[DownloadItem, 4].Replace("/", "\\");
                    var writeDirectory = Path.GetDirectoryName(writePath);
                    if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                    File.Delete(writePath);
                    await Task.Run(() =>
                    {
                        DownloadAssetsSpecialSub(assetBundleFolder, downloadfile, writePath, tmp[DownloadItem, 4],
                            ProgressBarValueAdd);
                    }).ConfigureAwait(false);
                }
                else
                {
                    var tmpname = tmp[DownloadItem, 4].Replace('/', '@') + ".unity3d";
                    var downloadName = CatAndMouseGame.GetShaName(tmpname);
                    var downloadfile = downloadName;
                    var writePath = AssetsFolder.FullName + tmpname.Replace('@', '\\').Replace("/", "\\");
                    var writeDirectory = Path.GetDirectoryName(writePath);
                    if (!Directory.Exists(writeDirectory)) Directory.CreateDirectory(writeDirectory);
                    File.Delete(writePath);
                    await Task.Run(() =>
                    {
                        DownloadAssetsSpecialSub(assetBundleFolder, downloadfile, writePath, tmp[DownloadItem, 4],
                            ProgressBarValueAdd);
                    }).ConfigureAwait(false);
                }
            });
        }

        private async Task<List<int>> FindASDiffer(int min, int max)
        {
            var resultlist = new List<int>();
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASOldLine = File.ReadAllLines(AssetStorageLastFilePath);
            tmpold = new string[ASOldLine.Length, 5];
            tmp = new string [ASLine.Length, 5];
            for (var kk = 0; kk < ASLine.Length; kk++)
            {
                var tmpkk = ASLine[kk].Split(',');
                if (tmpkk.Length != 5)
                {
                    tmp[kk, 0] = "0";
                    tmp[kk, 1] = "0";
                    tmp[kk, 2] = "0";
                    tmp[kk, 3] = "0";
                    tmp[kk, 4] = "0";
                    continue;
                }

                tmp[kk, 0] = tmpkk[0];
                tmp[kk, 1] = tmpkk[1];
                tmp[kk, 2] = tmpkk[2];
                tmp[kk, 3] = tmpkk[3];
                tmp[kk, 4] = tmpkk[4];
            }

            for (var jj = 0; jj < ASOldLine.Length; jj++)
            {
                var tmpkk = ASOldLine[jj].Split(',');
                if (tmpkk.Length != 5)
                {
                    tmpold[jj, 0] = "0";
                    tmpold[jj, 1] = "0";
                    tmpold[jj, 2] = "0";
                    tmpold[jj, 3] = "0";
                    tmpold[jj, 4] = "0";
                    continue;
                }

                tmpold[jj, 0] = tmpkk[0];
                tmpold[jj, 1] = tmpkk[1];
                tmpold[jj, 2] = tmpkk[2];
                tmpold[jj, 3] = tmpkk[3];
                tmpold[jj, 4] = tmpkk[4];
            }

            for (var i = max - 1; i >= min; i--)
            {
                if (tmp[i, 0] == "0") continue;
                try
                {
                    resultlist.Add(await FindASDifferNiceSub(tmp[i, 4], tmp[i, 2], tmp[i, 3]));
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            try
            {
                resultlist.RemoveAll(s => s == 0);
            }
            catch (Exception)
            {
                //ignore
            }

            GC.Collect();
            return resultlist;
        }

        private async Task<int> FindASDifferNiceSub(string FindStr, string check1, string check2)
        {
            var value = 0;
            Parallel.For(0, tmpold.GetLength(0), j =>
            {
                if (tmpold[j, 0] == "0") return;
                if (tmpold[j, 4] != FindStr) return;
                if (tmpold[j, 2] == check1 && tmpold[j, 3] == check2)
                {
                    _ = Dispatcher.InvokeAsync(() => { Download_Status.Items.Insert(0, "跳过: " + tmpold[j, 4]); });
                    return;
                }

                value = j;
                _ = Dispatcher.InvokeAsync(() => { Download_Status.Items.Insert(0, "差异: " + tmpold[j, 4]); });
            });
            return value;
        }

        private async void DownloadHighAcc()
        {
            _ = Dispatcher.InvokeAsync(() =>
            {
                Download_Status.Items.Insert(0, "正在检测需要下载的文件... ");
                Download_Progress.Value = 0;
            });
            await Task.Delay(2000);
            var ASLineCount = File.ReadAllLines(AssetStorageFilePath).Length;
            var n = ASLineCount / 8;
            var mod = ASLineCount % 8;
            var task1 = FindASDiffer(0, n);
            var task2 = FindASDiffer(n, 2 * n);
            var task3 = FindASDiffer(2 * n, 3 * n);
            var task4 = FindASDiffer(3 * n, 4 * n);
            var task5 = FindASDiffer(4 * n, 5 * n);
            var task6 = FindASDiffer(5 * n, 6 * n);
            var task7 = FindASDiffer(6 * n, 7 * n);
            var task8 = FindASDiffer(7 * n, 8 * n + mod);
            var DownloadLinePart1List = new List<int>();
            var DownloadLinePart2List = new List<int>();
            var DownloadLinePart3List = new List<int>();
            var DownloadLinePart4List = new List<int>();
            var DownloadLinePart5List = new List<int>();
            var DownloadLinePart6List = new List<int>();
            var DownloadLinePart7List = new List<int>();
            var DownloadLinePart8List = new List<int>();
            var actions = new Action[]
            {
                async () => { DownloadLinePart1List.AddRange(await task1); },
                async () => { DownloadLinePart2List.AddRange(await task2); },
                async () => { DownloadLinePart3List.AddRange(await task3); },
                async () => { DownloadLinePart4List.AddRange(await task4); },
                async () => { DownloadLinePart5List.AddRange(await task5); },
                async () => { DownloadLinePart6List.AddRange(await task6); },
                async () => { DownloadLinePart7List.AddRange(await task7); },
                async () => { DownloadLinePart8List.AddRange(await task8); }
            };
            Parallel.Invoke(actions);
            var DownloadLine = new List<int>();
            DownloadLine.AddRange(DownloadLinePart1List);
            DownloadLine.AddRange(DownloadLinePart2List);
            DownloadLine.AddRange(DownloadLinePart3List);
            DownloadLine.AddRange(DownloadLinePart4List);
            DownloadLine.AddRange(DownloadLinePart5List);
            DownloadLine.AddRange(DownloadLinePart6List);
            DownloadLine.AddRange(DownloadLinePart7List);
            DownloadLine.AddRange(DownloadLinePart8List);
            _ = Dispatcher.InvokeAsync(() =>
            {
                Download_Status.Items.Clear();
                Download_Status.Items.Insert(0, "核对完成,准备下载... ");
                Download_Progress.Value = 0;
            });
            await Task.Delay(2000);
            var DownloadLineArray = DownloadLine.ToArray();
            await Task.Run(() => { _ = DHASub(DownloadLineArray); }).ConfigureAwait(false);
            _ = Dispatcher.InvokeAsync(() =>
            {
                Download_Status.Items.Insert(0, "共需下载" + DownloadLineArray.Length + "个差异文件.");
                Download_Status.Items.Insert(0, "下载按钮将不再可用,如需再次点击请关闭窗口后再打开.");
            });
            await Task.Delay(2000);
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

                _ = Dispatcher.InvokeAsync(() =>
                {
                    Download_Status.Items.Insert(0, "下载: " + names);
                    Download_Progress.Value += ProgressBarValueAdd;
                });
            }
            catch (Exception ex)
            {
                _ = Dispatcher.InvokeAsync(() =>
                {
                    Download_Status.Items.Insert(0, "下载错误: " + names);
                    Download_Status.Items.Insert(0, ex);
                });
            }
        }

        private async void DownloadAssetsSub()
        {
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASLineCount = ASLine.Length;
            var ProgressBarValueAdd = Convert.ToInt32(50000 / ASLineCount);
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            var assetList = JArray.Parse(File.ReadAllText(gamedata.FullName + "AssetName.json"));
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            Parallel.ForEach(assetList, paralleloptions, asset =>
            {
                _ = Dispatcher.InvokeAsync(async () =>
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

                    await Task.Run(() =>
                    {
                        DownloadAssetSub1(assetBundleFolder, filename, writePath, names, ProgressBarValueAdd);
                    }).ConfigureAwait(false);
                });
            });
            _ = Dispatcher.InvokeAsync(() => { Start.IsEnabled = true; });
            GC.Collect();
        }

        private async void DownloadAudioSub()
        {
            var ASLine = File.ReadAllLines(AssetStorageFilePath);
            var ASLineCount = ASLine.Length;
            var ProgressBarValueAdd = Convert.ToInt32(50000 / ASLineCount);
            var assetBundleFolder = File.ReadAllText(gamedata.FullName + "assetBundleFolder.txt");
            var audioList = JArray.Parse(File.ReadAllText(gamedata.FullName + "AudioName.json"));
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
            _ = Dispatcher.InvokeAsync(() =>
            {
                if (isDownloadAudio.IsChecked == true)
                    Parallel.ForEach(audioList, paralleloptions, async audio =>
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
                        await Task.Run(() =>
                        {
                            DownloadAssetSub2(assetBundleFolder, realAudioDownloadName, writePath, names,
                                ProgressBarValueAdd);
                        }).ConfigureAwait(false);
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
            var paralleloptions = new ParallelOptions {MaxDegreeOfParallelism = 3};
            Dispatcher.InvokeAsync(() =>
            {
                if (isDownloadMovie.IsChecked == true)
                    Parallel.ForEach(movieList, paralleloptions, async movie =>
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
                        await Task.Run(() =>
                        {
                            DownloadAssetSub2(assetBundleFolder, realAudioDownloadName, writePath, names,
                                ProgressBarValueAdd);
                        }).ConfigureAwait(false);
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

                _ = Dispatcher.InvokeAsync(() =>
                {
                    Download_Status.Items.Insert(0, "下载: " + $"{string.Join(@"\", names)}");
                    Download_Progress.Value += ProgressBarValueAdd;
                });
            }
            catch (Exception ex)
            {
                _ = Dispatcher.InvokeAsync(() =>
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

                _ = Dispatcher.InvokeAsync(() =>
                {
                    Download_Status.Items.Insert(0, "下载: " + $"{string.Join(@"\", names)}");
                    Download_Progress.Value += ProgressBarValueAdd;
                });
            }
            catch (Exception ex)
            {
                _ = Dispatcher.InvokeAsync(() =>
                {
                    Download_Status.Items.Insert(0, "下载错误: " + $"{string.Join(@"\", names)}");
                    Download_Status.Items.Insert(0, ex);
                });
            }
        }
    }
}