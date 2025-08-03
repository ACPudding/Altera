using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Altera.Properties;
using HandyControl.Controls;
using LiveCharts.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Color = System.Drawing.Color;
using FontFamily = System.Windows.Media.FontFamily;
using MessageBox = HandyControl.Controls.MessageBox;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using TextBox = System.Windows.Controls.TextBox;

namespace Altera
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private const string V =
            "\r\n--------------------------------------------------\r\n{注:以下文本为满足相应条件之后显示的新牵绊文本.}\r\n--------------------------------------------------\r\n";

        private static string GameDataVersion;

        private static readonly string BuffTranslationListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/BuffTranslation.json";

        private static readonly string BuffTranslationListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/BuffTranslation.json";

        private static readonly string IndividualListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/SvtIndividualityTranslation.json";

        private static readonly string IndividualListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/SvtIndividualityTranslation.json";

        private static readonly string TDAttackNameTranslationListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/TDAttackNameTranslation.json";

        private static readonly string TDAttackNameTranslationListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/TDAttackNameTranslation.json";

        private static readonly string FuncListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/FuncList.json";

        private static readonly string FuncListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/FuncList.json";

        private static readonly string AppendSkillTranslationLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/AppendSkillTranslation.json";

        private static readonly string AppendSkillTranslationLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/AppendSkillTranslation.json";

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public string[] LabelX { get; set; }
        public int[] LineHP { get; set; }
        public int[] LineATK { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var Font1 = new FontFamily("FOT-Matisse Pro B");
            var Font2 = new FontFamily("FOT-Skip Std B");
            Button1.IsEnabled = false;
            textbox1.Text = Regex.Replace(textbox1.Text, @"\s", "");
            var ES = new Task(() => { EasternEggSvt(); });
            if (textbox1.Text == "ACPD" || textbox1.Text == "acpd")
            {
                ClearTexts();
                ES.Start();
                Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                FocusBasicInfo.IsSelected = true;
                return;
            }

            if (!Regex.IsMatch(textbox1.Text, "^\\d+$"))
            {
                MessageBox.Error("从者ID输入错误,请检查.", "温馨提示:");
                ClearTexts();
                Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                return;
            }

            if (ToggleTDSpecialFont.IsChecked == true)
            {
                npruby.FontFamily = Font1;
                npname.FontFamily = Font1;
            }
            else
            {
                npruby.FontFamily = Font2;
                npname.FontFamily = Font2;
            }

            var SA = new Task(StartAnalyze);
            SA.Start();
            SkillLvs.ClassPassiveforExcel = "";
            FocusBasicInfo.IsSelected = true;
        }

        private void StartAnalyze()
        {
            var svtID = "";
            var svtTDID = "";
            var SCAC = new Task(ServantCardsArrangementCheck);
            var SBIC = new Task(ServantBasicInformationCheck);
            var SCIC = new Task(ServantCVandIllustCheck);
            var SJTC = new Task(ServantJibanTextCheck);
            var STDI = new Task(() => { ServantTreasureDeviceInformationCheck(svtTDID); });
            var SSIC = new Task(() => { ServantSkillInformationCheck(svtID); });
            var SCLIC = new Task(ServantCombineLimitItemsCheck);
            var SCSIC = new Task(ServantCombineSkillItemsCheck);
            var SASC = new Task(() => { SvtAppendSkillCheck(svtID); });
            SkillLvs.skillID1 = "";
            SkillLvs.skillID2 = "";
            SkillLvs.skillID3 = "";
            IsNPStrengthened.Dispatcher.Invoke(() => { IsNPStrengthened.Text = "×"; });
            textbox1.Dispatcher.Invoke(() => { svtID = Convert.ToString(textbox1.Text); });
            JB.svtid = svtID;
            JB.JB1 = "";
            JB.JB2 = "";
            JB.JB3 = "";
            JB.JB4 = "";
            JB.JB5 = "";
            JB.JB6 = "";
            JB.JB7 = "";
            JB.JB1tmp = "";
            JB.JB2tmp = "";
            JB.JB3tmp = "";
            JB.JB4tmp = "";
            JB.JB5tmp = "";
            JB.JB6tmp = "";
            JB.JB7tmp = "";
            ClearTexts();
            RefreshTranslationsList();
            var TDStringBar = GetSvtTDID(svtID);
            if (TDStringBar[1] == "true") IsNPStrengthened.Dispatcher.Invoke(() => { IsNPStrengthened.Text = "▲"; });
            if (TDStringBar[1] == "truetrue")
                IsNPStrengthened.Dispatcher.Invoke(() => { IsNPStrengthened.Text = "▲▲"; });
            svtTDID = TDStringBar[0];
            textbox1.Dispatcher.Invoke(() => { textbox1.Text = svtID; });
            SCAC.Start();
            SBIC.Start();
            SCIC.Start();
            SJTC.Start();
            SCLIC.Start();
            SCSIC.Start();
            STDI.Start();
            SASC.Start();
            Task.WaitAll(SCLIC, STDI, SBIC);
            var STDSC = new Task(() => { ServantTreasureDeviceSvalCheck(svtTDID); });
            STDSC.Start();
            SSIC.Start();
            Task.WaitAll(STDSC, SSIC);
            AskForExcelOutput();
            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
            Dispatcher.Invoke(() =>
            {
                if (rarity.Text == "")
                {
                    MessageBox.Error("从者ID不存在或未实装,请重试.", "温馨提示:");
                    ClearTexts();
                    Button1.IsEnabled = true;
                    StarterItemTab.IsSelected = true;
                    return;
                }

                if (cards.Text == "[Q,Q,Q,Q,Q]" && svtclass.Text != "礼装")
                    Growl.Info("此ID为小怪(或部分boss以及种火芙芙),配卡、技能、宝具信息解析并不准确,请知悉.");
            });
            GC.Collect();
        }

        private void AskForExcelOutput()
        {
            Dispatcher.Invoke(() =>
            {
                switch (GlobalPathsAndDatas.classid)
                {
                    case 1:
                    case 4:
                    case 8:
                    case 10:
                    case 20:
                    case 22:
                    case 24:
                    case 26:
                    case 23:
                    case 25:
                    case 17:
                    case 28:
                        if (ToggleMsgboxOutputCheck.IsChecked != true || !GlobalPathsAndDatas.askxlsx) return;
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "是否需要以xlsx的形式导出该从者的基础数据?",
                                "导出?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            ExcelFileOutput();
                        break;
                    case 3:
                        if (ToggleMsgboxOutputCheck.IsChecked != true || !GlobalPathsAndDatas.askxlsx) return;
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "是否需要以xlsx的形式导出该从者的基础数据?",
                                "导出?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            ExcelFileOutput();
                        break;
                    case 5:
                    case 6:
                        if (ToggleMsgboxOutputCheck.IsChecked != true || !GlobalPathsAndDatas.askxlsx) return;
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "是否需要以xlsx的形式导出该从者的基础数据?",
                                "导出?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            ExcelFileOutput();
                        break;
                    case 2:
                        if (ToggleMsgboxOutputCheck.IsChecked != true || !GlobalPathsAndDatas.askxlsx) return;
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "是否需要以xlsx的形式导出该从者的基础数据?",
                                "导出?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            ExcelFileOutput();
                        break;
                    case 7:
                    case 9:
                    case 11:
                        if (ToggleMsgboxOutputCheck.IsChecked != true || !GlobalPathsAndDatas.askxlsx) return;
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "是否需要以xlsx的形式导出该从者的基础数据?",
                                "导出?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            ExcelFileOutput();
                        break;
                    case 1001:
                        Growl.Info("此ID为礼装ID,图鉴编号为礼装的图鉴编号.礼装描述在牵绊文本的文本1处.");
                        break;
                    default:
                        if (ToggleMsgboxOutputCheck.IsChecked != true || !GlobalPathsAndDatas.askxlsx) return;
                        if (cards.Text == "[Q,Q,Q,Q,Q]" && svtclass.Text != "礼装") return;
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "是否需要以xlsx的形式导出该从者的基础数据?",
                                "导出?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            ExcelFileOutput();
                        break;
                }
            });
        }

        private string[] GetSvtTDID(string svtID)
        {
            var svtTDID = "";
            var isNPStrengthen = "false";
            var result = new string[2];
            result[0] = svtTDID;
            result[1] = isNPStrengthen;
            GlobalPathsAndDatas.IDListStr = "";
            try
            {
                foreach (var svtTreasureDevicestmp in GlobalPathsAndDatas.mstSvtTreasureDevicedArray)
                {
                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "101")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "102")
                        switch (((JObject)svtTreasureDevicestmp)["condQuestId"].ToString().Substring(0, 1))
                        {
                            case "9":
                            {
                                var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                                svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                                isNPStrengthen = "true";
                                break;
                            }
                            case "0":
                            {
                                var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                                svtTDID += "*" + mstsvtTDobjtmp["treasureDeviceId"] + "^TD";
                                break;
                            }
                            case "3":
                            {
                                var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                                svtTDID += "*" + mstsvtTDobjtmp["treasureDeviceId"] + "^TD";
                                break;
                            }
                        }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "103")
                        switch (((JObject)svtTreasureDevicestmp)["condQuestId"].ToString().Substring(0, 1))
                        {
                            case "9":
                            {
                                var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                                svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                                isNPStrengthen = "truetrue";
                                break;
                            }
                            case "2":
                            {
                                var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                                svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                                isNPStrengthen = "true";
                                break;
                            }
                        }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "104")
                    {
                        if (((JObject)svtTreasureDevicestmp)["condQuestId"].ToString().Substring(0, 1) == "2")
                        {
                            var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                            svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                            isNPStrengthen = "true";
                        }

                        break;
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["num"].ToString() == "1" &&
                        ((JObject)svtTreasureDevicestmp)["treasureDeviceId"].ToString().Length <= 5)
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        if (svtTDID == "") svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["num"].ToString() == "98" &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "0")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        if (svtTDID == "") svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "198")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        if (svtTDID == "") svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() == "199")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        if (svtTDID == "") svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject)svtTreasureDevicestmp)["treasureDeviceId"].ToString().Length == svtID.Length)
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        if (svtTDID == "") svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject)svtTreasureDevicestmp)["svtId"].ToString() != svtID ||
                        ((JObject)svtTreasureDevicestmp)["priority"].ToString() != "105") continue;
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                        break;
                    }
                }

                if (svtTDID.Contains("*"))
                    Dispatcher.Invoke(() =>
                    {
                        GlobalPathsAndDatas.IDListStr = svtTDID;
                        var ChoiceTD = new SvtSTDIDChoice();
                        ChoiceTD.ShowDialog();
                        var ReturnStr = ChoiceTD.idreturn;
                        svtTDID = ReturnStr;
                    });
                result[0] = svtTDID;
                result[1] = isNPStrengthen;
                Dispatcher.Invoke(() => { TreasureDeviceID.Text = svtTDID; });
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Warning("您太心急了,稍等一下再解析吧!\r\n" + e, "温馨提示:");
                    Button1.IsEnabled = true;
                });
                return result;
            }

            return result;
        }

        private void DisplayNPRate(string svtTDID)
        {
            var NPRateTD = 0.0M;
            var NPRateArts = 0.0M;
            var NPRateBuster = 0.0M;
            var NPRateQuick = 0.0M;
            var NPRateEX = 0.0M;
            var NPRateDef = 0.0M;
            SkillLvs.NPA = "";
            SkillLvs.NPB = "";
            SkillLvs.NPQ = "";
            SkillLvs.NPEX = "";
            SkillLvs.NPTD = "";
            foreach (var TDlvtmp in GlobalPathsAndDatas.mstTreasureDeviceLvArray)
                if (((JObject)TDlvtmp)["treaureDeviceId"].ToString() == svtTDID)
                {
                    var TDlvobjtmp = JObject.Parse(TDlvtmp.ToString());
                    NPRateTD = Convert.ToDecimal(TDlvobjtmp["tdPoint"].ToString()) / 10000;
                    NPRateArts = Convert.ToDecimal(TDlvobjtmp["tdPointA"].ToString()) / 10000;
                    NPRateBuster = Convert.ToDecimal(TDlvobjtmp["tdPointB"].ToString()) / 10000;
                    NPRateQuick = Convert.ToDecimal(TDlvobjtmp["tdPointQ"].ToString()) / 10000;
                    NPRateEX = Convert.ToDecimal(TDlvobjtmp["tdPointEx"].ToString()) / 10000;
                    NPRateDef = Convert.ToDecimal(TDlvobjtmp["tdPointDef"].ToString()) / 10000;
                    break;
                }

            var nptmp = (int)(NPRateTD * 1000000 + NPRateArts * 1000000 + NPRateBuster * 1000000 +
                              NPRateQuick * 1000000 + NPRateEX * 1000000);
            var average = nptmp / 5;

            nprate.Dispatcher.Invoke(() =>
            {
                nprate.Text = "Quick: " + NPRateQuick.ToString("P") + "   Arts: " +
                              NPRateArts.ToString("P") + "   Buster: " +
                              NPRateBuster.ToString("P") + "\r\nExtra: " +
                              NPRateEX.ToString("P") + "   宝具: " + NPRateTD.ToString("P") +
                              "   受击: " + NPRateDef.ToString("P");
            });

            SkillLvs.NPA = NPRateArts.ToString("P");
            SkillLvs.NPB = NPRateBuster.ToString("P");
            SkillLvs.NPQ = NPRateQuick.ToString("P");
            SkillLvs.NPEX = NPRateEX.ToString("P");
            SkillLvs.NPTD = NPRateTD.ToString("P");

            if (GlobalPathsAndDatas.notrealnprate == 0.0M) return;

            if (average - (int)(NPRateTD * 1000000) != 0 || average - (int)(NPRateArts * 1000000) != 0 ||
                average - (int)(NPRateBuster * 1000000) != 0 || average - (int)(NPRateQuick * 1000000) != 0 ||
                average - (int)(NPRateEX * 1000000) != 0)
            {
                BeiZhu.Dispatcher.Invoke(() => { BeiZhu.Text += "NP率有特殊情况,请留意."; });
            }
            else
            {
                if (average - (int)(GlobalPathsAndDatas.notrealnprate * 1000000) > 0)
                    BeiZhu.Dispatcher.Invoke(() =>
                    {
                        BeiZhu.Text +=
                            $"实际NP率({(decimal)average / 10000:f2}%) > 理论值({GlobalPathsAndDatas.notrealnprate * 100:f2}%).";
                    });
                else if (average - (int)(GlobalPathsAndDatas.notrealnprate * 1000000) == 0)
                    BeiZhu.Dispatcher.Invoke(() =>
                    {
                        BeiZhu.Text +=
                            $"实际NP率({(decimal)average / 10000:f2}%) = 理论值({GlobalPathsAndDatas.notrealnprate * 100:f2}%).";
                    });
                else
                    BeiZhu.Dispatcher.Invoke(() =>
                    {
                        BeiZhu.Text +=
                            $"实际NP率({(decimal)average / 10000:f2}%) < 理论值({GlobalPathsAndDatas.notrealnprate * 100:f2}%).";
                    });
            }

            if (NPRateTD == 0.0M || NPRateArts == 0.0M || NPRateBuster == 0.0M || NPRateQuick == 0.0M ||
                NPRateEX == 0.0M ||
                NPRateDef == 0.0M)
                BeiZhu.Dispatcher.Invoke(() => { BeiZhu.Text = "实际NP率为0,为尚未实装的从者(敌方BOSS/NPC)或小怪."; });
        }

        private void ServantTreasureDeviceInformationCheck(object svtTDID)
        {
            var NPDetail = "unknown";
            var NPName = "";
            var NPrank = "";
            var NPruby = "";
            var NPtypeText = "";
            var svtNPDamageType = "";
            var svtNPCardhit = 1;
            var svtNPCardhitDamage = "";
            var svtNPCardType = "";
            var DNR = new Task(() => { DisplayNPRate(svtTDID.ToString()); });
            DNR.Start();
            foreach (var TDDtmp in GlobalPathsAndDatas.mstTreasureDeviceDetailArray)
                if (((JObject)TDDtmp)["id"].ToString() == svtTDID.ToString())
                {
                    var TDDobjtmp = JObject.Parse(TDDtmp.ToString());
                    NPDetail = TDDobjtmp["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.5]")
                        .Replace("[g]", "")
                        .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "");
                    break;
                }

            foreach (var TreasureDevicestmp in GlobalPathsAndDatas.mstTreasureDevicedArray)
            {
                if (((JObject)TreasureDevicestmp)["id"].ToString() != svtTDID.ToString()) continue;
                var mstTDobjtmp = JObject.Parse(TreasureDevicestmp.ToString());
                NPName = mstTDobjtmp["name"].ToString();
                npname.Dispatcher.Invoke(() => { npname.Text = NPName; });
                NPrank = mstTDobjtmp["rank"].ToString();
                NPruby = mstTDobjtmp["ruby"].ToString();
                npruby.Dispatcher.Invoke(() => { npruby.Text = NPruby; });
                NPtypeText = mstTDobjtmp["typeText"].ToString();
                nprank.Dispatcher.Invoke(() => { nprank.Text = NPrank + " ( " + NPtypeText + " ) "; });
                svtNPDamageType = mstTDobjtmp["effectFlag"].ToString().Replace("0", "辅助宝具")
                    .Replace("1", "全体宝具").Replace("2", "单体宝具");
                nptype.Dispatcher.Invoke(() => { nptype.Text = svtNPDamageType; });
                foreach (var svtTreasureDevicestmp in GlobalPathsAndDatas.mstSvtTreasureDevicedArray)
                    if (((JObject)svtTreasureDevicestmp)["treasureDeviceId"].ToString() ==
                        ((JObject)TreasureDevicestmp)["id"].ToString())
                    {
                        var mstsvtTDobjtmp2 = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtNPCardhitDamage = mstsvtTDobjtmp2["damage"].ToString().Replace("\n", "")
                            .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                        svtNPCardType = mstsvtTDobjtmp2["cardId"].ToString().Replace("2", "Buster")
                            .Replace("1", "Arts").Replace("3", "Quick");
                        var svtNPCardAdditional = mstsvtTDobjtmp2["cardId"].ToString().Replace("2", "B")
                            .Replace("1", "A").Replace("3", "Q");
                        var CCAI = new Task(() => { ChangeCardArrangeImage(6, svtNPCardAdditional); });
                        CCAI.Start();
                        break;
                    }

                cards.Dispatcher.Invoke(() =>
                {
                    if (svtNPDamageType != "辅助宝具" || cards.Text == "[Q,Q,Q,Q,Q]" ||
                        svtTDID.ToString() == "9935511") return;
                    svtNPCardhit = 0;
                    svtNPCardhitDamage = "[ - ]";
                });
                break;
            }

            treasuredevicescard.Dispatcher.Invoke(() =>
            {
                svtNPCardhit += svtNPCardhitDamage.Count(c => c == ',');
                treasuredevicescard.Text = svtNPCardhit + " hit " + svtNPCardhitDamage;
            });
            npcardtype.Dispatcher.Invoke(() => { npcardtype.Text = svtNPCardType; });

            var newtmpid = "";
            Dispatcher.Invoke(() =>
            {
                if (NPDetail != "unknown") return;
                foreach (var TreasureDevicestmp2 in GlobalPathsAndDatas.mstTreasureDevicedArray)
                    if (((JObject)TreasureDevicestmp2)["name"].ToString() == NPName)
                    {
                        var TreasureDevicesobjtmp2 = JObject.Parse(TreasureDevicestmp2.ToString());
                        newtmpid = TreasureDevicesobjtmp2["id"].ToString();
                        switch (newtmpid.Length)
                        {
                            case 6:
                            {
                                var FinTDID_TMP = newtmpid;
                                foreach (var TDDtmp2 in GlobalPathsAndDatas.mstTreasureDeviceDetailArray)
                                    if (((JObject)TDDtmp2)["id"].ToString() == FinTDID_TMP)
                                    {
                                        var TDDobjtmp2 = JObject.Parse(TDDtmp2.ToString());
                                        NPDetail = TDDobjtmp2["detail"].ToString()
                                            .Replace("[{0}]", "[Lv.1 - Lv.5]")
                                            .Replace("[g]", "")
                                            .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "");
                                    }

                                break;
                            }
                            case 7:
                            {
                                if (newtmpid.Substring(0, 2) == "10" || newtmpid.Substring(0, 2) == "11" ||
                                    newtmpid.Substring(0, 2) == "23" || newtmpid.Substring(0, 2) == "25")
                                {
                                    var FinTDID_TMP = newtmpid;
                                    foreach (var TDDtmp2 in GlobalPathsAndDatas.mstTreasureDeviceDetailArray
                                            )
                                        if (((JObject)TDDtmp2)["id"].ToString() == FinTDID_TMP)
                                        {
                                            var TDDobjtmp2 = JObject.Parse(TDDtmp2.ToString());
                                            NPDetail = TDDobjtmp2["detail"].ToString()
                                                .Replace("[{0}]", "[Lv.1 - Lv.5]").Replace("[g]", "")
                                                .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "");
                                        }
                                }

                                break;
                            }
                        }
                    }
            });
            npdetail.Dispatcher.Invoke(() => { npdetail.Text = NPDetail; });
            if (NPName == "" && NPDetail == "unknown")
                npdetail.Dispatcher.Invoke(() => { npdetail.Text = "该宝具暂时没有描述."; });
        }

        private void ServantBasicInformationCheck()
        {
            Dispatcher.Invoke(() =>
            {
                var RankString = new string[100];
                RankString[11] = "A";
                RankString[12] = "A+";
                RankString[13] = "A++";
                RankString[14] = "A-";
                RankString[15] = "A+++";
                RankString[16] = "A?";
                RankString[17] = "A(B)";
                RankString[18] = "A(C)";
                RankString[19] = "A(D)";
                RankString[20] = "A(E)";
                RankString[21] = "B";
                RankString[22] = "B+";
                RankString[23] = "B++";
                RankString[24] = "B-";
                RankString[25] = "B+++";
                RankString[26] = "B?";
                RankString[27] = "B(A)";
                RankString[28] = "B(C)";
                RankString[29] = "B(D)";
                RankString[30] = "B(E)";
                RankString[31] = "C";
                RankString[32] = "C+";
                RankString[33] = "C++";
                RankString[34] = "C-";
                RankString[35] = "C+++";
                RankString[36] = "C?";
                RankString[37] = "C(A)";
                RankString[38] = "C(B)";
                RankString[39] = "C(D)";
                RankString[40] = "C(E)";
                RankString[41] = "D";
                RankString[42] = "D+";
                RankString[43] = "D++";
                RankString[44] = "D-";
                RankString[45] = "D+++";
                RankString[46] = "D?";
                RankString[47] = "D(A)";
                RankString[48] = "D(B)";
                RankString[49] = "D(C)";
                RankString[50] = "D(E)";
                RankString[51] = "E";
                RankString[52] = "E+";
                RankString[53] = "E++";
                RankString[54] = "E-";
                RankString[55] = "E+++";
                RankString[56] = "E?";
                RankString[57] = "E(A)";
                RankString[58] = "E(B)";
                RankString[59] = "E(C)";
                RankString[60] = "E(D)";
                RankString[61] = "EX";
                RankString[98] = "?";
                RankString[0] = "-";
                RankString[99] = "?";
                var svtName = "";
                var svtNameDisplay = "unknown";
                var ClassName = new string[1500];
                ClassName[1] = "Saber";
                ClassName[2] = "Archer";
                ClassName[3] = "Lancer";
                ClassName[4] = "Rider";
                ClassName[5] = "Caster";
                ClassName[6] = "Assassin";
                ClassName[7] = "Berserker";
                ClassName[8] = "Shielder";
                ClassName[9] = "Ruler";
                ClassName[10] = "Alterego";
                ClassName[11] = "Avenger";
                ClassName[23] = "MoonCancer";
                ClassName[25] = "Foreigner";
                ClassName[20] = "Beast II";
                ClassName[22] = "Beast I";
                ClassName[24] = "Beast III/R";
                ClassName[26] = "Beast III/L";
                ClassName[27] = "Beast ?";
                ClassName[28] = "Pretender";
                ClassName[29] = "Beast IV";
                ClassName[30] = "?";
                ClassName[31] = "?";
                ClassName[32] = "?";
                ClassName[33] = "Beast";
                ClassName[34] = "Beast VI";
                ClassName[35] = "Beast VI";
                ClassName[36] = "???";
                ClassName[37] = "???";
                ClassName[38] = "Beast";
                ClassName[39] = "UNKNOWN";
                ClassName[40] = "UnBeast";
                ClassName[41] = "UNKNOWN";
                ClassName[97] = "不明";
                ClassName[1001] = "礼装";
                ClassName[107] = "Berserker";
                ClassName[21] = "?";
                ClassName[19] = "?";
                ClassName[18] = "?";
                ClassName[17] = "GrandCaster";
                ClassName[16] = "?";
                ClassName[15] = "?";
                ClassName[14] = "?";
                ClassName[13] = "?";
                ClassName[12] = "?";
                var svtClass = "unknown"; //ClassID
                var svtgender = "unknown";
                var gender = new string[4];
                gender[1] = "男";
                gender[2] = "女";
                gender[3] = "其他";
                var nprateclassbase = new decimal[150];
                nprateclassbase[1] = 1.5M;
                nprateclassbase[2] = 1.55M;
                nprateclassbase[3] = 1.45M;
                nprateclassbase[4] = 1.55M;
                nprateclassbase[5] = 1.6M;
                nprateclassbase[6] = 1.45M;
                nprateclassbase[7] = 1.4M;
                nprateclassbase[8] = 1.5M;
                nprateclassbase[9] = 1.5M;
                nprateclassbase[10] = 1.55M;
                nprateclassbase[11] = 1.45M;
                nprateclassbase[23] = 1.6M;
                nprateclassbase[25] = 1.5M;
                nprateclassbase[28] = 1.55M;
                nprateclassbase[33] = 1.5M;
                nprateclassbase[38] = 1.5M;
                nprateclassbase[40] = 1.5M; //待定
                nprateclassbase[20] = 0.0M;
                nprateclassbase[22] = 0.0M;
                nprateclassbase[24] = 0.0M;
                nprateclassbase[26] = 0.0M;
                nprateclassbase[27] = 0.0M;
                nprateclassbase[29] = 0.0M;
                nprateclassbase[97] = 0.0M;
                nprateclassbase[107] = 0.0M;
                nprateclassbase[21] = 0.0M;
                nprateclassbase[19] = 0.0M;
                nprateclassbase[18] = 0.0M;
                nprateclassbase[17] = 1.6M;
                nprateclassbase[16] = 0.0M;
                nprateclassbase[15] = 0.0M;
                nprateclassbase[14] = 0.0M;
                nprateclassbase[13] = 0.0M;
                nprateclassbase[12] = 0.0M;
                var nprateartscount = new decimal[4];
                nprateartscount[1] = 1.5M;
                nprateartscount[2] = 1.125M;
                nprateartscount[3] = 1.0M;
                var npratemagicbase = new decimal[100];
                npratemagicbase[11] = 1.02M;
                npratemagicbase[12] = 1.025M;
                npratemagicbase[13] = 1.03M;
                npratemagicbase[14] = 1.015M;
                npratemagicbase[15] = 1.035M;
                npratemagicbase[16] = 0.0M;
                npratemagicbase[17] = 1.0M;
                npratemagicbase[18] = 0.99M;
                npratemagicbase[19] = 0.98M;
                npratemagicbase[20] = 0.97M;
                npratemagicbase[21] = 1.0M;
                npratemagicbase[22] = 1.005M;
                npratemagicbase[23] = 1.01M;
                npratemagicbase[24] = 0.995M;
                npratemagicbase[25] = 1.015M;
                npratemagicbase[26] = 0.0M;
                npratemagicbase[27] = 1.02M;
                npratemagicbase[28] = 0.99M;
                npratemagicbase[29] = 0.98M;
                npratemagicbase[30] = 0.97M;
                npratemagicbase[31] = 0.99M;
                npratemagicbase[32] = 0.9925M;
                npratemagicbase[33] = 0.995M;
                npratemagicbase[34] = 0.985M;
                npratemagicbase[35] = 0.9975M;
                npratemagicbase[36] = 0.0M;
                npratemagicbase[37] = 1.02M;
                npratemagicbase[38] = 1.0M;
                npratemagicbase[39] = 0.98M;
                npratemagicbase[40] = 0.97M;
                npratemagicbase[41] = 0.98M;
                npratemagicbase[42] = 0.9825M;
                npratemagicbase[43] = 0.985M;
                npratemagicbase[44] = 0.975M;
                npratemagicbase[45] = 0.9875M;
                npratemagicbase[46] = 0.0M;
                npratemagicbase[47] = 1.02M;
                npratemagicbase[48] = 1.0M;
                npratemagicbase[49] = 0.99M;
                npratemagicbase[50] = 0.97M;
                npratemagicbase[51] = 0.97M;
                npratemagicbase[52] = 0.9725M;
                npratemagicbase[53] = 0.975M;
                npratemagicbase[54] = 0.965M;
                npratemagicbase[55] = 0.9775M;
                npratemagicbase[56] = 0.0M;
                npratemagicbase[57] = 1.02M;
                npratemagicbase[58] = 1.0M;
                npratemagicbase[59] = 0.99M;
                npratemagicbase[60] = 0.98M;
                npratemagicbase[61] = 1.04M;
                npratemagicbase[0] = 0.0M;
                npratemagicbase[99] = 0.0M;
                npratemagicbase[98] = 0.0M;
                npratemagicbase[97] = 0.0M;
                var svtstarrate = "";
                decimal NPrate = 0;
                float starrate = 0;
                float deathrate = 0;
                var svtdeathrate = "";
                var svtcollectionid = "";
                var svtrarity = "";
                var svthpBase = "";
                var svthpMax = "";
                var svtatkBase = "";
                var svtatkMax = "";
                GlobalPathsAndDatas.basicatk = 0;
                GlobalPathsAndDatas.basichp = 0;
                GlobalPathsAndDatas.CurveType = "";
                GlobalPathsAndDatas.maxatk = 0;
                GlobalPathsAndDatas.maxhp = 0;
                var svtcriticalWeight = "";
                var svtpower = "";
                var svtdefense = "";
                var svtagility = "";
                var svtmagic = "";
                var svtluck = "";
                var svttreasureDevice = "";
                var svtHideAttri = "";
                string svtClassPassiveID;
                var classData = 0;
                var powerData = 0;
                var defenseData = 0;
                var agilityData = 0;
                var magicData = 0;
                var luckData = 0;
                var TreasureData = 0;
                var genderData = 0;
                var CardArrange = "[Q,Q,Q,Q,Q]";
                var svtIndividualityInput = "";
                GlobalPathsAndDatas.askxlsx = true;
                GlobalPathsAndDatas.notrealnprate = 0.0M;
                foreach (var svtIDtmp in GlobalPathsAndDatas.mstSvtArray)
                    if (((JObject)svtIDtmp)["id"].ToString() == JB.svtid)
                    {
                        var mstSvtobjtmp = JObject.Parse(svtIDtmp.ToString());
                        svtName = mstSvtobjtmp["name"].ToString();
                        Svtname.Text = svtName;
                        JB.svtnme = svtName;
                        svtNameDisplay = mstSvtobjtmp["battleName"].ToString();
                        SvtBattlename.Text = svtNameDisplay;
                        Title += " - " + svtName;
                        svtClass = mstSvtobjtmp["classId"].ToString();
                        svtgender = mstSvtobjtmp["genderType"].ToString();
                        svtstarrate = mstSvtobjtmp["starRate"].ToString();
                        svtdeathrate = mstSvtobjtmp["deathRate"].ToString();
                        svtcollectionid = mstSvtobjtmp["collectionNo"].ToString();
                        GlobalPathsAndDatas.CurveType = mstSvtobjtmp["expType"].ToString();
                        svtIndividualityInput = mstSvtobjtmp["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        collection.Text = svtcollectionid;
                        svtHideAttri = mstSvtobjtmp["attri"].ToString().Replace("1", "人").Replace("2", "天")
                            .Replace("3", "地").Replace("4", "星").Replace("5", "兽");
                        CardArrange = mstSvtobjtmp["cardIds"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("2", "B").Replace("1", "A").Replace("3", "Q");
                        var SCI = new Task(() => { SetCardImgs(CardArrange); });
                        SCI.Start();
                        if (CardArrange == "[Q,Q,Q,Q,Q]") GlobalPathsAndDatas.askxlsx = false;
                        var SISI = new Task(() => { CheckSvtIndividuality(svtIndividualityInput); });
                        if (ToggleDispIndi.IsChecked == true)
                            SISI.Start();
                        else
                            svtIndividuality.Text = svtIndividualityInput;
                        cards.Text = CardArrange;
                        svtClassPassiveID = mstSvtobjtmp["classPassive"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        var SCPSC = new Task(() => { ServantClassPassiveSkillCheck(svtClassPassiveID); });
                        SCPSC.Start();
                        hiddenattri.Text = svtHideAttri;
                        classData = int.Parse(svtClass);
                        GlobalPathsAndDatas.classid = 0;
                        GlobalPathsAndDatas.classid = classData;
                        svtclass.Text = ClassName != null && ClassName[classData] != null
                            ? ClassName[classData]
                            : ReadClassName.ReadClassOriginName(classData);
                        var CheckShiZhuang = new Task(() => { CheckSvtIsFullinGame(classData); });
                        CheckShiZhuang.Start();
                        genderData = int.Parse(svtgender);
                        gendle.Text = gender[genderData];
                        starrate = float.Parse(svtstarrate) / 10;
                        ssvtstarrate.Text = starrate + "%";
                        deathrate = float.Parse(svtdeathrate) / 10;
                        ssvtdeathrate.Text = deathrate + "%";
                        break;
                    }

                foreach (var svtLimittmp in GlobalPathsAndDatas.mstSvtLimitArray)
                    if (((JObject)svtLimittmp)["svtId"].ToString() == JB.svtid &&
                        ((JObject)svtLimittmp)["limitCount"].ToString() == "4")
                    {
                        var mstsvtLimitobjtmp = JObject.Parse(svtLimittmp.ToString());
                        svtrarity = mstsvtLimitobjtmp["rarity"].ToString();
                        svthpBase = mstsvtLimitobjtmp["hpBase"].ToString();
                        svthpMax = mstsvtLimitobjtmp["hpMax"].ToString();
                        svtatkBase = mstsvtLimitobjtmp["atkBase"].ToString();
                        svtatkMax = mstsvtLimitobjtmp["atkMax"].ToString();
                        rarity.Text = svtrarity + " ☆";
                        var DSR = new Task(() => { DisplaySvtRarity(Convert.ToInt32(svtrarity)); });
                        DSR.Start();
                        var DSC = new Task(() => { DisplaySvtClassPng(classData, Convert.ToInt32(svtrarity)); });
                        DSC.Start();
                        maxhp.Text = svthpMax;
                        basichp.Text = svthpBase;
                        basicatk.Text = svtatkBase;
                        maxatk.Text = svtatkMax;
                        if (JB.svtid == "800100")
                        {
                            maxhp.Text = "12877";
                            maxatk.Text = "8730";
                        }

                        GlobalPathsAndDatas.basicatk = Convert.ToInt32(svtatkBase);
                        GlobalPathsAndDatas.basichp = Convert.ToInt32(svthpBase);
                        GlobalPathsAndDatas.maxatk = Convert.ToInt32(svtatkMax);
                        GlobalPathsAndDatas.maxhp = Convert.ToInt32(svthpMax);
                        var DSSCL = new Task(() => { DrawServantStrengthenCurveLine(GlobalPathsAndDatas.CurveType); });
                        DSSCL.Start();
                        svtcriticalWeight = mstsvtLimitobjtmp["criticalWeight"].ToString();
                        jixing.Text = svtcriticalWeight;
                        svtpower = mstsvtLimitobjtmp["power"].ToString();
                        svtdefense = mstsvtLimitobjtmp["defense"].ToString();
                        svtagility = mstsvtLimitobjtmp["agility"].ToString();
                        svtmagic = mstsvtLimitobjtmp["magic"].ToString();
                        svtluck = mstsvtLimitobjtmp["luck"].ToString();
                        svttreasureDevice = mstsvtLimitobjtmp["treasureDevice"].ToString();
                        var SHAB = new Task(() =>
                        {
                            ShowHPAtkBalance(JB.svtid, svtrarity, svtdefense, svthpBase, svtClass);
                        });
                        SHAB.Start();
                        powerData = int.Parse(svtpower);
                        defenseData = int.Parse(svtdefense);
                        agilityData = int.Parse(svtagility);
                        magicData = int.Parse(svtmagic);
                        luckData = int.Parse(svtluck);
                        TreasureData = int.Parse(svttreasureDevice);
                        sixwei.Text = "筋力: " + RankString[powerData] + "    耐久: " + RankString[defenseData] +
                                      "    敏捷: " +
                                      RankString[agilityData] +
                                      "    魔力: " + RankString[magicData] + "    幸运: " + RankString[luckData] +
                                      "    宝具: " +
                                      RankString[TreasureData];
                        break;
                    }

                if (svtrarity == "")
                    foreach (var svtLimittmp in GlobalPathsAndDatas.mstSvtLimitArray)
                        if (((JObject)svtLimittmp)["svtId"].ToString() == JB.svtid &&
                            ((JObject)svtLimittmp)["limitCount"].ToString() == "1")
                        {
                            var mstsvtLimitobjtmp = JObject.Parse(svtLimittmp.ToString());
                            svtrarity = mstsvtLimitobjtmp["rarity"].ToString();
                            svthpBase = mstsvtLimitobjtmp["hpBase"].ToString();
                            svthpMax = mstsvtLimitobjtmp["hpMax"].ToString();
                            svtatkBase = mstsvtLimitobjtmp["atkBase"].ToString();
                            svtatkMax = mstsvtLimitobjtmp["atkMax"].ToString();
                            rarity.Text = svtrarity + " ☆";
                            var DSR = new Task(() => { DisplaySvtRarity(Convert.ToInt32(svtrarity)); });
                            DSR.Start();
                            var DSC = new Task(() => { DisplaySvtClassPng(classData, Convert.ToInt32(svtrarity)); });
                            DSC.Start();
                            maxhp.Text = svthpMax;
                            basichp.Text = svthpBase;
                            basicatk.Text = svtatkBase;
                            maxatk.Text = svtatkMax;
                            if (JB.svtid == "800100")
                            {
                                maxhp.Text = "12877";
                                maxatk.Text = "8730";
                            }

                            GlobalPathsAndDatas.basicatk = Convert.ToInt32(svtatkBase);
                            GlobalPathsAndDatas.basichp = Convert.ToInt32(svthpBase);
                            GlobalPathsAndDatas.maxatk = Convert.ToInt32(svtatkMax);
                            GlobalPathsAndDatas.maxhp = Convert.ToInt32(svthpMax);
                            var DSSCL = new Task(() =>
                            {
                                DrawServantStrengthenCurveLine(GlobalPathsAndDatas.CurveType);
                            });
                            DSSCL.Start();
                            svtcriticalWeight = mstsvtLimitobjtmp["criticalWeight"].ToString();
                            jixing.Text = svtcriticalWeight;
                            svtpower = mstsvtLimitobjtmp["power"].ToString();
                            svtdefense = mstsvtLimitobjtmp["defense"].ToString();
                            svtagility = mstsvtLimitobjtmp["agility"].ToString();
                            svtmagic = mstsvtLimitobjtmp["magic"].ToString();
                            svtluck = mstsvtLimitobjtmp["luck"].ToString();
                            svttreasureDevice = mstsvtLimitobjtmp["treasureDevice"].ToString();
                            var SHAB = new Task(() =>
                            {
                                ShowHPAtkBalance(JB.svtid, svtrarity, svtdefense, svthpBase, svtClass);
                            });
                            SHAB.Start();
                            powerData = int.Parse(svtpower);
                            defenseData = int.Parse(svtdefense);
                            agilityData = int.Parse(svtagility);
                            magicData = int.Parse(svtmagic);
                            luckData = int.Parse(svtluck);
                            TreasureData = int.Parse(svttreasureDevice);
                            sixwei.Text = "筋力: " + RankString[powerData] + "    耐久: " + RankString[defenseData] +
                                          "    敏捷: " +
                                          RankString[agilityData] +
                                          "    魔力: " + RankString[magicData] + "    幸运: " + RankString[luckData] +
                                          "    宝具: " +
                                          RankString[TreasureData];
                            break;
                        }

                if (svtrarity == "")
                    foreach (var svtLimittmp in GlobalPathsAndDatas.mstSvtLimitArray)
                        if (((JObject)svtLimittmp)["svtId"].ToString() == JB.svtid &&
                            ((JObject)svtLimittmp)["limitCount"].ToString() == "0")
                        {
                            var mstsvtLimitobjtmp = JObject.Parse(svtLimittmp.ToString());
                            svtrarity = mstsvtLimitobjtmp["rarity"].ToString();
                            svthpBase = mstsvtLimitobjtmp["hpBase"].ToString();
                            svthpMax = mstsvtLimitobjtmp["hpMax"].ToString();
                            svtatkBase = mstsvtLimitobjtmp["atkBase"].ToString();
                            svtatkMax = mstsvtLimitobjtmp["atkMax"].ToString();
                            rarity.Text = svtrarity + " ☆";
                            var DSR = new Task(() => { DisplaySvtRarity(Convert.ToInt32(svtrarity)); });
                            DSR.Start();
                            var DSC = new Task(() => { DisplaySvtClassPng(classData, Convert.ToInt32(svtrarity)); });
                            DSC.Start();
                            maxhp.Text = svthpMax;
                            basichp.Text = svthpBase;
                            basicatk.Text = svtatkBase;
                            maxatk.Text = svtatkMax;
                            if (JB.svtid == "800100")
                            {
                                maxhp.Text = "12877";
                                maxatk.Text = "8730";
                            }

                            GlobalPathsAndDatas.basicatk = Convert.ToInt32(svtatkBase);
                            GlobalPathsAndDatas.basichp = Convert.ToInt32(svthpBase);
                            GlobalPathsAndDatas.maxatk = Convert.ToInt32(svtatkMax);
                            GlobalPathsAndDatas.maxhp = Convert.ToInt32(svthpMax);
                            var DSSCL = new Task(() =>
                            {
                                DrawServantStrengthenCurveLine(GlobalPathsAndDatas.CurveType);
                            });
                            DSSCL.Start();
                            svtcriticalWeight = mstsvtLimitobjtmp["criticalWeight"].ToString();
                            jixing.Text = svtcriticalWeight;
                            svtpower = mstsvtLimitobjtmp["power"].ToString();
                            svtdefense = mstsvtLimitobjtmp["defense"].ToString();
                            svtagility = mstsvtLimitobjtmp["agility"].ToString();
                            svtmagic = mstsvtLimitobjtmp["magic"].ToString();
                            svtluck = mstsvtLimitobjtmp["luck"].ToString();
                            svttreasureDevice = mstsvtLimitobjtmp["treasureDevice"].ToString();
                            var SHAB = new Task(() =>
                            {
                                ShowHPAtkBalance(JB.svtid, svtrarity, svtdefense, svthpBase, svtClass);
                            });
                            SHAB.Start();
                            powerData = int.Parse(svtpower);
                            defenseData = int.Parse(svtdefense);
                            agilityData = int.Parse(svtagility);
                            magicData = int.Parse(svtmagic);
                            luckData = int.Parse(svtluck);
                            TreasureData = int.Parse(svttreasureDevice);
                            sixwei.Text = "筋力: " + RankString[powerData] + "    耐久: " + RankString[defenseData] +
                                          "    敏捷: " +
                                          RankString[agilityData] +
                                          "    魔力: " + RankString[magicData] + "    幸运: " + RankString[luckData] +
                                          "    宝具: " +
                                          RankString[TreasureData];
                            break;
                        }

                var svtArtsCardQuantity = CardArrange.Count(c => c == 'A');
                if (svtArtsCardQuantity == 0)
                {
                    NPrate = 0;
                    notrealnprate.Text = NPrate.ToString("P");
                }
                else
                {
                    NPrate = nprateclassbase[classData] * nprateartscount[svtArtsCardQuantity] *
                        npratemagicbase[magicData] / GlobalPathsAndDatas.svtArtsCardhit / 100M;
                    NPrate = Math.Floor(NPrate * 10000M) / 10000M;
                    GlobalPathsAndDatas.notrealnprate = NPrate;
                    notrealnprate.Text = NPrate.ToString("P");
                    //notrealnprate.Text = Math.Round(NPrate * 100,4) + "%";
                }

                var attackRate = 0.00M; //atk补正

                foreach (var svtClasstmp in GlobalPathsAndDatas.mstClassArray)
                {
                    if (((JObject)svtClasstmp)["id"].ToString() == classData.ToString())
                    {
                        attackRate = Convert.ToDecimal(((JObject)svtClasstmp)["attackRate"].ToString()) / 1000M;
                        break;
                    }
                }

                var atkBalanceStr = "";
                if (attackRate == 1.00M)
                {
                    atkBalanceStr = $"( x {attackRate:#0.000} -)";
                }
                else if (attackRate < 1.00M)
                {
                    atkBalanceStr = $"( x {attackRate:#0.000} ▽)";
                }
                else
                {
                    atkBalanceStr = $"( x {attackRate:#0.000} △)";
                }

                if (classData == 1001)
                    Growl.Info("此ID为礼装ID,图鉴编号为礼装的图鉴编号.礼装描述在牵绊文本的文本1处.");

                atkbalance1.Text = atkBalanceStr;
                atkbalance2.Text = atkBalanceStr;

                /*switch (classData)
                {
                    case 1:
                    case 4:
                    case 8:
                    case 10:
                    case 20:
                    case 22:
                    case 24:
                    case 26:
                    case 23:
                    case 25:
                    case 17:
                    case 28:
                    case 33:
                    case 38:
                    case 40:
                        atkbalance1.Text = "( x 1.0 -)";
                        atkbalance2.Text = "( x 1.0 -)";
                        break;
                    case 3:
                        atkbalance1.Text = "( x 1.05 △)";
                        atkbalance2.Text = "( x 1.05 △)";
                        break;
                    case 5:
                    case 6:
                        atkbalance1.Text = "( x 0.9 ▽)";
                        atkbalance2.Text = "( x 0.9 ▽)";
                        break;
                    case 2:
                        atkbalance1.Text = "( x 0.95 ▽)";
                        atkbalance2.Text = "( x 0.95 ▽)";
                        break;
                    case 7:
                    case 9:
                    case 11:
                        atkbalance1.Text = "( x 1.1 △)";
                        atkbalance2.Text = "( x 1.1 △)";
                        break;
                    case 1001:
                        Growl.Info("此ID为礼装ID,图鉴编号为礼装的图鉴编号.礼装描述在牵绊文本的文本1处.");
                        break;
                    default:
                        atkbalance1.Text = "( x 1.0 -)";
                        atkbalance2.Text = "( x 1.0 -)";
                        break;
                }*/
            });
        }

        private void DisplaySvtClassPng(int classid, int rarity)
        {
            var ClassName = new string[50];
            ClassName[1] = "Saber";
            ClassName[2] = "Archer";
            ClassName[3] = "Lancer";
            ClassName[4] = "Rider";
            ClassName[5] = "Caster";
            ClassName[6] = "Assassin";
            ClassName[7] = "Berserker";
            ClassName[8] = "Shielder";
            ClassName[9] = "Ruler";
            ClassName[10] = "Alterego";
            ClassName[11] = "Avenger";
            ClassName[17] = "Caster";
            ClassName[23] = "MoonCancer";
            ClassName[25] = "Foreigner";
            ClassName[20] = "BeastII";
            ClassName[22] = "BeastI";
            ClassName[24] = "BeastIII";
            ClassName[26] = "BeastIII";
            ClassName[27] = "Beast？";
            ClassName[28] = "Pretender";
            ClassName[29] = "BeastIV";
            ClassName[30] = "？(30)";
            ClassName[31] = "？(31)";
            ClassName[32] = "？(32)";
            ClassName[33] = "Beast";
            ClassName[38] = "Beast";
            ClassName[40] = "UnBeast";
            ClassName[34] = "BeastVI";
            ClassName[35] = "BeastVI";
            var pngArr = 0;
            switch (rarity)
            {
                case 0:
                    pngArr = 0;
                    break;
                case 1:
                case 2:
                    pngArr = 1;
                    break;
                case 3:
                    pngArr = 2;
                    break;
                case 4:
                case 5:
                    pngArr = 3;
                    break;
                default:
                    pngArr = 3;
                    break;
            }

            Dispatcher.Invoke(() =>
            {
                ClassPng.Visibility = Visibility.Visible;
                try
                {
                    try
                    {
                        ClassPng.Source =
                            new BitmapImage(new Uri("images\\Class" + ClassName[classid] + pngArr + ".png",
                                UriKind.Relative));
                    }
                    catch (Exception)
                    {
                        ClassPng.Source = new BitmapImage(new Uri("images\\Class" + ClassName[classid] + "3" + ".png",
                            UriKind.Relative));
                    }
                }
                catch (Exception)
                {
                    ClassPng.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void DisplaySvtRarity(int Rarity)
        {
            var C = "images\\C.png";
            var UC = "images\\UC.png";
            var R = "images\\R.png";
            var SR = "images\\SR.png";
            var SSR = "images\\SSR.png";
            Dispatcher.Invoke(() =>
            {
                switch (Rarity)
                {
                    case 1:
                        raritystars.Source = new BitmapImage(new Uri(C, UriKind.Relative));
                        raritystars.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        raritystars.Source = new BitmapImage(new Uri(UC, UriKind.Relative));
                        raritystars.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        raritystars.Source = new BitmapImage(new Uri(R, UriKind.Relative));
                        raritystars.Visibility = Visibility.Visible;
                        break;
                    case 4:
                        raritystars.Source = new BitmapImage(new Uri(SR, UriKind.Relative));
                        raritystars.Visibility = Visibility.Visible;
                        break;
                    case 5:
                        raritystars.Source = new BitmapImage(new Uri(SSR, UriKind.Relative));
                        raritystars.Visibility = Visibility.Visible;
                        break;
                    case 0:
                        raritystars.Visibility = Visibility.Hidden;
                        break;
                }
            });
        }

        private string GetCondType(string val)
        {
            var yu = val.Replace("[", "").Replace("]", "").Replace("\r\n", "").Replace(" ", "");
            var yuarray = yu.Split(',');
            if (yuarray.Length != 1) return $"[开放条件: 通关关卡 {yu}]\r\n";
            switch (yuarray[0])
            {
                case "0":
                    return "[开放条件: 牵绊Lv.0]\r\n";
                case "1":
                    return "[开放条件: 牵绊Lv.1]\r\n";
                case "2":
                    return "[开放条件: 牵绊Lv.2]\r\n";
                case "3":
                    return "[开放条件: 牵绊Lv.3]\r\n";
                case "4":
                    return "[开放条件: 牵绊Lv.4]\r\n";
                case "5":
                    return "[开放条件: 牵绊Lv.5]\r\n";
                default:
                    return $"[开放条件: 通关关卡 {yuarray[0]}]\r\n";
            }
        }

        private string ParseScriptJson(string str)
        {
            try
            {
                var JObj = (JObject)JsonConvert.DeserializeObject(str);
                return $"[额外条件: {JObj["condTitle"].ToString().Replace("(", "").Replace(")", "")}]\r\n\r\n";
            }
            catch (Exception)
            {
                return "\r\n";
            }
        }

        private void ServantJibanTextCheck()
        {
            var isJBChangeByCond = false;
            foreach (var SCTMP in GlobalPathsAndDatas.mstSvtCommentArray)
            {
                if (((JObject)SCTMP)["svtId"].ToString() == JB.svtid && ((JObject)SCTMP)["id"].ToString() == "1")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext1.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext1.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB1 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB1tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (jibantext1.Text != "")
                            JBOutput.Dispatcher.Invoke(() => { JBOutput.IsEnabled = true; });
                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext1.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB1tmp)
                        {
                            var c = jibantext1.Text.IndexOf(']') + 1;
                            jibantext1.Text = jibantext1.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB1 = JB.JB1.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext1.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB1 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }

                if (((JObject)SCTMP)["svtId"].ToString() == JB.svtid && ((JObject)SCTMP)["id"].ToString() == "2")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext2.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext2.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB2 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB2tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext2.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB2tmp)
                        {
                            var c = jibantext2.Text.IndexOf(']') + 1;
                            jibantext2.Text = jibantext2.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB2 = JB.JB2.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext2.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB2 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }

                if (((JObject)SCTMP)["svtId"].ToString() == JB.svtid && ((JObject)SCTMP)["id"].ToString() == "3")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext3.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext3.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB3 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB3tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext3.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB3tmp)
                        {
                            var c = jibantext3.Text.IndexOf(']') + 1;
                            jibantext3.Text = jibantext3.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB3 = JB.JB3.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext3.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB3 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }

                if (((JObject)SCTMP)["svtId"].ToString() == JB.svtid && ((JObject)SCTMP)["id"].ToString() == "4")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext4.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext4.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB4 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB4tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext4.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB4tmp)
                        {
                            var c = jibantext4.Text.IndexOf(']') + 1;
                            jibantext4.Text = jibantext4.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB4 = JB.JB4.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext4.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB4 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }

                if (((JObject)SCTMP)["svtId"].ToString() == JB.svtid && ((JObject)SCTMP)["id"].ToString() == "5")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext5.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext5.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB5 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB5tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext5.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB5tmp)
                        {
                            var c = jibantext5.Text.IndexOf(']') + 1;
                            jibantext5.Text = jibantext5.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB5 = JB.JB5.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext5.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB5 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }

                if (((JObject)SCTMP)["svtId"].ToString() == JB.svtid && ((JObject)SCTMP)["id"].ToString() == "6")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext6.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext6.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB6 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB6tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext6.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB6tmp)
                        {
                            var c = jibantext6.Text.IndexOf(']') + 1;
                            jibantext6.Text = jibantext6.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB6 = JB.JB6.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext6.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB6 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }

                if (((JObject)SCTMP)["svtId"].ToString() != JB.svtid ||
                    ((JObject)SCTMP)["id"].ToString() != "7") continue;
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext7.Dispatcher.Invoke(() =>
                    {
                        if (((JObject)SCTMP)["priority"].ToString() == "0")
                        {
                            jibantext7.Text += GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB7 += GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB7tmp = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        if (((JObject)SCTMP)["priority"].ToString() != "1") return;
                        var compareStr = GetCondType(SCobjtmp["condValues"].ToString()) +
                                         ParseScriptJson(SCobjtmp["script"].ToString()) +
                                         SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext7.Text == compareStr) return;
                        if (SCobjtmp["comment"].ToString().Replace("\n", "\r\n") == JB.JB7tmp)
                        {
                            var c = jibantext7.Text.IndexOf(']') + 1;
                            jibantext7.Text = jibantext7.Text.Insert(c,
                                " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                ParseScriptJson(SCobjtmp["script"].ToString())).Replace("\r\n\r\n", "\r\n");
                            JB.JB7 = JB.JB7.Insert(c, " 或 " + GetCondType(SCobjtmp["condValues"].ToString()) +
                                                      ParseScriptJson(SCobjtmp["script"].ToString()))
                                .Replace("\r\n\r\n", "\r\n");
                        }
                        else
                        {
                            jibantext7.Text += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                               ParseScriptJson(SCobjtmp["script"].ToString()) +
                                               SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                            JB.JB7 += V + GetCondType(SCobjtmp["condValues"].ToString()) +
                                      ParseScriptJson(SCobjtmp["script"].ToString()) +
                                      SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        }

                        isJBChangeByCond = true;
                    });
                }
            }

            if (isJBChangeByCond)
                Dispatcher.Invoke(() => { Growl.Info("注意,该从者的牵绊文本会随部分条件而发生改变，详情可查看各个文本框!"); });
        }

        private void ServantCombineLimitItemsCheck()
        {
            foreach (var mstCombineLimittmp in GlobalPathsAndDatas.mstCombineLimitArray)
                if (((JObject)mstCombineLimittmp)["id"].ToString() == JB.svtid)
                {
                    var LimitID = ((JObject)mstCombineLimittmp)["svtLimit"].ToString();
                    switch (Convert.ToInt64(LimitID))
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            var itemIds = ((JObject)mstCombineLimittmp)["itemIds"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var itemNums = ((JObject)mstCombineLimittmp)["itemNums"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var qp = ((JObject)mstCombineLimittmp)["qp"].ToString();
                            var itemIdArray = itemIds.Split(',');
                            var itemNumsArray = itemNums.Split(',');
                            var itemDisplay = "";
                            for (var i = 0; i < itemIdArray.Length; i++)
                                itemDisplay += CheckItemName(itemIdArray[i]) + "(" + itemNumsArray[i] + "),";
                            itemDisplay = itemDisplay.Substring(0, itemDisplay.Length - 1);
                            LimitCombineItems.Dispatcher.Invoke(() =>
                            {
                                LimitCombineItems.Items.Add(
                                    new ItemList(LimitID + " → " + (Convert.ToInt64(LimitID) + 1), itemDisplay,
                                        qp));
                            });
                            break;
                    }
                }
        }

        private void ServantCombineSkillItemsCheck()
        {
            foreach (var mstCombineSkilltmp in GlobalPathsAndDatas.mstCombineSkillArray)
                if (((JObject)mstCombineSkilltmp)["id"].ToString() == JB.svtid)
                {
                    var LimitID = ((JObject)mstCombineSkilltmp)["skillLv"].ToString();
                    switch (Convert.ToInt64(LimitID))
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            var itemIds = ((JObject)mstCombineSkilltmp)["itemIds"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var itemNums = ((JObject)mstCombineSkilltmp)["itemNums"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var qp = ((JObject)mstCombineSkilltmp)["qp"].ToString();
                            var itemIdArray = itemIds.Split(',');
                            var itemNumsArray = itemNums.Split(',');
                            var itemDisplay = "";
                            for (var i = 0; i < itemIdArray.Length; i++)
                                itemDisplay += CheckItemName(itemIdArray[i]) + "(" + itemNumsArray[i] + "),";
                            itemDisplay = itemDisplay.Substring(0, itemDisplay.Length - 1);
                            SkillCombineItems.Dispatcher.Invoke(() =>
                            {
                                SkillCombineItems.Items.Add(
                                    new ItemList(LimitID + " → " + (Convert.ToInt64(LimitID) + 1), itemDisplay,
                                        qp));
                            });
                            break;
                    }
                }

            foreach (var mstCombineAppendSkilltmp in GlobalPathsAndDatas.mstCombineAppendPassiveSkillArray)
                if (((JObject)mstCombineAppendSkilltmp)["svtId"].ToString() == JB.svtid)
                {
                    var LimitID = ((JObject)mstCombineAppendSkilltmp)["skillLv"].ToString();
                    switch (Convert.ToInt64(LimitID))
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                            var itemIds = ((JObject)mstCombineAppendSkilltmp)["itemIds"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var itemNums = ((JObject)mstCombineAppendSkilltmp)["itemNums"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var qp = ((JObject)mstCombineAppendSkilltmp)["qp"].ToString();
                            var itemIdArray = itemIds.Split(',');
                            var itemNumsArray = itemNums.Split(',');
                            var itemDisplay = "";
                            for (var i = 0; i < itemIdArray.Length; i++)
                                itemDisplay += CheckItemName(itemIdArray[i]) + "(" + itemNumsArray[i] + "),";
                            itemDisplay = itemDisplay.Substring(0, itemDisplay.Length - 1);
                            AppendSkillCombineItems.Dispatcher.Invoke(() =>
                            {
                                AppendSkillCombineItems.Items.Add(
                                    new ItemList(LimitID + " → " + (Convert.ToInt64(LimitID) + 1), itemDisplay,
                                        qp));
                            });
                            break;
                    }
                }
        }

        private string CheckItemName(object ID)
        {
            switch (Convert.ToInt64(ID))
            {
                case 8:
                    return "宝具强化";
                case 9:
                    return "技能强化";
            }

            foreach (var mstItemtmp in GlobalPathsAndDatas.mstItemArray)
            {
                if (((JObject)mstItemtmp)["id"].ToString() != ID.ToString()) continue;
                var mstItemtmpobjtmp = JObject.Parse(mstItemtmp.ToString());
                return mstItemtmpobjtmp["name"].ToString();
            }

            if (ID.ToString().Length == 7) return GetSvtName(ID.ToString(), 0) + "(礼装)";

            return "未知材料" + ID;
        }

        private void ServantCVandIllustCheck()
        {
            var svtillust = "unknown"; //illustID 不输出
            var svtcv = "unknown"; //CVID 不输出
            var svtCVName = "unknown";
            var svtILLUSTName = "unknown";
            foreach (var svtIDtmp in GlobalPathsAndDatas.mstSvtArray)
                if (((JObject)svtIDtmp)["id"].ToString() == JB.svtid)
                {
                    var mstSvtobjtmp = JObject.Parse(svtIDtmp.ToString());
                    svtillust = mstSvtobjtmp["illustratorId"].ToString(); //illustID
                    svtcv = mstSvtobjtmp["cvId"].ToString(); //CVID
                    break;
                }

            foreach (var cvidtmp in GlobalPathsAndDatas.mstCvArray)
                if (((JObject)cvidtmp)["id"].ToString() == svtcv)
                {
                    var mstCVobjtmp = JObject.Parse(cvidtmp.ToString());
                    svtCVName = mstCVobjtmp["name"].ToString();
                    cv.Dispatcher.Invoke(() => { cv.Text = svtCVName; });
                    break;
                }

            foreach (var illustidtmp in GlobalPathsAndDatas.mstIllustratorArray)
                if (((JObject)illustidtmp)["id"].ToString() == svtillust)
                {
                    var mstillustobjtmp = JObject.Parse(illustidtmp.ToString());
                    svtILLUSTName = mstillustobjtmp["name"].ToString();
                    illust.Dispatcher.Invoke(() => { illust.Text = svtILLUSTName; });
                    break;
                }
        }

        private void ServantCardsArrangementCheck() //必须比basic先解析
        {
            var svtArtsCardhit = 1;
            var svtArtsCardhitDamage = "unknown";
            var svtArtsCardAttckType = "";
            var svtBustersCardhit = 1;
            var svtBustersCardhitDamage = "unknown";
            var svtBustersCardAttckType = "";
            var svtQuicksCardhit = 1;
            var svtQuicksCardhitDamage = "unknown";
            var svtQuicksCardAttckType = "";
            var svtExtraCardhit = 1;
            var svtExtraCardhitDamage = "unknown";
            var svtExtraCardAttckType = "";
            GlobalPathsAndDatas.svtArtsCardhit = 1;
            var svtEnemyCommonCardhit = 1;
            var svtEnemyCommonCardhitDamage = "unknown";
            var svtEnemyCommonCardAttckType = "";
            var svtEnemyCriticalCardhit = 1;
            var svtEnemyCriticalCardhitDamage = "unknown";
            var svtEnemyCriticalCardAttckType = "";
            foreach (var svtCardtmp in GlobalPathsAndDatas.mstSvtCardArray)
            {
                if (((JObject)svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject)svtCardtmp)["cardId"].ToString() == "10")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtEnemyCommonCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtEnemyCommonCardhit += svtEnemyCommonCardhitDamage.Count(c => c == ',');
                    svtEnemyCommonCardAttckType = mstSvtCardobjtmp["attackType"].ToString();
                    bustercard.Dispatcher.Invoke(() =>
                    {
                        bustercard.Text = "敌方通常攻击(弱): " + svtEnemyCommonCardhit + " hit " +
                                          svtEnemyCommonCardhitDamage +
                                          (svtEnemyCommonCardAttckType == "2" ? " (群体攻击)" : "");
                    });
                }

                if (((JObject)svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject)svtCardtmp)["cardId"].ToString() == "11")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtEnemyCriticalCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtEnemyCriticalCardhit += svtEnemyCriticalCardhitDamage.Count(c => c == ',');
                    svtEnemyCriticalCardAttckType = mstSvtCardobjtmp["attackType"].ToString();
                    artscard.Dispatcher.Invoke(() =>
                    {
                        artscard.Text = "敌方通常攻击(强): " + svtEnemyCriticalCardhit + " hit " +
                                        svtEnemyCriticalCardhitDamage +
                                        (svtEnemyCriticalCardAttckType == "2" ? " (群体攻击)" : "");
                    });
                }

                if (((JObject)svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject)svtCardtmp)["cardId"].ToString() == "1")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtArtsCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtArtsCardhit += svtArtsCardhitDamage.Count(c => c == ',');
                    svtArtsCardAttckType = mstSvtCardobjtmp["attackType"].ToString();
                    GlobalPathsAndDatas.svtArtsCardhit = svtArtsCardhit;
                    artscard.Dispatcher.Invoke(() =>
                    {
                        artscard.Text = svtArtsCardhit + " hit " + svtArtsCardhitDamage +
                                        (svtArtsCardAttckType == "2" ? " (群体攻击)" : "");
                    });
                }

                if (((JObject)svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject)svtCardtmp)["cardId"].ToString() == "2")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtBustersCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtBustersCardhit += svtBustersCardhitDamage.Count(c => c == ',');
                    svtBustersCardAttckType = mstSvtCardobjtmp["attackType"].ToString();
                    bustercard.Dispatcher.Invoke(() =>
                    {
                        bustercard.Text = svtBustersCardhit + " hit " + svtBustersCardhitDamage +
                                          (svtBustersCardAttckType == "2" ? " (群体攻击)" : "");
                    });
                }

                if (((JObject)svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject)svtCardtmp)["cardId"].ToString() == "3")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtQuicksCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtQuicksCardhit += svtQuicksCardhitDamage.Count(c => c == ',');
                    svtQuicksCardAttckType = mstSvtCardobjtmp["attackType"].ToString();
                    quickcard.Dispatcher.Invoke(() =>
                    {
                        quickcard.Text = svtQuicksCardhit + " hit " + svtQuicksCardhitDamage +
                                         (svtQuicksCardAttckType == "2" ? " (群体攻击)" : "");
                    });
                }

                if (((JObject)svtCardtmp)["svtId"].ToString() != JB.svtid ||
                    ((JObject)svtCardtmp)["cardId"].ToString() != "4") continue;
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtExtraCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtExtraCardhit += svtExtraCardhitDamage.Count(c => c == ',');
                    svtExtraCardAttckType = mstSvtCardobjtmp["attackType"].ToString();
                    extracard.Dispatcher.Invoke(() =>
                    {
                        extracard.Text = svtExtraCardhit + " hit " + svtExtraCardhitDamage +
                                         (svtExtraCardAttckType == "2" ? " (群体攻击)" : "");
                    });
                }
            }
        }

        private void ServantTreasureDeviceSvalCheck(string svtTDID)
        {
            string svtTreasureDeviceFuncID;
            string[] svtTreasureDeviceFuncIDArray = null;
            string[] svtTreasureDeviceFuncArray;
            string[] TDFuncstrArray = null;
            string[] TDlv1OC1strArray = null;
            string[] TDlv2OC2strArray = null;
            string[] TDlv3OC3strArray = null;
            string[] TDlv4OC4strArray = null;
            string[] TDlv5OC5strArray = null;
            var targettmp = "";
            var targetstr = "";
            var tvalstr = "";
            var popupIcon = "";
            var applyTarget = "";
            var funcType = "";
            var tvalstrxls = "";
            var svtTDTargetList = new List<string>();
            string[] svtTDTargetArray;
            var svtTDTarget = string.Empty;
            var svtTDTargetRawList = new List<string>();
            string[] svtTDTargetRawArray;
            var svtTDbufficonList = new List<string>();
            string[] svtTDbufficonArray;
            var svtTDbufficon = string.Empty;
            var svtTDapplyTargetList = new List<string>();
            string[] svtTDapplyTargetArray;
            var svtTDapplyTarget = string.Empty;
            var svtTDfuncTypeList = new List<string>();
            string[] svtTDfuncTypeArray;
            var svtTDfuncType = string.Empty;
            var svtTDtargetIconList = new List<string>();
            string[] svtTDtargetIconArray;
            var svtTDtargetIcon = string.Empty;
            SkillLvs.TDforExcel = "";
            foreach (var TDLVtmp in GlobalPathsAndDatas.mstTreasureDeviceLvArray)
            {
                if (((JObject)TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject)TDLVtmp)["lv"].ToString() == "1")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval1 = TDLVobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval1.Length >= 2) NPval1 = NPval1.Substring(0, NPval1.Length - 2);
                    TDlv1OC1strArray = NPval1.Split('|');
                }

                if (((JObject)TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject)TDLVtmp)["lv"].ToString() == "2")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval2 = TDLVobjtmp["svals2"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval2.Length >= 2) NPval2 = NPval2.Substring(0, NPval2.Length - 2);
                    TDlv2OC2strArray = NPval2.Split('|');
                }

                if (((JObject)TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject)TDLVtmp)["lv"].ToString() == "3")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval3 = TDLVobjtmp["svals3"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval3.Length >= 2) NPval3 = NPval3.Substring(0, NPval3.Length - 2);
                    TDlv3OC3strArray = NPval3.Split('|');
                }

                if (((JObject)TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject)TDLVtmp)["lv"].ToString() == "4")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval4 = TDLVobjtmp["svals4"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval4.Length >= 2) NPval4 = NPval4.Substring(0, NPval4.Length - 2);
                    TDlv4OC4strArray = NPval4.Split('|');
                }

                if (((JObject)TDLVtmp)["treaureDeviceId"].ToString() != svtTDID ||
                    ((JObject)TDLVtmp)["lv"].ToString() != "5") continue;
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval5 = TDLVobjtmp["svals5"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval5.Length >= 2) NPval5 = NPval5.Substring(0, NPval5.Length - 2);
                    TDlv5OC5strArray = NPval5.Split('|');
                    svtTreasureDeviceFuncID = TDLVobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    svtTreasureDeviceFuncIDArray = svtTreasureDeviceFuncID.Split(',');
                }
            }

            try
            {
                Dispatcher.Invoke(() =>
                {
                    var NeedTranslate = false;
                    if (ToggleBuffFuncTranslate.IsChecked == true) NeedTranslate = true;
                    var funcnametmp = "";
                    var TmpList = new List<string>();
                    TmpList.Clear();
                    foreach (var skfuncidtmp in svtTreasureDeviceFuncIDArray)
                    {
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != skfuncidtmp) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            funcnametmp = mstFuncobjtmp["popupText"].ToString();
                            targettmp = mstFuncobjtmp["targetType"].ToString();
                            popupIcon = mstFuncobjtmp["popupIconId"].ToString();
                            applyTarget = mstFuncobjtmp["applyTarget"].ToString();
                            funcType = mstFuncobjtmp["funcType"].ToString();
                            tvalstr = mstFuncobjtmp["tvals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            targetstr = FuncTargetStr(targettmp);
                            tvalstrxls = "「" + FuncTargetDisplayIconStr(targettmp);
                            if (tvalstr != "") targetstr += $"\r\n{CheckUniqueIndividuality(tvalstr)}";
                            if (tvalstr != "")
                                tvalstrxls += $"<{CheckUniqueIndividuality(tvalstr)}>」";
                            else
                                tvalstrxls += "」";
                            if (funcnametmp != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }

                        svtTDTargetList.Add(targetstr);
                        svtTDbufficonList.Add(popupIcon);
                        svtTDapplyTargetList.Add(applyTarget);
                        svtTDTargetRawList.Add(targettmp);
                        svtTDfuncTypeList.Add(funcType);
                        svtTDtargetIconList.Add(tvalstrxls);
                        if (NeedTranslate)
                            TmpList.Add(TranslateBuff(funcnametmp));
                        else
                            TmpList.Add(funcnametmp);
                    }

                    svtTreasureDeviceFuncArray = TmpList.ToArray();

                    TDFuncstrArray = svtTreasureDeviceFuncArray;

                    svtTDbufficonArray = svtTDbufficonList.ToArray();
                    svtTDTargetArray = svtTDTargetList.ToArray();
                    svtTDTargetRawArray = svtTDTargetRawList.ToArray();
                    svtTDapplyTargetArray = svtTDapplyTargetList.ToArray();
                    svtTDfuncTypeArray = svtTDfuncTypeList.ToArray();
                    svtTDtargetIconArray = svtTDtargetIconList.ToArray();
                    svtTDtargetIcon = string.Join(",", svtTDtargetIconArray);
                    //SkillLvs.specialIconXlsTD = svtTDtargetIcon;
                    var bufficonBitmaps = new BitmapImage[svtTDbufficonArray.Length];
                    for (var m = 0; m <= svtTDbufficonArray.Length - 1; m++)
                    {
                        bufficonBitmaps[m] = new BitmapImage(new Uri("bufficons\\bufficon_0.png", UriKind.Relative));
                        try
                        {
                            bufficonBitmaps[m] =
                                new BitmapImage(new Uri($"bufficons\\bufficon_{svtTDbufficonArray[m]}.png",
                                    UriKind.Relative));
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }

                    for (var i = 0; i <= TDFuncstrArray.Length - 1; i++)
                    {
                        if ((TDFuncstrArray[i] == "なし" || (TDFuncstrArray[i] == "" &&
                                                           TDlv1OC1strArray[i].Contains("Hide"))) &&
                            TDlv1OC1strArray[i].Count(c => c == ',') > 0 
                            || (TDFuncstrArray[i] == "" && svtTreasureDeviceFuncIDArray[i] == "21989") 
                            || (TDFuncstrArray[i] == "" && svtTreasureDeviceFuncIDArray[i] == "21990"))
                            TDFuncstrArray[i] = TranslateTDAttackName(svtTreasureDeviceFuncIDArray[i]);

                        if (TDFuncstrArray[i] == "生贄" && svtTreasureDeviceFuncIDArray[i] == "3851")
                            TDFuncstrArray[i] = "活祭";

                        if (TDFuncstrArray[i] == "" && svtTreasureDeviceFuncIDArray[i] == "7011")
                            TDFuncstrArray[i] = "从者位置变更";

                        if (TDFuncstrArray[i] == "" && TDlv1OC1strArray[i].Count(c => c == ',') == 1 &&
                            !TDlv1OC1strArray[i].Contains("Hide")) TDFuncstrArray[i] = "HP回复";

                        if (svtTreasureDeviceFuncIDArray[i] == "5826" || svtTreasureDeviceFuncIDArray[i] == "6323")
                            TDFuncstrArray[i] = "无特效特殊(快速)即死";

                        if (svtTreasureDeviceFuncIDArray[i] == "5") continue;

                        if (svtTreasureDeviceFuncIDArray[i] == "21039") continue;

                        if (ToggleDisplayEnemyFunc.IsChecked == false)
                        {
                            if (svtTDapplyTargetArray[i] == "2")
                            {
                                if (TDFuncstrArray[i].Contains("チャージ増加") || TDFuncstrArray[i].Contains("充能增加") ||
                                    TDFuncstrArray[i].Contains("クリティカル発生") || TDFuncstrArray[i].Contains("暴击发生率") ||
                                    TDFuncstrArray[i].Contains("チャージ減少") || TDFuncstrArray[i].Contains("充能减少") || 
                                    TDFuncstrArray[i].Contains("宝具タイプチェンジ") || TDFuncstrArray[i].Contains("宝具类型改変"))
                                    switch (Convert.ToInt32(svtTDTargetRawArray[i]))
                                    {
                                        case 0:
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 7:
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 14:
                                        case 16:
                                        case 17:
                                        case 18:
                                        case 25:
                                            continue;
                                    }
                                else
                                    continue;
                            }

                            if (svtTDapplyTargetArray[i] == "1")
                                if (TDFuncstrArray[i].Contains("NP増加") || TDFuncstrArray[i].Contains("スター発生") ||
                                    TDFuncstrArray[i].Contains("暴击星掉落率") || TDFuncstrArray[i].Contains("NP减少") ||
                                    TDFuncstrArray[i].Contains("NP減少"))
                                    switch (Convert.ToInt32(svtTDTargetRawArray[i]))
                                    {
                                        case 4:
                                        case 5:
                                        case 6:
                                        case 7:
                                        case 12:
                                        case 13:
                                        case 15:
                                        case 20:
                                        case 27:
                                            continue;
                                    }
                        }

                        if (isTDFunc(svtTreasureDeviceFuncIDArray[i]))
                        {
                            if (ToggleFuncDiffer.IsChecked != true) return;
                            TDlv2OC2strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv2OC2strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv3OC3strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv3OC3strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv4OC4strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv4OC4strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            var tmp = ModifyFuncSvalDisplay.TDStrForExcel(svtTreasureDeviceFuncIDArray[i],
                                TDlv1OC1strArray[i], TDlv5OC5strArray[i], TDFuncstrArray[i].Replace("\r\n", ""));
                            if (tmp != "false")
                            {
                                SkillLvs.TDforExcel += tmp;
                            }
                            else
                            {
                                if (ToggleFuncDiffer.IsChecked != true) return;
                                TDlv1OC1strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                    TDlv1OC1strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                                TDlv5OC5strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                    TDlv5OC5strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                                SkillLvs.TDforExcel += (TDFuncstrArray[i] != ""
                                                           ? TDFuncstrArray[i].Replace("\r\n", "")
                                                           : "未知效果") +
                                                       " 【{" + (TDlv1OC1strArray[i].Replace("\r\n", " ") ==
                                                                TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                           ? TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                           : TDlv1OC1strArray[i].Replace("\r\n", " ") + "} - {" +
                                                             TDlv5OC5strArray[i].Replace("\r\n", " ")) + "}】\r\n";
                                /*if (npdetail.Text == "" || npdetail.Text == "unknown" || npdetail.Text == "该宝具暂时没有描述.")
                                {
                                    SkillLvs.TDforExcel += (TDFuncstrArray[i] != ""
                                                               ? svtTDtargetIconArray[i] +
                                                                 (svtTDTargetArray[i].Contains("[") && !svtTDtargetIconArray[i].Contains("<特殊·参考实际情况>") ? "·<特殊·参考实际情况> " : " ") +
                                                                 TDFuncstrArray[i].Replace("\r\n", "")
                                                               : svtTDtargetIconArray[i] + "未知效果") +
                                                           " 【{" + (TDlv1OC1strArray[i].Replace("\r\n", " ") ==
                                                                    TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                               ? TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                               : TDlv1OC1strArray[i].Replace("\r\n", " ") + "} - {" +
                                                                 TDlv5OC5strArray[i].Replace("\r\n", " ")) + "}】\r\n";
                                }
                                else
                                {
                                    SkillLvs.TDforExcel += (TDFuncstrArray[i] != ""
                                                               ? TDFuncstrArray[i].Replace("\r\n", "")
                                                               : "未知效果") +
                                                           " 【{" + (TDlv1OC1strArray[i].Replace("\r\n", " ") ==
                                                                    TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                               ? TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                               : TDlv1OC1strArray[i].Replace("\r\n", " ") + "} - {" +
                                                                 TDlv5OC5strArray[i].Replace("\r\n", " ")) + "}】\r\n";
                                }*/
                            }

                            if (ToggleFuncDiffer.IsChecked != true) return;
                            TDlv1OC1strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv1OC1strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv5OC5strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv5OC5strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                        }
                        else
                        {
                            if (ToggleFuncDiffer.IsChecked != true) return;
                            TDlv1OC1strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv1OC1strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv2OC2strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv2OC2strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv3OC3strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv3OC3strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv4OC4strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv4OC4strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDlv5OC5strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                                TDlv5OC5strArray[i], ToggleDisplayEnemyFunc.IsChecked == false);
                            TDFuncstrArray[i] = translateOtherFunc(TDFuncstrArray[i]);
                            if (npdetail.Text == "" || npdetail.Text == "unknown" || npdetail.Text == "该宝具暂时没有描述.")
                                SkillLvs.TDforExcel += (TDFuncstrArray[i] != ""
                                                           ? svtTDtargetIconArray[i] +
                                                             TDFuncstrArray[i].Replace("\r\n", "")
                                                           : svtTDtargetIconArray[i] + "未知效果") +
                                                       " 【{" + (TDlv1OC1strArray[i].Replace("\r\n", " ") ==
                                                                TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                           ? TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                           : TDlv1OC1strArray[i].Replace("\r\n", " ") + "} - {" +
                                                             TDlv5OC5strArray[i].Replace("\r\n", " ")) + "}】\r\n";
                            else
                                SkillLvs.TDforExcel += (TDFuncstrArray[i] != ""
                                                           ? TDFuncstrArray[i].Replace("\r\n", "")
                                                           : "未知效果") +
                                                       " 【{" + (TDlv1OC1strArray[i].Replace("\r\n", " ") ==
                                                                TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                           ? TDlv5OC5strArray[i].Replace("\r\n", " ")
                                                           : TDlv1OC1strArray[i].Replace("\r\n", " ") + "} - {" +
                                                             TDlv5OC5strArray[i].Replace("\r\n", " ")) + "}】\r\n";
                        }

                        var DisplaySval = TDlv1OC1strArray[i] == TDlv5OC5strArray[i]
                            ? $"固定: {TDlv5OC5strArray[i]}"
                            : $"Lv.1/OC1: {TDlv1OC1strArray[i]}\r\nLv.2/OC2: {TDlv2OC2strArray[i]}\r\nLv.3/OC3: {TDlv3OC3strArray[i]}\r\nLv.4/OC4: {TDlv4OC4strArray[i]}\r\nLv.5/OC5: {TDlv5OC5strArray[i]}";
                        if (isDisplayFuncType.IsChecked == true)
                            TDFuncList.Items.Add(new TDlistSval(
                                TDFuncstrArray[i] != ""
                                    ? TDFuncstrArray[i] + $"\r\n({FindFuncTypeNameDebugger(svtTDfuncTypeArray[i])})"
                                    : "未知效果",
                                DisplaySval, svtTDTargetArray[i], bufficonBitmaps[i]));
                        else
                            TDFuncList.Items.Add(new TDlistSval(TDFuncstrArray[i] != "" ? TDFuncstrArray[i] : "未知效果",
                                DisplaySval, svtTDTargetArray[i], bufficonBitmaps[i]));
                    }

                    try
                    {
                        SkillLvs.TDforExcel = SkillLvs.TDforExcel.Substring(0, SkillLvs.TDforExcel.Length - 2);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                });
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static string translateOtherFunc(string origin_str) //替换部分历史遗留原因无法准确翻译的内容（（（
        {
            origin_str = origin_str.Replace("防御無視", "防御无视").Replace("無敵貫通", "无敌贯通")
                .Replace("無効", "无效").Replace("強化", "强化").Replace("無敵", "无敌").Replace("攻撃", "攻击").Replace("減少", "减少")
                .Replace("対", "对").Replace("異常", "异常").Replace("待機", "待机").Replace("呪厄", "咒厄").Replace("効果", "效果").Replace("無視", "无视")
                .Replace("魅了", "魅惑").Replace("延焼", "延烧").Replace("攻击時", "攻击时").Replace("状態", "状态").Replace("発動", "发动").Replace("消費", "消费");
            return origin_str;
        }

        private static bool isTDFunc(string funcid)
        {
            try
            {
                foreach (var TdAttackNmeTmp in GlobalPathsAndDatas.TDAttackNameTranslation)
                    if (((JObject)TdAttackNmeTmp)["id"].ToString() == funcid)
                    {
                        var tdFuncName = JObject.Parse(TdAttackNmeTmp.ToString());
                        return true;
                    }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void RefreshTranslationsList()
        {
            GlobalPathsAndDatas.SvtIndividualityTranslation =
                File.Exists(GlobalPathsAndDatas.gamedata.FullName + "SvtIndividualityTranslation.json")
                    ? (JArray)JsonConvert.DeserializeObject(File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName +
                                                                             "SvtIndividualityTranslation.json"))
                    : (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(IndividualListLinkA,
                        IndividualListLinkB));
            GlobalPathsAndDatas.TDAttackNameTranslation =
                File.Exists(GlobalPathsAndDatas.gamedata.FullName + "TDAttackNameTranslation.json")
                    ? (JArray)JsonConvert.DeserializeObject(
                        File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "TDAttackNameTranslation.json"))
                    : (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(TDAttackNameTranslationListLinkA,
                        TDAttackNameTranslationListLinkB));
            GlobalPathsAndDatas.appendSkillTranslationArray =
                File.Exists(GlobalPathsAndDatas.gamedata.FullName + "AppendSkillTranslation.json")
                    ? (JArray)JsonConvert.DeserializeObject(
                        File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "AppendSkillTranslation.json"))
                    : (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(AppendSkillTranslationLinkA,
                        AppendSkillTranslationLinkB));
            var tmpFuncTypeData = File.Exists(GlobalPathsAndDatas.gamedata.FullName + "FuncList.json")
                ? File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "FuncList.json")
                : HttpRequest.GetList(FuncListLinkA, FuncListLinkB);
            GlobalPathsAndDatas.funcListDebuggerArray =
                (JArray)JsonConvert.DeserializeObject(tmpFuncTypeData);
            if (File.Exists(GlobalPathsAndDatas.gamedata.FullName + "BuffTranslation.json"))
                GlobalPathsAndDatas.TranslationList =
                    (JArray)JsonConvert.DeserializeObject(
                        File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "BuffTranslation.json"));
            else
                GlobalPathsAndDatas.TranslationList =
                    (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(BuffTranslationListLinkA,
                        BuffTranslationListLinkB));
        }

        private void UpdateTranslationData(object sender, RoutedEventArgs e)
        {
            UTD();
        }

        private void UTD()
        {
            GlobalPathsAndDatas.TDAttackNameTranslation = (JArray)JsonConvert.DeserializeObject(HttpRequest
                .GetList(TDAttackNameTranslationListLinkA, TDAttackNameTranslationListLinkB));
            GlobalPathsAndDatas.SvtIndividualityTranslation =
                (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(IndividualListLinkA, IndividualListLinkB));
            GlobalPathsAndDatas.TranslationList =
                (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(BuffTranslationListLinkA,
                    BuffTranslationListLinkB));
            var tmpFuncTypeData = HttpRequest.GetList(FuncListLinkA, FuncListLinkB);
            GlobalPathsAndDatas.funcListDebuggerArray =
                (JArray)JsonConvert.DeserializeObject(tmpFuncTypeData);
            GlobalPathsAndDatas.appendSkillTranslationArray =
                (JArray)JsonConvert.DeserializeObject(HttpRequest.GetList(AppendSkillTranslationLinkA,
                    AppendSkillTranslationLinkB));
            File.WriteAllText(GlobalPathsAndDatas.gamedata.FullName + "SvtIndividualityTranslation.json",
                HttpRequest.GetList(IndividualListLinkA, IndividualListLinkB));
            File.WriteAllText(GlobalPathsAndDatas.gamedata.FullName + "TDAttackNameTranslation.json",
                HttpRequest
                    .GetList(TDAttackNameTranslationListLinkA, TDAttackNameTranslationListLinkB));
            File.WriteAllText(GlobalPathsAndDatas.gamedata.FullName + "BuffTranslation.json",
                HttpRequest.GetList(BuffTranslationListLinkA, BuffTranslationListLinkB));
            File.WriteAllText(GlobalPathsAndDatas.gamedata.FullName + "FuncList.json",
                tmpFuncTypeData);
            File.WriteAllText(GlobalPathsAndDatas.gamedata.FullName + "AppendSkillTranslation.json",
                HttpRequest.GetList(AppendSkillTranslationLinkA,
                    AppendSkillTranslationLinkB));
            Dispatcher.Invoke(() => { Growl.Info("翻译列表更新完成."); });
            GC.Collect();
        }

        private string TranslateTDAttackName(string TDFuncID)
        {
            try
            {
                foreach (var TdAttackNmeTmp in GlobalPathsAndDatas.TDAttackNameTranslation)
                    if (((JObject)TdAttackNmeTmp)["id"].ToString() == TDFuncID)
                    {
                        var tdFuncName = JObject.Parse(TdAttackNmeTmp.ToString());
                        return tdFuncName["tdFuncname"].ToString();
                    }

                return "未知Func\r\nID: " + TDFuncID;
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        Application.Current.MainWindow, "翻译列表损坏.\r\n" + e, "错误", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
                return "FuncID: " + TDFuncID;
            }
        }

        public static string TranslateBuff(string buffname)
        {
            try
            {
                foreach (var buffNmeTmp in GlobalPathsAndDatas.TranslationList)
                    if (buffname.Contains(((JObject)buffNmeTmp)["buffStr"].ToString()))
                    {
                        var buffNameObj = JObject.Parse(buffNmeTmp.ToString());
                        buffname = buffname.Replace(((JObject)buffNmeTmp)["buffStr"].ToString(),
                            ((JObject)buffNmeTmp)["buffTrans"].ToString());
                    }

                return buffname;
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        Application.Current.MainWindow, "翻译列表损坏.\r\n" + e, "错误", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
                return buffname;
            }
        }

        private void CheckSvtIsFullinGame(object classid)
        {
            Dispatcher.Invoke(() =>
            {
                if (collection.Text != "0" || cards.Text == "[Q,Q,Q,Q,Q]") return;
                switch (Convert.ToInt64(classid))
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 23:
                    case 25:
                    case 28:
                        Growl.Info("该从者尚未实装或为敌方数据,故最终实装的数据可能会与目前的解析结果不同,请以实装之后的数据为准!望知悉.");
                        break;
                }
            });
        }

        private void ServantClassPassiveSkillCheck(string ClassPassiveID)
        {
            SkillLvs.ClassPassiveforExcel = "";
            try
            {
                string[] svtClassPassiveIDArray = null;
                string[] svtClassPassiveArray;
                var svtClassPassive = string.Empty;
                var Extratmp = ServantClassPassiveSkillExtraCheck(JB.svtid);
                var ClassPassiveIDTrue = ClassPassiveID + (Extratmp == "" ? "" : "," + Extratmp);
                svtClassPassiveIDArray = ClassPassiveIDTrue.Split(',');
                var tmpList = new List<string>();
                foreach (var classpassiveidtmp in svtClassPassiveIDArray)
                foreach (var skilltmp in GlobalPathsAndDatas.mstSkillArray)
                {
                    if (((JObject)skilltmp)["id"].ToString() != classpassiveidtmp) continue;
                    var mstsvtPskillobjtmp = JObject.Parse(skilltmp.ToString());
                    var AddVals = SkillAddNameCheck(classpassiveidtmp);
                    tmpList.Add(mstsvtPskillobjtmp["name"].ToString() != ""
                        ? mstsvtPskillobjtmp["name"] + (AddVals == "" ? "" : "[满足特定条件后显示为\"" + AddVals + "\"]") + " (" +
                          mstsvtPskillobjtmp["id"] + ")"
                        : "未知技能???" + (AddVals == "" ? "" : "[满足特定条件后显示为\"" + AddVals + "\"]") + " (" +
                          mstsvtPskillobjtmp["id"] + ")");
                    break;
                }

                svtClassPassiveArray = tmpList.ToArray();
                svtClassPassive = string.Join(", ", svtClassPassiveArray);
                classskill.Dispatcher.Invoke(() => { classskill.Text = svtClassPassive; });
                var SCPSSLC = new Task(() => { ServantClassPassiveSkillSvalListCheck(ClassPassiveIDTrue); });
                SCPSSLC.Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private string ServantClassPassiveSkillExtraCheck(string svtID)
        {
            var ExtraList = (from skilltmp in GlobalPathsAndDatas.mstSvtPassiveSkillArray
                where ((JObject)skilltmp)["svtId"].ToString() == svtID
                where ((JObject)skilltmp)["skillId"].ToString().Substring(0, 1) != "9" ||
                      ((JObject)skilltmp)["skillId"].ToString().Length != 6
                select JObject.Parse(skilltmp.ToString())
                into mstsvtPskillobjtmp
                select mstsvtPskillobjtmp["skillId"].ToString()).ToList();
            if (ExtraList.Count == 0) return "";
            var result = string.Join(",", ExtraList);
            return result;
        }

        private string SkillAddNameCheck(string skillid)
        {
            var result = "";
            foreach (var mstSkillAddtmp in GlobalPathsAndDatas.mstSkillAddArray)
            {
                if (((JObject)mstSkillAddtmp)["skillId"].ToString() != skillid) continue;
                result = ((JObject)mstSkillAddtmp)["name"].ToString();
                break;
            }

            return result;
        }

        private void ServantClassPassiveSkillSvalListCheck(string ClassPassiveID)
        {
            try
            {
                var svtClassPassiveIDListArray = ClassPassiveID.Split(',');
                var ClassPassiveSkillFuncName = "";
                var NeedTranslate = false;
                string[] lv10svalArray = null;
                ToggleBuffFuncTranslate.Dispatcher.Invoke(() =>
                {
                    if (ToggleBuffFuncTranslate.IsChecked == true) NeedTranslate = true;
                });
                for (var i = 0; i <= svtClassPassiveIDListArray.Length - 1; i++)
                {
                    foreach (var skilltmp in GlobalPathsAndDatas.mstSkillArray)
                    {
                        if (((JObject)skilltmp)["id"].ToString() != svtClassPassiveIDListArray[i]) continue;
                        var skillobjtmp = JObject.Parse(skilltmp.ToString());
                        var AddVals = SkillAddNameCheck(svtClassPassiveIDListArray[i]);
                        ClassPassiveSkillFuncName = NeedTranslate
                            ? TranslateBuff(skillobjtmp["name"].ToString()) +
                              (AddVals == "" ? "" : "\r\n(" + AddVals + ")")
                            : skillobjtmp["name"].ToString();
                    }

                    var CPDetail = "";
                    foreach (var skillDetailtmp in GlobalPathsAndDatas.mstSkillDetailArray)
                    {
                        if (((JObject)skillDetailtmp)["id"].ToString() != svtClassPassiveIDListArray[i]) continue;
                        var skillobjtmp = JObject.Parse(skillDetailtmp.ToString());
                        CPDetail = skillobjtmp["detail"].ToString();
                        break;
                    }

                    var SkillSquare = GetSvtClassPassiveSval(svtClassPassiveIDListArray[i]);
                    lv10svalArray = SkillSquare[0].Split('|');
                    var SKLFuncstrArray = SkillSquare[1].Replace(" ", "").Split(',');
                    for (var j = 0; j <= SKLFuncstrArray.Length - 1; j++)
                    {
                        if (SKLFuncstrArray[j] == "" &&
                            lv10svalArray[j].Count(c => c == ',') == 1 &&
                            !lv10svalArray[j].Contains("Hide"))
                            SKLFuncstrArray[j] = "HP回复";
                        if (SKLFuncstrArray[j] == "" &&
                            (lv10svalArray[j].Contains("ShowQuestNoEffect") || lv10svalArray[j].Contains("1000,-1,-1")))
                            SKLFuncstrArray[j] = "∅";
                        if (SKLFuncstrArray[j] == "" &&
                            lv10svalArray[j].Contains("ShowState") && lv10svalArray[j].Contains("5000,-1,-1"))
                            SKLFuncstrArray[j] = "∅";
                        ToggleFuncDiffer.Dispatcher.Invoke(() =>
                        {
                            if (ToggleFuncDiffer.IsChecked == true)
                                lv10svalArray[j] =
                                    ModifyFuncSvalDisplay.ModifyFuncStr(SKLFuncstrArray[j],
                                        lv10svalArray[j], ToggleDisplayEnemyFunc.IsChecked == false);
                        });
                    }

                    var tmpexcelText = "";
                    for (var k = 0; k <= SKLFuncstrArray.Length - 1; k++)
                    {
                        if (svtClassPassiveIDListArray[i] == "2342350" && k == 3) break; //屏蔽有珠职介技能显示
                        if (lv10svalArray[k].Contains("5000,-1,-1,OnFieldCount:-1,ShowState:-1")) continue;
                        SKLFuncstrArray[k] = translateOtherFunc(SKLFuncstrArray[k]);
                        tmpexcelText += SKLFuncstrArray[k] + "[" + lv10svalArray[k].Replace("\r\n", "") + "]" + " & ";
                    }

                    try
                    {
                        tmpexcelText = tmpexcelText.Substring(0, tmpexcelText.Length - 3);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    SkillLvs.ClassPassiveforExcel += "「" +
                                                     (i + 1) + "」 " + ClassPassiveSkillFuncName.Replace("\r\n", "") +
                                                     " (" +
                                                     svtClassPassiveIDListArray[i] + ")" + " |【描述】: " + CPDetail +
                                                     " 【效果】: " + tmpexcelText +
                                                     "\r\n";
                    var FuncStr = "\r\n" + string.Join("\r\n", SKLFuncstrArray) + "\r\n";
                    var SvalStr = "\r\n";
                    for (var q = 0; q < lv10svalArray.Length; q++)
                    {
                        if (svtClassPassiveIDListArray[i] == "2342350" && q == 3) break; //屏蔽有珠职介技能显示
                        if (lv10svalArray[q].Contains("5000,-1,-1,OnFieldCount:-1,ShowState:-1")) continue;
                        SvalStr += SKLFuncstrArray[q] + $"[{lv10svalArray[q]}]" + "\r\n";
                    }

                    ClassPassiveFuncList.Dispatcher.Invoke(() =>
                    {
                        var fixedIndex = (CPDetail.Length - CPDetail.Length % 24) / 24;
                        var fixedDetail = CPDetail;
                        if (fixedIndex > 0)
                            for (var j = 1; j <= fixedIndex; j++)
                                fixedDetail = fixedDetail.Insert(24 * j - 1, "\r\n");
                        /*var fixedDetail = CPDetail.Length > 100 ? CPDetail.Insert(24, "\r\n").Insert(49, "\r\n").Insert(74, "\r\n").Insert(99, "\r\n") : CPDetail.Length > 75 ? CPDetail.Insert(24, "\r\n").Insert(49, "\r\n").Insert(74, "\r\n") : CPDetail.Length > 50
                            ? CPDetail.Insert(24, "\r\n").Insert(49, "\r\n")
                            : CPDetail.Length > 25
                                ? CPDetail.Insert(24, "\r\n")
                                : CPDetail;*/
                        ClassPassiveFuncList.Items.Add(new ClassPassiveSvalList(ClassPassiveSkillFuncName,
                            svtClassPassiveIDListArray[i], fixedDetail, SvalStr));
                    });
                }

                try
                {
                    SkillLvs.ClassPassiveforExcel =
                        SkillLvs.ClassPassiveforExcel.Substring(0, SkillLvs.ClassPassiveforExcel.Length - 2);
                }
                catch (Exception)
                {
                    //ignore
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private string[] GetSvtClassPassiveSval(string SkillID)
        {
            var skilllv10sval = "";
            string svtSKFuncID;
            string[] svtSKFuncIDArray;
            List<string> svtSKFuncIDList;
            var svtSKFuncList = new List<string>();
            string[] svtSKFuncArray;
            var svtSKFunc = string.Empty;
            var NeedTranslate = false;
            Dispatcher.Invoke(() => { NeedTranslate = ToggleBuffFuncTranslate.IsChecked == true; });
            foreach (var SKLTMP in GlobalPathsAndDatas.mstSkillLvArray)
            {
                if (((JObject)SKLTMP)["skillId"].ToString() == SkillID &&
                    ((JObject)SKLTMP)["lv"].ToString() == "1")
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv10sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv10sval = skilllv10sval.Substring(0, skilllv10sval.Length - 2);
                    skilllv10sval =
                        skilllv10sval.Replace(
                            "|1000,-1,-1,970496,Value2:1,ShowState:-1|1000,-1,-1,970497,Value2:1,ShowState:-1|1000,-1,-1,970505,Value2:1,ShowState:-1",
                            ""); //屏蔽青子被动检测
                    svtSKFuncID = SKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    svtSKFuncIDList = new List<string>(svtSKFuncID.Split(','));
                    svtSKFuncIDArray = svtSKFuncIDList.ToArray();
                    if (NeedTranslate)
                    {
                        var funcnametmp = "";
                        foreach (var skfuncidtmp in svtSKFuncIDArray)
                        {
                            if (skfuncidtmp == "21663" || skfuncidtmp == "21664" || skfuncidtmp == "21697")
                                continue; //屏蔽青子被动检测
                            foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                            {
                                if (((JObject)functmp)["id"].ToString() != skfuncidtmp) continue;
                                var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                                funcnametmp = mstFuncobjtmp["popupText"].ToString();
                                if (funcnametmp != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                                var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                    .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                                foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                                {
                                    if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                    funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                                    break;
                                }
                            }

                            svtSKFuncList.Add(TranslateBuff(funcnametmp));
                        }
                    }
                    else
                    {
                        var funcnametmp = "";
                        foreach (var skfuncidtmp in svtSKFuncIDArray)
                        {
                            foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                            {
                                if (((JObject)functmp)["id"].ToString() != skfuncidtmp) continue;
                                var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                                funcnametmp = mstFuncobjtmp["popupText"].ToString();
                                if (funcnametmp != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                                var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                    .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                                foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                                {
                                    if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                    funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                                    break;
                                }
                            }

                            svtSKFuncList.Add(funcnametmp);
                        }
                    }

                    break;
                }

                if (((JObject)SKLTMP)["skillId"].ToString() == SkillID &&
                    ((JObject)SKLTMP)["lv"].ToString() == "10")
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv10sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv10sval = skilllv10sval.Substring(0, skilllv10sval.Length - 2);
                    skilllv10sval =
                        skilllv10sval.Replace(
                            "|1000,-1,-1,970496,Value2:1,ShowState:-1|1000,-1,-1,970497,Value2:1,ShowState:-1|1000,-1,-1,970505,Value2:1,ShowState:-1",
                            ""); //屏蔽青子被动检测
                    svtSKFuncID = SKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    svtSKFuncIDList = new List<string>(svtSKFuncID.Split(','));
                    svtSKFuncIDArray = svtSKFuncIDList.ToArray();
                    if (NeedTranslate)
                    {
                        var funcnametmp = "";
                        foreach (var skfuncidtmp in svtSKFuncIDArray)
                        {
                            if (skfuncidtmp == "21663" || skfuncidtmp == "21664" || skfuncidtmp == "21697")
                                continue; //屏蔽青子被动检测
                            foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                            {
                                if (((JObject)functmp)["id"].ToString() != skfuncidtmp) continue;
                                var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                                funcnametmp = mstFuncobjtmp["popupText"].ToString();
                                if (funcnametmp != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                                var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                    .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                                foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                                {
                                    if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                    funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                                    break;
                                }
                            }

                            svtSKFuncList.Add(TranslateBuff(funcnametmp));
                        }
                    }
                    else
                    {
                        var funcnametmp = "";
                        foreach (var skfuncidtmp in svtSKFuncIDArray)
                        {
                            foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                            {
                                if (((JObject)functmp)["id"].ToString() != skfuncidtmp) continue;
                                var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                                funcnametmp = mstFuncobjtmp["popupText"].ToString();
                                if (funcnametmp != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                                var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                    .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                                foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                                {
                                    if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                    funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                                    break;
                                }
                            }

                            svtSKFuncList.Add(funcnametmp);
                        }
                    }

                    break;
                }
            }

            svtSKFuncArray = svtSKFuncList.ToArray();
            svtSKFunc = string.Join(", ", svtSKFuncArray);
            var result = new string[2];
            result[0] = skilllv10sval;
            result[1] = svtSKFunc;
            return result;
        }

        private void SvtAppendSkillCheck(string svtID)
        {
            var ASID1 = "";
            var ASID2 = "";
            var ASID3 = "";
            var ASID4 = "";
            var ASID5 = "";
            var AS1NME = "";
            var AS2NME = "";
            var AS3NME = "";
            var AS4NME = "";
            var AS5NME = "";
            var AS1DTL = "";
            var AS2DTL = "";
            var AS3DTL = "";
            var AS4DTL = "";
            var AS5DTL = "";
            GlobalPathsAndDatas.AS1D = "";
            GlobalPathsAndDatas.AS1N = "";
            GlobalPathsAndDatas.AS2D = "";
            GlobalPathsAndDatas.AS2N = "";
            GlobalPathsAndDatas.AS3D = "";
            GlobalPathsAndDatas.AS3N = "";
            GlobalPathsAndDatas.AS4D = "";
            GlobalPathsAndDatas.AS4N = "";
            GlobalPathsAndDatas.AS5D = "";
            GlobalPathsAndDatas.AS5N = "";
            foreach (var item in GlobalPathsAndDatas.mstSvtAppendPassiveSkillArray)
                if (((JObject)item)["svtId"].ToString() == svtID &&
                    ((JObject)item)["num"].ToString() == "100" &&
                    ((JObject)item)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(item.ToString());
                    ASID1 = mstsvtskillobjtmp["skillId"].ToString();
                }
                else if (((JObject)item)["svtId"].ToString() == svtID &&
                         ((JObject)item)["num"].ToString() == "101" &&
                         ((JObject)item)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(item.ToString());
                    ASID2 = mstsvtskillobjtmp["skillId"].ToString();
                }
                else if (((JObject)item)["svtId"].ToString() == svtID &&
                         ((JObject)item)["num"].ToString() == "102" &&
                         ((JObject)item)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(item.ToString());
                    ASID3 = mstsvtskillobjtmp["skillId"].ToString();
                }
                else if (((JObject)item)["svtId"].ToString() == svtID &&
                         ((JObject)item)["num"].ToString() == "103" &&
                         ((JObject)item)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(item.ToString());
                    ASID4 = mstsvtskillobjtmp["skillId"].ToString();
                }
                else if (((JObject)item)["svtId"].ToString() == svtID &&
                         ((JObject)item)["num"].ToString() == "104" &&
                         ((JObject)item)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(item.ToString());
                    ASID5 = mstsvtskillobjtmp["skillId"].ToString();
                    break;
                }

            if (ASID1 == "") return;

            foreach (var asTranslationName in GlobalPathsAndDatas.appendSkillTranslationArray)
                if (((JObject)asTranslationName)["id"].ToString() == ASID1)
                {
                    var asTranslationNameobjtmp = JObject.Parse(asTranslationName.ToString());
                    AS1NME = asTranslationNameobjtmp["name"].ToString();
                    AS1DTL = asTranslationNameobjtmp["translation"].ToString();
                    GlobalPathsAndDatas.AS1N = AS1NME;
                }
                else if (((JObject)asTranslationName)["id"].ToString() == ASID2)
                {
                    var asTranslationNameobjtmp = JObject.Parse(asTranslationName.ToString());
                    AS2NME = asTranslationNameobjtmp["name"].ToString();
                    AS2DTL = asTranslationNameobjtmp["translation"].ToString();
                    GlobalPathsAndDatas.AS2N = AS2NME;
                }
                else if (((JObject)asTranslationName)["id"].ToString() == ASID3)
                {
                    var asTranslationNameobjtmp = JObject.Parse(asTranslationName.ToString());
                    AS3NME = asTranslationNameobjtmp["name"].ToString();
                    AS3DTL = asTranslationNameobjtmp["translation"].ToString();
                    GlobalPathsAndDatas.AS3N = AS3NME;
                }
                else if (((JObject)asTranslationName)["id"].ToString() == ASID4)
                {
                    var asTranslationNameobjtmp = JObject.Parse(asTranslationName.ToString());
                    AS4NME = asTranslationNameobjtmp["name"].ToString();
                    AS4DTL = asTranslationNameobjtmp["translation"].ToString();
                    GlobalPathsAndDatas.AS4N = AS4NME;
                }
                else if (((JObject)asTranslationName)["id"].ToString() == ASID5)
                {
                    var asTranslationNameobjtmp = JObject.Parse(asTranslationName.ToString());
                    AS5NME = asTranslationNameobjtmp["name"].ToString();
                    AS5DTL = asTranslationNameobjtmp["translation"].ToString();
                    GlobalPathsAndDatas.AS5N = AS5NME;
                    break;
                }

            foreach (var mstSkilltmp in GlobalPathsAndDatas.mstSkillArray)
                if (((JObject)mstSkilltmp)["id"].ToString() == ASID1 && AS1NME == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkilltmp.ToString());
                    AS1NME = mstsvtskillobjtmp["name"].ToString();
                    GlobalPathsAndDatas.AS1N = AS1NME;
                }
                else if (((JObject)mstSkilltmp)["id"].ToString() == ASID2 && AS2NME == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkilltmp.ToString());
                    AS2NME = mstsvtskillobjtmp["name"].ToString();
                    GlobalPathsAndDatas.AS2N = AS2NME;
                }
                else if (((JObject)mstSkilltmp)["id"].ToString() == ASID3 && AS3NME == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkilltmp.ToString());
                    AS3NME = mstsvtskillobjtmp["name"].ToString();
                    GlobalPathsAndDatas.AS3N = AS3NME;
                }
                else if (((JObject)mstSkilltmp)["id"].ToString() == ASID4 && AS4NME == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkilltmp.ToString());
                    AS4NME = mstsvtskillobjtmp["name"].ToString();
                    GlobalPathsAndDatas.AS4N = AS4NME;
                }
                else if (((JObject)mstSkilltmp)["id"].ToString() == ASID5 && AS5NME == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkilltmp.ToString());
                    AS5NME = mstsvtskillobjtmp["name"].ToString();
                    GlobalPathsAndDatas.AS5N = AS5NME;
                    break;
                }

            foreach (var mstSkillDetailtmp in GlobalPathsAndDatas.mstSkillDetailArray)
                if (((JObject)mstSkillDetailtmp)["id"].ToString() == ASID1 && AS1DTL == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkillDetailtmp.ToString());
                    AS1DTL = mstsvtskillobjtmp["detailShort"].ToString().Replace("[{0}]", "");
                }
                else if (((JObject)mstSkillDetailtmp)["id"].ToString() == ASID2 && AS2DTL == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkillDetailtmp.ToString());
                    AS2DTL = mstsvtskillobjtmp["detailShort"].ToString().Replace("[{0}]", "");
                }
                else if (((JObject)mstSkillDetailtmp)["id"].ToString() == ASID3 && AS3DTL == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkillDetailtmp.ToString());
                    AS3DTL = mstsvtskillobjtmp["detailShort"].ToString().Replace("[{0}]", "");
                }
                else if (((JObject)mstSkillDetailtmp)["id"].ToString() == ASID4 && AS4DTL == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkillDetailtmp.ToString());
                    AS4DTL = mstsvtskillobjtmp["detailShort"].ToString().Replace("[{0}]", "");
                }
                else if (((JObject)mstSkillDetailtmp)["id"].ToString() == ASID5 && AS5DTL == "")
                {
                    var mstsvtskillobjtmp = JObject.Parse(mstSkillDetailtmp.ToString());
                    AS5DTL = mstsvtskillobjtmp["detailShort"].ToString().Replace("[{0}]", "");
                }

            var AS1Fid = "";
            var AS2Fid = "";
            var AS3Fid = "";
            var AS4Fid = "";
            var AS5Fid = "";
            var AS1sval_1 = "";
            var AS2sval_1 = "";
            var AS3sval_1 = "";
            var AS4sval_1 = "";
            var AS5sval_1 = "";
            var AS1sval_10 = "";
            var AS2sval_10 = "";
            var AS3sval_10 = "";
            var AS4sval_10 = "";
            var AS5sval_10 = "";
            var AS1Fnme = "";
            var AS2Fnme = "";
            var AS3Fnme = "";
            var AS4Fnme = "";
            var AS5Fnme = "";

            foreach (var ASSKLTMP in GlobalPathsAndDatas.mstSkillLvArray)
            {
                var ASSKLobjtmp = JObject.Parse(ASSKLTMP.ToString());
                if (ASSKLobjtmp["skillId"].ToString() == ASID1)
                {
                    if (ASSKLobjtmp["lv"].ToString() == "1")
                    {
                        AS1sval_1 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS1sval_1 = AS1sval_1.Substring(0, AS1sval_1.Length - 2);
                        AS1Fid = ASSKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != AS1Fid) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            AS1Fnme = mstFuncobjtmp["popupText"].ToString();
                            if (AS1Fnme != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                AS1Fnme = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }
                    }
                    else if (ASSKLobjtmp["lv"].ToString() == "10")
                    {
                        AS1sval_10 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS1sval_10 = AS1sval_10.Substring(0, AS1sval_10.Length - 2);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (ASSKLobjtmp["skillId"].ToString() == ASID2)
                {
                    if (ASSKLobjtmp["lv"].ToString() == "1")
                    {
                        AS2sval_1 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS2sval_1 = AS2sval_1.Substring(0, AS2sval_1.Length - 2);
                        AS2Fid = ASSKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != AS2Fid) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            AS2Fnme = mstFuncobjtmp["popupText"].ToString();
                            if (AS2Fnme != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                AS2Fnme = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }
                    }
                    else if (ASSKLobjtmp["lv"].ToString() == "10")
                    {
                        AS2sval_10 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS2sval_10 = AS2sval_10.Substring(0, AS2sval_10.Length - 2);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (ASSKLobjtmp["skillId"].ToString() == ASID3)
                {
                    if (ASSKLobjtmp["lv"].ToString() == "1")
                    {
                        AS3sval_1 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS3sval_1 = AS3sval_1.Substring(0, AS3sval_1.Length - 2);
                        AS3Fid = ASSKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != AS3Fid) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            AS3Fnme = mstFuncobjtmp["popupText"].ToString();
                            if (AS3Fnme != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                AS3Fnme = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }
                    }
                    else if (ASSKLobjtmp["lv"].ToString() == "10")
                    {
                        AS3sval_10 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS3sval_10 = AS3sval_10.Substring(0, AS3sval_10.Length - 2);
                    }
                }

                if (ASSKLobjtmp["skillId"].ToString() == ASID4)
                {
                    if (ASSKLobjtmp["lv"].ToString() == "1")
                    {
                        AS4sval_1 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS4sval_1 = AS4sval_1.Substring(0, AS4sval_1.Length - 2);
                        AS4Fid = ASSKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != AS4Fid) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            AS4Fnme = mstFuncobjtmp["popupText"].ToString();
                            if (AS4Fnme != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                AS4Fnme = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }
                    }
                    else if (ASSKLobjtmp["lv"].ToString() == "10")
                    {
                        AS4sval_10 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS4sval_10 = AS4sval_10.Substring(0, AS4sval_10.Length - 2);
                    }
                }

                if (ASSKLobjtmp["skillId"].ToString() == ASID5)
                {
                    if (ASSKLobjtmp["lv"].ToString() == "1")
                    {
                        AS5sval_1 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS5sval_1 = AS5sval_1.Substring(0, AS5sval_1.Length - 2);
                        AS5Fid = ASSKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != AS5Fid) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            AS5Fnme = mstFuncobjtmp["popupText"].ToString();
                            if (AS5Fnme != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                AS5Fnme = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }
                    }
                    else if (ASSKLobjtmp["lv"].ToString() == "10")
                    {
                        AS5sval_10 = ASSKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                            .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                        AS5sval_10 = AS5sval_10.Substring(0, AS5sval_10.Length - 2);
                    }
                }
            }

            /*AS1DTL = AS1DTL.Replace("を{{1:Value:m}}%アップする",
                $"提升[{ModifyFuncSvalDisplay.ModifyFuncStr(AS1Fnme, AS1sval_1, true)} ~ {ModifyFuncSvalDisplay.ModifyFuncStr(AS1Fnme, AS1sval_10, true)}]").Replace("自身の","自身").Replace("カードの","指令卡");*/
            AS1DTL = AS1DTL.Replace("{{1:Value:m}}%",
                $"[{ModifyFuncSvalDisplay.ModifyFuncStr(AS1Fnme, AS1sval_1, true)} ~ {ModifyFuncSvalDisplay.ModifyFuncStr(AS1Fnme, AS1sval_10, true)}]");
            GlobalPathsAndDatas.AS1D = AS1DTL;
            /*AS2DTL =
                $"自身以NP为[{ModifyFuncSvalDisplay.ModifyFuncStr(AS2Fnme, AS2sval_1, true)} ~ {ModifyFuncSvalDisplay.ModifyFuncStr(AS2Fnme, AS2sval_10, true)}]的状态开始战斗";*/
            AS2DTL = AS2DTL.Replace("{{1:Value:y}}%",
                $"[{ModifyFuncSvalDisplay.ModifyFuncStr(AS2Fnme, AS2sval_1, true)} ~ {ModifyFuncSvalDisplay.ModifyFuncStr(AS2Fnme, AS2sval_10, true)}]");
            GlobalPathsAndDatas.AS2D = AS2DTL;
            AS3DTL = AS3DTL.Replace("{{1:Value:m}}%",
                $"[{ModifyFuncSvalDisplay.ModifyFuncStr(AS3Fnme, AS3sval_1, true)} ~ {ModifyFuncSvalDisplay.ModifyFuncStr(AS3Fnme, AS3sval_10, true)}]");
            GlobalPathsAndDatas.AS3D = AS3DTL;
            AS4DTL = AS4DTL.Replace("{{1:Value:m}}%",
                $"[{ModifyFuncSvalDisplay.ModifyFuncStr(AS4Fnme, AS4sval_1, true)} ~ {ModifyFuncSvalDisplay.ModifyFuncStr(AS4Fnme, AS4sval_10, true)}]");
            GlobalPathsAndDatas.AS4D = AS4DTL;
            AS5DTL = AS5DTL.Insert(26, "\r\n");
            GlobalPathsAndDatas.AS5D = AS5DTL;

            Dispatcher.Invoke(() =>
            {
                AppendClassPassiveFuncList.Items.Clear();
                AppendClassPassiveFuncList.Items.Add(new AppendClassPassiveSvalList(AS1NME, ASID1,
                    AS1DTL));
                AppendClassPassiveFuncList.Items.Add(new AppendClassPassiveSvalList(AS2NME, ASID2,
                    AS2DTL));
                AppendClassPassiveFuncList.Items.Add(new AppendClassPassiveSvalList(AS3NME, ASID3,
                    AS3DTL));
                AppendClassPassiveFuncList.Items.Add(new AppendClassPassiveSvalList(AS4NME, ASID4,
                    AS4DTL));
                AppendClassPassiveFuncList.Items.Add(new AppendClassPassiveSvalList(AS5NME, ASID5,
                    AS5DTL));
            });
        }

        private string[] GetSvtSkillIDs(string svtID)
        {
            var skillID1 = "";
            var skillID2 = "";
            var skillID3 = "";
            var sk1IsStrengthened = "false";
            var sk2IsStrengthened = "false";
            var sk3IsStrengthened = "false";
            var result = new string[6];
            foreach (var svtskill in GlobalPathsAndDatas.mstSvtSkillArray)
            {
                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "1" &&
                    ((JObject)svtskill)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID1 = mstsvtskillobjtmp["skillId"].ToString();
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "1" &&
                    ((JObject)svtskill)["priority"].ToString() == "2")
                {
                    if (((JObject)svtskill)["condQuestId"].ToString().Substring(0, 1) != "0")
                    {
                        var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                        skillID1 = mstsvtskillobjtmp["skillId"].ToString();
                        sk1IsStrengthened = "true";
                    }
                    else
                    {
                        var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                        skillID1 += "*" + mstsvtskillobjtmp["skillId"] + "^SK";
                    }
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "1" &&
                    ((JObject)svtskill)["priority"].ToString() == "3")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID1 = mstsvtskillobjtmp["skillId"].ToString();
                    sk1IsStrengthened = "twice";
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "2" &&
                    ((JObject)svtskill)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID2 = mstsvtskillobjtmp["skillId"].ToString();
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "2" &&
                    ((JObject)svtskill)["priority"].ToString() == "2")
                {
                    if (((JObject)svtskill)["condQuestId"].ToString().Substring(0, 1) != "0")
                    {
                        var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                        skillID2 = mstsvtskillobjtmp["skillId"].ToString();
                        sk2IsStrengthened = "true";
                    }
                    else
                    {
                        var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                        skillID2 += "*" + mstsvtskillobjtmp["skillId"] + "^SK";
                    }
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "2" &&
                    ((JObject)svtskill)["priority"].ToString() == "3")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID2 = mstsvtskillobjtmp["skillId"].ToString();
                    sk2IsStrengthened = "twice";
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "3" &&
                    ((JObject)svtskill)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID3 = mstsvtskillobjtmp["skillId"].ToString();
                }

                if (((JObject)svtskill)["svtId"].ToString() == svtID &&
                    ((JObject)svtskill)["num"].ToString() == "3" &&
                    ((JObject)svtskill)["priority"].ToString() == "2")
                {
                    if (((JObject)svtskill)["condQuestId"].ToString().Substring(0, 1) != "0")
                    {
                        var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                        skillID3 = mstsvtskillobjtmp["skillId"].ToString();
                        sk3IsStrengthened = "true";
                    }
                    else
                    {
                        var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                        skillID3 += "*" + mstsvtskillobjtmp["skillId"] + "^SK";
                    }
                }

                if (((JObject)svtskill)["svtId"].ToString() != svtID ||
                    ((JObject)svtskill)["num"].ToString() != "3" ||
                    ((JObject)svtskill)["priority"].ToString() != "3") continue;
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID3 = mstsvtskillobjtmp["skillId"].ToString();
                    sk3IsStrengthened = "twice";
                }
            }

            if (skillID1 == "") skillID1 = FindSkillIDinNPCSvt(svtID, 1);

            if (skillID2 == "") skillID2 = FindSkillIDinNPCSvt(svtID, 2);

            if (skillID3 == "") skillID3 = FindSkillIDinNPCSvt(svtID, 3);

            if (skillID1.Contains("*"))
                Dispatcher.Invoke(() =>
                {
                    GlobalPathsAndDatas.IDListStr = skillID1;
                    var ChoiceSK = new SvtSTDIDChoice();
                    ChoiceSK.ShowDialog();
                    var ReturnStr = ChoiceSK.idreturn;
                    skillID1 = ReturnStr;
                });

            if (skillID2.Contains("*"))
                Dispatcher.Invoke(() =>
                {
                    GlobalPathsAndDatas.IDListStr = skillID2;
                    var ChoiceSK = new SvtSTDIDChoice();
                    ChoiceSK.ShowDialog();
                    var ReturnStr = ChoiceSK.idreturn;
                    skillID2 = ReturnStr;
                });

            if (skillID3.Contains("*"))
                Dispatcher.Invoke(() =>
                {
                    GlobalPathsAndDatas.IDListStr = skillID3;
                    var ChoiceSK = new SvtSTDIDChoice();
                    ChoiceSK.ShowDialog();
                    var ReturnStr = ChoiceSK.idreturn;
                    skillID3 = ReturnStr;
                });

            result[0] = skillID1;
            result[2] = skillID2;
            result[4] = skillID3;
            result[1] = sk1IsStrengthened;
            result[3] = sk2IsStrengthened;
            result[5] = sk3IsStrengthened;
            return result;
        }

        private void ServantSkillInformationCheck(string svtID)
        {
            IsSk1Strengthened.Dispatcher.Invoke(() => { IsSk1Strengthened.Text = "×"; });
            IsSk2Strengthened.Dispatcher.Invoke(() => { IsSk2Strengthened.Text = "×"; });
            IsSk3Strengthened.Dispatcher.Invoke(() => { IsSk3Strengthened.Text = "×"; });
            var skillSquare = GetSvtSkillIDs(svtID);
            var sk1IsStrengthened = skillSquare[1] != "false";
            var sk2IsStrengthened = skillSquare[3] != "false";
            var sk3IsStrengthened = skillSquare[5] != "false";
            var skillID1 = skillSquare[0];
            var skillID2 = skillSquare[2];
            var skillID3 = skillSquare[4];
            SkillLvs.skill1forExcel = "";
            SkillLvs.skill2forExcel = "";
            SkillLvs.skill3forExcel = "";
            switch (skillSquare[1])
            {
                case "true":
                    IsSk1Strengthened.Dispatcher.Invoke(() => { IsSk1Strengthened.Text = "▲"; });
                    svtskill1_header.Dispatcher.Invoke(() => { svtskill1_header.Header += "▲"; });
                    break;
                case "twice":
                    IsSk1Strengthened.Dispatcher.Invoke(() => { IsSk1Strengthened.Text = "▲▲"; });
                    svtskill1_header.Dispatcher.Invoke(() => { svtskill1_header.Header += "▲▲"; });
                    break;
            }

            switch (skillSquare[3])
            {
                case "true":
                    IsSk2Strengthened.Dispatcher.Invoke(() => { IsSk2Strengthened.Text = "▲"; });
                    svtskill2_header.Dispatcher.Invoke(() => { svtskill2_header.Header += "▲"; });
                    break;
                case "twice":
                    IsSk2Strengthened.Dispatcher.Invoke(() => { IsSk2Strengthened.Text = "▲▲"; });
                    svtskill2_header.Dispatcher.Invoke(() => { svtskill2_header.Header += "▲▲"; });
                    break;
            }

            switch (skillSquare[5])
            {
                case "true":
                    IsSk3Strengthened.Dispatcher.Invoke(() => { IsSk3Strengthened.Text = "▲"; });
                    svtskill3_header.Dispatcher.Invoke(() => { svtskill3_header.Header += "▲"; });
                    break;
                case "twice":
                    IsSk3Strengthened.Dispatcher.Invoke(() => { IsSk3Strengthened.Text = "▲▲"; });
                    svtskill3_header.Dispatcher.Invoke(() => { svtskill3_header.Header += "▲▲"; });
                    break;
            }

            Dispatcher.Invoke(() =>
            {
                skill1ID.Text = skillID1;
                skill2ID.Text = skillID2;
                skill3ID.Text = skillID3;
            });
            var Display1 = new Task(() => { DisplaySkills(skillID1, sk1IsStrengthened, 1); });
            var Display2 = new Task(() => { DisplaySkills(skillID2, sk2IsStrengthened, 2); });
            var Display3 = new Task(() => { DisplaySkills(skillID3, sk3IsStrengthened, 3); });
            Display1.Start();
            Display2.Start();
            Display3.Start();
        }

        private void DisplaySkills(string SkillID, bool isStrengthed, int SkillNum)
        {
            var skillName = "";
            var skillDetail = "";
            var skillIconUri = "";
            foreach (var skilltmp in GlobalPathsAndDatas.mstSkillArray)
            {
                if (((JObject)skilltmp)["id"].ToString() != SkillID) continue;
                var skillobjtmp = JObject.Parse(skilltmp.ToString());
                skillName = skillobjtmp["name"].ToString();
                var iconid = skillobjtmp["iconId"].ToString();
                switch (iconid.Length)
                {
                    case 3:
                        skillIconUri = "skillicons\\skill_00" + iconid + ".png";
                        break;
                    case 4:
                        skillIconUri = "skillicons\\skill_0" + iconid + ".png";
                        break;
                    case 5:
                        skillIconUri = "skillicons\\skill_" + iconid + ".png";
                        break;
                    case 6:
                        skillIconUri = "skillicons\\skill_" + iconid + ".png";
                        break;
                    default:
                        skillIconUri = "skillicons\\skill_00000.png";
                        break;
                }

                if (isStrengthed) skillName += " ▲";
            }

            var AddVals = SkillAddNameCheck(SkillID);

            Dispatcher.Invoke(() =>
            {
                foreach (var skillDetailtmp in GlobalPathsAndDatas.mstSkillDetailArray)
                {
                    if (((JObject)skillDetailtmp)["id"].ToString() != SkillID) continue;
                    var skillDetailobjtmp = JObject.Parse(skillDetailtmp.ToString());
                    skillDetail = skillDetailobjtmp["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.10]")
                                      .Replace("[g]", "").Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "") +
                                  (AddVals == "" ? "" : "\r\n(bot备注:满足特定条件后技能名称改变为\"" + AddVals + "\")");
                }
            });
            Dispatcher.Invoke(() =>
            {
                switch (SkillNum)
                {
                    case 1:
                        skill1name.Text = skillName;
                        skill1details.Text = skillDetail;
                        try
                        {
                            sk1_icon.Source = new BitmapImage(new Uri(skillIconUri, UriKind.Relative));
                        }
                        catch (Exception)
                        {
                            sk1_icon.Source = new BitmapImage(new Uri("skillicons\\skill_00000.png", UriKind.Relative));
                        }

                        var SSC = new Task(() => { SkillSvalsCheck(SkillID, 1); });
                        SSC.Start();
                        break;
                    case 2:
                        skill2name.Text = skillName;
                        skill2details.Text = skillDetail;
                        try
                        {
                            sk2_icon.Source = new BitmapImage(new Uri(skillIconUri, UriKind.Relative));
                        }
                        catch (Exception)
                        {
                            sk2_icon.Source = new BitmapImage(new Uri("skillicons\\skill_00000.png", UriKind.Relative));
                        }

                        var SSC2 = new Task(() => { SkillSvalsCheck(SkillID, 2); });
                        SSC2.Start();
                        break;
                    case 3:
                        skill3name.Text = skillName;
                        skill3details.Text = skillDetail;
                        try
                        {
                            sk3_icon.Source = new BitmapImage(new Uri(skillIconUri, UriKind.Relative));
                        }
                        catch (Exception)
                        {
                            sk3_icon.Source = new BitmapImage(new Uri("skillicons\\skill_00000.png", UriKind.Relative));
                        }

                        var SSC3 = new Task(() => { SkillSvalsCheck(SkillID, 3); });
                        SSC3.Start();
                        break;
                }
            });
        }

        private string FindSkillIDinNPCSvt(string svtid, int skillnum)
        {
            foreach (var npcSvtFollowertmp in GlobalPathsAndDatas.npcSvtFollowerArray)
            {
                if (((JObject)npcSvtFollowertmp)["svtId"].ToString() != svtid) continue;
                var npcSvtFollowerobjtmp = JObject.Parse(npcSvtFollowertmp.ToString());
                switch (skillnum)
                {
                    case 1:
                        return npcSvtFollowerobjtmp["skillId1"].ToString();
                    case 2:
                        return npcSvtFollowerobjtmp["skillId2"].ToString();
                    case 3:
                        return npcSvtFollowerobjtmp["skillId3"].ToString();
                }
            }

            return "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var OSI = new Task(() => { OutputSVTIDs(); });
            OSI.Start();
        }

        private void OutputSVTIDs()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    var output = GlobalPathsAndDatas.mstSvtArray.Aggregate("",
                        (current, svtIDtmp) => current + "ID: " + ((JObject)svtIDtmp)["id"] + "    " + "名称: " +
                                               ((JObject)svtIDtmp)["name"] + "\r\n");
                    File.WriteAllText(GlobalPathsAndDatas.path + "/SearchIDList.txt", output);
                    Dispatcher.Invoke(() => { MessageBox.Success("导出成功, 文件名为 SearchIDList.txt", "完成"); });
                    Process.Start(GlobalPathsAndDatas.path + "/SearchIDList.txt");
                }
                catch (Exception)
                {
                    //ignore
                }
            });
        }

        private void ClearTexts()
        {
            Dispatcher.Invoke(() =>
            {
                var g = Content as Grid;
                var childrensX = Starter.Children;
                foreach (UIElement ui in childrensX)
                    if (ui is TextBox box)
                        box.Text = "";
                var childrens = g.Children;
                foreach (UIElement ui in childrens)
                    if (ui is TextBox box)
                        box.Text = "";
                var childrens1 = svtdetail.Children;
                foreach (UIElement ui2 in childrens1)
                    if (ui2 is TextBox box)
                        box.Text = "";
                var childrens2 = svttexts.Children;
                foreach (UIElement ui2 in childrens2)
                    if (ui2 is TextBox box)
                        box.Text = "";
                var childrens3 = svtcards.Children;
                foreach (UIElement ui2 in childrens3)
                    if (ui2 is TextBox box)
                        box.Text = "";
                var childrens4 = svtTDs.Children;
                foreach (UIElement ui2 in childrens4)
                    if (ui2 is TextBox box)
                        box.Text = "";
                var childrens5 = svtskill1.Children;
                foreach (UIElement ui2 in childrens5)
                    if (ui2 is TextBox box)
                        box.Text = "";
                var childrens6 = svtskill2.Children;
                foreach (UIElement ui2 in childrens6)
                    if (ui2 is TextBox box)
                        box.Text = "";
                var childrens7 = svtskill3.Children;
                foreach (UIElement ui2 in childrens7)
                    if (ui2 is TextBox box)
                        box.Text = "";
                jibantext1.Text = "";
                jibantext2.Text = "";
                jibantext3.Text = "";
                jibantext4.Text = "";
                jibantext5.Text = "";
                jibantext6.Text = "";
                jibantext7.Text = "";
                atkbalance1.Text = "( x 1.0 -)";
                atkbalance2.Text = "( x 1.0 -)";
                npcardtype.Text = "";
                nptype.Text = "";
                nprank.Text = "";
                npruby.Text = "";
                npname.Text = "";
                npdetail.Text = "";
                JBOutput.IsEnabled = false;
                sixwei.Text = "";
                Skill1FuncList.Items.Clear();
                Skill2FuncList.Items.Clear();
                Skill3FuncList.Items.Clear();
                LimitCombineItems.Items.Clear();
                SkillCombineItems.Items.Clear();
                HpAtkListView.Items.Clear();
                TDFuncList.Items.Clear();
                ClassPassiveFuncList.Items.Clear();
                AppendClassPassiveFuncList.Items.Clear();
                AppendSkillCombineItems.Items.Clear();
                Button1.IsEnabled = true;
                Title = "Altera";
                svtskill1_header.Header = "技能1";
                svtskill2_header.Header = "技能2";
                svtskill3_header.Header = "技能3";
                sk1_icon.Source = new BitmapImage(new Uri("skillicons\\skill_00000.png", UriKind.Relative));
                sk2_icon.Source = new BitmapImage(new Uri("skillicons\\skill_00000.png", UriKind.Relative));
                sk3_icon.Source = new BitmapImage(new Uri("skillicons\\skill_00000.png", UriKind.Relative));
                var Zeros = new int[121];
                var levels = new int[121];
                for (var i = 0; i <= 120; i++)
                {
                    Zeros[i] = 0;
                    levels[i] = i;
                }

                XZhou.MaxValue = 120;
                LineHP = Zeros;
                LineATK = Zeros;
                HPCurveX.Values = LineHP.AsChartValues();
                ATKCurveX.Values = LineATK.AsChartValues();
                for (var j = 0; j <= 120; j++) LabelX[j] = levels[j].ToString();
                LabelX = new string[121];
                XZhou_Step.Step = 5;
                HPAtkXCurve.Update();
                TreasureDeviceID.Text = "";
                hpatkbalance.Text = "( 攻防倾向: 均衡 )";
                var QuickUri = "images\\Quick.png";
                card1.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card2.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card3.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card4.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card5.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                cardTD.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                raritystars.Visibility = Visibility.Hidden;
                ClassPng.Visibility = Visibility.Collapsed;
            });
            IsSk1Strengthened.Dispatcher.Invoke(() => { IsSk1Strengthened.Text = "×"; });
            IsSk2Strengthened.Dispatcher.Invoke(() => { IsSk2Strengthened.Text = "×"; });
            IsSk3Strengthened.Dispatcher.Invoke(() => { IsSk3Strengthened.Text = "×"; });
            IsNPStrengthened.Dispatcher.Invoke(() => { IsNPStrengthened.Text = "×"; });
            GC.Collect();
        }

        private void ClearLists()
        {
            Dispatcher.Invoke(() =>
            {
                PickupEventList.Items.Clear();
                PickupEndedEventList.Items.Clear();
                ClassList.Items.Clear();
                PickupQuestList.Items.Clear();
                PickupGachaList.Items.Clear();
                PickupEndedGachaList.Items.Clear();
                SvtFilterList.Items.Clear();
            });
        }

        private void ChangeCardArrangeImage(int ImgNo, string CardID)
        {
            var ArtsUri = "images\\Arts.png";
            var BusterUri = "images\\Buster.png";
            var QuickUri = "images\\Quick.png";
            Dispatcher.Invoke(() =>
            {
                switch (ImgNo)
                {
                    case 1:
                        switch (CardID)
                        {
                            case "A":
                                card1.Source = new BitmapImage(new Uri(ArtsUri, UriKind.Relative));
                                break;
                            case "B":
                                card1.Source = new BitmapImage(new Uri(BusterUri, UriKind.Relative));
                                break;
                            case "Q":
                                card1.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                                break;
                        }

                        break;
                    case 2:
                        switch (CardID)
                        {
                            case "A":
                                card2.Source = new BitmapImage(new Uri(ArtsUri, UriKind.Relative));
                                break;
                            case "B":
                                card2.Source = new BitmapImage(new Uri(BusterUri, UriKind.Relative));
                                break;
                            case "Q":
                                card2.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                                break;
                        }

                        break;
                    case 3:
                        switch (CardID)
                        {
                            case "A":
                                card3.Source = new BitmapImage(new Uri(ArtsUri, UriKind.Relative));
                                break;
                            case "B":
                                card3.Source = new BitmapImage(new Uri(BusterUri, UriKind.Relative));
                                break;
                            case "Q":
                                card3.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                                break;
                        }

                        break;
                    case 4:
                        switch (CardID)
                        {
                            case "A":
                                card4.Source = new BitmapImage(new Uri(ArtsUri, UriKind.Relative));
                                break;
                            case "B":
                                card4.Source = new BitmapImage(new Uri(BusterUri, UriKind.Relative));
                                break;
                            case "Q":
                                card4.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                                break;
                        }

                        break;
                    case 5:
                        switch (CardID)
                        {
                            case "A":
                                card5.Source = new BitmapImage(new Uri(ArtsUri, UriKind.Relative));
                                break;
                            case "B":
                                card5.Source = new BitmapImage(new Uri(BusterUri, UriKind.Relative));
                                break;
                            case "Q":
                                card5.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                                break;
                        }

                        break;
                    case 6:
                        switch (CardID)
                        {
                            case "A":
                                cardTD.Source = new BitmapImage(new Uri(ArtsUri, UriKind.Relative));
                                break;
                            case "B":
                                cardTD.Source = new BitmapImage(new Uri(BusterUri, UriKind.Relative));
                                break;
                            case "Q":
                                cardTD.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                                break;
                        }

                        break;
                }
            });
        }

        private void SetCardImgs(string CardArrange)
        {
            var CardArrangementArray = CardArrange.Replace("[", "").Replace("]", "").Split(',');
            for (var i = 1; i <= 5; i++) ChangeCardArrangeImage(i, CardArrangementArray[i - 1]);
        }

        private void SkillSvalsCheck(string SkillID, int SkillNum)
        {
            var skilllv1chargetime = "";
            var skilllv6chargetime = "";
            var skilllv10chargetime = "";
            var skilllv1sval = "";
            var skilllv6sval = "";
            var skilllv10sval = "";
            string svtSKFuncID;
            string[] svtSKFuncIDArray;
            List<string> svtSKFuncIDList;
            var svtSKFuncList = new List<string>();
            string[] svtSKFuncArray;
            var svtSKFunc = string.Empty;
            var svtSKTargetList = new List<string>();
            string[] svtSKTargetArray;
            var svtSKTarget = string.Empty;
            var svtSKTargetRawList = new List<string>();
            string[] svtSKTargetRawArray;
            var svtSKTargetRaw = string.Empty;
            var svtSKbufficonList = new List<string>();
            string[] svtSKbufficonArray;
            var svtSKbufficon = string.Empty;
            var svtSKapplyTargetList = new List<string>();
            string[] svtSKapplyTargetArray;
            var svtSKapplyTarget = string.Empty;
            var svtSKfuncTypeList = new List<string>();
            string[] svtSKfuncTypeArray;
            var svtSKtargetIconList = new List<string>();
            string[] svtSKtargetIconArray;
            var svtSKtargetIcon = string.Empty;
            var svtSKfuncType = string.Empty;
            var NeedTranslate = false;
            Dispatcher.Invoke(() => { NeedTranslate = ToggleBuffFuncTranslate.IsChecked == true; });
            foreach (var SKLTMP in GlobalPathsAndDatas.mstSkillLvArray)
            {
                if (((JObject)SKLTMP)["skillId"].ToString() == SkillID && ((JObject)SKLTMP)["lv"].ToString() == "1")
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv1sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]\"", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv1sval = skilllv1sval.Substring(0, skilllv1sval.Length - 2);
                    skilllv1chargetime = SKLobjtmp["chargeTurn"].ToString();
                    svtSKFuncID = SKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    svtSKFuncIDList = new List<string>(svtSKFuncID.Split(','));
                    svtSKFuncIDArray = svtSKFuncIDList.ToArray();
                    var funcnametmp = "";
                    var targettmp = "";
                    var targetstr = "";
                    var tvalstr = "";
                    var tvalstrxls = "";
                    var popupIcon = "";
                    var applyTarget = "";
                    var funcType = "";
                    foreach (var skfuncidtmp in svtSKFuncIDArray)
                    {
                        if (skfuncidtmp == "21039") continue;
                        if (skfuncidtmp == "21448") continue;
                        foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                        {
                            if (((JObject)functmp)["id"].ToString() != skfuncidtmp) continue;
                            var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                            funcnametmp = mstFuncobjtmp["popupText"].ToString();
                            targettmp = mstFuncobjtmp["targetType"].ToString();
                            popupIcon = mstFuncobjtmp["popupIconId"].ToString();
                            applyTarget = mstFuncobjtmp["applyTarget"].ToString();
                            funcType = mstFuncobjtmp["funcType"].ToString();
                            tvalstr = mstFuncobjtmp["tvals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            targetstr = FuncTargetStr(targettmp);
                            tvalstrxls = "「" + FuncTargetDisplayIconStr(targettmp);
                            if (tvalstr != "") targetstr += $"\r\n{CheckUniqueIndividuality(tvalstr)}";
                            if (tvalstr != "")
                                tvalstrxls += $"<{CheckUniqueIndividuality(tvalstr)}>」";
                            else
                                tvalstrxls += "」";
                            if (funcnametmp != "" || mstFuncobjtmp["funcType"].ToString() == "2") continue;
                            var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                            {
                                if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                                funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                                break;
                            }
                        }

                        if (skfuncidtmp == "21302" || skfuncidtmp == "21303") funcnametmp = "血塗れの皇子(条件满足时)";

                        svtSKTargetList.Add(targetstr);
                        svtSKbufficonList.Add(popupIcon);
                        svtSKapplyTargetList.Add(applyTarget);
                        svtSKTargetRawList.Add(targettmp);
                        svtSKfuncTypeList.Add(funcType);
                        svtSKtargetIconList.Add(tvalstrxls);

                        if (NeedTranslate)
                            svtSKFuncList.Add(TranslateBuff(funcnametmp));
                        else
                            svtSKFuncList.Add(funcnametmp);
                    }
                }

                if (((JObject)SKLTMP)["skillId"].ToString() == SkillID && ((JObject)SKLTMP)["lv"].ToString() == "6")
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv6sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]\"", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv6sval = skilllv6sval.Substring(0, skilllv6sval.Length - 2);
                    skilllv6chargetime = SKLobjtmp["chargeTurn"].ToString();
                }

                if (((JObject)SKLTMP)["skillId"].ToString() != SkillID ||
                    ((JObject)SKLTMP)["lv"].ToString() != "10") continue;
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv10sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]\"", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv10sval = skilllv10sval.Substring(0, skilllv10sval.Length - 2);
                    skilllv10chargetime = SKLobjtmp["chargeTurn"].ToString();
                }
            }

            if (skilllv6chargetime == "" && skilllv1chargetime != "") skilllv6chargetime = skilllv1chargetime;
            if (skilllv6sval == "" && skilllv1sval != "") skilllv6sval = skilllv1sval;
            if (skilllv10chargetime == "" && skilllv1chargetime != "") skilllv10chargetime = skilllv1chargetime;
            if (skilllv10sval == "" && skilllv1sval != "") skilllv10sval = skilllv1sval;
            svtSKbufficonArray = svtSKbufficonList.ToArray();
            svtSKbufficon = string.Join(",", svtSKbufficonArray);
            svtSKTargetArray = svtSKTargetList.ToArray();
            svtSKTarget = string.Join(",", svtSKTargetArray);
            svtSKTargetRawArray = svtSKTargetRawList.ToArray();
            svtSKTargetRaw = string.Join(",", svtSKTargetRawArray);
            svtSKapplyTargetArray = svtSKapplyTargetList.ToArray();
            svtSKapplyTarget = string.Join(",", svtSKapplyTargetArray);
            svtSKfuncTypeArray = svtSKfuncTypeList.ToArray();
            svtSKfuncType = string.Join(",", svtSKfuncTypeArray);
            svtSKtargetIconArray = svtSKtargetIconList.ToArray();
            svtSKtargetIcon = string.Join(",", svtSKtargetIconArray);
            switch (SkillNum)
            {
                case 1:
                    Dispatcher.Invoke(() =>
                    {
                        skill1cdlv1.Text = skilllv1chargetime;
                        skill1cdlv6.Text = skilllv6chargetime;
                        skill1cdlv10.Text = skilllv10chargetime;
                    });
                    break;
                case 2:
                    Dispatcher.Invoke(() =>
                    {
                        skill2cdlv1.Text = skilllv1chargetime;
                        skill2cdlv6.Text = skilllv6chargetime;
                        skill2cdlv10.Text = skilllv10chargetime;
                    });
                    break;
                case 3:
                    Dispatcher.Invoke(() =>
                    {
                        skill3cdlv1.Text = skilllv1chargetime;
                        skill3cdlv6.Text = skilllv6chargetime;
                        skill3cdlv10.Text = skilllv10chargetime;
                    });
                    break;
            }

            svtSKFuncArray = svtSKFuncList.ToArray();
            svtSKFunc = string.Join(", ", svtSKFuncArray);
            var SSD = new Task(() =>
            {
                SkillSvalsDisplay(skilllv1sval, skilllv6sval, skilllv10sval, svtSKFunc, SkillNum, svtSKTarget,
                    svtSKbufficon, svtSKapplyTarget, svtSKTargetRaw, svtSKfuncType, svtSKtargetIcon);
            });
            SSD.Start();
        }

        public string FuncTargetDisplayIconStr(string targetType)
        {
            string targetstr = null;
            /*switch (targetType)
            {
                case "0":
                    targetstr = "☺";
                    break;
                case "1":
                    targetstr = "▶";
                    break;
                case "3":
                    targetstr = "▶☑";
                    break;
                case "4":
                    targetstr = "◁";
                    break;
                case "6":
                    targetstr = "◁☑";
                    break;
                default:
                    targetstr = "◑";
                    break;
            }
            */
            /*switch (targetType)
            {
                case "0":
                    targetstr = "<自身>";
                    break;
                case "1":
                    targetstr = "<己方·单体>";
                    break;
                case "3":
                    targetstr = "<己方·全体>";
                    break;
                case "4":
                    targetstr = "<敌方·单体>";
                    break;
                case "6":
                    targetstr = "<敌方·全体>";
                    break;
                default:
                    targetstr = "<特殊·参考实际情况>";
                    break;
            }*/
            switch (targetType)
            {
                case "0":
                    targetstr = "<自身>";
                    break;
                case "1":
                    targetstr = "<己方·单体>";
                    break;
                case "2":
                    targetstr = "<己方·ANOTHER>";
                    break;
                case "3":
                    targetstr = "<己方·全体>";
                    break;
                case "4":
                    targetstr = "<敌方·单体>";
                    break;
                case "5":
                    targetstr = "<敌方·ANOTHER>";
                    break;
                case "6":
                    targetstr = "<敌方·全体>";
                    break;
                case "7":
                    targetstr = "<己方·FULL>";
                    break;
                case "8":
                    targetstr = "<敌方·FULL>";
                    break;
                case "9":
                    targetstr = "<己方·全体(自身除外)>";
                    break;
                case "10":
                    targetstr = "<己方·单体反选>";
                    break;
                case "11":
                    targetstr = "<己方·随机>";
                    break;
                case "12":
                    targetstr = "<敌方·OTHER>";
                    break;
                case "13":
                    targetstr = "<敌方·随机>";
                    break;
                case "14":
                    targetstr = "<己方·OTHER_FULL>";
                    break;
                case "15":
                    targetstr = "<敌方·OTHER_FULL>";
                    break;
                case "16":
                    targetstr = "<己方·SELECT_ONE_SUB>";
                    break;
                case "17":
                    targetstr = "<己方·SELECT_SUB>";
                    break;
                case "18":
                    targetstr = "<己方·ONE_ANOTHER_RANDOM>";
                    break;
                case "19":
                    targetstr = "<己方·SELF_ANOTHER_RANDOM>";
                    break;
                case "20":
                    targetstr = "<敌方·ONE_ANOTHER_RANDOM>";
                    break;
                case "21":
                    targetstr = "<己方·SELF_ANOTHER_FIRST>";
                    break;
                case "22":
                    targetstr = "<己方·SELF_BEFORE>";
                    break;
                case "23":
                    targetstr = "<己方·SELF_AFTER>";
                    break;
                case "24":
                    targetstr = "<己方·SELF_ANOTHER_LAST>";
                    break;
                case "25":
                    targetstr = "<COMMAND_TYPE_SELF_TREASURE_DEVICE>";
                    break;
                case "26":
                    targetstr = "<FIELD_OTHER>";
                    break;
                case "27":
                    targetstr = "<敌方·ONE_NO_TARGET_NO_ACTION>";
                    break;
                case "28":
                    targetstr = "<己方·HP最少>";
                    break;
                case "29":
                    targetstr = "<己方·HP比率最少>";
                    break;
                default:
                    targetstr = $"<目标类型:{targetstr}\r\n(请参考宝具/技能描述)>";
                    break;
            }

            return targetstr;
        }

        public string FuncTargetStr(string targetType)
        {
            string targetstr = null;
            switch (targetType)
            {
                case "0":
                    targetstr = "自身";
                    break;
                case "1":
                    targetstr = "己方·单体";
                    break;
                case "2":
                    targetstr = "己方·ANOTHER";
                    break;
                case "3":
                    targetstr = "己方·全体";
                    break;
                case "4":
                    targetstr = "敌方·单体";
                    break;
                case "5":
                    targetstr = "敌方·ANOTHER";
                    break;
                case "6":
                    targetstr = "敌方·全体";
                    break;
                case "7":
                    targetstr = "己方·FULL";
                    break;
                case "8":
                    targetstr = "敌方·FULL";
                    break;
                case "9":
                    targetstr = "己方·全体\r\n(自身除外)";
                    break;
                case "10":
                    targetstr = "己方·单体反选";
                    break;
                case "11":
                    targetstr = "己方·随机";
                    break;
                case "12":
                    targetstr = "敌方·OTHER";
                    break;
                case "13":
                    targetstr = "敌方·随机";
                    break;
                case "14":
                    targetstr = "己方·OTHER_FULL";
                    break;
                case "15":
                    targetstr = "敌方·OTHER_FULL";
                    break;
                case "16":
                    targetstr = "己方·SELECT_ONE_SUB";
                    break;
                case "17":
                    targetstr = "己方·SELECT_SUB";
                    break;
                case "18":
                    targetstr = "己方·ONE_ANOTHER_RANDOM";
                    break;
                case "19":
                    targetstr = "己方·SELF_ANOTHER_RANDOM";
                    break;
                case "20":
                    targetstr = "敌方·ONE_ANOTHER_RANDOM";
                    break;
                case "21":
                    targetstr = "己方·SELF_ANOTHER_FIRST";
                    break;
                case "22":
                    targetstr = "己方·SELF_BEFORE";
                    break;
                case "23":
                    targetstr = "己方·SELF_AFTER";
                    break;
                case "24":
                    targetstr = "己方·SELF_ANOTHER_LAST";
                    break;
                case "25":
                    targetstr = "COMMAND_TYPE_SELF_TREASURE_DEVICE";
                    break;
                case "26":
                    targetstr = "FIELD_OTHER";
                    break;
                case "27":
                    targetstr = "敌方·ONE_NO_TARGET_NO_ACTION";
                    break;
                case "28":
                    targetstr = "己方·HP最少";
                    break;
                case "29":
                    targetstr = "己方·HP比率最少";
                    break;
                default:
                    targetstr = $"目标类型:{targetstr}\r\n(请参考宝具/技能描述)";
                    break;
            }

            return targetstr;
        }

        public string CheckUniqueIndividuality(string id)
        {
            var idList = id.Split(',');
            var Outputs = new List<string>();
            foreach (var ids in idList)
            {
                var trigger1 = true;
                if (!ids.Contains("-"))
                {
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                        if (((JObject)svtInditmp)["id"].ToString() == ids)
                        {
                            var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                            Outputs.Add($"[{svtIndiObj["individualityName"]}]");
                            trigger1 = false;
                            break;
                        }

                    if (trigger1) Outputs.Add($"[特性/从者{ids}]");
                }
                else
                {
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                        if (((JObject)svtInditmp)["id"].ToString() == ids.Replace("-", ""))
                        {
                            var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                            Outputs.Add($"非[{svtIndiObj["individualityName"]}]");
                            trigger1 = false;
                            break;
                        }

                    if (trigger1) Outputs.Add($"非[特性/从者{ids.Replace("-", "")}]");
                }
            }

            var outputArray = Outputs.ToArray();
            var strout = string.Join("+", outputArray);
            return strout;
        }

        private void SkillSvalsDisplay(string lv1, string lv6, string lv10, string FuncName, int SkillNum,
            string target, string bufficon, string applyTarget, string targetraw, string funcTypeList,
            string specialIconXls)
        {
            Dispatcher.Invoke(() =>
            {
                if (skill1ID.Text == "") return;
                var lv1Array = lv1.Split('|');
                var lv6Array = lv6.Split('|');
                var lv10Array = lv10.Split('|');
                var FuncArray = FuncName.Replace(" ", "").Split(',');
                var targetlistArray = target.Split(',');
                var targetrawArray = targetraw.Split(',');
                var bufficonidArray = bufficon.Split(',');
                var bufficonBitmaps = new BitmapImage[bufficonidArray.Length];
                var applyTargetArray = applyTarget.Split(',');
                var funcTypeListArray = funcTypeList.Split(',');
                var xlsSvalIcon = specialIconXls.Split(',');
                var specialTmp = "";
                var noticeStr = "注:该技能暂无可显示的技能描述，请参考下方技能数据效果.";
                for (var m = 0; m <= bufficonidArray.Length - 1; m++)
                {
                    bufficonBitmaps[m] = new BitmapImage(new Uri("bufficons\\bufficon_0.png", UriKind.Relative));
                    try
                    {
                        bufficonBitmaps[m] = new BitmapImage(new Uri($"bufficons\\bufficon_{bufficonidArray[m]}.png",
                            UriKind.Relative));
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

                for (var i = 0; i <= FuncArray.Length - 1; i++)
                {
                    if (FuncArray[i] == "" && lv1Array[i].Count(c => c == ',') == 1 &&
                        !lv1Array[i].Contains("Hide")) FuncArray[i] = "HP回复";
                    if (FuncArray[i] == "" && lv1Array[i].Count(c => c == ',') == 3 &&
                        lv1Array[i].Contains("DependFuncId1")) FuncArray[i] = "HP吸収";
                    if (ToggleFuncDiffer.IsChecked == true)
                    {
                        lv1Array[i] = ModifyFuncSvalDisplay.ModifyFuncStr(FuncArray[i],
                            lv1Array[i], ToggleDisplayEnemyFunc.IsChecked == false);
                        lv6Array[i] = ModifyFuncSvalDisplay.ModifyFuncStr(FuncArray[i],
                            lv6Array[i], ToggleDisplayEnemyFunc.IsChecked == false);
                        lv10Array[i] = ModifyFuncSvalDisplay.ModifyFuncStr(FuncArray[i],
                            lv10Array[i], ToggleDisplayEnemyFunc.IsChecked == false);
                    }

                    if (FuncArray[i] == "") continue;
                    if (lv1Array[i] == "5000,-1,-1,ShowState:-1,HideMiss:1,HideNoEffect:1") continue;
                    if (ToggleDisplayEnemyFunc.IsChecked == false)
                    {
                        if (applyTargetArray[i] == "2")
                        {
                            if (FuncArray[i].Contains("チャージ増加") || FuncArray[i].Contains("充能增加") ||
                                FuncArray[i].Contains("クリティカル発生") || FuncArray[i].Contains("暴击发生率") ||
                                FuncArray[i].Contains("チャージ減少") || FuncArray[i].Contains("充能减少") ||
                                FuncArray[i].Contains("ハッピーハロウィン") ||
                                FuncArray[i].Contains("宝具タイプチェンジ") || FuncArray[i].Contains("宝具类型改変"))
                                switch (Convert.ToInt32(targetrawArray[i]))
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 7:
                                    case 9:
                                    case 10:
                                    case 11:
                                    case 14:
                                    case 16:
                                    case 17:
                                    case 18:
                                    case 25:
                                        continue;
                                }
                            else
                                continue;
                        }

                        if (applyTargetArray[i] == "1")
                        {
                            if (FuncArray[i].Contains("NP増加") || FuncArray[i].Contains("スター発生") ||
                                FuncArray[i].Contains("暴击星掉落率") || FuncArray[i].Contains("NP减少") ||
                                FuncArray[i].Contains("ハッピーハロウィン") || FuncArray[i].Contains("NP減少"))
                                switch (Convert.ToInt32(targetrawArray[i]))
                                {
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                    case 12:
                                    case 13:
                                    case 15:
                                    case 20:
                                    case 27:
                                        continue;
                                }
                        }
                            
                    }

                    var DisplaySval = lv1Array[i] == lv10Array[i]
                        ? $"固定: {lv10Array[i]}"
                        : $"Lv.1: {lv1Array[i]}\r\nLv.6: {lv6Array[i]}\r\nLv.10: {lv10Array[i]}";
                    FuncArray[i] = translateOtherFunc(FuncArray[i]);
                    switch (SkillNum)
                    {
                        case 1:
                            if (isDisplayFuncType.IsChecked == true)
                                Skill1FuncList.Items.Add(new SkillListSval(
                                    FuncArray[i] + $"\r\n({FindFuncTypeNameDebugger(funcTypeListArray[i])})",
                                    targetlistArray[i],
                                    $"{DisplaySval}", bufficonBitmaps[i]));
                            else
                                Skill1FuncList.Items.Add(new SkillListSval(FuncArray[i], targetlistArray[i],
                                    $"{DisplaySval}", bufficonBitmaps[i]));
                            if (skill1details.Text == "" || skill1details.Text == noticeStr)
                            {
                                skill1details.Text = noticeStr;
                                /*SkillLvs.skill1forExcel +=
                                    xlsSvalIcon[i] +
                                    (targetlistArray[i].Contains("[") && !xlsSvalIcon[i].Contains("<特殊·参考实际情况>")
                                        ? "·<特殊·参考实际情况> "
                                        : " ") +
                                    FuncArray[i].Replace("\r\n", "") + " 【{" +
                                    (lv1Array[i].Replace("\r\n", " ") ==
                                     lv10Array[i].Replace("\r\n", " ")
                                        ? lv10Array[i].Replace("\r\n", " ")
                                        : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                          lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";*/

                                SkillLvs.skill1forExcel +=
                                    xlsSvalIcon[i] +
                                    FuncArray[i].Replace("\r\n", "") + " 【{" +
                                    (lv1Array[i].Replace("\r\n", " ") ==
                                     lv10Array[i].Replace("\r\n", " ")
                                        ? lv10Array[i].Replace("\r\n", " ")
                                        : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                          lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";

                                break;
                            }

                            SkillLvs.skill1forExcel += FuncArray[i].Replace("\r\n", "") + " 【{" +
                                                       (lv1Array[i].Replace("\r\n", " ") ==
                                                        lv10Array[i].Replace("\r\n", " ")
                                                           ? lv10Array[i].Replace("\r\n", " ")
                                                           : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                                             lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";
                            break;
                        case 2:
                            if (isDisplayFuncType.IsChecked == true)
                                Skill2FuncList.Items.Add(new SkillListSval(
                                    FuncArray[i] + $"\r\n({FindFuncTypeNameDebugger(funcTypeListArray[i])})",
                                    targetlistArray[i],
                                    $"{DisplaySval}", bufficonBitmaps[i]));
                            else
                                Skill2FuncList.Items.Add(new SkillListSval(FuncArray[i], targetlistArray[i],
                                    $"{DisplaySval}", bufficonBitmaps[i]));
                            if (skill2details.Text == "" || skill2details.Text == noticeStr)
                            {
                                skill2details.Text = noticeStr;
                                SkillLvs.skill2forExcel +=
                                    xlsSvalIcon[i] +
                                    FuncArray[i].Replace("\r\n", "") + " 【{" +
                                    (lv1Array[i].Replace("\r\n", " ") ==
                                     lv10Array[i].Replace("\r\n", " ")
                                        ? lv10Array[i].Replace("\r\n", " ")
                                        : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                          lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";

                                /*SkillLvs.skill2forExcel +=
                                    xlsSvalIcon[i] +
                                    (targetlistArray[i].Contains("[") && !xlsSvalIcon[i].Contains("<特殊·参考实际情况>")
                                        ? "·<特殊·参考实际情况> "
                                        : " ") +
                                    FuncArray[i].Replace("\r\n", "") + " 【{" +
                                    (lv1Array[i].Replace("\r\n", " ") ==
                                     lv10Array[i].Replace("\r\n", " ")
                                        ? lv10Array[i].Replace("\r\n", " ")
                                        : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                          lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";*/
                                break;
                            }

                            SkillLvs.skill2forExcel += FuncArray[i].Replace("\r\n", "") + " 【{" +
                                                       (lv1Array[i].Replace("\r\n", " ") ==
                                                        lv10Array[i].Replace("\r\n", " ")
                                                           ? lv10Array[i].Replace("\r\n", " ")
                                                           : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                                             lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";
                            break;
                        case 3:
                            if (isDisplayFuncType.IsChecked == true)
                                Skill3FuncList.Items.Add(new SkillListSval(
                                    FuncArray[i] + $"\r\n({FindFuncTypeNameDebugger(funcTypeListArray[i])})",
                                    targetlistArray[i],
                                    $"{DisplaySval}", bufficonBitmaps[i]));
                            else
                                Skill3FuncList.Items.Add(new SkillListSval(FuncArray[i], targetlistArray[i],
                                    $"{DisplaySval}", bufficonBitmaps[i]));
                            if (skill3details.Text == "" || skill3details.Text == noticeStr)
                            {
                                skill3details.Text = noticeStr;
                                /*SkillLvs.skill3forExcel +=
                                    xlsSvalIcon[i] +
                                    (targetlistArray[i].Contains("[") && !xlsSvalIcon[i].Contains("<特殊·参考实际情况>")
                                        ? "·<特殊·参考实际情况> "
                                        : " ") +
                                    FuncArray[i].Replace("\r\n", "") + " 【{" +
                                    (lv1Array[i].Replace("\r\n", " ") ==
                                     lv10Array[i].Replace("\r\n", " ")
                                        ? lv10Array[i].Replace("\r\n", " ")
                                        : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                          lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";*/

                                SkillLvs.skill3forExcel +=
                                    xlsSvalIcon[i] +
                                    FuncArray[i].Replace("\r\n", "") + " 【{" +
                                    (lv1Array[i].Replace("\r\n", " ") ==
                                     lv10Array[i].Replace("\r\n", " ")
                                        ? lv10Array[i].Replace("\r\n", " ")
                                        : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                          lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";
                                break;
                            }

                            SkillLvs.skill3forExcel += FuncArray[i].Replace("\r\n", "") + " 【{" +
                                                       (lv1Array[i].Replace("\r\n", " ") ==
                                                        lv10Array[i].Replace("\r\n", " ")
                                                           ? lv10Array[i].Replace("\r\n", " ")
                                                           : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                                             lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";
                            break;
                    }
                }

                try
                {
                    switch (SkillNum)
                    {
                        case 1:
                            SkillLvs.skill1forExcel =
                                SkillLvs.skill1forExcel.Substring(0, SkillLvs.skill1forExcel.Length - 2);
                            break;
                        case 2:
                            SkillLvs.skill2forExcel =
                                SkillLvs.skill2forExcel.Substring(0, SkillLvs.skill2forExcel.Length - 2);
                            break;
                        case 3:
                            SkillLvs.skill3forExcel =
                                SkillLvs.skill3forExcel.Substring(0, SkillLvs.skill3forExcel.Length - 2);
                            break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            });
        }

        private string FindFuncTypeNameDebugger(string funcTypeId)
        {
            foreach (var funcListObjectDebugger in GlobalPathsAndDatas.funcListDebuggerArray)
                if (((JObject)funcListObjectDebugger)["funcTypeId"].ToString() == funcTypeId)
                    return funcListObjectDebugger["typeName"].ToString();
            return funcTypeId + "(未知)";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var clean = new Task(() => { ClearTexts(); });
            var clean2 = new Task(() => { ClearLists(); });
            clean2.Start();
            clean.Start();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)

        {
            if (sender is Hyperlink source) Process.Start(source.NavigateUri.ToString());
        }

        private async void HttpRequestData()
        {
            var path = Directory.GetCurrentDirectory();
            var gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
            var folder = new DirectoryInfo(path + @"\Android\");
            JObject res;
            var result = "";
            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = false; });
            updatedatabutton.Dispatcher.Invoke(() => { updatedatabutton.IsEnabled = false; });
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
            updatestatus.Dispatcher.Invoke(() => { updatesign.Text = "数据下载进行中,请勿退出!"; });
            AlteraGif.Dispatcher.Invoke(() => { AlteraGif.Visibility = Visibility.Visible; });
            progressbar.Dispatcher.Invoke(() =>
            {
                progressbar.Value = 0;
                progressbar.Visibility = Visibility.Visible;
            });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 1500; });
            progressring.Dispatcher.Invoke(() =>
            {
                progressring.Value = 0;
                progressring.Visibility = Visibility.Visible;
            });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 250; });
            progressloading.Dispatcher.Invoke(() => { progressloading.Visibility = Visibility.Visible; });
            if (!Directory.Exists(folder.FullName))
            {
                updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "正在创建Android目录..."; });
                Directory.CreateDirectory(folder.FullName);
            }

            if (!Directory.Exists(gamedata.FullName))
                Directory.CreateDirectory(gamedata.FullName);
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "开始下载/更新游戏数据......"; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 1250; });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 250; });
            try
            {
                var resulttmp = HttpRequest.Get("https://game.fate-go.jp/gamedata/top?appVer=2.83.0");
                result = resulttmp.ToString();
                res = resulttmp.ToJson();
                if (res["response"][0]["fail"]["action"] != null)
                    switch (res["response"][0]["fail"]["action"].ToString())
                    {
                        case "reconnection":
                        {
                            var tmp = res["response"][0]["fail"]["detail"].ToString();
                            Dispatcher.Invoke(() =>
                            {
                                GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                    Application.Current.MainWindow,
                                    $"Debug Mode: 检测到游戏服务器发送的重定向.\r\n重定向地址:\r\n\r\n【sandboxWebviewDomain】: {res["response"][0]["fail"]["sandboxWebviewDomain"]}\r\n【sandboxDomain】: {res["response"][0]["fail"]["sandboxDomain"]}\r\n【sandboxAssetsDomain】: {res["response"][0]["fail"]["sandboxAssetsDomain"]}",
                                    "重定向", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                            });
                            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
                            updatestatus.Dispatcher.Invoke(() => { updatesign.Text = ""; });
                            AlteraGif.Dispatcher.Invoke(() => { AlteraGif.Visibility = Visibility.Hidden; });
                            progressbar.Dispatcher.Invoke(() =>
                            {
                                progressbar.Visibility = Visibility.Hidden;
                                updatedatabutton.IsEnabled = true;
                            });
                            progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
                            progressloading.Dispatcher.Invoke(() =>
                            {
                                progressloading.Visibility = Visibility.Hidden;
                            });
                            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
                            return;
                        }
                        case "app_version_up":
                        {
                            var tmp = res["response"][0]["fail"]["detail"].ToString();
                            tmp = Regex.Replace(tmp, @".*新ver.：(.*)、現.*", "$1", RegexOptions.Singleline);
                            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "当前游戏版本: " + tmp; });
                            var result2 = HttpRequest.Get($"https://game.fate-go.jp/gamedata/top?appVer={tmp}")
                                .ToText();
                            res = JObject.Parse(result2);
                            if (!Directory.Exists(gamedata.FullName))
                                Directory.CreateDirectory(gamedata.FullName);
                            break;
                        }
                        case "goto_title":
                        case "retry":
                        {
                            var tmp = res["response"][0]["fail"]["detail"].ToString();
                            Dispatcher.Invoke(() =>
                            {
                                GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                    Application.Current.MainWindow,
                                    "游戏服务器内部错误，请稍后尝试下载/更新数据. \r\n以下为服务器公告内容:\r\n\r\n『" +
                                    tmp.Replace("[00FFFF]", "").Replace("[url=", "")
                                        .Replace("][u]公式サイト お知らせ[/u][/url][-]", "") + "』\r\n\r\n",
                                    "错误", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                            });
                            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
                            updatestatus.Dispatcher.Invoke(() => { updatesign.Text = ""; });
                            AlteraGif.Dispatcher.Invoke(() => { AlteraGif.Visibility = Visibility.Hidden; });
                            progressbar.Dispatcher.Invoke(() =>
                            {
                                progressbar.Visibility = Visibility.Hidden;
                                updatedatabutton.IsEnabled = true;
                            });
                            progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
                            progressloading.Dispatcher.Invoke(() =>
                            {
                                progressloading.Visibility = Visibility.Hidden;
                            });
                            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
                            return;
                        }
                        case "maint":
                        {
                            var tmp = res["response"][0]["fail"]["detail"].ToString();
                            Dispatcher.Invoke(() =>
                            {
                                GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                    Application.Current.MainWindow,
                                    "游戏服务器正在维护，请在维护后下载/更新数据. \r\n以下为服务器公告内容:\r\n\r\n『" +
                                    tmp.Replace("[00FFFF]", "").Replace("[url=", "")
                                        .Replace("][u]公式サイト お知らせ[/u][/url][-]", "") + "』\r\n\r\n点击\"确定\"可打开公告页面.",
                                    "维护中", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                            });
                            if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                            {
                                var re = new Regex(@"(?<url>http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?)");
                                var mc = re.Matches(tmp);
                                foreach (Match m in mc)
                                {
                                    var url = m.Result("${url}");
                                    Process.Start(url);
                                }
                            }

                            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
                            updatestatus.Dispatcher.Invoke(() => { updatesign.Text = ""; });
                            AlteraGif.Dispatcher.Invoke(() => { AlteraGif.Visibility = Visibility.Hidden; });
                            progressbar.Dispatcher.Invoke(() =>
                            {
                                progressbar.Visibility = Visibility.Hidden;
                                updatedatabutton.IsEnabled = true;
                            });
                            progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
                            progressloading.Dispatcher.Invoke(() =>
                            {
                                progressloading.Visibility = Visibility.Hidden;
                            });
                            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
                            return;
                        }
                    }
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(Application.Current.MainWindow, "网络连接异常,请检查网络连接并重试.\r\n" + e, "网络连接异常",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
                updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
                updatestatus.Dispatcher.Invoke(() => { updatesign.Text = ""; });
                progressbar.Dispatcher.Invoke(() =>
                {
                    progressbar.Visibility = Visibility.Hidden;
                    updatedatabutton.IsEnabled = true;
                });
                progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
                AlteraGif.Dispatcher.Invoke(() => { AlteraGif.Visibility = Visibility.Hidden; });
                progressloading.Dispatcher.Invoke(() => { progressloading.Visibility = Visibility.Hidden; });
                Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
                return;
            }

            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 1250; });
            if (!Directory.Exists(gamedata.FullName + "decrypted_masterdata"))
                Directory.CreateDirectory(gamedata.FullName + "decrypted_masterdata");
            try
            {
                File.WriteAllText(gamedata.FullName + "raw.json", res.ToString());
                File.WriteAllText(gamedata.FullName + "assetbundle.json",
                    res["response"][0]["success"]["assetbundle"].ToString());
            }
            catch (Exception)
            {
                //ignore
            }

            updatestatus.Dispatcher.Invoke(() =>
            {
                updatestatus.Text = "写入: " + gamedata.FullName + "assetbundle.json";
            });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 300; });
            File.WriteAllText(gamedata.FullName + "master.json",
                res["response"][0]["success"]["master"].ToString());
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "master.json"; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 300; });
            File.WriteAllText(gamedata.FullName + "webview.json",
                res["response"][0]["success"]["webview"].ToString());
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "webview.json"; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 300; });
            File.WriteAllText(gamedata.FullName + "decrypted_masterdata/" + "dateVer.json",
                res["response"][0]["success"]["dateVer"].ToString());
            File.WriteAllText(gamedata.FullName + "decrypted_masterdata/" + "dataVer.json",
                res["response"][0]["success"]["dataVer"].ToString());
            updatestatus.Dispatcher.Invoke(() =>
            {
                updatestatus.Text = "写入: " + gamedata.FullName + "assetbundleKey.json";
            });
            File.WriteAllText(gamedata.FullName + "assetbundlekey.json",
                JsonConvert.SerializeObject(MasterDataUnpacker.MouseInfoMsgPack(
                    Convert.FromBase64String(res["response"][0]["success"]["assetbundleKey"].ToString()))));
            var unixdatatime = res["response"][0]["success"]["dateVer"];
            updatestatus.Dispatcher.Invoke(() =>
            {
                updatestatus.Text = "已获取游戏数据时间(dateVer): " + Convert.ToString(
                    TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)) +
                    new TimeSpan(long.Parse(unixdatatime + "0000000")) + "  数据版本(dataVer):" +
                    res["response"][0]["success"]["dataVer"]);
            });
            var data = File.ReadAllText(gamedata.FullName + "master.json");
            var masterData =
                (Dictionary<string, byte[]>)MasterDataUnpacker.MouseGame2Unpacker(
                    Convert.FromBase64String(data));
            var miniMessagePacker = new MiniMessagePacker();
            foreach (var item in masterData)
            {
                var DataCount = masterData.Count;
                var ProgressValue = (double)9800 / DataCount;
                var unpackeditem = (List<object>)miniMessagePacker.Unpack(item.Value);
                var json = JsonConvert.SerializeObject(unpackeditem, Formatting.Indented);
                var t = "";
                try
                {
                    t = File.ReadAllText(gamedata.FullName + "decrypted_masterdata/" + item.Key + ".json");
                }
                catch (Exception)
                {
                    t = "";
                }

                if (t != json)
                {
                    File.WriteAllText(gamedata.FullName + "decrypted_masterdata/" + item.Key + ".json", json);
                    updatestatus.Dispatcher.Invoke(() =>
                    {
                        updatestatus.Text = "写入: " + gamedata.FullName +
                                            "decrypted__masterdata\\" + item.Key + ".json";
                    });
                    await LoadorRenewCommonDatas.ReloadGivenData(item.Key, json);
                }
                else
                {
                    updatestatus.Dispatcher.Invoke(() =>
                    {
                        updatestatus.Text = "跳过: " + gamedata.FullName +
                                            "decrypted__masterdata\\" + item.Key + ".json";
                    });
                }

                progressring.Dispatcher.Invoke(() => { progressring.Value += ProgressValue; });
            }

            GC.Collect();
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 1500; });
            var data2 = File.ReadAllText(gamedata.FullName + "assetbundle.json");
            var dictionary =
                (Dictionary<string, object>)MasterDataUnpacker.MouseInfoMsgPack(
                    Convert.FromBase64String(data2));
            var str = dictionary.Aggregate<KeyValuePair<string, object>, string>(null,
                (current, a) => current + a.Key + ": " + a.Value + "\r\n");
            File.WriteAllText(gamedata.FullName + "assetbundle.txt", str);
            updatestatus.Dispatcher.Invoke(() =>
            {
                updatestatus.Text = "folder name: " + dictionary["folderName"] + " 正在下载AssetStorage.txt...";
            });
            var AssetStorageRes = GetAssetStorage(dictionary["folderName"].ToString());
            File.WriteAllText(gamedata.FullName + "assetBundleFolder.txt", dictionary["folderName"].ToString());
            if (File.Exists(gamedata.FullName + "AssetStorage.txt"))
            {
                var ASReadtmp = File.ReadAllText(gamedata.FullName + "AssetStorage.txt");
                if (ASReadtmp != AssetStorageRes)
                {
                    if (File.Exists(gamedata.FullName + "AssetStorage_last.txt"))
                        File.Delete(gamedata.FullName + "AssetStorage_last.txt");
                    File.Move(gamedata.FullName + "AssetStorage.txt", gamedata.FullName + "AssetStorage_last.txt");
                    File.WriteAllText(gamedata.FullName + "AssetStorage.txt", AssetStorageRes);
                    var ASLine = File.ReadAllLines(gamedata.FullName + "AssetStorage.txt");
                    DecryptBinfileSub(ASLine, gamedata);
                }
                else
                {
                    if (!File.Exists(gamedata.FullName + "AssetStorage_last.txt"))
                        File.Copy(gamedata.FullName + "AssetStorage.txt", gamedata.FullName + "AssetStorage_last.txt");
                    var ASLine = File.ReadAllLines(gamedata.FullName + "AssetStorage.txt");
                    DecryptBinfileSub(ASLine, gamedata);
                }
            }
            else
            {
                File.WriteAllText(gamedata.FullName + "AssetStorage.txt", AssetStorageRes);
                var ASLine = File.ReadAllLines(gamedata.FullName + "AssetStorage.txt");
                DecryptBinfileSub(ASLine, gamedata);
            }

            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 150; });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 40; });
            var data3 = File.ReadAllText(gamedata.FullName + "webview.json");
            var dictionary2 =
                (Dictionary<string, object>)MasterDataUnpacker.MouseGame2MsgPack(
                    Convert.FromBase64String(data3));
            var str2 = "baseURL: " + dictionary2["baseURL"] + "\r\n contactURL: " +
                       dictionary2["contactURL"] + "\r\n";
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = str2; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 150; });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 40; });
            var filePassInfo = (Dictionary<string, object>)dictionary2["filePass"];
            str = filePassInfo.Aggregate(str, (current, a) => current + a.Key + ": " + a.Value + "\r\n");
            File.WriteAllText(gamedata.FullName + "webview.txt", str2);
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "webview.txt"; });
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "正在更新数据..."; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 450; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 450; });
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "下载/更新完成，可以开始解析."; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value = progressbar.Maximum; });
            progressring.Dispatcher.Invoke(() => { progressring.Value = progressring.Maximum; });
            Dispatcher.Invoke(() =>
            {
                MessageBox.Success("下载/更新完成，可以开始解析.", "完成");
                StarterItemTab.IsSelected = true;
            });
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
            updatestatus.Dispatcher.Invoke(() => { updatesign.Text = ""; });
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Hidden;
                updatedatabutton.IsEnabled = true;
            });
            Dispatcher.Invoke(() =>
            {
                if (!File.Exists(gamedata.FullName + "decrypted_masterdata/" + "dateVer.json")) return;
                GameDataVersion =
                    File.ReadAllText(gamedata.FullName + "decrypted_masterdata/" + "dateVer.json");
                var dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                var DataVersionTimer = new TimeSpan(long.Parse(GameDataVersion + "0000000"));
                var DataVerTag = Convert.ToString(dateTimeStart + DataVersionTimer);
                DataVersionLabel.Text = "游戏数据时间:" + DataVerTag;
                SvtIDHelper.Visibility = Visibility.Visible;
                if (!File.Exists(gamedata.FullName + "decrypted_masterdata/" + "dataVer.json")) return;
                var dataVersion =
                    File.ReadAllText(gamedata.FullName + "decrypted_masterdata/" + "dataVer.json");
                DataVersionLabel.Text += "\r\n游戏数据版本:" + dataVersion;
            });
            progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
            AlteraGif.Dispatcher.Invoke(() => { AlteraGif.Visibility = Visibility.Hidden; });
            progressloading.Dispatcher.Invoke(() => { progressloading.Visibility = Visibility.Hidden; });
            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
            GC.Collect();
        }

        private void DecryptBinfileSub(string[] assetStore, DirectoryInfo outputdest)
        {
            var decrypt = outputdest;
            var AudioArray = new JArray();
            var MovieArray = new JArray();
            var AssetArray = new JArray();
            for (var i = 2; i < assetStore.Length; ++i)
            {
                var tmp = assetStore[i].Split(',');
                string assetName;
                string fileName;
                if (tmp[4].Contains("Audio"))
                {
                    string keyIdA;
                    switch (tmp.Length)
                    {
                        case 5:
                            assetName = tmp[tmp.Length - 1].Replace('/', '@');
                            keyIdA = "0";
                            break;
                        case 6:
                            assetName = tmp[tmp.Length - 2].Replace('/', '@');
                            keyIdA = tmp[tmp.Length - 1];
                            break;
                        default:
                            assetName = tmp[4].Replace('/', '@') + ".unity3d";
                            keyIdA = "0";
                            break;
                    }

                    fileName = CatAndMouseGame.GetMD5String(assetName);
                    AudioArray.Add(new JObject(new JProperty("audioName", assetName),
                        new JProperty("fileName", fileName), new JProperty("keyId", keyIdA)));
                }
                else if (tmp[4].Contains("Movie") || tmp[4].Contains(".usm"))
                {
                    string keyIdM;
                    switch (tmp.Length)
                    {
                        case 5:
                            assetName = tmp[tmp.Length - 1].Replace('/', '@');
                            keyIdM = "0";
                            break;
                        case 6:
                            assetName = tmp[tmp.Length - 2].Replace('/', '@');
                            keyIdM = tmp[tmp.Length - 1];
                            break;
                        default:
                            assetName = tmp[4].Replace('/', '@') + ".unity3d";
                            keyIdM = "0";
                            break;
                    }

                    fileName = CatAndMouseGame.GetMD5String(assetName);
                    MovieArray.Add(new JObject(new JProperty("movieName", assetName),
                        new JProperty("fileName", fileName), new JProperty("keyId", keyIdM)));
                }
                else if (!tmp[4].Contains("Movie"))
                {
                    string keyId;
                    switch (tmp.Length)
                    {
                        case 5:
                            assetName = tmp[tmp.Length - 1].Replace('/', '@') + ".unity3d";
                            keyId = "0";
                            break;
                        case 6:
                            assetName = tmp[tmp.Length - 2].Replace('/', '@') + ".unity3d";
                            keyId = tmp[tmp.Length - 1];
                            break;
                        default:
                            assetName = tmp[4].Replace('/', '@') + ".unity3d";
                            keyId = "0";
                            break;
                    }

                    fileName = CatAndMouseGame.GetShaName(assetName);
                    AssetArray.Add(new JObject(new JProperty("assetName", assetName),
                        new JProperty("fileName", fileName), new JProperty("keyId", keyId)));
                }
            }

            File.WriteAllText(decrypt.FullName + @"\AudioName.json", AudioArray.ToString());
            File.WriteAllText(decrypt.FullName + @"\MovieName.json", MovieArray.ToString());
            File.WriteAllText(decrypt.FullName + @"\AssetName.json", AssetArray.ToString());
        }

        private static string GetAssetStorage(string assetBundleKey)
        {
            var assetStorage = HttpRequest
                .Get($"https://cdn.data.fate-go.jp/AssetStorages/{assetBundleKey}Android/AssetStorage.txt")
                .ToText();
            return CatAndMouseGame.MouseGame8(assetStorage);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var UpgradeMasterData = new Task(() => { HttpRequestData(); });
            UpgradeMasterData.Start();
        }

        private void JBOut()
        {
            var output = "";
            output = "【文本1】:\n\r" + JB.JB1 + "\n\r" +
                     "【文本2】:\n\r" + JB.JB2 + "\n\r" +
                     "【文本3】:\n\r" + JB.JB3 + "\n\r" +
                     "【文本4】:\n\r" + JB.JB4 + "\n\r" +
                     "【文本5】:\n\r" + JB.JB5 + "\n\r" +
                     "【文本6】:\n\r" + JB.JB6 + "\n\r" +
                     "【文本7】:\n\r" + JB.JB7;
            if (!Directory.Exists(GlobalPathsAndDatas.outputdir.FullName))
                Directory.CreateDirectory(GlobalPathsAndDatas.outputdir.FullName);
            File.WriteAllText(GlobalPathsAndDatas.outputdir.FullName + "牵绊文本_" + JB.svtid + "_" + JB.svtnme + ".txt",
                output);
            Dispatcher.Invoke(() =>
            {
                MessageBox.Success("导出完成.\n\r文件名为: " + GlobalPathsAndDatas.outputdir.FullName +
                                   "牵绊文本_" + JB.svtid + "_" + JB.svtnme +
                                   ".txt", "完成");
            });

            Process.Start(GlobalPathsAndDatas.outputdir.FullName + "/" + "牵绊文本_" + JB.svtid + "_" + JB.svtnme + ".txt");
        }

        private void JBOutput_Click(object sender, RoutedEventArgs e)
        {
            var JO = new Task(() => { JBOut(); });
            JO.Start();
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink source) Process.Start(source.NavigateUri.ToString());
        }

        private async void LoadingProgress()
        {
            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
            var path = Directory.GetCurrentDirectory();
            var gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
            VersionLabel.Dispatcher.Invoke(() => { VersionLabel.Text = CommonStrings.Version; });
            DataLoadingRing.Dispatcher.Invoke(() => { DataLoadingRing.Visibility = Visibility.Visible; });
            SvtIDHelper.Dispatcher.Invoke(() => { SvtIDHelper.Visibility = Visibility.Collapsed; });
            if (!Directory.Exists(gamedata.FullName))
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Info("没有游戏数据,请先下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:");
                    DataUpdate.IsSelected = true;
                    SvtIDHelper.Visibility = Visibility.Collapsed;
                });
                Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
                DataLoadingRing.Dispatcher.Invoke(() => { DataLoadingRing.Visibility = Visibility.Collapsed; });
            }
            else
            {
                try
                {
                    Dispatcher.Invoke(() => { Growl.Info("正在读取数据..."); });
                    await LoadorRenewCommonDatas.ReloadData();
                    Dispatcher.Invoke(() => { Growl.Info("读取完毕."); });
                    Dispatcher.Invoke(() =>
                    {
                        if (!File.Exists(gamedata.FullName + "decrypted_masterdata/" + "dateVer.json")) return;
                        GameDataVersion =
                            File.ReadAllText(gamedata.FullName + "decrypted_masterdata/" + "dateVer.json");
                        var dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                        var DataVersionTimer = new TimeSpan(long.Parse(GameDataVersion + "0000000"));
                        var DataVerTag = Convert.ToString(dateTimeStart + DataVersionTimer);
                        DataVersionLabel.Text += DataVerTag;
                        SvtIDHelper.Visibility = Visibility.Visible;
                        if (!File.Exists(gamedata.FullName + "decrypted_masterdata/" + "dataVer.json")) return;
                        var dataVersion =
                            File.ReadAllText(gamedata.FullName + "decrypted_masterdata/" + "dataVer.json");
                        DataVersionLabel.Text += "\r\n游戏数据版本:" + dataVersion;
                    });
                    Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                    DataLoadingRing.Dispatcher.Invoke(() => { DataLoadingRing.Visibility = Visibility.Collapsed; });
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Info("游戏数据损坏,请重新下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:");
                        DataUpdate.IsSelected = true;
                        SvtIDHelper.Visibility = Visibility.Collapsed;
                    });
                    Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
                    DataLoadingRing.Dispatcher.Invoke(() => { DataLoadingRing.Visibility = Visibility.Collapsed; });
                }
            }
        }

        private void Form_Load(object sender, EventArgs eventArgs)
        {
            var Loading = new Task(() => { LoadingProgress(); });
            Loading.Start();
            var Zeros = new int[121];
            var levels = new int[121];
            for (var i = 0; i <= 120; i++)
            {
                Zeros[i] = 0;
                levels[i] = i;
            }

            LabelX = new string[121];
            LineHP = Zeros;
            LineATK = Zeros;
            HPCurveX.Values = LineHP.AsChartValues();
            ATKCurveX.Values = LineATK.AsChartValues();
            for (var j = 0; j <= 120; j++) LabelX[j] = levels[j].ToString();
            DataContext = this;
            GlobalPathsAndDatas.SvtIndividualityTranslation =
                File.Exists(GlobalPathsAndDatas.gamedata.FullName + "SvtIndividualityTranslation.json")
                    ? (JArray)JsonConvert.DeserializeObject(File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName +
                                                                             "SvtIndividualityTranslation.json"))
                    : null;
            GlobalPathsAndDatas.TDAttackNameTranslation =
                File.Exists(GlobalPathsAndDatas.gamedata.FullName + "TDAttackNameTranslation.json")
                    ? (JArray)JsonConvert.DeserializeObject(
                        File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "TDAttackNameTranslation.json"))
                    : null;
            GlobalPathsAndDatas.appendSkillTranslationArray =
                File.Exists(GlobalPathsAndDatas.gamedata.FullName + "AppendSkillTranslation.json")
                    ? (JArray)JsonConvert.DeserializeObject(
                        File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "AppendSkillTranslation.json"))
                    : null;
            if (File.Exists(GlobalPathsAndDatas.gamedata.FullName + "BuffTranslation.json"))
                GlobalPathsAndDatas.TranslationList =
                    (JArray)JsonConvert.DeserializeObject(
                        File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "BuffTranslation.json"));
            else
                GlobalPathsAndDatas.TranslationList = null;

            if (!File.Exists(GlobalPathsAndDatas.gamedata.FullName + "SvtIndividualityTranslation.json") ||
                !File.Exists(GlobalPathsAndDatas.gamedata.FullName + "TDAttackNameTranslation.json") ||
                !File.Exists(GlobalPathsAndDatas.gamedata.FullName + "BuffTranslation.json") ||
                !File.Exists(GlobalPathsAndDatas.gamedata.FullName + "AppendSkillTranslation.json"))
                Dispatcher.Invoke(() => { Growl.Info("翻译列表缺失,建议先前往数据更新选项卡更新."); });
        }

        private void ExcelFileOutput()
        {
            try
            {
                var path = Directory.GetCurrentDirectory();
                var svtData = new DirectoryInfo(path + @"\ServantData\");
                var mstData = new DirectoryInfo(path + @"\Android\masterdata\");
                if (!Directory.Exists(svtData.FullName))
                    Directory.CreateDirectory(svtData.FullName);
                Stream streamget;
                if (!File.Exists(mstData.FullName + @"\SvtBasicInfoBotBrandNew.xlsx"))
                {
                    streamget = HttpRequest.GetXlsx();
                    var fileStream = File.Create(mstData.FullName + @"\SvtBasicInfoBotBrandNew.xlsx");
                    streamget.CopyTo(fileStream);
                    fileStream.Close();
                    streamget.Close();
                }

                var xlsx =
                    new ExcelPackage(new FileStream(mstData.FullName + @"\SvtBasicInfoBotBrandNew.xlsx",
                        FileMode.Open));
                var worksheet = xlsx.Workbook.Worksheets[0];
                var Pickup = new ExcelAddress("C30");
                //worksheet.ConditionalFormatting.RemoveAll();
                worksheet.Cells["C4"].Value = DateTime.Now.ToString();
                worksheet.Cells["M8"].Value = JB.svtid;
                worksheet.Cells["A1"].Value += "(" + JB.svtnme + ")";
                worksheet.Cells["C6"].Value = Svtname.Text;
                worksheet.Cells["C7"].Value = IndividualalityClean.Text;
                worksheet.Cells["C19"].Value = BeiZhu.Text;
                worksheet.Cells["J6"].Value = svtclass.Text;
                worksheet.Cells["G7"].Value = rarity.Text;
                worksheet.Cells["J7"].Value = gendle.Text;
                worksheet.Cells["M7"].Value = hiddenattri.Text;
                worksheet.Cells["M4"].Value = collection.Text;
                worksheet.Cells["C9"].Value = cv.Text;
                worksheet.Cells["J9"].Value = illust.Text;
                worksheet.Cells["C8"].Value = ssvtstarrate.Text;
                worksheet.Cells["G8"].Value = ssvtdeathrate.Text;
                worksheet.Cells["J8"].Value = jixing.Text;
                worksheet.Cells["M15"].Value = notrealnprate.Text;
                worksheet.Cells["C17"].Value = nprate.Text;
                worksheet.Cells["E12"].Value = Convert.ToDecimal(basichp.Text);
                worksheet.Cells["E13"].Value = Convert.ToDecimal(basicatk.Text);
                worksheet.Cells["G12"].Value = Convert.ToDecimal(maxhp.Text);
                worksheet.Cells["G13"].Value = Convert.ToDecimal(maxatk.Text);
                worksheet.Cells["I12"].Value = GlobalPathsAndDatas.lv100hp;
                worksheet.Cells["I13"].Value = GlobalPathsAndDatas.lv100atk;
                worksheet.Cells["K12"].Value = GlobalPathsAndDatas.lv120hp;
                worksheet.Cells["K13"].Value = GlobalPathsAndDatas.lv120atk;
                worksheet.Cells["C15"].Value = cards.Text;
                worksheet.Cells["G24"].Value = bustercard.Text;
                worksheet.Cells["G23"].Value = artscard.Text;
                worksheet.Cells["G22"].Value = quickcard.Text;
                worksheet.Cells["G25"].Value = extracard.Text;
                worksheet.Cells["G26"].Value = treasuredevicescard.Text;
                worksheet.Cells["C30"].Value = npcardtype.Text;
                worksheet.Cells["M30"].Value = nptype.Text;
                worksheet.Cells["G30"].Value = nprank.Text;
                worksheet.Cells["C28"].Value = npruby.Text;
                worksheet.Cells["C29"].Value = npname.Text;
                worksheet.Cells["C31"].Value = npdetail.Text;
                worksheet.Row(31).Height = npdetail.Text.Length / 25 * 24; //自适应高度 Detail
                //if (npdetail.Text.Length >= 240) worksheet.Cells["E30"].Style.Font.Size = 7.5f;
                worksheet.Cells["C35"].Value = skill1name.Text;
                //if (skill1name.Text.Length >= 25) worksheet.Cells["Q3"].Style.Font.Size = 9;
                worksheet.Cells["M35"].Value = skill1cdlv1.Text + " → " + skill1cdlv6.Text + " → " + skill1cdlv10.Text;
                if (skill1cdlv1.Text == "") worksheet.Cells["M35"].Value = "";
                worksheet.Cells["C37"].Value = skill1details.Text;
                worksheet.Row(37).Height = skill1details.Text.Length / 25 * 24; //自适应高度 Detail
                //worksheet.Row(37).Height = (Regex.Matches(skill1details.Text, "\r\n").Count + 1 ) * 24;     //自适应高度
                // if (skill1details.Text.Length >= 150) worksheet.Cells["Q4"].Style.Font.Size = 7.5f;
                worksheet.Cells["C40"].Value = skill2name.Text;
                //if (skill2name.Text.Length >= 15) worksheet.Cells["Q14"].Style.Font.Size = 9;
                worksheet.Cells["M40"].Value = skill2cdlv1.Text + " → " + skill2cdlv6.Text + " → " + skill2cdlv10.Text;
                if (skill2cdlv1.Text == "") worksheet.Cells["M40"].Value = "";
                worksheet.Cells["C42"].Value = skill2details.Text;
                worksheet.Row(42).Height = skill2details.Text.Length / 25 * 24; //自适应高度 Detail
                //worksheet.Row(42).Height = (Regex.Matches(skill2details.Text, "\r\n").Count + 1) * 24;     //自适应高度
                //if (skill2details.Text.Length >= 150) worksheet.Cells["Q15"].Style.Font.Size = 7.5f;
                worksheet.Cells["C45"].Value = skill3name.Text;
                //if (skill3name.Text.Length >= 15) worksheet.Cells["Q25"].Style.Font.Size = 9;
                worksheet.Cells["M45"].Value = skill3cdlv1.Text + " → " + skill3cdlv6.Text + " → " + skill3cdlv10.Text;
                if (skill3cdlv1.Text == "") worksheet.Cells["M45"].Value = "";
                worksheet.Cells["C47"].Value = skill3details.Text;
                worksheet.Row(47).Height = skill3details.Text.Length / 25 * 24; //自适应高度 Detail
                //worksheet.Row(47).Height = (Regex.Matches(skill3details.Text, "\r\n").Count + 1) * 24;     //自适应高度
                //if (skill3details.Text.Length >= 150) worksheet.Cells["Q26"].Style.Font.Size = 7.5f;
                worksheet.Cells["C14"].Value = svtIndividuality.Text;
                worksheet.Cells["C10"].Value = Convert.ToString(sixwei.Text);
                worksheet.Cells["M12"].Value = SkillLvs.HpBalanceForExcel;
                worksheet.Cells["C38"].Value = SkillLvs.skill1forExcel;
                worksheet.Row(38).Height = (double)(Regex.Matches(SkillLvs.skill1forExcel, "\r\n").Count +
                                                    SkillLvs.skill1forExcel.Count(c => c == '(') / 4 + 1) * 24 >= 80
                    ? (double)(Regex.Matches(SkillLvs.skill1forExcel, "\r\n").Count +
                               SkillLvs.skill1forExcel.Count(c => c == '(') / 4 + 1) * 24
                    : 80; //自适应高度
                /*if (Regex.Matches(SkillLvs.ClassPassiveforExcel, "效果").Count > 7)
                    worksheet.Cells["C46"].Style.Font.Size = 7f;*/
                /*if (SkillLvs.skill1forExcel.Length >= 300) worksheet.Cells["Q8"].Style.Font.Size = 7.5f;
                if (Regex.Matches(SkillLvs.skill1forExcel, "【").Count >= 6 &&
                    Regex.Matches(SkillLvs.skill1forExcel, "【").Count < 7)
                    worksheet.Cells["Q8"].Style.Font.Size = 7.5f;
                else if (Regex.Matches(SkillLvs.skill1forExcel, "【").Count >= 7 &&
                         Regex.Matches(SkillLvs.skill1forExcel, "【").Count < 10)
                    worksheet.Cells["Q8"].Style.Font.Size = 6.5f;
                else if (Regex.Matches(SkillLvs.skill1forExcel, "【").Count >= 10)
                    worksheet.Cells["Q8"].Style.Font.Size = 5.5f;*/
                worksheet.Cells["C43"].Value = SkillLvs.skill2forExcel;
                worksheet.Row(43).Height = (double)(Regex.Matches(SkillLvs.skill2forExcel, "\r\n").Count +
                                                    SkillLvs.skill2forExcel.Count(c => c == '(') / 4 + 1) * 24 >= 80
                    ? (double)(Regex.Matches(SkillLvs.skill2forExcel, "\r\n").Count +
                               SkillLvs.skill2forExcel.Count(c => c == '(') / 4 + 1) * 24
                    : 80; //自适应高度
                /* if (SkillLvs.skill2forExcel.Length >= 300) worksheet.Cells["Q19"].Style.Font.Size = 7.5f;
                 if (Regex.Matches(SkillLvs.skill2forExcel, "【").Count >= 6 &&
                     Regex.Matches(SkillLvs.skill2forExcel, "【").Count < 7)
                     worksheet.Cells["Q19"].Style.Font.Size = 7.5f;
                 else if (Regex.Matches(SkillLvs.skill2forExcel, "【").Count >= 7 &&
                          Regex.Matches(SkillLvs.skill2forExcel, "【").Count < 10)
                     worksheet.Cells["Q19"].Style.Font.Size = 6.5f;
                 else if (Regex.Matches(SkillLvs.skill2forExcel, "【").Count >= 10)
                     worksheet.Cells["Q19"].Style.Font.Size = 5.5f;*/
                worksheet.Cells["C48"].Value = SkillLvs.skill3forExcel;
                worksheet.Row(48).Height = (double)(Regex.Matches(SkillLvs.skill3forExcel, "\r\n").Count +
                                                    SkillLvs.skill3forExcel.Count(c => c == '(') / 4 + 1) * 24 >= 80
                    ? (double)(Regex.Matches(SkillLvs.skill3forExcel, "\r\n").Count +
                               SkillLvs.skill3forExcel.Count(c => c == '(') / 4 + 1) * 24
                    : 80; //自适应高度
                /*if (SkillLvs.skill3forExcel.Length >= 300) worksheet.Cells["Q30"].Style.Font.Size = 7.5f;
                if (Regex.Matches(SkillLvs.skill3forExcel, "【").Count >= 6 &&
                    Regex.Matches(SkillLvs.skill3forExcel, "【").Count < 7)
                    worksheet.Cells["Q30"].Style.Font.Size = 7.5f;
                else if (Regex.Matches(SkillLvs.skill3forExcel, "【").Count >= 7 &&
                         Regex.Matches(SkillLvs.skill3forExcel, "【").Count < 10)
                    worksheet.Cells["Q30"].Style.Font.Size = 6.5f;
                else if (Regex.Matches(SkillLvs.skill3forExcel, "【").Count >= 10)
                    worksheet.Cells["Q30"].Style.Font.Size = 5.5f;*/
                worksheet.Cells["C32"].Value = SkillLvs.TDforExcel;
                worksheet.Row(32).Height = (double)(Regex.Matches(SkillLvs.TDforExcel, "\r\n").Count +
                                                    SkillLvs.TDforExcel.Count(c => c == '(') / 4 + 1) * 24 >= 80
                    ? (double)(Regex.Matches(SkillLvs.TDforExcel, "\r\n").Count +
                               SkillLvs.TDforExcel.Count(c => c == '(') / 4 + 1) * 24
                    : 80; //自适应高度
                /*if (Regex.Matches(SkillLvs.TDforExcel, "【").Count >= 7 || SkillLvs.TDforExcel.Length >= 400)
                    worksheet.Cells["E37"].Style.Font.Size = 7.5f;*/
                worksheet.Cells["E50"].Value = GlobalPathsAndDatas.AS1N;
                worksheet.Cells["E52"].Value = GlobalPathsAndDatas.AS2N;
                worksheet.Cells["E54"].Value = GlobalPathsAndDatas.AS3N;
                worksheet.Cells["E56"].Value = GlobalPathsAndDatas.AS4N;
                worksheet.Cells["E58"].Value = GlobalPathsAndDatas.AS5N;
                worksheet.Cells["E51"].Value = GlobalPathsAndDatas.AS1D;
                worksheet.Cells["E53"].Value = GlobalPathsAndDatas.AS2D;
                worksheet.Cells["E55"].Value = GlobalPathsAndDatas.AS3D;
                worksheet.Cells["E57"].Value = GlobalPathsAndDatas.AS4D;
                worksheet.Cells["E59"].Value = GlobalPathsAndDatas.AS5D;
                worksheet.Cells["E24"].Value = SkillLvs.NPB;
                worksheet.Cells["E23"].Value = SkillLvs.NPA;
                worksheet.Cells["E22"].Value = SkillLvs.NPQ;
                worksheet.Cells["E25"].Value = SkillLvs.NPEX;
                worksheet.Cells["E26"].Value = SkillLvs.NPTD;
                worksheet.Cells["A61"].Value = SkillLvs.ClassPassiveforExcel;
                //worksheet.Row(61).Height = (Regex.Matches(SkillLvs.ClassPassiveforExcel, "效果").Count + Regex.Matches(SkillLvs.ClassPassiveforExcel, "＋" + 1).Count) * 24;     //自适应高度
                worksheet.Row(61).Height = SkillLvs.ClassPassiveforExcel.Length / 48 * 24; //自适应高度 Detail
                /*if (GlobalPathsAndDatas.lv150atk != 0.0M)
                {
                    var txtOut = "Lv.150 HP/ATK:\r\n" + GlobalPathsAndDatas.lv150hp + "/" +
                                 GlobalPathsAndDatas.lv150atk;
                    worksheet.Cells["K10"].Value = txtOut;
                    worksheet.Cells["K10"].Style.WrapText = true;
                }
                else
                {
                    worksheet.Cells["K10"].Value = "";
                    worksheet.Cells["K10"].Style.WrapText = true;
                }
                */
                try
                {
                    var classicon = BitmapImage2Bitmap((BitmapSource)ClassPng.Source);
                    var classi = worksheet.Drawings.AddPicture("ClassIcon", classicon);
                    classi.SetPosition(5, 2, 12, 48);
                    classi.SetSize(48, 48);
                }
                catch (Exception)
                {
                    //ignore
                }

                try
                {
                    var sk1icon = BitmapImage2Bitmap((BitmapSource)sk1_icon.Source);
                    var sk1i = worksheet.Drawings.AddPicture("Skill1Icon", sk1icon);
                    sk1i.SetPosition(35, 20, 12, 48);
                    sk1i.SetSize(48, 48);
                }
                catch (Exception)
                {
                    //ignore
                }

                try
                {
                    var sk2icon = BitmapImage2Bitmap((BitmapSource)sk2_icon.Source);
                    var sk2i = worksheet.Drawings.AddPicture("Skill2Icon", sk2icon);
                    sk2i.SetPosition(40, 20, 12, 48);
                    sk2i.SetSize(48, 48);
                }
                catch (Exception)
                {
                    //ignore
                }

                try
                {
                    var sk3icon = BitmapImage2Bitmap((BitmapSource)sk3_icon.Source);
                    var sk3i = worksheet.Drawings.AddPicture("Skill3Icon", sk3icon);
                    sk3i.SetPosition(45, 20, 12, 48);
                    sk3i.SetSize(48, 48);
                }
                catch (Exception)
                {
                    //ignore
                }

                switch (worksheet.Cells["C30"].Value.ToString())
                {
                    case "Quick":
                        var GreenExp = worksheet.ConditionalFormatting.AddExpression(Pickup);
                        GreenExp.Formula = "C30=\"Quick\"";
                        GreenExp.Style.Font.Bold = true;
                        GreenExp.Style.Font.Color.Color = Color.LightGreen;
                        worksheet.Cells["C28"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C28"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(169, 208, 142));
                        worksheet.Cells["C29"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C29"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(169, 208, 142));
                        break;
                    case "Arts":
                        var BlueExp = worksheet.ConditionalFormatting.AddExpression(Pickup);
                        BlueExp.Formula = "C30=\"Arts\"";
                        BlueExp.Style.Font.Bold = true;
                        BlueExp.Style.Font.Color.Color = Color.Blue;
                        worksheet.Cells["C28"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C28"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(155, 194, 230));
                        worksheet.Cells["C29"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C29"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(155, 194, 230));
                        break;
                    case "Buster":
                        var RedExp = worksheet.ConditionalFormatting.AddExpression(Pickup);
                        RedExp.Formula = "C30=\"Buster\"";
                        RedExp.Style.Font.Bold = true;
                        RedExp.Style.Font.Color.Color = Color.Red;
                        worksheet.Cells["C28"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C28"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 150, 137));
                        worksheet.Cells["C29"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells["C29"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 150, 137));
                        break;
                }

                xlsx.SaveAs(new FileInfo(svtData.FullName + JB.svtnme + "_" + JB.svtid + ".xlsx"));
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        "导出成功,文件名为: " + svtData.FullName + JB.svtnme + "_" + JB.svtid + ".xlsx", "导出完成",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
                Process.Start(svtData.FullName + JB.svtnme + "_" + JB.svtid + ".xlsx");
                GC.Collect();
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                        Application.Current.MainWindow,
                        "导出时遇到错误,请查看该从者的xlsx是否被占用?\r\n\r\n点击\"确认\"进行重试.\r\n\r\n" + e,
                        "导出错误",
                        MessageBoxButton.OKCancel, MessageBoxImage.Error);
                });
                if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK) ExcelFileOutput();
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapSource m)
        {
            var bmp = new Bitmap(m.PixelWidth, m.PixelHeight, PixelFormat.Format32bppPArgb);

            var data = bmp.LockBits(
                new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        private void VersionCheckEvent()
        {
            string VerChkRaw;
            JObject VerChk;
            JArray VerAssetsJArray;
            GlobalPathsAndDatas.ExeUpdateUrl = "";
            GlobalPathsAndDatas.NewerVersion = "";
            try
            {
                VerChkRaw = HttpRequest.GetApplicationUpdateJson();
                VerChk = JObject.Parse(VerChkRaw);
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => { MessageBox.Error("网络连接异常,请检查网络连接并重试.\r\n" + e, "网络连接异常"); });
                CheckUpdate.Dispatcher.Invoke(() => { CheckUpdate.IsEnabled = true; });
                return;
            }

            if (CommonStrings.VersionTag != VerChk["tag_name"].ToString())
            {
                Dispatcher.Invoke(() =>
                {
                    GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                        Application.Current.MainWindow,
                        "检测到软件更新\r\n\r\n新版本为:  " + VerChk["tag_name"] + "    当前版本为:  " + CommonStrings.VersionTag +
                        "\r\n\r\nChangeLog:\r\n" + VerChk["body"] + "\r\n\r\n点击\"确认\"按钮可选择更新.", "检查更新",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question);
                });
                if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                {
                    VerAssetsJArray = (JArray)JsonConvert.DeserializeObject(VerChk["assets"].ToString());
                    for (var i = 0; i <= VerAssetsJArray.Count - 1; i++)
                        if (VerAssetsJArray[i]["name"].ToString() == "Altera.exe")
                            GlobalPathsAndDatas.ExeUpdateUrl = VerAssetsJArray[i]["browser_download_url"].ToString();
                    if (GlobalPathsAndDatas.ExeUpdateUrl == "")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(
                                Application.Current.MainWindow, "确认到新版本更新,但是获取下载Url失败.\r\n", "获取Url失败",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        });
                        MessageBox.Error("确认到新版本更新,但是获取下载Url失败.\r\n", "获取Url失败");
                        CheckUpdate.Dispatcher.Invoke(() => { CheckUpdate.IsEnabled = true; });
                        return;
                    }

                    var Sub = new Task(() => { DownloadFilesSub(VerChk["tag_name"].ToString()); });
                    Sub.Start();
                }
                else
                {
                    CheckUpdate.Dispatcher.Invoke(() => { CheckUpdate.IsEnabled = true; });
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Info("当前版本为:  " + CommonStrings.VersionTag + "\r\n\r\n无需更新.", "检查更新");
                });
                CheckUpdate.Dispatcher.Invoke(() => { CheckUpdate.IsEnabled = true; });
            }
        }

        private void DownloadFilesSub(object VerChk)
        {
            var path = Directory.GetCurrentDirectory();
            try
            {
                DownloadFile(GlobalPathsAndDatas.ExeUpdateUrl, path + "/Altera(Update " + VerChk + ").exe");
                GlobalPathsAndDatas.NewerVersion = VerChk.ToString();
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        Application.Current.MainWindow, "写入文件异常.\r\n" + e, "异常", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
                CheckUpdate.Dispatcher.Invoke(() => { CheckUpdate.IsEnabled = true; });
                throw;
            }
        }

        public void DownloadFile(string url, string filePath)
        {
            var Downloads = new WebClient();
            GlobalPathsAndDatas.StartTime = DateTime.Now;
            progressbar1.Dispatcher.Invoke(() =>
            {
                progressbar1.Visibility = Visibility.Visible;
                progressbar1.Value = 0;
            });
            updatestatus2.Dispatcher.Invoke(() => { updatestatus2.Text = ""; });
            Downloads.DownloadProgressChanged += OnDownloadProgressChanged;
            Downloads.DownloadFileCompleted += OnDownloadFileCompleted;
            Downloads.DownloadFileAsync(new Uri(url), filePath);
        }

        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            Dispatcher.Invoke(() =>
            {
                GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                    Application.Current.MainWindow,
                    "下载完成.下载目录为: \r\n" + path + "\\Altera(Update " +
                    GlobalPathsAndDatas.NewerVersion +
                    ").exe\r\n\r\n请自行替换文件.\r\n\r\n您是否要关闭当前版本的程序?", "检查更新", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            });
            if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.Yes)
                Dispatcher.Invoke(Close);
            CheckUpdate.Dispatcher.Invoke(() => { CheckUpdate.IsEnabled = true; });
            progressbar1.Dispatcher.Invoke(() =>
            {
                progressbar1.Visibility = Visibility.Hidden;
                progressbar1.Value = 0;
            });
            updatestatus2.Dispatcher.Invoke(() => { updatestatus2.Text = ""; });
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressbar1.Dispatcher.Invoke(() => { progressbar1.Value = e.ProgressPercentage; });
            var s = (DateTime.Now - GlobalPathsAndDatas.StartTime).TotalSeconds;
            var sd = HttpRequest.ReadableFilesize(e.BytesReceived / s);
            updatestatus2.Dispatcher.Invoke(() =>
            {
                updatestatus2.Text = "下载速度: " + sd + "/s" + ", 已下载: " +
                                     HttpRequest.ReadableFilesize(e.BytesReceived) + " / 总计: " +
                                     HttpRequest.ReadableFilesize(e.TotalBytesToReceive);
            });
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            CheckUpdate.IsEnabled = false;
            var VCE = new Task(VersionCheckEvent);
            VCE.Start();
        }

        private void Button_Click_Quest(object sender, RoutedEventArgs e)
        {
            var LPQL = new Task(LoadPickUPQuestList);
            ButtonQuest.IsEnabled = false;
            LPQL.Start();
        }

        private void LoadPickUPQuestList()
        {
            var path = Directory.GetCurrentDirectory();
            var gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
            var dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var QuestName = "";
            var questid = "";
            PickupQuestList.Dispatcher.Invoke(() => { PickupQuestList.Items.Clear(); });
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstQuest.json") &&
                File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstQuestPickup.json"))
            {
                foreach (var mstQuestPickup in GlobalPathsAndDatas.mstQuestPickupArray)
                {
                    var QuestPUEndTimeStamp = Convert.ToInt32(((JObject)mstQuestPickup)["endedAt"]);
                    var QuestPUStartTimeStamp = Convert.ToInt32(((JObject)mstQuestPickup)["startedAt"]);
                    var TimeMinus = (DateTime.Now.Ticks - dateTimeStart.Ticks) / 10000000;
                    if (TimeMinus > QuestPUEndTimeStamp) continue;
                    questid = ((JObject)mstQuestPickup)["questId"].ToString();
                    QuestName = GetQuestNameAndType(questid);
                    var QuestPUStartTime = new TimeSpan(long.Parse(QuestPUStartTimeStamp + "0000000"));
                    var QuestPUEndTime = new TimeSpan(long.Parse(QuestPUEndTimeStamp + "0000000"));
                    var StartStr = Convert.ToString(dateTimeStart + QuestPUStartTime);
                    var EndStr = Convert.ToString(dateTimeStart + QuestPUEndTime);
                    PickupQuestList.Dispatcher.Invoke(() =>
                    {
                        PickupQuestList.Items.Add(new QuestList(questid, QuestName, StartStr, EndStr));
                    });
                }

                ButtonQuest.Dispatcher.Invoke(() => { ButtonQuest.IsEnabled = true; });
                Dispatcher.Invoke(() => { Growl.Info("显示完毕."); });
            }
            else
            {
                Dispatcher.Invoke(() => { MessageBox.Warning("游戏数据损坏,请重新下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:"); });
                ButtonQuest.Dispatcher.Invoke(() => { ButtonQuest.IsEnabled = true; });
            }

            GC.Collect();
        }

        private string GetQuestNameAndType(string questid)
        {
            var QuestName = "";
            foreach (var mstQuest in GlobalPathsAndDatas.mstQuestArray)
            {
                if (((JObject)mstQuest)["id"].ToString() != questid) continue;
                var CharaID = ((JObject)mstQuest)["charaIconId"].ToString();
                try
                {
                    CharaID = CharaID.Substring(0, CharaID.Length - 3) + "00";
                }
                catch (Exception)
                {
                    //ignore
                }

                var TempName = ((JObject)mstQuest)["name"].ToString();
                if (TempName.Length > 14) TempName = TempName.Insert(14, "\r\n");
                QuestName = "\r\n" + TempName + "\r\n\r\nAP消耗: " + ((JObject)mstQuest)["actConsume"] + "   等级推荐: lv." +
                            ((JObject)mstQuest)["recommendLv"] + (CharaID != "0"
                                ? "\r\n从者: " + CharaID + " - " +
                                  GetSvtName(CharaID, 1)
                                : "") + "\r\n";
                if (((JObject)mstQuest)["giftId"].ToString() != "0")
                {
                    var giftid = ((JObject)mstQuest)["giftId"].ToString();
                    QuestName += "任务奖励: " + CheckGiftNames(giftid) + "\r\n";
                }
                else
                {
                    var itemid = ((JObject)mstQuest)["giftIconId"].ToString();
                    QuestName += "任务奖励: " + CheckItemName(itemid) + "\r\n";
                }

                break;
            }

            return QuestName;
        }

        private string CheckGiftNames(string giftid)
        {
            var giftids = "";
            var giftquantities = "";
            var giftNames = "";
            foreach (var mstGifttmp in GlobalPathsAndDatas.mstGiftArray)
            {
                if (((JObject)mstGifttmp)["id"].ToString() != giftid) continue;
                giftids += ((JObject)mstGifttmp)["objectId"] + ",";
                giftquantities += ((JObject)mstGifttmp)["num"] + ",";
            }

            try
            {
                giftids = giftids.Substring(0, giftids.Length - 1);
                giftquantities = giftquantities.Substring(0, giftquantities.Length - 1);
                var giftidArray = giftids.Split(',');
                var giftQuantityArray = giftquantities.Split(',');
                for (var ii = 0; ii < giftidArray.Length; ii++)
                    giftNames += CheckItemName(giftidArray[ii]) +
                                 (giftQuantityArray[ii] == "0" ? "" : " x " + giftQuantityArray[ii]) + ",";
                giftNames = giftNames.Substring(0, giftNames.Length - 1);
                return giftNames;
            }
            catch (Exception)
            {
                return "Error Loading GiftNames.";
            }
        }

        private void Button_Click_Class(object sender, RoutedEventArgs e)
        {
            var LCAR = new Task(LoadClassAndRelations);
            ButtonClass.IsEnabled = false;
            LCAR.Start();
        }

        private void LoadClassAndRelations()
        {
            var path = Directory.GetCurrentDirectory();
            var gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
            ClassList.Dispatcher.Invoke(() => { ClassList.Items.Clear(); });
            var outputStr = "";
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstClass.json") &&
                File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstClassRelation.json"))
            {
                foreach (var mstClasstmp in GlobalPathsAndDatas.mstClassArray)
                {
                    var ClassName = "";
                    var WeakClassA = "\r\n";
                    var ResistClassA = "\r\n";
                    var WeakClassD = "\r\n";
                    var ResistClassD = "\r\n";
                    ClassName = GetClassName(((JObject)mstClasstmp)["id"].ToString()) + "(" +
                                ((JObject)mstClasstmp)["id"] + ")";
                    var tmpid = ((JObject)mstClasstmp)["id"].ToString();
                    foreach (var mstClassRelationtmp in GlobalPathsAndDatas.mstClassRelationArray)
                    {
                        if (((JObject)mstClassRelationtmp)["atkClass"].ToString() != tmpid) continue;
                        var ATKRATE = Convert.ToInt64(((JObject)mstClassRelationtmp)["attackRate"].ToString());
                        if (ATKRATE > 1000)
                        {
                            var tmpweakid = ((JObject)mstClassRelationtmp)["defClass"].ToString();
                            WeakClassA +=
                                (GetClassName(tmpweakid) == "？"
                                    ? GetClassName(tmpweakid) + "(ID:" + tmpweakid + ")"
                                    : GetClassName(tmpweakid)) + " (" + (float)ATKRATE / 1000 + "x)\r\n";
                        }
                        else if (ATKRATE < 1000)
                        {
                            var tmpresistid = ((JObject)mstClassRelationtmp)["defClass"].ToString();
                            ResistClassA +=
                                (GetClassName(tmpresistid) == "？"
                                    ? GetClassName(tmpresistid) + "(ID:" + tmpresistid + ")"
                                    : GetClassName(tmpresistid)) + " (" + (float)ATKRATE / 1000 + "x)\r\n";
                        }
                    }

                    foreach (var mstClassRelationtmp in GlobalPathsAndDatas.mstClassRelationArray)
                    {
                        if (((JObject)mstClassRelationtmp)["defClass"].ToString() != tmpid) continue;
                        var ATKRATE = Convert.ToInt64(((JObject)mstClassRelationtmp)["attackRate"].ToString());
                        if (ATKRATE > 1000)
                        {
                            var tmpweakid = ((JObject)mstClassRelationtmp)["atkClass"].ToString();
                            WeakClassD +=
                                (GetClassName(tmpweakid) == "？"
                                    ? GetClassName(tmpweakid) + "(ID:" + tmpweakid + ")"
                                    : GetClassName(tmpweakid)) + " (" + (float)ATKRATE / 1000 + "x)\r\n";
                        }
                        else if (ATKRATE < 1000)
                        {
                            var tmpresistid = ((JObject)mstClassRelationtmp)["atkClass"].ToString();
                            ResistClassD +=
                                (GetClassName(tmpresistid) == "？"
                                    ? GetClassName(tmpresistid) + "(ID:" + tmpresistid + ")"
                                    : GetClassName(tmpresistid)) + " (" + (float)ATKRATE / 1000 + "x)\r\n";
                        }
                    }

                    WeakClassA = WeakClassA.Substring(0, WeakClassA.Length - 2) + "\r\n";
                    ResistClassA = ResistClassA.Substring(0, ResistClassA.Length - 2) + "\r\n";
                    WeakClassD = WeakClassD.Substring(0, WeakClassD.Length - 2) + "\r\n";
                    ResistClassD = ResistClassD.Substring(0, ResistClassD.Length - 2) + "\r\n";
                    ClassList.Dispatcher.Invoke(() =>
                    {
                        ClassList.Items.Add(new ClassRelationList(ClassName, WeakClassA, WeakClassD, ResistClassA,
                            ResistClassD));
                    });
                    outputStr += "职介名:" + ClassName + "\r\n" + " - 攻击侧:\r\n" + "Weak:\r\n" +
                                 WeakClassA.Substring(2, WeakClassA.Length - 2).Replace("\r\n", ",") +
                                 "\r\nResist:\r\n" + ResistClassA.Substring(2, ResistClassA.Length - 2)
                                     .Replace("\r\n", ",")
                                 + "\r\n\r\n - 防御侧:\r\nWeak:\r\n" +
                                 WeakClassD.Substring(2, WeakClassD.Length - 2).Replace("\r\n", ",") +
                                 "\r\nResist:\r\n" +
                                 ResistClassD.Substring(2, ResistClassD.Length - 2).Replace("\r\n", ",") +
                                 "\r\n\r\n---------------------------------\r\n\r\n";
                }

                ButtonClass.Dispatcher.Invoke(() => { ButtonClass.IsEnabled = true; });
                Dispatcher.Invoke(() => { Growl.Info("显示完毕."); });
                File.WriteAllText(path + "/ClassRelation.txt", outputStr);
                Process.Start(path + "/ClassRelation.txt");
            }
            else
            {
                Dispatcher.Invoke(() => { MessageBox.Warning("游戏数据损坏,请重新下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:"); });
                ButtonClass.Dispatcher.Invoke(() => { ButtonClass.IsEnabled = true; });
            }
        }

        private static string GetClassName(string id)
        {
            var ClassName = "";
            switch (Convert.ToInt32(id))
            {
                case 1:
                    ClassName = "Saber";
                    break;
                case 2:
                    ClassName = "Archer";
                    break;
                case 3:
                    ClassName = "Lancer";
                    break;
                case 4:
                    ClassName = "Rider";
                    break;
                case 5:
                    ClassName = "Caster";
                    break;
                case 6:
                    ClassName = "Assassin";
                    break;
                case 7:
                    ClassName = "Berserker";
                    break;
                case 8:
                    ClassName = "Shielder";
                    break;
                case 9:
                    ClassName = "Ruler";
                    break;
                case 10:
                    ClassName = "AlterEgo";
                    break;
                case 11:
                    ClassName = "Avenger";
                    break;
                case 17:
                    ClassName = "Grand Caster";
                    break;
                case 20:
                    ClassName = "Beast II";
                    break;
                case 22:
                    ClassName = "Beast I";
                    break;
                case 23:
                    ClassName = "MoonCancer";
                    break;
                case 24:
                    ClassName = "Beast Ⅲ／R";
                    break;
                case 25:
                    ClassName = "Foreigner";
                    break;
                case 26:
                    ClassName = "Beast Ⅲ／L";
                    break;
                case 27:
                    ClassName = "Beast ?";
                    break;
                case 28:
                    ClassName = "Pretender";
                    break;
                case 29:
                    ClassName = "Beast IV";
                    break;
                case 33:
                    ClassName = "Beast";
                    break;
                case 34:
                    ClassName = "Beast VI";
                    break;
                case 35:
                    ClassName = "Beast VI";
                    break;
                default:
                    foreach (var mstClasstmp in GlobalPathsAndDatas.mstClassArray)
                    {
                        if (((JObject)mstClasstmp)["id"].ToString() != id) continue;
                        ClassName = ((JObject)mstClasstmp)["name"].ToString();
                        break;
                    }

                    break;
            }

            return ClassName;
        }

        private void LoadEventList()
        {
            var path = Directory.GetCurrentDirectory();
            var gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
            var dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var EventName = "";
            var Eventid = "";
            PickupEventList.Dispatcher.Invoke(() => { PickupEventList.Items.Clear(); });
            PickupEndedEventList.Dispatcher.Invoke(() => { PickupEndedEventList.Items.Clear(); });
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstEvent.json"))
            {
                foreach (var mstEventtmp in GlobalPathsAndDatas.mstEventArray)
                {
                    var EventEndTimeStamp = Convert.ToInt32(((JObject)mstEventtmp)["endedAt"]);
                    var EventStartTimeStamp = Convert.ToInt32(((JObject)mstEventtmp)["startedAt"]);
                    var TimeMinus = (DateTime.Now.Ticks - dateTimeStart.Ticks) / 10000000;
                    EventName = ((JObject)mstEventtmp)["name"].ToString();
                    if (EventName.Length > 40) EventName = EventName.Insert(40, "\r\n");
                    Eventid = ((JObject)mstEventtmp)["id"].ToString();
                    var EventEndTime = new TimeSpan(long.Parse(EventEndTimeStamp + "0000000"));
                    var EventStartTime = new TimeSpan(long.Parse(EventStartTimeStamp + "0000000"));
                    var EndStr = Convert.ToString(dateTimeStart + EventEndTime);
                    var StartStr = Convert.ToString(dateTimeStart + EventStartTime);
                    if (EventEndTimeStamp == 1893423600)
                    {
                        PickupEndedEventList.Dispatcher.Invoke(() =>
                        {
                            PickupEndedEventList.Items.Add(
                                new EventList(Eventid, EventName + "\r\n - 永久活动", EndStr));
                        });
                        continue;
                    }

                    if (TimeMinus > EventEndTimeStamp)
                        PickupEndedEventList.Dispatcher.Invoke(() =>
                        {
                            PickupEndedEventList.Items.Add(new EventList(Eventid, EventName,
                                StartStr + " ~ " + EndStr));
                        });
                    else
                        PickupEventList.Dispatcher.Invoke(() =>
                        {
                            PickupEventList.Items.Add(new EventList(Eventid, EventName, StartStr + " ~ " + EndStr));
                        });
                }

                ButtonEvent.Dispatcher.Invoke(() => { ButtonEvent.IsEnabled = true; });
                Dispatcher.Invoke(() => { Growl.Info("显示完毕."); });
            }
            else
            {
                Dispatcher.Invoke(() => { MessageBox.Warning("游戏数据损坏,请重新下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:"); });
                ButtonEvent.Dispatcher.Invoke(() => { ButtonEvent.IsEnabled = true; });
            }

            GC.Collect();
        }

        private void Button_Click_Gacha(object sender, RoutedEventArgs e)
        {
            ButtonGacha.IsEnabled = false;
            var LG = new Task(LoadGacha);
            LG.Start();
        }

        private void LoadGacha()
        {
            var path = Directory.GetCurrentDirectory();
            var gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
            var dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var EventName = "";
            var Eventid = "";
            PickupGachaList.Dispatcher.Invoke(() => { PickupEventList.Items.Clear(); });
            PickupEndedGachaList.Dispatcher.Invoke(() => { PickupEndedEventList.Items.Clear(); });
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstGacha.json"))
            {
                foreach (var mstGachatmp in GlobalPathsAndDatas.mstGachaArray)
                {
                    var EventEndTimeStamp = Convert.ToInt32(((JObject)mstGachatmp)["closedAt"]);
                    var EventStartTimeStamp = Convert.ToInt32(((JObject)mstGachatmp)["openedAt"]);
                    var TimeMinus = (DateTime.Now.Ticks - dateTimeStart.Ticks) / 10000000;
                    EventName = ((JObject)mstGachatmp)["name"].ToString();
                    if (EventName.Length > 40) EventName = EventName.Insert(40, "\r\n");
                    Eventid = ((JObject)mstGachatmp)["id"].ToString();
                    var EventEndTime = new TimeSpan(long.Parse(EventEndTimeStamp + "0000000"));
                    var EventStartTime = new TimeSpan(long.Parse(EventStartTimeStamp + "0000000"));
                    var EndStr = Convert.ToString(dateTimeStart + EventEndTime);
                    var StartStr = Convert.ToString(dateTimeStart + EventStartTime);
                    if (EventEndTimeStamp == 1911653999)
                    {
                        PickupEndedEventList.Dispatcher.Invoke(() =>
                        {
                            PickupEndedGachaList.Items.Add(
                                new EventList(Eventid, EventName + "\r\n - 永久卡池", EndStr));
                        });
                        continue;
                    }

                    if (TimeMinus > EventEndTimeStamp)
                        PickupEndedEventList.Dispatcher.Invoke(() =>
                        {
                            PickupEndedGachaList.Items.Add(new EventList(Eventid, EventName,
                                StartStr + " ~ " + EndStr));
                        });
                    else
                        PickupEventList.Dispatcher.Invoke(() =>
                        {
                            PickupGachaList.Items.Add(new EventList(Eventid, EventName, StartStr + " ~ " + EndStr));
                        });
                }

                ButtonGacha.Dispatcher.Invoke(() => { ButtonGacha.IsEnabled = true; });
                Dispatcher.Invoke(() => { Growl.Info("显示完毕."); });
            }
            else
            {
                Dispatcher.Invoke(() => { MessageBox.Warning("游戏数据损坏,请重新下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:"); });
                ButtonGacha.Dispatcher.Invoke(() => { ButtonGacha.IsEnabled = true; });
            }

            GC.Collect();
        }

        private void ShowHPAtkBalance(string svtID, string rarity, string endurance, string basichp, string ClassID)
        {
            SkillLvs.HpBalanceForExcel = "";
            Dispatcher.Invoke(() =>
            {
                var endurancebase = new decimal[100];
                endurancebase[11] = 1.02M;
                endurancebase[12] = 1.025M;
                endurancebase[13] = 1.03M;
                endurancebase[14] = 1.015M;
                endurancebase[15] = 1.035M;
                endurancebase[16] = 0.0M;
                endurancebase[17] = 1.0M;
                endurancebase[18] = 0.99M;
                endurancebase[19] = 0.98M;
                endurancebase[20] = 0.97M;
                endurancebase[21] = 1.0M;
                endurancebase[22] = 1.005M;
                endurancebase[23] = 1.01M;
                endurancebase[24] = 0.995M;
                endurancebase[25] = 1.015M;
                endurancebase[26] = 0.0M;
                endurancebase[27] = 1.02M;
                endurancebase[28] = 0.99M;
                endurancebase[29] = 0.98M;
                endurancebase[30] = 0.97M;
                endurancebase[31] = 0.99M;
                endurancebase[32] = 0.9925M;
                endurancebase[33] = 0.995M;
                endurancebase[34] = 0.985M;
                endurancebase[35] = 0.9975M;
                endurancebase[36] = 0.0M;
                endurancebase[37] = 1.02M;
                endurancebase[38] = 1.0M;
                endurancebase[39] = 0.98M;
                endurancebase[40] = 0.97M;
                endurancebase[41] = 0.98M;
                endurancebase[42] = 0.9825M;
                endurancebase[43] = 0.985M;
                endurancebase[44] = 0.975M;
                endurancebase[45] = 0.9875M;
                endurancebase[46] = 0.0M;
                endurancebase[47] = 1.02M;
                endurancebase[48] = 1.0M;
                endurancebase[49] = 0.99M;
                endurancebase[50] = 0.97M;
                endurancebase[51] = 0.97M;
                endurancebase[52] = 0.9725M;
                endurancebase[53] = 0.975M;
                endurancebase[54] = 0.965M;
                endurancebase[55] = 0.9775M;
                endurancebase[56] = 0.0M;
                endurancebase[57] = 1.02M;
                endurancebase[58] = 1.0M;
                endurancebase[59] = 0.99M;
                endurancebase[60] = 0.98M;
                endurancebase[61] = 1.04M;
                endurancebase[0] = 0.0M;
                endurancebase[99] = 0.0M;
                endurancebase[98] = 0.0M;
                endurancebase[97] = 0.0M;
                var HPBasicWithRarity = new decimal[6];
                HPBasicWithRarity[0] = 1600M;
                HPBasicWithRarity[1] = 1500M;
                HPBasicWithRarity[2] = 1600M;
                HPBasicWithRarity[3] = 1800M;
                HPBasicWithRarity[4] = 2000M;
                HPBasicWithRarity[5] = 2200M;
                var ClassBasicBase = new decimal[100];
                ClassBasicBase[1] = 1.01M;
                ClassBasicBase[2] = 0.98M;
                ClassBasicBase[3] = 1.02M;
                ClassBasicBase[4] = 0.96M;
                ClassBasicBase[5] = 0.98M;
                ClassBasicBase[6] = 0.95M;
                ClassBasicBase[7] = 0.90M;
                ClassBasicBase[8] = 1.01M;
                ClassBasicBase[9] = 1.00M;
                ClassBasicBase[10] = 0.95M;
                ClassBasicBase[11] = 0.88M;
                ClassBasicBase[17] = 0.98M;
                ClassBasicBase[23] = 1.05M;
                ClassBasicBase[25] = 1.00M;
                ClassBasicBase[28] = 0.95M;
                ClassBasicBase[33] = 0.97M;
                ClassBasicBase[38] = 1.03M;
                ClassBasicBase[40] = 0.99M; //待定
                var ShowString = new string[8];
                ShowString[1] = "( 攻防倾向: 全HP )";
                ShowString[2] = "( 攻防倾向: 偏HP )";
                ShowString[3] = "( 攻防倾向: 均衡 )";
                ShowString[4] = "( 攻防倾向: 偏ATK )";
                ShowString[5] = "( 攻防倾向: 全ATK )";
                ShowString[6] = "( 攻防倾向: 特殊 )";
                ShowString[7] = "( 攻防倾向: - )";
                decimal resultHPBaseCheck;
                if (ClassID != "1" && ClassID != "2" && ClassID != "3" && ClassID != "4" && ClassID != "5" &&
                    ClassID != "6" && ClassID != "7" && ClassID != "8" && ClassID != "9" && ClassID != "10" &&
                    ClassID != "11" && ClassID != "17" && ClassID != "23" && ClassID != "25" && ClassID != "28" &&
                    ClassID != "33" && ClassID != "38" && ClassID != "40")
                {
                    hpatkbalance.Text = ShowString[7];
                    SkillLvs.HpBalanceForExcel = ShowString[7].Replace("(", "").Replace(")", "");
                    return;
                }

                if (svtID == "100300")
                {
                    hpatkbalance.Text = ShowString[6];
                    SkillLvs.HpBalanceForExcel = ShowString[6].Replace("(", "").Replace(")", "");
                }
                else
                {
                    var inserthp = Convert.ToDecimal(basichp);
                    resultHPBaseCheck = inserthp / (HPBasicWithRarity[Convert.ToInt64(rarity)] *
                                                    endurancebase[Convert.ToInt64(endurance)] *
                                                    ClassBasicBase[Convert.ToInt64(ClassID)]);
                    if (Math.Abs(resultHPBaseCheck - 1.10M) <= 0.005M)
                    {
                        hpatkbalance.Text = ShowString[1];
                        SkillLvs.HpBalanceForExcel = ShowString[1].Replace("(", "").Replace(")", "") + "\r\nHP补正: " +
                                                     Math.Round(resultHPBaseCheck, 3);
                    }
                    else if (Math.Abs(resultHPBaseCheck - 1.05M) <= 0.005M)
                    {
                        hpatkbalance.Text = ShowString[2];
                        SkillLvs.HpBalanceForExcel = ShowString[2].Replace("(", "").Replace(")", "") + "\r\nHP补正: " +
                                                     Math.Round(resultHPBaseCheck, 3);
                    }
                    else if (Math.Abs(resultHPBaseCheck - 1.00M) <= 0.005M)
                    {
                        hpatkbalance.Text = ShowString[3];
                        SkillLvs.HpBalanceForExcel = ShowString[3].Replace("(", "").Replace(")", "") + "\r\nHP补正: " +
                                                     Math.Round(resultHPBaseCheck, 3);
                    }
                    else if (Math.Abs(resultHPBaseCheck - 0.95M) <= 0.005M)
                    {
                        hpatkbalance.Text = ShowString[4];
                        SkillLvs.HpBalanceForExcel = ShowString[4].Replace("(", "").Replace(")", "") + "\r\nHP补正: " +
                                                     Math.Round(resultHPBaseCheck, 3);
                    }
                    else if (Math.Abs(resultHPBaseCheck - 0.90M) <= 0.005M)
                    {
                        hpatkbalance.Text = ShowString[5];
                        SkillLvs.HpBalanceForExcel = ShowString[5].Replace("(", "").Replace(")", "") + "\r\nHP补正: " +
                                                     Math.Round(resultHPBaseCheck, 3);
                    }
                    else
                    {
                        hpatkbalance.Text = ShowString[7];
                        SkillLvs.HpBalanceForExcel = ShowString[7].Replace("(", "").Replace(")", "") +
                                                     (resultHPBaseCheck >= 0.90M && resultHPBaseCheck <= 1.10M
                                                         ? "\r\nHP补正: " +
                                                           Math.Round(resultHPBaseCheck, 3)
                                                         : "");
                    }
                }
            });
        }

        private void AddChart(int[] Array)
        {
            GlobalPathsAndDatas.CurveBaseData = null;
            GlobalPathsAndDatas.ymax = 0.0M;
            GlobalPathsAndDatas.lv100atk = 0.0M;
            GlobalPathsAndDatas.lv100hp = 0.0M;
            GlobalPathsAndDatas.lv120atk = 0.0M;
            GlobalPathsAndDatas.lv120hp = 0.0M;
            GlobalPathsAndDatas.lv150atk = 0.0M;
            GlobalPathsAndDatas.lv150hp = 0.0M;
            Dispatcher.Invoke(() =>
            {
                if (Array == null) throw new ArgumentNullException(nameof(Array));
                var xmax = Convert.ToDecimal(GlobalPathsAndDatas.LvExpCurveLvCount - 1);
                var ymax = 0.0M;
                var AdjustHPCurve = new decimal[GlobalPathsAndDatas.LvExpCurveLvCount];
                var AdjustATKCurve = new decimal[GlobalPathsAndDatas.LvExpCurveLvCount];
                for (var lv = 0; lv < GlobalPathsAndDatas.LvExpCurveLvCount; lv++)
                {
                    AdjustHPCurve[lv] = GlobalPathsAndDatas.basichp +
                                        Array[lv] * (GlobalPathsAndDatas.maxhp - GlobalPathsAndDatas.basichp) /
                                        1000;
                    AdjustATKCurve[lv] = GlobalPathsAndDatas.basicatk +
                                         Array[lv] * (GlobalPathsAndDatas.maxatk - GlobalPathsAndDatas.basicatk) /
                                         1000;
                    if (lv == 0) continue;
                    if (lv == 100)
                    {
                        GlobalPathsAndDatas.lv100atk = AdjustATKCurve[lv];
                        GlobalPathsAndDatas.lv100hp = AdjustHPCurve[lv];
                    }

                    if (lv == 120)
                    {
                        GlobalPathsAndDatas.lv120atk = AdjustATKCurve[lv];
                        GlobalPathsAndDatas.lv120hp = AdjustHPCurve[lv];
                    }

                    if (lv == 150 && GlobalPathsAndDatas.LvExpCurveLvCount == 151)
                    {
                        GlobalPathsAndDatas.lv150atk = AdjustATKCurve[lv];
                        GlobalPathsAndDatas.lv150hp = AdjustHPCurve[lv];
                    }

                    HpAtkListView.Items.Add(new HpAtkList(lv.ToString(), Convert.ToInt32(AdjustHPCurve[lv]).ToString(),
                        AdjustATKCurve[lv].ToString()));
                }

                ymax = Math.Max(AdjustATKCurve[GlobalPathsAndDatas.LvExpCurveLvCount - 1],
                    AdjustHPCurve[GlobalPathsAndDatas.LvExpCurveLvCount - 1]);
                GlobalPathsAndDatas.ymax = ymax;
                XZhou.MaxValue = GlobalPathsAndDatas.LvExpCurveLvCount - 1;
                LineHP = new int[GlobalPathsAndDatas.LvExpCurveLvCount];
                LineATK = new int[GlobalPathsAndDatas.LvExpCurveLvCount];
                for (var q = 0; q <= GlobalPathsAndDatas.LvExpCurveLvCount - 1; q++)
                {
                    LineHP[q] = Convert.ToInt32(AdjustHPCurve[q]);
                    LineATK[q] = Convert.ToInt32(AdjustATKCurve[q]);
                }

                HPCurveX.Values = LineHP.AsChartValues();
                ATKCurveX.Values = LineATK.AsChartValues();
                DataContext = this;
                HPAtkXCurve.Update();
                if (GlobalPathsAndDatas.LvExpCurveLvCount - 1 != 120)
                {
                    LabelX = new string[GlobalPathsAndDatas.LvExpCurveLvCount];
                    for (var j = 0; j <= GlobalPathsAndDatas.LvExpCurveLvCount - 1; j++) LabelX[j] = j.ToString();
                    XZhou_Step.Step = 10;
                }
            });
            GlobalPathsAndDatas.CurveBaseData = Array;
        }

        private static int[] GetSvtCurveData(object TypeID)
        {
            GlobalPathsAndDatas.LvExpCurveLvCount = 0;
            var lvcount = GlobalPathsAndDatas.mstSvtExpArray.Count(mstSvtExptmp =>
                ((JObject)mstSvtExptmp)["type"].ToString() == TypeID.ToString());
            var TempData = new int[lvcount + 1];
            foreach (var mstSvtExptmp in GlobalPathsAndDatas.mstSvtExpArray)
            {
                if (((JObject)mstSvtExptmp)["type"].ToString() != TypeID.ToString()) continue;
                TempData[Convert.ToInt32(((JObject)mstSvtExptmp)["lv"])] =
                    Convert.ToInt32(((JObject)mstSvtExptmp)["curve"].ToString());
            }

            GlobalPathsAndDatas.LvExpCurveLvCount = lvcount;
            return TempData;
        }

        private void DrawServantStrengthenCurveLine(object TypeID)
        {
            try
            {
                var BaseCurveData = GetSvtCurveData(TypeID);
                AddChart(BaseCurveData);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public void CheckSvtIndividuality(object Input)
        {
            var IndividualityStringArray = Input.ToString().Split(',');
            var CleanIndi = "";
            var Outputs = "";
            var trigger1 = true;
            foreach (var Cases in IndividualityStringArray)
            {
                if (Cases.Length >= 6) continue;
                if (Cases == "5010" || Cases == "5000") continue;
                foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                    if (((JObject)svtInditmp)["id"].ToString() == Cases)
                    {
                        var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                        if (Cases.Substring(0, 1) == "3" && Cases.Length == 3)
                            CleanIndi += svtIndiObj["individualityName"] + "·";
                        if (Cases.Length != 3 && Cases.Length != 1)
                            Outputs += svtIndiObj["individualityName"] + ",";
                        trigger1 = false;
                        break;
                    }

                if (trigger1) Outputs += "未知特性(" + Cases + "),";
            }

            var SvtIndividualityAdd1 = SvtIndiSpec1(JB.svtid);
            var SvtIndividualityAdd2 = SvtIndiSpec2(JB.svtid, IndividualityStringArray);
            if (SvtIndividualityAdd1 != "") Outputs += SvtIndividualityAdd1;
            if (SvtIndividualityAdd2 != "") Outputs += SvtIndividualityAdd2;
            if (!Outputs.Contains("被EA特攻")) Outputs += "不被EA特攻,";
            try
            {
                CleanIndi = CleanIndi.Substring(0, CleanIndi.Length - 1);
                Outputs = Outputs.Substring(0, Outputs.Length - 1);
                if (Outputs[Outputs.Length - 1] == ',') Outputs = Outputs.Substring(0, Outputs.Length - 1);
            }
            catch (Exception)
            {
                //ignore
            }

            svtIndividuality.Dispatcher.Invoke(() => { svtIndividuality.Text = Outputs; });
            IndividualalityClean.Dispatcher.Invoke(() => { IndividualalityClean.Text = CleanIndi; });
        }

        private string SvtIndiSpec1(string SvtID)
        {
            var resultstring = new string[4];
            foreach (var mstSvtIndividualitytmp in GlobalPathsAndDatas.mstSvtIndividualityArray)
            {
                if (((JObject)mstSvtIndividualitytmp)["svtId"].ToString() != SvtID) continue;
                if (((JObject)mstSvtIndividualitytmp)["eventId"].ToString() != "0") continue;
                switch (((JObject)mstSvtIndividualitytmp)["limitCount"].ToString())
                {
                    case "-1":
                        resultstring[0] = ((JObject)mstSvtIndividualitytmp)["individuality"].ToString()
                            .Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "0":
                        resultstring[1] = ((JObject)mstSvtIndividualitytmp)["individuality"].ToString()
                            .Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "1":
                        resultstring[2] = ((JObject)mstSvtIndividualitytmp)["individuality"].ToString()
                            .Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "3":
                        resultstring[3] = ((JObject)mstSvtIndividualitytmp)["individuality"].ToString()
                            .Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                }
            }

            var tmpIndiList = new List<string>();

            for (var i = 1; i <= 3; i++)
                if (!string.IsNullOrEmpty(resultstring[i]))
                {
                    var tmpIndiSplitItems = resultstring[i].Split(',');
                    foreach (var k in tmpIndiSplitItems) tmpIndiList.Add(k);
                }

            if (resultstring[0] == "" && tmpIndiList.Count == 0) return "";
            string[] SpIndiList1 = null;
            try
            {
                SpIndiList1 = resultstring[0].Split(',');
            }
            catch (Exception)
            {
                return "";
            }

            var CheckedName = "";
            foreach (var Cases in SpIndiList1)
            {
                if (Cases == "5010" || Cases == "5000" || Cases == "400" || Cases == "401" || Cases == "402" ||
                    Cases == "403" || Cases == "404" || Cases == "405") continue;
                foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                    if (((JObject)svtInditmp)["id"].ToString() == Cases)
                    {
                        var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                        CheckedName += svtIndiObj["individualityName"] + ",";
                        break;
                    }
            }

            var hashArray = new HashSet<string>(tmpIndiList).ToArray();
            var selectionSumArray = new int[hashArray.Length];
            for (var zeroValue = 0; zeroValue < selectionSumArray.Length; zeroValue++) selectionSumArray[zeroValue] = 0;

            var additionVal = new int[4];
            additionVal[1] = 1;
            additionVal[2] = 2;
            additionVal[3] = 4;
            foreach (var items in hashArray)
                for (var i = 1; i <= 3; i++)
                    if (!string.IsNullOrEmpty(resultstring[i]))
                        foreach (var key in resultstring[i].Split(','))
                            if (items == key)
                                selectionSumArray[Array.IndexOf(hashArray, items)] += additionVal[i];

            for (var selectionIndex = 1; selectionIndex <= 7; selectionIndex++)
            {
                var dispList = new List<string>();
                var tmpOutputStr = "";

                for (var ii = 0; ii < hashArray.Length; ii++)
                    if (selectionSumArray[ii] == selectionIndex)
                        dispList.Add(hashArray[ii]);
                if (dispList.Count == 0) break;
                tmpOutputStr += "[";
                foreach (var Cases in dispList.ToArray())
                {
                    if (Cases == "5010" || Cases == "5000") continue;
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                        if (((JObject)svtInditmp)["id"].ToString() == Cases)
                        {
                            var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                            tmpOutputStr += svtIndiObj["individualityName"] + "、";
                            break;
                        }
                }

                try
                {
                    tmpOutputStr = tmpOutputStr.Substring(0, tmpOutputStr.Length - 1) + "]";
                }
                catch (Exception)
                {
                    //ignore
                }

                switch (selectionIndex)
                {
                    case 1:
                        CheckedName += tmpOutputStr + "(再临阶段Ⅰ),";
                        break;
                    case 2:
                        CheckedName += tmpOutputStr + "(再临阶段Ⅱ),";
                        break;
                    case 3:
                        CheckedName += tmpOutputStr + "(再临阶段Ⅰ,Ⅱ),";
                        break;
                    case 4:
                        CheckedName += tmpOutputStr + "(再临阶段Ⅲ),";
                        break;
                    case 5:
                        CheckedName += tmpOutputStr + "(再临阶段Ⅰ,Ⅲ),";
                        break;
                    case 6:
                        CheckedName += tmpOutputStr + "(再临阶段Ⅱ,Ⅲ),";
                        break;
                    case 7:
                        CheckedName += tmpOutputStr + ",";
                        break;
                }
            }

            return CheckedName;
        }

        private string SvtIndiSpec2(string SvtID, string[] originList)
        {
            var Limitindi0 = "";
            var Limitindi1 = "";
            var Limitindi2 = "";
            var Limitindi3 = "";
            var Limitindi4 = "";
            var LimitindiOthers = "";
            foreach (var mstSvtLimitAddtmp in GlobalPathsAndDatas.mstSvtLimitAddArray)
            {
                if (((JObject)mstSvtLimitAddtmp)["svtId"].ToString() != SvtID) continue;
                switch (((JObject)mstSvtLimitAddtmp)["limitCount"].ToString())
                {
                    case "0":
                        Limitindi0 = ((JObject)mstSvtLimitAddtmp)["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "1":
                        Limitindi1 = ((JObject)mstSvtLimitAddtmp)["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "2":
                        Limitindi2 = ((JObject)mstSvtLimitAddtmp)["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "3":
                        Limitindi3 = ((JObject)mstSvtLimitAddtmp)["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    case "4":
                        Limitindi4 = ((JObject)mstSvtLimitAddtmp)["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        break;
                    default:
                        var tmp = ((JObject)mstSvtLimitAddtmp)["individuality"].ToString().Replace("\n", "")
                            .Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        if (tmp == "") break;
                        LimitindiOthers += tmp + ",";
                        break;
                }
            }

            var OutputSpecialOther = "";
            if (LimitindiOthers != "")
            {
                try
                {
                    LimitindiOthers = LimitindiOthers.Substring(0, LimitindiOthers.Length - 1);
                }
                catch (Exception)
                {
                    //ignore
                }

                var LimitindiOtherArray = LimitindiOthers.Split(',');
                LimitindiOtherArray = new HashSet<string>(LimitindiOtherArray).ToArray();
                var Othertmp = "[";
                foreach (var Cases in LimitindiOtherArray)
                {
                    if (Cases == "5010" || Cases == "5000") continue;
                    if (originList.Contains(Cases)) continue;
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                        if (((JObject)svtInditmp)["id"].ToString() == Cases)
                        {
                            var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                            Othertmp += svtIndiObj["individualityName"] + "、";
                            break;
                        }
                }

                try
                {
                    Othertmp = Othertmp.Substring(0, Othertmp.Length - 1) + "]";
                }
                catch (Exception)
                {
                    //ignore
                }

                OutputSpecialOther = Othertmp + "(灵衣或特殊条件),";
            }

            var LimitindiArray = new string[5];
            LimitindiArray[0] = Limitindi0;
            LimitindiArray[1] = Limitindi1;
            LimitindiArray[2] = Limitindi2;
            LimitindiArray[3] = Limitindi3;
            LimitindiArray[4] = Limitindi4;
            for (var i = 0; i < 5; i++)
            {
                if (LimitindiArray[i] == "") continue;
                var SpIndiList = LimitindiArray[i].Split(',');
                var CheckedName = "[";
                foreach (var Cases in SpIndiList)
                {
                    if (Cases == "5010" || Cases == "5000") continue;
                    if (originList.Contains(Cases)) continue;
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                        if (((JObject)svtInditmp)["id"].ToString() == Cases)
                        {
                            var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                            CheckedName += svtIndiObj["individualityName"] + "、";
                            break;
                        }
                }

                try
                {
                    CheckedName = CheckedName.Substring(0, CheckedName.Length - 1) + "]";
                }
                catch (Exception)
                {
                    //ignore
                }

                LimitindiArray[i] = CheckedName;
            }

            var UniqueIndiStringList = new HashSet<string>(LimitindiArray);
            var UniqueIndiStringArray = UniqueIndiStringList.ToArray();
            var ListDisplayBefore = new List<string>();
            foreach (var t in UniqueIndiStringArray)
            {
                var tmpLimitStatus = "";
                for (var i = 0; i < 5; i++)
                    if (LimitindiArray[i] == t)
                        tmpLimitStatus += i + ",";
                try
                {
                    tmpLimitStatus = tmpLimitStatus.Substring(0, tmpLimitStatus.Length - 1);
                }
                catch (Exception)
                {
                    //ignore
                }

                ListDisplayBefore.Add(tmpLimitStatus);
            }

            var ListDisplayAfter = ListDisplayBefore.ToArray();
            var OutputString = "";
            for (var j = 0; j < ListDisplayAfter.Length; j++)
            {
                if (UniqueIndiStringArray[j] == "" || UniqueIndiStringArray[j] == "]") continue;
                OutputString += UniqueIndiStringArray[j] + "(再临阶段" +
                                ListDisplayAfter[j].Replace("0", "Ⅰ").Replace("1,2", "Ⅱ").Replace("3,4", "Ⅲ") + "),";
            }

            OutputString += OutputSpecialOther;
            return OutputString;
        }

        private void EasternEggSvt()
        {
            //Cleared
        }

        private void Button_Click_Event(object sender, RoutedEventArgs e)
        {
            ButtonEvent.IsEnabled = false;
            var LEL = new Task(LoadEventList);
            LEL.Start();
        }

        private string GetSvtName(string svtID, int Option)
        {
            foreach (var svtIDtmp in GlobalPathsAndDatas.mstSvtArray)
            {
                if (((JObject)svtIDtmp)["id"].ToString() != svtID) continue;
                var mstSvtobjtmp = JObject.Parse(svtIDtmp.ToString());
                var svtName = Option == 1 ? mstSvtobjtmp["battleName"].ToString() : mstSvtobjtmp["name"].ToString();
                return svtName;
            }

            return "???";
        }

        private void Button_Click_SvtFilter(object sender, RoutedEventArgs e)
        {
            ButtonSvtFilter.IsEnabled = false;
            var LSF = new Task(LoadSvtFilter);
            LSF.Start();
        }

        private void LoadSvtFilter()
        {
            Dispatcher.Invoke(() => { SvtFilterList.Items.Clear(); });
            var svtIDlist = "";
            var FilterID = "";
            foreach (var svtFiltertmp in GlobalPathsAndDatas.mstSvtFilterArray)
            {
                if (((JObject)svtFiltertmp)["name"].ToString() != "次回イベント対象") continue;
                var svtFilterobjtmp = JObject.Parse(svtFiltertmp.ToString());
                FilterID = svtFilterobjtmp["id"].ToString();
                svtIDlist = svtFilterobjtmp["svtIds"].ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "")
                    .Replace(" ", "").Replace("[", "").Replace("]", "");
            }

            if (svtIDlist == "")
            {
                Dispatcher.Invoke(() => { ButtonSvtFilter.IsEnabled = true; });
                return;
            }

            var SvtIDArray = svtIDlist.Split(',');
            var SvtOutputStr = SvtIDArray.Aggregate("\r\n",
                (current, svtIDtmp) => current + GetSvtName(svtIDtmp, 0) + "(" + svtIDtmp + ")\r\n");
            Dispatcher.Invoke(() => { SvtFilterList.Items.Add(new FilterList(FilterID, "次回イベント対象", SvtOutputStr)); });
            Dispatcher.Invoke(() =>
            {
                GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                    Application.Current.MainWindow,
                    "是否需要导出该加成列表?",
                    "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            });
            if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
            {
                File.WriteAllText(GlobalPathsAndDatas.path + "/次回イベント対象" + ".txt", SvtOutputStr);
                Dispatcher.Invoke(() => { MessageBox.Success("导出完成.\n\r文件名为: " + "次回イベント対象.txt", "完成"); });
                Process.Start(GlobalPathsAndDatas.path + "/次回イベント対象" + ".txt");
            }

            Dispatcher.Invoke(() => { ButtonSvtFilter.IsEnabled = true; });
        }

        private void JumpToClassPassive(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SkillInfoTI.IsSelected = true;
                ClassPassiveInfo.IsSelected = true;
            });
        }

        private void EnterSvtIDExtraWindow(object sender, RoutedEventArgs e)
        {
            var SvtExtraWindow = new SvtIDExtraInputBox();
            SvtExtraWindow.ShowDialog();
            var ReturnStr = SvtExtraWindow.svtidreturnS;
            if (string.IsNullOrEmpty(ReturnStr)) return;
            Dispatcher.Invoke(() => { textbox1.Text = ReturnStr; });
            Button_Click(sender, e);
        }

        private void ResetZoomSettings(object sender, RoutedEventArgs e)
        {
            XZhou.MinValue = 1.0;
            XZhou.MaxValue = GlobalPathsAndDatas.LvExpCurveLvCount - 1;
            YZhou.MinValue = 0.0;
            YZhou.MaxValue = double.NaN;
        }

        private void CallHime(object sender, RoutedEventArgs e)
        {
            var path = Directory.GetCurrentDirectory();
            if (File.Exists(path + @"\Osakabehime.exe"))
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = path + @"\Osakabehime.exe",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
            }
            else
            {
                Dispatcher.Invoke(() => { MessageBox.Error("未找到相关模块,请检查.", "错误"); });
            }
        }

        private void svtLimitSelChange(object sender, RoutedEventArgs e)
        {

        }
        private void svtLimitSelTDChange(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_Press_Enter(object sender, KeyEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Key == Key.Enter && Button1.IsEnabled) Button_Click(sender, e);
            });
        }


        private struct SkillListSval
        {
            public string SkillName { get; }
            public string SkillTarget { get; }
            public string SkillSvals { get; }
            public ImageSource Bufficons { get; }

            public SkillListSval(string v1, string v2, string v3, ImageSource v4) : this()
            {
                SkillName = v1;
                SkillTarget = v2;
                SkillSvals = v3;
                Bufficons = v4;
            }
        }

        private struct ClassPassiveSvalList
        {
            public string ClassPassiveName { get; }
            public string ClassPassiveID { get; }
            public string ClassPassiveFuncName { get; }
            public string ClassPassiveFuncSval { get; }

            public ClassPassiveSvalList(string v1, string v2, string v3, string v4) : this()
            {
                ClassPassiveName = v1;
                ClassPassiveID = v2;
                ClassPassiveFuncName = v3;
                ClassPassiveFuncSval = v4;
            }
        }

        private struct AppendClassPassiveSvalList
        {
            public string AppendClassPassiveName { get; }
            public string AppendClassPassiveID { get; }
            public string AppendClassPassiveFuncName { get; }

            public AppendClassPassiveSvalList(string v1, string v2, string v3) : this()
            {
                AppendClassPassiveName = v1;
                AppendClassPassiveID = v2;
                AppendClassPassiveFuncName = v3;
            }
        }

        private struct TDlistSval
        {
            public string TDFuncName { get; }
            public string TDSvals { get; }
            public string TDBuffTarget { get; }
            public ImageSource Bufficons { get; }


            public TDlistSval(string v1, string v2, string v3, ImageSource v4) : this()
            {
                TDFuncName = v1;
                TDSvals = v2;
                TDBuffTarget = v3;
                Bufficons = v4;
            }
        }

        private struct QuestList
        {
            public string QuestNumber { get; }
            public string QuestName { get; }
            public string QuestStartTime { get; }
            public string QuestEndTime { get; }

            public QuestList(string v1, string v2, string v3, string v4) : this()
            {
                QuestNumber = v1;
                QuestName = v2;
                QuestStartTime = v3;
                QuestEndTime = v4;
            }
        }

        private struct EventList
        {
            public string EventNumber { get; }
            public string EventName { get; }
            public string EventEndTime { get; }

            public EventList(string v1, string v2, string v3) : this()
            {
                EventNumber = v1;
                EventName = v2;
                EventEndTime = v3;
            }
        }

        private struct ItemList
        {
            public string Status { get; }
            public string Items { get; }
            public string QP { get; }

            public ItemList(string v1, string v2, string v3) : this()
            {
                Status = v1;
                Items = v2;
                QP = v3;
            }
        }

        private struct ClassRelationList
        {
            public string ClassName { get; }
            public string WeakClassA { get; }
            public string ResistClassA { get; }
            public string WeakClassD { get; }
            public string ResistClassD { get; }

            public ClassRelationList(string v1, string v2, string v3, string v4, string v5) : this()
            {
                ClassName = v1;
                WeakClassA = v2;
                ResistClassA = v4;
                WeakClassD = v3;
                ResistClassD = v5;
            }
        }

        private struct FilterList
        {
            public string FilterID { get; }
            public string FilterTag { get; }
            public string SvtName { get; }

            public FilterList(string v1, string v2, string v3) : this()
            {
                FilterID = v1;
                FilterTag = v2;
                SvtName = v3;
            }
        }

        private struct HpAtkList
        {
            public string SvtLevel { get; }
            public string SvtHp { get; }
            public string SvtAtk { get; }

            public HpAtkList(string v1, string v2, string v3) : this()
            {
                SvtLevel = v1;
                SvtHp = v2;
                SvtAtk = v3;
            }
        }
    }
}