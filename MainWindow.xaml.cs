using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Altera.Properties;
using HandyControl.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using MessageBox = HandyControl.Controls.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace Altera
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private readonly string BuffTranslationListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/BuffTranslation";

        private readonly string BuffTranslationListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/BuffTranslation";

        private readonly string IndividualListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/IndividualityList";

        private readonly string IndividualListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/IndividualityList";

        private readonly string TDAttackNameTranslationListLinkA =
            "https://raw.githubusercontent.com/ACPudding/ACPudding.github.io/master/fileserv/TDAttackName";

        private readonly string TDAttackNameTranslationListLinkB =
            "https://gitee.com/ACPudding/ACPudding.github.io/raw/master/fileserv/TDAttackName";

        private Polyline platk;
        private Polyline plhp;


        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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

            var SA = new Task(StartAnalyze);
            SA.Start();
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
            ClearTexts();
            var TDStringBar = GetSvtTDID(svtID);
            if (TDStringBar[1] == "true") IsNPStrengthened.Dispatcher.Invoke(() => { IsNPStrengthened.Text = "√"; });
            svtTDID = TDStringBar[0];
            textbox1.Dispatcher.Invoke(() => { textbox1.Text = svtID; });

            ToggleBuffFuncTranslate.Dispatcher.Invoke(() =>
            {
                if (ToggleBuffFuncTranslate.IsChecked == true)
                    GlobalPathsAndDatas.TranslationList =
                        HttpRequest.GetList(BuffTranslationListLinkA, BuffTranslationListLinkB);
            });
            SCAC.Start();
            SBIC.Start();
            SCIC.Start();
            SJTC.Start();
            SCLIC.Start();
            SCSIC.Start();
            STDI.Start();
            Task.WaitAny(SCLIC);
            var STDSC = new Task(() => { ServantTreasureDeviceSvalCheck(svtTDID); });
            STDSC.Start();
            SSIC.Start();
            Task.WaitAll(STDSC, SSIC);
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

        private string[] GetSvtTDID(string svtID)
        {
            var svtTDID = "";
            var isNPStrengthen = "false";
            var result = new string[2];
            result[0] = svtTDID;
            result[1] = isNPStrengthen;
            try
            {
                foreach (var svtTreasureDevicestmp in GlobalPathsAndDatas.mstSvtTreasureDevicedArray)
                {
                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["num"].ToString() == "1" &&
                        ((JObject) svtTreasureDevicestmp)["treasureDeviceId"].ToString().Length <= 5)
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["num"].ToString() == "98" &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "0")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "198")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "199")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "101")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "102")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                        isNPStrengthen = "true";
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "103")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() == svtID &&
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() == "104")
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                    }

                    if (((JObject) svtTreasureDevicestmp)["svtId"].ToString() != svtID ||
                        ((JObject) svtTreasureDevicestmp)["priority"].ToString() != "105") continue;
                    {
                        var mstsvtTDobjtmp = JObject.Parse(svtTreasureDevicestmp.ToString());
                        svtTDID = mstsvtTDobjtmp["treasureDeviceId"].ToString();
                        break;
                    }
                }

                result[0] = svtTDID;
                result[1] = isNPStrengthen;
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
            var NPRateTD = 0.0;
            var NPRateArts = 0.0;
            var NPRateBuster = 0.0;
            var NPRateQuick = 0.0;
            var NPRateEX = 0.0;
            var NPRateDef = 0.0;
            foreach (var TDlvtmp in GlobalPathsAndDatas.mstTreasureDeviceLvArray)
                if (((JObject) TDlvtmp)["treaureDeviceId"].ToString() == svtTDID)
                {
                    var TDlvobjtmp = JObject.Parse(TDlvtmp.ToString());
                    NPRateTD = Convert.ToDouble(TDlvobjtmp["tdPoint"].ToString());
                    NPRateArts = Convert.ToDouble(TDlvobjtmp["tdPointA"].ToString());
                    NPRateBuster = Convert.ToDouble(TDlvobjtmp["tdPointB"].ToString());
                    NPRateQuick = Convert.ToDouble(TDlvobjtmp["tdPointQ"].ToString());
                    NPRateEX = Convert.ToDouble(TDlvobjtmp["tdPointEx"].ToString());
                    NPRateDef = Convert.ToDouble(TDlvobjtmp["tdPointDef"].ToString());
                    break;
                }

            nprate.Dispatcher.Invoke(() =>
            {
                nprate.Text = "Quick: " + (NPRateQuick / 10000).ToString("P") + "   Arts: " +
                              (NPRateArts / 10000).ToString("P") + "   Buster: " +
                              (NPRateBuster / 10000).ToString("P") + "\r\nExtra: " +
                              (NPRateEX / 10000).ToString("P") + "   宝具: " + (NPRateTD / 10000).ToString("P") +
                              "   受击: " + (NPRateDef / 10000).ToString("P");
            });
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
                if (((JObject) TDDtmp)["id"].ToString() == svtTDID.ToString())
                {
                    var TDDobjtmp = JObject.Parse(TDDtmp.ToString());
                    ToggleDetailbr.Dispatcher.Invoke(() =>
                    {
                        if (ToggleDetailbr.IsChecked == true)
                            NPDetail = TDDobjtmp["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.5]")
                                .Replace("[g]", "")
                                .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "").Replace(@"＆", "\r\n ＋")
                                .Replace(@"＋", "\r\n ＋").Replace("\r\n \r\n", "\r\n");
                        else
                            NPDetail = TDDobjtmp["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.5]")
                                .Replace("[g]", "")
                                .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "").Replace(@"＆", " ＋");
                    });
                    break;
                }

            foreach (var TreasureDevicestmp in GlobalPathsAndDatas.mstTreasureDevicedArray)
            {
                if (((JObject) TreasureDevicestmp)["id"].ToString() != svtTDID.ToString()) continue;
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
                cards.Dispatcher.Invoke(() =>
                {
                    if (svtNPDamageType != "辅助宝具" || cards.Text == "[Q,Q,Q,Q,Q]") return;
                    svtNPCardhit = 0;
                    svtNPCardhitDamage = "[ - ]";
                });
                foreach (var svtTreasureDevicestmp in GlobalPathsAndDatas.mstSvtTreasureDevicedArray)
                    if (((JObject) svtTreasureDevicestmp)["treasureDeviceId"].ToString() ==
                        ((JObject) TreasureDevicestmp)["id"].ToString())
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

                break;
            }

            treasuredevicescard.Dispatcher.Invoke(() =>
            {
                svtNPCardhit += svtNPCardhitDamage.Count(c => c == ',');
                treasuredevicescard.Text = svtNPCardhit + " hit " + svtNPCardhitDamage;
            });
            npcardtype.Dispatcher.Invoke(() => { npcardtype.Text = svtNPCardType; });

            var newtmpid = "";
            if (NPDetail == "unknown")
                foreach (var TreasureDevicestmp2 in GlobalPathsAndDatas.mstTreasureDevicedArray)
                    if (((JObject) TreasureDevicestmp2)["name"].ToString() == NPName)
                    {
                        var TreasureDevicesobjtmp2 = JObject.Parse(TreasureDevicestmp2.ToString());
                        newtmpid = TreasureDevicesobjtmp2["id"].ToString();
                        switch (newtmpid.Length)
                        {
                            case 6:
                            {
                                var FinTDID_TMP = newtmpid;
                                foreach (var TDDtmp2 in GlobalPathsAndDatas.mstTreasureDeviceDetailArray)
                                    if (((JObject) TDDtmp2)["id"].ToString() == FinTDID_TMP)
                                    {
                                        var TDDobjtmp2 = JObject.Parse(TDDtmp2.ToString());
                                        if (ToggleDetailbr.IsChecked == true)
                                            NPDetail = TDDobjtmp2["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.5]")
                                                .Replace("[g]", "")
                                                .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "")
                                                .Replace(@"＆", "\r\n ＋")
                                                .Replace(@"＋", "\r\n ＋").Replace("\r\n \r\n", "\r\n");
                                        else
                                            NPDetail = TDDobjtmp2["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.5]")
                                                .Replace("[g]", "")
                                                .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "")
                                                .Replace(@"＆", " ＋");
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
                                        if (((JObject) TDDtmp2)["id"].ToString() == FinTDID_TMP)
                                        {
                                            var TDDobjtmp2 = JObject.Parse(TDDtmp2.ToString());
                                            if (ToggleDetailbr.IsChecked == true)
                                                NPDetail = TDDobjtmp2["detail"].ToString()
                                                    .Replace("[{0}]", "[Lv.1 - Lv.5]").Replace("[g]", "")
                                                    .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "")
                                                    .Replace(@"＆", "\r\n ＋")
                                                    .Replace(@"＋", "\r\n ＋").Replace("\r\n \r\n", "\r\n");
                                            else
                                                NPDetail = TDDobjtmp2["detail"].ToString()
                                                    .Replace("[{0}]", "[Lv.1 - Lv.5]").Replace("[g]", "")
                                                    .Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "")
                                                    .Replace(@"＆", " ＋");
                                        }
                                }

                                break;
                            }
                        }
                    }

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
                RankString[21] = "B";
                RankString[22] = "B+";
                RankString[23] = "B++";
                RankString[24] = "B-";
                RankString[25] = "B+++";
                RankString[31] = "C";
                RankString[32] = "C+";
                RankString[33] = "C++";
                RankString[34] = "C-";
                RankString[35] = "C+++";
                RankString[41] = "D";
                RankString[42] = "D+";
                RankString[43] = "D++";
                RankString[44] = "D-";
                RankString[45] = "D+++";
                RankString[51] = "E";
                RankString[52] = "E+";
                RankString[53] = "E++";
                RankString[54] = "E-";
                RankString[55] = "E+++";
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
                gender[1] = "男性";
                gender[2] = "女性";
                gender[3] = "其他";
                var nprateclassbase = new double[150];
                nprateclassbase[1] = 1.5;
                nprateclassbase[2] = 1.55;
                nprateclassbase[3] = 1.45;
                nprateclassbase[4] = 1.55;
                nprateclassbase[5] = 1.6;
                nprateclassbase[6] = 1.45;
                nprateclassbase[7] = 1.4;
                nprateclassbase[8] = 1.5;
                nprateclassbase[9] = 1.5;
                nprateclassbase[10] = 1.55;
                nprateclassbase[11] = 1.45;
                nprateclassbase[23] = 1.6;
                nprateclassbase[25] = 1.5;
                nprateclassbase[20] = 0.0;
                nprateclassbase[22] = 0.0;
                nprateclassbase[24] = 0.0;
                nprateclassbase[26] = 0.0;
                nprateclassbase[27] = 0.0;
                nprateclassbase[97] = 0.0;
                nprateclassbase[107] = 0.0;
                nprateclassbase[21] = 0.0;
                nprateclassbase[19] = 0.0;
                nprateclassbase[18] = 0.0;
                nprateclassbase[17] = 1.6;
                nprateclassbase[16] = 0.0;
                nprateclassbase[15] = 0.0;
                nprateclassbase[14] = 0.0;
                nprateclassbase[13] = 0.0;
                nprateclassbase[12] = 0.0;
                var nprateartscount = new double[4];
                nprateartscount[1] = 1.5;
                nprateartscount[2] = 1.125;
                nprateartscount[3] = 1;
                var npratemagicbase = new double[100];
                npratemagicbase[11] = 1.02;
                npratemagicbase[12] = 1.025;
                npratemagicbase[13] = 1.03;
                npratemagicbase[14] = 1.015;
                npratemagicbase[15] = 1.035;
                npratemagicbase[21] = 1;
                npratemagicbase[22] = 1.005;
                npratemagicbase[23] = 1.01;
                npratemagicbase[24] = 0.995;
                npratemagicbase[25] = 1.015;
                npratemagicbase[31] = 0.99;
                npratemagicbase[32] = 0.9925;
                npratemagicbase[33] = 0.995;
                npratemagicbase[34] = 0.985;
                npratemagicbase[35] = 0.9975;
                npratemagicbase[41] = 0.98;
                npratemagicbase[42] = 0.9825;
                npratemagicbase[43] = 0.985;
                npratemagicbase[44] = 0.975;
                npratemagicbase[45] = 0.9875;
                npratemagicbase[51] = 0.97;
                npratemagicbase[52] = 0.9725;
                npratemagicbase[53] = 0.975;
                npratemagicbase[54] = 0.965;
                npratemagicbase[55] = 0.9775;
                npratemagicbase[61] = 1.04;
                npratemagicbase[0] = 0.0;
                npratemagicbase[99] = 0.0;
                npratemagicbase[98] = 0.0;
                npratemagicbase[97] = 0.0;
                var svtstarrate = "";
                double NPrate = 0;
                float starrate = 0;
                float deathrate = 0;
                var svtdeathrate = "";
                var svtillust = "unknown"; //illustID 不输出
                var svtcv = "unknown"; //CVID 不输出
                var svtcollectionid = "";
                var svtCVName = "unknown";
                var svtILLUSTName = "unknown";
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
                foreach (var svtIDtmp in GlobalPathsAndDatas.mstSvtArray)
                    if (((JObject) svtIDtmp)["id"].ToString() == JB.svtid)
                    {
                        var mstSvtobjtmp = JObject.Parse(svtIDtmp.ToString());
                        svtName = mstSvtobjtmp["name"].ToString();
                        Svtname.Text = svtName;
                        JB.svtnme = svtName;
                        svtNameDisplay = mstSvtobjtmp["battleName"].ToString();
                        SvtBattlename.Text = svtNameDisplay;
                        Title += " - " + svtNameDisplay;
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
                        svtclass.Text = ClassName[classData] != null
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
                    if (((JObject) svtLimittmp)["svtId"].ToString() == JB.svtid)
                    {
                        var mstsvtLimitobjtmp = JObject.Parse(svtLimittmp.ToString());
                        svtrarity = mstsvtLimitobjtmp["rarity"].ToString();
                        var DSR = new Task(() => { DisplaySvtRarity(Convert.ToInt32(svtrarity)); });
                        DSR.Start();
                        rarity.Text = svtrarity + " ☆";
                        svthpBase = mstsvtLimitobjtmp["hpBase"].ToString();
                        basichp.Text = svthpBase;
                        svthpMax = mstsvtLimitobjtmp["hpMax"].ToString();
                        maxhp.Text = svthpMax;
                        svtatkBase = mstsvtLimitobjtmp["atkBase"].ToString();
                        basicatk.Text = svtatkBase;
                        svtatkMax = mstsvtLimitobjtmp["atkMax"].ToString();
                        maxatk.Text = svtatkMax;
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

                var svtArtsCardQuantity = CardArrange.Count(c => c == 'A');
                if (svtArtsCardQuantity == 0)
                {
                    NPrate = 0;
                    notrealnprate.Text = NPrate.ToString("P");
                }
                else
                {
                    NPrate = nprateclassbase[classData] * nprateartscount[svtArtsCardQuantity] *
                        npratemagicbase[magicData] / GlobalPathsAndDatas.svtArtsCardhit / 100;
                    NPrate = Math.Floor(NPrate * 10000) / 10000;
                    notrealnprate.Text = NPrate.ToString("P");
                }

                switch (classData)
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
                        atkbalance1.Text = "( x 1.0 -)";
                        atkbalance2.Text = "( x 1.0 -)";
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
                        atkbalance1.Text = "( x 1.05 △)";
                        atkbalance2.Text = "( x 1.05 △)";
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
                        atkbalance1.Text = "( x 0.9 ▽)";
                        atkbalance2.Text = "( x 0.9 ▽)";
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
                        atkbalance1.Text = "( x 0.95 ▽)";
                        atkbalance2.Text = "( x 0.95 ▽)";
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
                        atkbalance1.Text = "( x 1.1 △)";
                        atkbalance2.Text = "( x 1.1 △)";
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
                        Growl.Info("此ID为礼装ID,图鉴编号为礼装的图鉴编号.礼装描述在羁绊文本的文本1处.");
                        break;
                    default:
                        atkbalance1.Text = "( x 1.0 -)";
                        atkbalance2.Text = "( x 1.0 -)";
                        break;
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

        private void ServantJibanTextCheck()
        {
            foreach (var SCTMP in GlobalPathsAndDatas.mstSvtCommentArray)
            {
                if (((JObject) SCTMP)["svtId"].ToString() == JB.svtid && ((JObject) SCTMP)["id"].ToString() == "1")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext1.Dispatcher.Invoke(() =>
                    {
                        jibantext1.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                        if (jibantext1.Text != "")
                            JBOutput.Dispatcher.Invoke(() => { JBOutput.IsEnabled = true; });
                    });
                    JB.JB1 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }

                if (((JObject) SCTMP)["svtId"].ToString() == JB.svtid && ((JObject) SCTMP)["id"].ToString() == "2")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext2.Dispatcher.Invoke(() =>
                    {
                        jibantext2.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                    });
                    JB.JB2 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }

                if (((JObject) SCTMP)["svtId"].ToString() == JB.svtid && ((JObject) SCTMP)["id"].ToString() == "3")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext3.Dispatcher.Invoke(() =>
                    {
                        jibantext3.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                    });
                    JB.JB3 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }

                if (((JObject) SCTMP)["svtId"].ToString() == JB.svtid && ((JObject) SCTMP)["id"].ToString() == "4")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext4.Dispatcher.Invoke(() =>
                    {
                        jibantext4.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                    });
                    JB.JB4 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }

                if (((JObject) SCTMP)["svtId"].ToString() == JB.svtid && ((JObject) SCTMP)["id"].ToString() == "5")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext5.Dispatcher.Invoke(() =>
                    {
                        jibantext5.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                    });
                    JB.JB5 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }

                if (((JObject) SCTMP)["svtId"].ToString() == JB.svtid && ((JObject) SCTMP)["id"].ToString() == "6")
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext6.Dispatcher.Invoke(() =>
                    {
                        jibantext6.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                    });
                    JB.JB6 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }

                if (((JObject) SCTMP)["svtId"].ToString() != JB.svtid ||
                    ((JObject) SCTMP)["id"].ToString() != "7") continue;
                {
                    var SCobjtmp = JObject.Parse(SCTMP.ToString());
                    jibantext7.Dispatcher.Invoke(() =>
                    {
                        jibantext7.Text = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                    });
                    JB.JB7 = SCobjtmp["comment"].ToString().Replace("\n", "\r\n");
                }
            }
        }

        private void ServantCombineLimitItemsCheck()
        {
            foreach (var mstCombineLimittmp in GlobalPathsAndDatas.mstCombineLimitArray)
                if (((JObject) mstCombineLimittmp)["id"].ToString() == JB.svtid)
                {
                    var LimitID = ((JObject) mstCombineLimittmp)["svtLimit"].ToString();
                    switch (Convert.ToInt64(LimitID))
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            var itemIds = ((JObject) mstCombineLimittmp)["itemIds"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var itemNums = ((JObject) mstCombineLimittmp)["itemNums"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var qp = ((JObject) mstCombineLimittmp)["qp"].ToString();
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
                if (((JObject) mstCombineSkilltmp)["id"].ToString() == JB.svtid)
                {
                    var LimitID = ((JObject) mstCombineSkilltmp)["skillLv"].ToString();
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
                            var itemIds = ((JObject) mstCombineSkilltmp)["itemIds"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var itemNums = ((JObject) mstCombineSkilltmp)["itemNums"].ToString().Replace("\n", "")
                                .Replace("\t", "")
                                .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                            var qp = ((JObject) mstCombineSkilltmp)["qp"].ToString();
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
                if (((JObject) mstItemtmp)["id"].ToString() != ID.ToString()) continue;
                var mstItemtmpobjtmp = JObject.Parse(mstItemtmp.ToString());
                return mstItemtmpobjtmp["name"].ToString();
            }

            return "未知材料" + ID;
        }

        private void ServantCVandIllustCheck()
        {
            var svtillust = "unknown"; //illustID 不输出
            var svtcv = "unknown"; //CVID 不输出
            var svtCVName = "unknown";
            var svtILLUSTName = "unknown";
            foreach (var svtIDtmp in GlobalPathsAndDatas.mstSvtArray)
                if (((JObject) svtIDtmp)["id"].ToString() == JB.svtid)
                {
                    var mstSvtobjtmp = JObject.Parse(svtIDtmp.ToString());
                    svtillust = mstSvtobjtmp["illustratorId"].ToString(); //illustID
                    svtcv = mstSvtobjtmp["cvId"].ToString(); //CVID
                    break;
                }

            foreach (var cvidtmp in GlobalPathsAndDatas.mstCvArray)
                if (((JObject) cvidtmp)["id"].ToString() == svtcv)
                {
                    var mstCVobjtmp = JObject.Parse(cvidtmp.ToString());
                    svtCVName = mstCVobjtmp["name"].ToString();
                    cv.Dispatcher.Invoke(() => { cv.Text = svtCVName; });
                    break;
                }

            foreach (var illustidtmp in GlobalPathsAndDatas.mstIllustratorArray)
                if (((JObject) illustidtmp)["id"].ToString() == svtillust)
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
            var svtArtsCardQuantity = 0;
            var svtBustersCardhit = 1;
            var svtBustersCardhitDamage = "unknown";
            var svtQuicksCardhit = 1;
            var svtQuicksCardhitDamage = "unknown";
            var svtExtraCardhit = 1;
            var svtExtraCardhitDamage = "unknown";
            GlobalPathsAndDatas.svtArtsCardhit = 1;
            foreach (var svtCardtmp in GlobalPathsAndDatas.mstSvtCardArray)
            {
                if (((JObject) svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject) svtCardtmp)["cardId"].ToString() == "1")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtArtsCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtArtsCardhit += svtArtsCardhitDamage.Count(c => c == ',');
                    GlobalPathsAndDatas.svtArtsCardhit = svtArtsCardhit;
                    artscard.Dispatcher.Invoke(() =>
                    {
                        artscard.Text = svtArtsCardhit + " hit " + svtArtsCardhitDamage;
                    });
                }

                if (((JObject) svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject) svtCardtmp)["cardId"].ToString() == "2")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtBustersCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtBustersCardhit += svtBustersCardhitDamage.Count(c => c == ',');
                    bustercard.Dispatcher.Invoke(() =>
                    {
                        bustercard.Text = svtBustersCardhit + " hit " + svtBustersCardhitDamage;
                    });
                }

                if (((JObject) svtCardtmp)["svtId"].ToString() == JB.svtid &&
                    ((JObject) svtCardtmp)["cardId"].ToString() == "3")
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtQuicksCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtQuicksCardhit += svtQuicksCardhitDamage.Count(c => c == ',');
                    quickcard.Dispatcher.Invoke(() =>
                    {
                        quickcard.Text = svtQuicksCardhit + " hit " + svtQuicksCardhitDamage;
                    });
                }

                if (((JObject) svtCardtmp)["svtId"].ToString() != JB.svtid ||
                    ((JObject) svtCardtmp)["cardId"].ToString() != "4") continue;
                {
                    var mstSvtCardobjtmp = JObject.Parse(svtCardtmp.ToString());
                    svtExtraCardhitDamage = mstSvtCardobjtmp["normalDamage"].ToString().Replace("\n", "")
                        .Replace("\t", "").Replace("\r", "").Replace(" ", "");
                    svtExtraCardhit += svtExtraCardhitDamage.Count(c => c == ',');
                    extracard.Dispatcher.Invoke(() =>
                    {
                        extracard.Text = svtExtraCardhit + " hit " + svtExtraCardhitDamage;
                    });
                }
            }
        }

        private void ServantTreasureDeviceSvalCheck(string svtTDID)
        {
            string svtTreasureDeviceFuncID;
            string[] svtTreasureDeviceFuncIDArray = null;
            string[] svtTreasureDeviceFuncArray;
            var svtTreasureDeviceFunc = string.Empty;
            string[] TDFuncstrArray = null;
            string[] TDlv1OC1strArray = null;
            string[] TDlv2OC2strArray = null;
            string[] TDlv3OC3strArray = null;
            string[] TDlv4OC4strArray = null;
            string[] TDlv5OC5strArray = null;
            SkillLvs.TDforExcel = "";
            foreach (var TDLVtmp in GlobalPathsAndDatas.mstTreasureDeviceLvArray)
            {
                if (((JObject) TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject) TDLVtmp)["lv"].ToString() == "1")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval1 = TDLVobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval1.Length >= 2) NPval1 = NPval1.Substring(0, NPval1.Length - 2);
                    TDlv1OC1strArray = NPval1.Split('|');
                }

                if (((JObject) TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject) TDLVtmp)["lv"].ToString() == "2")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval2 = TDLVobjtmp["svals2"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval2.Length >= 2) NPval2 = NPval2.Substring(0, NPval2.Length - 2);
                    TDlv2OC2strArray = NPval2.Split('|');
                }

                if (((JObject) TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject) TDLVtmp)["lv"].ToString() == "3")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval3 = TDLVobjtmp["svals3"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval3.Length >= 2) NPval3 = NPval3.Substring(0, NPval3.Length - 2);
                    TDlv3OC3strArray = NPval3.Split('|');
                }

                if (((JObject) TDLVtmp)["treaureDeviceId"].ToString() == svtTDID &&
                    ((JObject) TDLVtmp)["lv"].ToString() == "4")
                {
                    var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                    var NPval4 = TDLVobjtmp["svals4"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    if (NPval4.Length >= 2) NPval4 = NPval4.Substring(0, NPval4.Length - 2);
                    TDlv4OC4strArray = NPval4.Split('|');
                }

                if (((JObject) TDLVtmp)["treaureDeviceId"].ToString() != svtTDID ||
                    ((JObject) TDLVtmp)["lv"].ToString() != "5") continue;
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
                var NeedTranslate = false;
                ToggleBuffFuncTranslate.Dispatcher.Invoke(() =>
                {
                    if (ToggleBuffFuncTranslate.IsChecked == true) NeedTranslate = true;
                });
                if (NeedTranslate)
                    svtTreasureDeviceFuncArray = (from skfuncidtmp in svtTreasureDeviceFuncIDArray
                        from functmp in GlobalPathsAndDatas.mstFuncArray
                        where ((JObject) functmp)["id"].ToString() == skfuncidtmp
                        select JObject.Parse(functmp.ToString())
                        into mstFuncobjtmp
                        select TranslateBuff(mstFuncobjtmp["popupText"].ToString())).ToArray();
                else
                    svtTreasureDeviceFuncArray = (from skfuncidtmp in svtTreasureDeviceFuncIDArray
                        from functmp in GlobalPathsAndDatas.mstFuncArray
                        where ((JObject) functmp)["id"].ToString() == skfuncidtmp
                        select JObject.Parse(functmp.ToString())
                        into mstFuncobjtmp
                        select mstFuncobjtmp["popupText"].ToString()).ToArray();
                TDFuncstrArray = svtTreasureDeviceFuncArray;
                svtTreasureDeviceFunc = string.Join(", ", svtTreasureDeviceFuncArray);
                for (var i = 0; i <= TDFuncstrArray.Length - 1; i++)
                {
                    if ((TDFuncstrArray[i] == "なし" || TDFuncstrArray[i] == "" &&
                            TDlv1OC1strArray[i].Contains("Hide")) &&
                        TDlv1OC1strArray[i].Count(c => c == ',') > 0)
                        TDFuncstrArray[i] = TranslateTDAttackName(svtTreasureDeviceFuncIDArray[i]);

                    if (TDFuncstrArray[i] == "" && TDlv1OC1strArray[i].Count(c => c == ',') == 1 &&
                        !TDlv1OC1strArray[i].Contains("Hide")) TDFuncstrArray[i] = "HP回復";
                    ToggleFuncDiffer.Dispatcher.Invoke(() =>
                    {
                        if (ToggleFuncDiffer.IsChecked != true) return;
                        TDlv1OC1strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                            TDlv1OC1strArray[i]);
                        TDlv2OC2strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                            TDlv2OC2strArray[i]);
                        TDlv3OC3strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                            TDlv3OC3strArray[i]);
                        TDlv4OC4strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                            TDlv4OC4strArray[i]);
                        TDlv5OC5strArray[i] = ModifyFuncSvalDisplay.ModifyFuncStr(TDFuncstrArray[i],
                            TDlv5OC5strArray[i]);
                    });
                    TDFuncList.Dispatcher.Invoke(() =>
                    {
                        TDFuncList.Items.Add(new TDlistSval(
                            TDFuncstrArray[i] != "" ? TDFuncstrArray[i] : "未知效果",
                            TDlv1OC1strArray[i], TDlv2OC2strArray[i], TDlv3OC3strArray[i],
                            TDlv4OC4strArray[i], TDlv5OC5strArray[i]));
                    });
                    SkillLvs.TDforExcel += (TDFuncstrArray[i] != ""
                                               ? TDFuncstrArray[i].Replace("\r\n", "")
                                               : "未知效果") +
                                           " 【{" + (TDlv1OC1strArray[i].Replace("\r\n", " ") ==
                                                    TDlv5OC5strArray[i].Replace("\r\n", " ")
                                               ? TDlv5OC5strArray[i].Replace("\r\n", " ")
                                               : TDlv1OC1strArray[i].Replace("\r\n", " ") + "} - {" +
                                                 TDlv5OC5strArray[i].Replace("\r\n", " ")) + "}】\r\n";
                }

                try
                {
                    SkillLvs.TDforExcel = SkillLvs.TDforExcel.Substring(0, SkillLvs.TDforExcel.Length - 2);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private string TranslateTDAttackName(string TDFuncID)
        {
            try
            {
                var GetTDFuncTranslationListArray = HttpRequest
                    .GetList(TDAttackNameTranslationListLinkA, TDAttackNameTranslationListLinkB).Replace("\r\n", "")
                    .Replace("+", Environment.NewLine).Split('|');
                var TDTranslistFullArray = new string[GetTDFuncTranslationListArray.Length][];
                for (var i = 0; i < GetTDFuncTranslationListArray.Length; i++)
                {
                    var TempSplit2 = GetTDFuncTranslationListArray[i].Split(',');
                    TDTranslistFullArray[i] = new string[TempSplit2.Length];
                    for (var j = 0; j < TempSplit2.Length; j++) TDTranslistFullArray[i][j] = TempSplit2[j];
                }

                for (var k = 0; k < GetTDFuncTranslationListArray.Length; k++)
                    if (TDTranslistFullArray[k][0] == TDFuncID)
                        return TDTranslistFullArray[k][1];
                return "暫無翻譯\r\nID: " + TDFuncID;
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

        private string TranslateBuff(string buffname)
        {
            try
            {
                var TranslationListArray = GlobalPathsAndDatas.TranslationList.Replace("\r\n", "").Split('|');
                var TranslationListFullArray = new string[TranslationListArray.Length][];
                for (var i = 0; i < TranslationListArray.Length; i++)
                {
                    var TempSplit2 = TranslationListArray[i].Split(',');
                    TranslationListFullArray[i] = new string[TempSplit2.Length];
                    for (var j = 0; j < TempSplit2.Length; j++) TranslationListFullArray[i][j] = TempSplit2[j];
                }

                for (var k = 0; k < TranslationListArray.Length; k++)
                    if (buffname.Contains(TranslationListFullArray[k][0]))
                        buffname = buffname.Replace(TranslationListFullArray[k][0], TranslationListFullArray[k][1]);
                return buffname;
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() =>
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
                        Growl.Info("该从者尚未实装或为敌方数据,故最终实装的数据可能会与目前的解析结果不同,请以实装之后的数据为准!望知悉.");
                        break;
                }
            });
        }

        private void ServantClassPassiveSkillCheck(string ClassPassiveID)
        {
            try
            {
                string[] svtClassPassiveIDArray = null;
                string[] svtClassPassiveArray;
                var svtClassPassive = string.Empty;
                svtClassPassiveIDArray = ClassPassiveID.Split(',');
                svtClassPassiveArray = (from skilltmp in GlobalPathsAndDatas.mstSkillArray
                    from classpassiveidtmp in svtClassPassiveIDArray
                    where ((JObject) skilltmp)["id"].ToString() == classpassiveidtmp
                    select JObject.Parse(skilltmp.ToString())
                    into mstsvtPskillobjtmp
                    select mstsvtPskillobjtmp["name"].ToString() != ""
                        ? mstsvtPskillobjtmp["name"] + " (" + mstsvtPskillobjtmp["id"] + ")"
                        : "未知技能???" + " (" + mstsvtPskillobjtmp["id"] + ")").ToArray();
                svtClassPassive = string.Join(", ", svtClassPassiveArray);
                classskill.Dispatcher.Invoke(() => { classskill.Text = svtClassPassive; });
                var SCPSSLC = new Task(() => { ServantClassPassiveSkillSvalListCheck(ClassPassiveID); });
                SCPSSLC.Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void ServantClassPassiveSkillSvalListCheck(string ClassPassiveID)
        {
            try
            {
                var svtClassPassiveIDListArray = ClassPassiveID.Split(',');
                var ClassPassiveSkillFuncName = "";
                var SvalStr = "";
                var NeedTranslate = false;
                var lv10sval = "";
                string[] lv10svalArray = null;
                ToggleBuffFuncTranslate.Dispatcher.Invoke(() =>
                {
                    if (ToggleBuffFuncTranslate.IsChecked == true) NeedTranslate = true;
                });
                for (var i = 0; i <= svtClassPassiveIDListArray.Length - 1; i++)
                {
                    foreach (var skilltmp in GlobalPathsAndDatas.mstSkillArray)
                    {
                        if (((JObject) skilltmp)["id"].ToString() != svtClassPassiveIDListArray[i]) continue;
                        var skillobjtmp = JObject.Parse(skilltmp.ToString());
                        ClassPassiveSkillFuncName = NeedTranslate
                            ? TranslateBuff(skillobjtmp["name"].ToString())
                            : skillobjtmp["name"].ToString();
                    }

                    var SkillSquare = GetSvtClassPassiveSval(svtClassPassiveIDListArray[i]);
                    lv10svalArray = SkillSquare[0].Split('|');
                    var SKLFuncstrArray = SkillSquare[1].Replace(" ", "").Split(',');
                    for (var j = 0; j <= SKLFuncstrArray.Length - 1; j++)
                    {
                        if (SKLFuncstrArray[j] == "" &&
                            lv10svalArray[j].Count(c => c == ',') == 1 &&
                            !lv10svalArray[j].Contains("Hide"))
                            SKLFuncstrArray[j] = "HP回復";
                        ToggleFuncDiffer.Dispatcher.Invoke(() =>
                        {
                            if (ToggleFuncDiffer.IsChecked == true)
                                lv10svalArray[j] =
                                    ModifyFuncSvalDisplay.ModifyFuncStr(SKLFuncstrArray[j],
                                        lv10svalArray[j]);
                        });
                    }

                    var FuncStr = "\r\n" + string.Join("\r\n", SKLFuncstrArray) + "\r\n";
                    SvalStr = "\r\n" + string.Join("\r\n", lv10svalArray) + "\r\n";
                    ClassPassiveFuncList.Dispatcher.Invoke(() =>
                    {
                        ClassPassiveFuncList.Items.Add(new ClassPassiveSvalList(ClassPassiveSkillFuncName,
                            svtClassPassiveIDListArray[i], FuncStr, SvalStr));
                    });
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
            var SkillFuncStr = "";
            string[] SKLFuncstrArray = null;
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
                if (((JObject) SKLTMP)["skillId"].ToString() != SkillID ||
                    ((JObject) SKLTMP)["lv"].ToString() != "10") continue;
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv10sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv10sval = skilllv10sval.Substring(0, skilllv10sval.Length - 2);
                    svtSKFuncID = SKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    svtSKFuncIDList = new List<string>(svtSKFuncID.Split(','));
                    svtSKFuncIDArray = svtSKFuncIDList.ToArray();
                    if (NeedTranslate)
                        svtSKFuncList.AddRange(from skfuncidtmp in svtSKFuncIDArray
                            from functmp in GlobalPathsAndDatas.mstFuncArray
                            where ((JObject) functmp)["id"].ToString() == skfuncidtmp
                            select JObject.Parse(functmp.ToString())
                            into mstFuncobjtmp
                            select TranslateBuff(mstFuncobjtmp["popupText"].ToString()));
                    else
                        svtSKFuncList.AddRange(from skfuncidtmp in svtSKFuncIDArray
                            from functmp in GlobalPathsAndDatas.mstFuncArray
                            where ((JObject) functmp)["id"].ToString() == skfuncidtmp
                            select JObject.Parse(functmp.ToString())
                            into mstFuncobjtmp
                            select mstFuncobjtmp["popupText"].ToString());
                }
            }

            svtSKFuncArray = svtSKFuncList.ToArray();
            svtSKFunc = string.Join(", ", svtSKFuncArray);
            var result = new string[2];
            result[0] = skilllv10sval;
            result[1] = svtSKFunc;
            return result;
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
                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "1" &&
                    ((JObject) svtskill)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID1 = mstsvtskillobjtmp["skillId"].ToString();
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "1" &&
                    ((JObject) svtskill)["priority"].ToString() == "2")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID1 = mstsvtskillobjtmp["skillId"].ToString();
                    sk1IsStrengthened = "true";
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "1" &&
                    ((JObject) svtskill)["priority"].ToString() == "3")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID1 = mstsvtskillobjtmp["skillId"].ToString();
                    sk1IsStrengthened = "twice";
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "2" &&
                    ((JObject) svtskill)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID2 = mstsvtskillobjtmp["skillId"].ToString();
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "2" &&
                    ((JObject) svtskill)["priority"].ToString() == "2")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID2 = mstsvtskillobjtmp["skillId"].ToString();
                    sk2IsStrengthened = "true";
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "2" &&
                    ((JObject) svtskill)["priority"].ToString() == "3")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID2 = mstsvtskillobjtmp["skillId"].ToString();
                    sk2IsStrengthened = "twice";
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "3" &&
                    ((JObject) svtskill)["priority"].ToString() == "1")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID3 = mstsvtskillobjtmp["skillId"].ToString();
                }

                if (((JObject) svtskill)["svtId"].ToString() == svtID &&
                    ((JObject) svtskill)["num"].ToString() == "3" &&
                    ((JObject) svtskill)["priority"].ToString() == "2")
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID3 = mstsvtskillobjtmp["skillId"].ToString();
                    sk3IsStrengthened = "true";
                }

                if (((JObject) svtskill)["svtId"].ToString() != svtID ||
                    ((JObject) svtskill)["num"].ToString() != "3" ||
                    ((JObject) svtskill)["priority"].ToString() != "3") continue;
                {
                    var mstsvtskillobjtmp = JObject.Parse(svtskill.ToString());
                    skillID3 = mstsvtskillobjtmp["skillId"].ToString();
                    sk3IsStrengthened = "twice";
                }
            }

            if (skillID1 == "" || skillID2 == "" || skillID3 == "")
            {
                skillID1 = FindSkillIDinNPCSvt(svtID, 1);
                skillID2 = FindSkillIDinNPCSvt(svtID, 2);
                skillID3 = FindSkillIDinNPCSvt(svtID, 3);
            }

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
                    IsSk1Strengthened.Dispatcher.Invoke(() => { IsSk1Strengthened.Text = "√"; });
                    break;
                case "twice":
                    IsSk1Strengthened.Dispatcher.Invoke(() => { IsSk1Strengthened.Text = "√(2)"; });
                    break;
            }

            switch (skillSquare[3])
            {
                case "true":
                    IsSk2Strengthened.Dispatcher.Invoke(() => { IsSk2Strengthened.Text = "√"; });
                    break;
                case "twice":
                    IsSk2Strengthened.Dispatcher.Invoke(() => { IsSk2Strengthened.Text = "√(2)"; });
                    break;
            }

            switch (skillSquare[5])
            {
                case "true":
                    IsSk3Strengthened.Dispatcher.Invoke(() => { IsSk3Strengthened.Text = "√"; });
                    break;
                case "twice":
                    IsSk3Strengthened.Dispatcher.Invoke(() => { IsSk3Strengthened.Text = "√(2)"; });
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
            foreach (var skilltmp in GlobalPathsAndDatas.mstSkillArray)
            {
                if (((JObject) skilltmp)["id"].ToString() != SkillID) continue;
                var skillobjtmp = JObject.Parse(skilltmp.ToString());
                skillName = skillobjtmp["name"].ToString();
                if (isStrengthed) skillName += " ▲";
            }

            Dispatcher.Invoke(() =>
            {
                foreach (var skillDetailtmp in GlobalPathsAndDatas.mstSkillDetailArray)
                {
                    if (((JObject) skillDetailtmp)["id"].ToString() != SkillID) continue;
                    var skillDetailobjtmp = JObject.Parse(skillDetailtmp.ToString());
                    if (ToggleDetailbr.IsChecked == true)
                        skillDetail = skillDetailobjtmp["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.10]")
                            .Replace("[g]", "").Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "")
                            .Replace(@"＆", "\r\n ＋").Replace(@"＋", "\r\n ＋").Replace("\r\n \r\n", "\r\n");
                    else
                        skillDetail = skillDetailobjtmp["detail"].ToString().Replace("[{0}]", "[Lv.1 - Lv.10]")
                            .Replace("[g]", "").Replace("[o]", "").Replace("[/g]", "").Replace("[/o]", "")
                            .Replace(@"＆", " ＋");
                }
            });
            Dispatcher.Invoke(() =>
            {
                switch (SkillNum)
                {
                    case 1:
                        skill1name.Text = skillName;
                        skill1details.Text = skillDetail;
                        var SSC = new Task(() => { SkillSvalsCheck(SkillID, 1); });
                        SSC.Start();
                        break;
                    case 2:
                        skill2name.Text = skillName;
                        skill2details.Text = skillDetail;
                        var SSC2 = new Task(() => { SkillSvalsCheck(SkillID, 2); });
                        SSC2.Start();
                        break;
                    case 3:
                        skill3name.Text = skillName;
                        skill3details.Text = skillDetail;
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
                if (((JObject) npcSvtFollowertmp)["svtId"].ToString() != svtid) continue;
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
                        (current, svtIDtmp) => current + "ID: " + ((JObject) svtIDtmp)["id"] + "    " + "名称: " +
                                               ((JObject) svtIDtmp)["name"] + "\r\n");
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
                GlobalPathsAndDatas.TranslationListArray = null;
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
                Title = "Altera";
                chartCanvas.Children.Remove(plhp);
                chartCanvas.Children.Remove(platk);
                hpatkbalance.Text = "( 攻防倾向: 均衡 )";
                var QuickUri = "images\\Quick.png";
                card1.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card2.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card3.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card4.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                card5.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                cardTD.Source = new BitmapImage(new Uri(QuickUri, UriKind.Relative));
                raritystars.Visibility = Visibility.Hidden;
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
            var SkillFuncStr = "";
            string[] SKLFuncstrArray = null;
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
                if (((JObject) SKLTMP)["skillId"].ToString() == SkillID && ((JObject) SKLTMP)["lv"].ToString() == "1")
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv1sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv1sval = skilllv1sval.Substring(0, skilllv1sval.Length - 2);
                    skilllv1chargetime = SKLobjtmp["chargeTurn"].ToString();
                }

                if (((JObject) SKLTMP)["skillId"].ToString() == SkillID && ((JObject) SKLTMP)["lv"].ToString() == "6")
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv6sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv6sval = skilllv6sval.Substring(0, skilllv6sval.Length - 2);
                    skilllv6chargetime = SKLobjtmp["chargeTurn"].ToString();
                }

                if (((JObject) SKLTMP)["skillId"].ToString() != SkillID ||
                    ((JObject) SKLTMP)["lv"].ToString() != "10") continue;
                {
                    var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                    skilllv10sval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                        .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                    skilllv10sval = skilllv10sval.Substring(0, skilllv10sval.Length - 2);
                    skilllv10chargetime = SKLobjtmp["chargeTurn"].ToString();
                    svtSKFuncID = SKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    svtSKFuncIDList = new List<string>(svtSKFuncID.Split(','));
                    svtSKFuncIDArray = svtSKFuncIDList.ToArray();
                    if (NeedTranslate)
                        svtSKFuncList.AddRange(from skfuncidtmp in svtSKFuncIDArray
                            from functmp in GlobalPathsAndDatas.mstFuncArray
                            where ((JObject) functmp)["id"].ToString() == skfuncidtmp
                            select JObject.Parse(functmp.ToString())
                            into mstFuncobjtmp
                            select TranslateBuff(mstFuncobjtmp["popupText"].ToString()));
                    else
                        svtSKFuncList.AddRange(from skfuncidtmp in svtSKFuncIDArray
                            from functmp in GlobalPathsAndDatas.mstFuncArray
                            where ((JObject) functmp)["id"].ToString() == skfuncidtmp
                            select JObject.Parse(functmp.ToString())
                            into mstFuncobjtmp
                            select mstFuncobjtmp["popupText"].ToString());
                }
            }

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
                SkillSvalsDisplay(skilllv1sval, skilllv6sval, skilllv10sval, svtSKFunc, SkillNum);
            });
            SSD.Start();
        }

        private void SkillSvalsDisplay(string lv1, string lv6, string lv10, string FuncName, int SkillNum)
        {
            Dispatcher.Invoke(() =>
            {
                if (skill1ID.Text == "") return;
                var lv1Array = lv1.Split('|');
                var lv6Array = lv6.Split('|');
                var lv10Array = lv10.Split('|');
                var FuncArray = FuncName.Replace(" ", "").Split(',');
                for (var i = 0; i <= FuncArray.Length - 1; i++)
                {
                    if (FuncArray[i] == "" && lv1Array[i].Count(c => c == ',') == 1 &&
                        !lv1Array[i].Contains("Hide")) FuncArray[i] = "HP回復";
                    if (ToggleFuncDiffer.IsChecked == true)
                    {
                        lv1Array[i] = ModifyFuncSvalDisplay.ModifyFuncStr(FuncArray[i],
                            lv1Array[i]);
                        lv6Array[i] = ModifyFuncSvalDisplay.ModifyFuncStr(FuncArray[i],
                            lv6Array[i]);
                        lv10Array[i] =
                            ModifyFuncSvalDisplay.ModifyFuncStr(FuncArray[i],
                                lv10Array[i]);
                    }

                    switch (SkillNum)
                    {
                        case 1:
                            Skill1FuncList.Items.Add(new SkillListSval(FuncArray[i],
                                lv1Array[i], lv6Array[i], lv10Array[i]));
                            SkillLvs.skill1forExcel += FuncArray[i] + " 【{" +
                                                       (lv1Array[i].Replace("\r\n", " ") ==
                                                        lv10Array[i].Replace("\r\n", " ")
                                                           ? lv10Array[i].Replace("\r\n", " ")
                                                           : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                                             lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";
                            break;
                        case 2:
                            Skill2FuncList.Items.Add(new SkillListSval(FuncArray[i],
                                lv1Array[i], lv6Array[i], lv10Array[i]));
                            SkillLvs.skill2forExcel += FuncArray[i] + " 【{" +
                                                       (lv1Array[i].Replace("\r\n", " ") ==
                                                        lv10Array[i].Replace("\r\n", " ")
                                                           ? lv10Array[i].Replace("\r\n", " ")
                                                           : lv1Array[i].Replace("\r\n", " ") + "} - {" +
                                                             lv10Array[i].Replace("\r\n", " ")) + "}】\r\n";
                            break;
                        case 3:
                            Skill3FuncList.Items.Add(new SkillListSval(FuncArray[i],
                                lv1Array[i], lv6Array[i], lv10Array[i]));
                            SkillLvs.skill3forExcel += FuncArray[i] + " 【{" +
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
            string result;
            JObject res;
            var Check = true;
            ToggleDeleteLastData.Dispatcher.Invoke(() =>
            {
                if (ToggleDeleteLastData.IsChecked == true) Check = !Check;
            });
            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = false; });
            updatedatabutton.Dispatcher.Invoke(() => { updatedatabutton.IsEnabled = false; });
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
            updatestatus.Dispatcher.Invoke(() => { updatesign.Text = "数据下载进行中,请勿退出!"; });
            progressbar.Dispatcher.Invoke(() =>
            {
                progressbar.Value = 0;
                progressbar.Visibility = Visibility.Visible;
            });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 250; });
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
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 250; });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 250; });
            try
            {
                result = HttpRequest.PhttpReq("https://game.fate-go.jp/gamedata/top", "appVer=2.17.0");
                res = JObject.Parse(result);
                if (res["response"][0]["fail"]["action"] != null)
                    switch (res["response"][0]["fail"]["action"].ToString())
                    {
                        case "app_version_up":
                        {
                            var tmp = res["response"][0]["fail"]["detail"].ToString();
                            tmp = Regex.Replace(tmp, @".*新ver.：(.*)、現.*", "$1", RegexOptions.Singleline);
                            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "当前游戏版本: " + tmp; });

                            result = HttpRequest.PhttpReq("https://game.fate-go.jp/gamedata/top",
                                "appVer=" + tmp);
                            res = JObject.Parse(result);
                            break;
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
                progressloading.Dispatcher.Invoke(() => { progressloading.Visibility = Visibility.Hidden; });
                Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
                return;
            }

            if (File.Exists(gamedata.FullName + "webview") || File.Exists(gamedata.FullName + "raw") ||
                File.Exists(gamedata.FullName + "assetbundle") || File.Exists(gamedata.FullName + "webview") ||
                File.Exists(gamedata.FullName + "master"))
            {
                var oldRaw = File.ReadAllText(gamedata.FullName + "raw");

                if (string.CompareOrdinal(oldRaw, result) == 0 && Check)
                {
                    Dispatcher.Invoke(() =>
                    {
                        GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                            Application.Current.MainWindow,
                            "当前的MasterData已是最新版本,无需重复下载.\r\n\r\n您确定要重新覆盖当前的MasterData数据吗?\r\n\r\n点击\"确认\"进行覆盖.",
                            "无需更新",
                            MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    });
                    if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.Cancel)
                    {
                        updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = ""; });
                        updatestatus.Dispatcher.Invoke(() => { updatesign.Text = ""; });
                        progressbar.Dispatcher.Invoke(() =>
                        {
                            progressbar.Visibility = Visibility.Hidden;
                            updatedatabutton.IsEnabled = true;
                        });
                        progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
                        progressloading.Dispatcher.Invoke(() => { progressloading.Visibility = Visibility.Hidden; });
                        Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                        OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
                        return;
                    }
                }
            }

            File.WriteAllText(gamedata.FullName + "raw", result);
            File.WriteAllText(gamedata.FullName + "assetbundle",
                res["response"][0]["success"]["assetbundle"].ToString());
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "assetbundle"; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 40; });
            File.WriteAllText(gamedata.FullName + "master",
                res["response"][0]["success"]["master"].ToString());
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "master"; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 40; });
            File.WriteAllText(gamedata.FullName + "webview",
                res["response"][0]["success"]["webview"].ToString());
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "webview"; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 40; });
            var data = File.ReadAllText(gamedata.FullName + "master");
            if (!Directory.Exists(gamedata.FullName + "decrypted_masterdata"))
                Directory.CreateDirectory(gamedata.FullName + "decrypted_masterdata");
            var masterData =
                (Dictionary<string, byte[]>) MasterDataUnpacker.MouseGame2Unpacker(
                    Convert.FromBase64String(data));
            var miniMessagePacker = new MiniMessagePacker();
            foreach (var item in masterData)
            {
                var DataCount = masterData.Count;
                var ProgressValue = (double) 9800 / DataCount;
                var unpackeditem = (List<object>) miniMessagePacker.Unpack(item.Value);
                var json = JsonConvert.SerializeObject(unpackeditem, Formatting.Indented);
                var t = "";
                try
                {
                    t = File.ReadAllText(gamedata.FullName + "decrypted_masterdata/" + item.Key);
                }
                catch (Exception)
                {
                    t = "";
                }

                if (string.CompareOrdinal(t, json) != 0)
                {
                    File.WriteAllText(gamedata.FullName + "decrypted_masterdata/" + item.Key, json);
                    updatestatus.Dispatcher.Invoke(() =>
                    {
                        updatestatus.Text = "写入: " + gamedata.FullName +
                                            "decrypted__masterdata\\" + item.Key;
                    });
                }
                else
                {
                    updatestatus.Dispatcher.Invoke(() =>
                    {
                        updatestatus.Text = "跳过: " + gamedata.FullName +
                                            "decrypted__masterdata\\" + item.Key;
                    });
                }

                progressbar.Dispatcher.Invoke(() => { progressbar.Value += ProgressValue; });
                progressring.Dispatcher.Invoke(() => { progressring.Value += ProgressValue; });
            }

            var data2 = File.ReadAllText(gamedata.FullName + "assetbundle");
            var dictionary =
                (Dictionary<string, object>) MasterDataUnpacker.MouseInfoMsgPack(
                    Convert.FromBase64String(data2));
            var str = dictionary.Aggregate<KeyValuePair<string, object>, string>(null,
                (current, a) => current + a.Key + ": " + a.Value + "\r\n");
            File.WriteAllText(gamedata.FullName + "assetbundle.txt", str);
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "folder name: " + dictionary["folderName"]; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 40; });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 40; });
            var data3 = File.ReadAllText(gamedata.FullName + "webview");
            var dictionary2 =
                (Dictionary<string, object>) MasterDataUnpacker.MouseGame2MsgPack(
                    Convert.FromBase64String(data3));
            var str2 = "baseURL: " + dictionary2["baseURL"] + "\r\n contactURL: " +
                       dictionary2["contactURL"] + "\r\n";
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = str2; });
            progressbar.Dispatcher.Invoke(() => { progressbar.Value += 40; });
            progressring.Dispatcher.Invoke(() => { progressring.Value += 40; });
            var filePassInfo = (Dictionary<string, object>) dictionary2["filePass"];
            str = filePassInfo.Aggregate(str, (current, a) => current + a.Key + ": " + a.Value + "\r\n");
            File.WriteAllText(gamedata.FullName + "webview.txt", str2);
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "写入: " + gamedata.FullName + "webview.txt"; });
            updatestatus.Dispatcher.Invoke(() => { updatestatus.Text = "正在更新数据..."; });
            await LoadorRenewCommonDatas.ReloadData();
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
            progressring.Dispatcher.Invoke(() => { progressring.Visibility = Visibility.Hidden; });
            progressloading.Dispatcher.Invoke(() => { progressloading.Visibility = Visibility.Hidden; });
            Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
            OutputIDs.Dispatcher.Invoke(() => { OutputIDs.IsEnabled = true; });
            GC.Collect();
        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var UpgradeMasterData = new Task(() => { HttpRequestData(); });
            UpgradeMasterData.Start();
        }

        private void JBOut()
        {
            var output = "";
            output = "文本1:\n\r" + JB.JB1 + "\n\r" +
                     "文本2:\n\r" + JB.JB2 + "\n\r" +
                     "文本3:\n\r" + JB.JB3 + "\n\r" +
                     "文本4:\n\r" + JB.JB4 + "\n\r" +
                     "文本5:\n\r" + JB.JB5 + "\n\r" +
                     "文本6:\n\r" + JB.JB6 + "\n\r" +
                     "文本7:\n\r" + JB.JB7;
            if (!Directory.Exists(GlobalPathsAndDatas.outputdir.FullName))
                Directory.CreateDirectory(GlobalPathsAndDatas.outputdir.FullName);
            File.WriteAllText(GlobalPathsAndDatas.outputdir.FullName + "羁绊文本_" + JB.svtid + "_" + JB.svtnme + ".txt",
                output);
            Dispatcher.Invoke(() =>
            {
                MessageBox.Success("导出完成.\n\r文件名为: " + GlobalPathsAndDatas.outputdir.FullName +
                                   "羁绊文本_" + JB.svtid + "_" + JB.svtnme +
                                   ".txt", "完成");
            });

            Process.Start(GlobalPathsAndDatas.outputdir.FullName + "/" + "羁绊文本_" + JB.svtid + "_" + JB.svtnme + ".txt");
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
            var DS = new Task(() => { DrawScale(); });
            VersionLabel.Dispatcher.Invoke(() => { VersionLabel.Text = CommonStrings.Version; });
            DS.Start();
            if (!Directory.Exists(gamedata.FullName))
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Info("没有游戏数据,请先下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:");
                    DataUpdate.IsSelected = true;
                });
                Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
            }
            else
            {
                try
                {
                    Dispatcher.Invoke(() => { Growl.Info("正在读取数据..."); });
                    await LoadorRenewCommonDatas.ReloadData();
                    Dispatcher.Invoke(() => { Growl.Info("读取完毕."); });
                    Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = true; });
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Info("游戏数据损坏,请重新下载游戏数据(位于\"数据更新\"选项卡).", "温馨提示:");
                        DataUpdate.IsSelected = true;
                    });
                    Button1.Dispatcher.Invoke(() => { Button1.IsEnabled = false; });
                }
            }
        }

        private void Form_Load(object sender, EventArgs eventArgs)
        {
            var Loading = new Task(() => { LoadingProgress(); });
            Loading.Start();
        }

        private void ExcelFileOutput()
        {
            try
            {
                var path = Directory.GetCurrentDirectory();
                var svtData = new DirectoryInfo(path + @"\ServantData\");
                if (!Directory.Exists(svtData.FullName))
                    Directory.CreateDirectory(svtData.FullName);
                var streamget = HttpRequest.GetXlsx();
                var xlsx =
                    new ExcelPackage(streamget);
                var worksheet = xlsx.Workbook.Worksheets[0];
                worksheet.Cells["H3"].Value = JB.svtid;
                worksheet.Cells["A1"].Value += "(" + JB.svtnme + ")";
                worksheet.Cells["B3"].Value = Svtname.Text;
                worksheet.Cells["B4"].Value = svtclass.Text;
                worksheet.Cells["E4"].Value = rarity.Text;
                worksheet.Cells["H4"].Value = gendle.Text;
                worksheet.Cells["L4"].Value = hiddenattri.Text;
                worksheet.Cells["K3"].Value = collection.Text;
                worksheet.Cells["B5"].Value = cv.Text;
                worksheet.Cells["H5"].Value = illust.Text;
                worksheet.Cells["B6"].Value = ssvtstarrate.Text;
                worksheet.Cells["E6"].Value = ssvtdeathrate.Text;
                worksheet.Cells["I6"].Value = jixing.Text;
                worksheet.Cells["L6"].Value = notrealnprate.Text;
                worksheet.Cells["C7"].Value = nprate.Text;
                worksheet.Cells["C10"].Value = classskill.Text;
                worksheet.Cells["C9"].Value = basichp.Text;
                worksheet.Cells["F9"].Value = basicatk.Text;
                worksheet.Cells["I9"].Value = maxhp.Text;
                worksheet.Cells["L9"].Value = maxatk.Text;
                worksheet.Cells["C13"].Value = cards.Text;
                worksheet.Cells["D15"].Value = bustercard.Text;
                worksheet.Cells["I15"].Value = artscard.Text;
                worksheet.Cells["D16"].Value = quickcard.Text;
                worksheet.Cells["I16"].Value = extracard.Text;
                worksheet.Cells["E17"].Value = treasuredevicescard.Text;
                worksheet.Cells["C18"].Value = npcardtype.Text;
                worksheet.Cells["G18"].Value = nptype.Text;
                worksheet.Cells["J18"].Value = nprank.Text;
                worksheet.Cells["C19"].Value = npruby.Text;
                worksheet.Cells["C20"].Value = npname.Text;
                worksheet.Cells["C21"].Value = npdetail.Text;
                worksheet.Cells["P1"].Value = skill1name.Text;
                worksheet.Cells["T1"].Value = skill1cdlv1.Text;
                worksheet.Cells["V1"].Value = skill1cdlv6.Text;
                worksheet.Cells["X1"].Value = skill1cdlv10.Text;
                worksheet.Cells["P2"].Value = skill1details.Text;
                worksheet.Cells["P10"].Value = skill2name.Text;
                worksheet.Cells["T10"].Value = skill2cdlv1.Text;
                worksheet.Cells["V10"].Value = skill2cdlv6.Text;
                worksheet.Cells["X10"].Value = skill2cdlv10.Text;
                worksheet.Cells["P11"].Value = skill2details.Text;
                worksheet.Cells["P19"].Value = skill3name.Text;
                worksheet.Cells["T19"].Value = skill3cdlv1.Text;
                worksheet.Cells["V19"].Value = skill3cdlv6.Text;
                worksheet.Cells["X19"].Value = skill3cdlv10.Text;
                worksheet.Cells["P20"].Value = skill3details.Text;
                worksheet.Cells["C28"].Value = svtIndividuality.Text;
                worksheet.Cells["C12"].Value = Convert.ToString(sixwei.Text) + "    " +
                                               hpatkbalance.Text;
                worksheet.Cells["P6"].Value = SkillLvs.skill1forExcel;
                worksheet.Cells["P15"].Value = SkillLvs.skill2forExcel;
                worksheet.Cells["P24"].Value = SkillLvs.skill3forExcel;
                worksheet.Cells["C24"].Value = SkillLvs.TDforExcel;
                xlsx.SaveAs(new FileInfo(svtData.FullName + JB.svtnme + "_" + JB.svtid + ".xlsx"));
                streamget.Close();
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
                    VerAssetsJArray = (JArray) JsonConvert.DeserializeObject(VerChk["assets"].ToString());
                    for (var i = 0; i <= VerAssetsJArray.Count - 1; i++)
                        if (VerAssetsJArray[i]["name"].ToString() == "FGOSBIAReloaded.exe")
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
                DownloadFile(GlobalPathsAndDatas.ExeUpdateUrl, path + "/FGOSBIAReloaded(Update " + VerChk + ").exe");
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
                    "下载完成.下载目录为: \r\n" + path + "\\FGOSBIAReloaded(Update " +
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
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstQuest") &&
                File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstQuestPickup"))
            {
                foreach (var mstQuestPickup in GlobalPathsAndDatas.mstQuestPickupArray)
                {
                    var QuestPUEndTimeStamp = Convert.ToInt32(((JObject) mstQuestPickup)["endedAt"]);
                    var QuestPUStartTimeStamp = Convert.ToInt32(((JObject) mstQuestPickup)["startedAt"]);
                    var TimeMinus = (DateTime.Now.Ticks - dateTimeStart.Ticks) / 10000000;
                    if (TimeMinus > QuestPUEndTimeStamp) continue;
                    questid = ((JObject) mstQuestPickup)["questId"].ToString();
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
                if (((JObject) mstQuest)["id"].ToString() != questid) continue;
                var CharaID = ((JObject) mstQuest)["charaIconId"].ToString();
                try
                {
                    CharaID = CharaID.Substring(0, CharaID.Length - 3) + "00";
                }
                catch (Exception)
                {
                    //ignore
                }

                var TempName = ((JObject) mstQuest)["name"].ToString();
                if (TempName.Length > 14) TempName = TempName.Insert(14, "\r\n");
                QuestName = "\r\n" + TempName + "\r\n\r\nAP消耗: " + ((JObject) mstQuest)["actConsume"] + "   等级推荐: lv." +
                            ((JObject) mstQuest)["recommendLv"] + "\r\n从者: " + CharaID + " - " +
                            GetSvtName(CharaID, 1) + "\r\n";
                if (((JObject) mstQuest)["giftId"].ToString() != "0")
                {
                    var giftid = ((JObject) mstQuest)["giftId"].ToString();
                    QuestName += "任务奖励: " + CheckGiftNames(giftid) + "\r\n";
                }
                else
                {
                    var itemid = ((JObject) mstQuest)["giftIconId"].ToString();
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
                if (((JObject) mstGifttmp)["id"].ToString() != giftid) continue;
                giftids += ((JObject) mstGifttmp)["objectId"] + ",";
                giftquantities += ((JObject) mstGifttmp)["num"] + ",";
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
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstClass") &&
                File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstClassRelation"))
            {
                foreach (var mstClasstmp in GlobalPathsAndDatas.mstClassArray)
                {
                    var ClassName = "";
                    var WeakClass = "\r\n";
                    var ResistClass = "\r\n";
                    ClassName = ((JObject) mstClasstmp)["name"] + "(" + ((JObject) mstClasstmp)["id"] + ")";
                    var tmpid = ((JObject) mstClasstmp)["id"].ToString();
                    foreach (var mstClassRelationtmp in GlobalPathsAndDatas.mstClassRelationArray)
                    {
                        if (((JObject) mstClassRelationtmp)["atkClass"].ToString() != tmpid) continue;
                        var ATKRATE = Convert.ToInt64(((JObject) mstClassRelationtmp)["attackRate"].ToString());
                        if (ATKRATE > 1000)
                        {
                            var tmpweakid = ((JObject) mstClassRelationtmp)["defClass"].ToString();
                            WeakClass +=
                                (GetClassName(tmpweakid) == "？"
                                    ? GetClassName(tmpweakid) + "(ID:" + tmpweakid + ")"
                                    : GetClassName(tmpweakid)) + " (" + (float) ATKRATE / 1000 + "x)\r\n";
                        }
                        else if (ATKRATE < 1000)
                        {
                            var tmpresistid = ((JObject) mstClassRelationtmp)["defClass"].ToString();
                            ResistClass +=
                                (GetClassName(tmpresistid) == "？"
                                    ? GetClassName(tmpresistid) + "(ID:" + tmpresistid + ")"
                                    : GetClassName(tmpresistid)) + "(" + (float) ATKRATE / 1000 + "x)\r\n";
                        }
                    }

                    WeakClass = WeakClass.Substring(0, WeakClass.Length - 2) + "\r\n";
                    ResistClass = ResistClass.Substring(0, ResistClass.Length - 2) + "\r\n";
                    ClassList.Dispatcher.Invoke(() =>
                    {
                        ClassList.Items.Add(new ClassRelationList(ClassName, WeakClass, ResistClass));
                    });
                }

                ButtonClass.Dispatcher.Invoke(() => { ButtonClass.IsEnabled = true; });
                Dispatcher.Invoke(() => { Growl.Info("显示完毕."); });
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
            foreach (var mstClasstmp in GlobalPathsAndDatas.mstClassArray)
            {
                if (((JObject) mstClasstmp)["id"].ToString() != id) continue;
                ClassName = ((JObject) mstClasstmp)["name"].ToString();
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
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstEvent"))
            {
                foreach (var mstEventtmp in GlobalPathsAndDatas.mstEventArray)
                {
                    var EventEndTimeStamp = Convert.ToInt32(((JObject) mstEventtmp)["endedAt"]);
                    var TimeMinus = (DateTime.Now.Ticks - dateTimeStart.Ticks) / 10000000;
                    EventName = ((JObject) mstEventtmp)["name"].ToString();
                    if (EventName.Length > 40) EventName = EventName.Insert(40, "\r\n");
                    Eventid = ((JObject) mstEventtmp)["id"].ToString();
                    var EventEndTime = new TimeSpan(long.Parse(EventEndTimeStamp + "0000000"));
                    var EndStr = Convert.ToString(dateTimeStart + EventEndTime);
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
                            PickupEndedEventList.Items.Add(new EventList(Eventid, EventName, EndStr));
                        });
                    else
                        PickupEventList.Dispatcher.Invoke(() =>
                        {
                            PickupEventList.Items.Add(new EventList(Eventid, EventName, EndStr));
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
            if (File.Exists(gamedata.FullName + "decrypted_masterdata/" + "mstGacha"))
            {
                foreach (var mstGachatmp in GlobalPathsAndDatas.mstGachaArray)
                {
                    var EventEndTimeStamp = Convert.ToInt32(((JObject) mstGachatmp)["closedAt"]);
                    var TimeMinus = (DateTime.Now.Ticks - dateTimeStart.Ticks) / 10000000;
                    EventName = ((JObject) mstGachatmp)["name"].ToString();
                    if (EventName.Length > 40) EventName = EventName.Insert(40, "\r\n");
                    Eventid = ((JObject) mstGachatmp)["id"].ToString();
                    var EventEndTime = new TimeSpan(long.Parse(EventEndTimeStamp + "0000000"));
                    var EndStr = Convert.ToString(dateTimeStart + EventEndTime);
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
                            PickupEndedGachaList.Items.Add(new EventList(Eventid, EventName, EndStr));
                        });
                    else
                        PickupEventList.Dispatcher.Invoke(() =>
                        {
                            PickupGachaList.Items.Add(new EventList(Eventid, EventName, EndStr));
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
            Dispatcher.Invoke(() =>
            {
                var endurancebase = new double[100];
                endurancebase[11] = 1.02;
                endurancebase[12] = 1.025;
                endurancebase[13] = 1.03;
                endurancebase[14] = 1.015;
                endurancebase[15] = 1.035;
                endurancebase[21] = 1;
                endurancebase[22] = 1.005;
                endurancebase[23] = 1.01;
                endurancebase[24] = 0.995;
                endurancebase[25] = 1.015;
                endurancebase[31] = 0.99;
                endurancebase[32] = 0.9925;
                endurancebase[33] = 0.995;
                endurancebase[34] = 0.985;
                endurancebase[35] = 0.9975;
                endurancebase[41] = 0.98;
                endurancebase[42] = 0.9825;
                endurancebase[43] = 0.985;
                endurancebase[44] = 0.975;
                endurancebase[45] = 0.9875;
                endurancebase[51] = 0.97;
                endurancebase[52] = 0.9725;
                endurancebase[53] = 0.975;
                endurancebase[54] = 0.965;
                endurancebase[55] = 0.9775;
                endurancebase[61] = 1.04;
                endurancebase[0] = 0.0;
                endurancebase[99] = 0.0;
                endurancebase[98] = 0.0;
                endurancebase[97] = 0.0;
                var HPBasicWithRarity = new double[6];
                HPBasicWithRarity[0] = 1600;
                HPBasicWithRarity[1] = 1500;
                HPBasicWithRarity[2] = 1600;
                HPBasicWithRarity[3] = 1800;
                HPBasicWithRarity[4] = 2000;
                HPBasicWithRarity[5] = 2200;
                var ClassBasicBase = new double[30];
                ClassBasicBase[1] = 1.01;
                ClassBasicBase[2] = 0.98;
                ClassBasicBase[3] = 1.02;
                ClassBasicBase[4] = 0.96;
                ClassBasicBase[5] = 0.98;
                ClassBasicBase[6] = 0.95;
                ClassBasicBase[7] = 0.90;
                ClassBasicBase[8] = 1.01;
                ClassBasicBase[9] = 1.00;
                ClassBasicBase[10] = 0.95;
                ClassBasicBase[11] = 0.88;
                ClassBasicBase[17] = 0.98;
                ClassBasicBase[23] = 1.05;
                ClassBasicBase[25] = 1.00;
                var ShowString = new string[8];
                ShowString[1] = "( 攻防倾向: 全HP )";
                ShowString[2] = "( 攻防倾向: 偏HP )";
                ShowString[3] = "( 攻防倾向: 均衡 )";
                ShowString[4] = "( 攻防倾向: 偏ATK )";
                ShowString[5] = "( 攻防倾向: 全ATK )";
                ShowString[6] = "( 攻防倾向: 特殊 )";
                ShowString[7] = "( 攻防倾向: - )";
                double resultHPBaseCheck;
                if (ClassID != "1" && ClassID != "2" && ClassID != "3" && ClassID != "4" && ClassID != "5" &&
                    ClassID != "6" && ClassID != "7" && ClassID != "8" && ClassID != "9" && ClassID != "10" &&
                    ClassID != "11" && ClassID != "17" && ClassID != "23" && ClassID != "25")
                {
                    hpatkbalance.Text = ShowString[7];
                    return;
                }

                if (svtID == "100300")
                {
                    hpatkbalance.Text = ShowString[6];
                }
                else
                {
                    var inserthp = Convert.ToDouble(basichp);
                    resultHPBaseCheck = inserthp / (HPBasicWithRarity[Convert.ToInt64(rarity)] *
                                                    endurancebase[Convert.ToInt64(endurance)] *
                                                    ClassBasicBase[Convert.ToInt64(ClassID)]);
                    if (Math.Abs(resultHPBaseCheck - 1.10) <= 0.005)
                        hpatkbalance.Text = ShowString[1];
                    else if (Math.Abs(resultHPBaseCheck - 1.05) <= 0.005)
                        hpatkbalance.Text = ShowString[2];
                    else if (Math.Abs(resultHPBaseCheck - 1.00) <= 0.005)
                        hpatkbalance.Text = ShowString[3];
                    else if (Math.Abs(resultHPBaseCheck - 0.95) <= 0.005)
                        hpatkbalance.Text = ShowString[4];
                    else if (Math.Abs(resultHPBaseCheck - 0.90) <= 0.005)
                        hpatkbalance.Text = ShowString[5];
                    else
                        hpatkbalance.Text = ShowString[7];
                }
            });
        }

        private void AddChart(int[] Array)
        {
            Dispatcher.Invoke(() =>
            {
                if (Array == null) throw new ArgumentNullException(nameof(Array));
                var xmin = 0.0;
                var xmax = 100.0;
                var ymin = 0.0;
                var ymax = 0.0;
                var AdjustHPCurve = new int[101];
                var AdjustATKCurve = new int[101];
                for (var lv = 0; lv < 101; lv++)
                {
                    AdjustHPCurve[lv] = GlobalPathsAndDatas.basichp +
                                        Array[lv] * (GlobalPathsAndDatas.maxhp - GlobalPathsAndDatas.basichp) / 1000;
                    AdjustATKCurve[lv] = GlobalPathsAndDatas.basicatk +
                                         Array[lv] * (GlobalPathsAndDatas.maxatk - GlobalPathsAndDatas.basicatk) / 1000;
                    if (lv == 0) continue;
                    HpAtkListView.Items.Add(new HpAtkList(lv.ToString(), AdjustHPCurve[lv].ToString(),
                        AdjustATKCurve[lv].ToString()));
                }

                chartCanvas.Children.Remove(plhp);
                chartCanvas.Children.Remove(platk);
                ymin = 0.0;
                ymax = Math.Max(AdjustATKCurve[100], AdjustHPCurve[100]);
                // Draw ATK curve:
                platk = new Polyline {Stroke = Brushes.Red, StrokeThickness = 2};
                for (var i = 1; i < 101; i++)
                {
                    double x = i;
                    double y = AdjustATKCurve[i];
                    var atklinep = CurvePoint(new Point(x, y), xmin, xmax, ymin, ymax);
                    platk.Points.Add(atklinep);
                }

                chartCanvas.Children.Add(platk);
                // Draw HP curve:
                plhp = new Polyline {Stroke = Brushes.Blue, StrokeThickness = 2};
                for (var i = 1; i < 101; i++)
                {
                    double x = i;
                    double y = AdjustHPCurve[i];
                    var hplinep = CurvePoint(new Point(x, y), xmin, xmax, ymin, ymax);
                    plhp.Points.Add(hplinep);
                }

                chartCanvas.Children.Add(plhp);
            });
        }

        private Point CurvePoint(Point pt, double xmin, double xmax, double ymin, double ymax)
        {
            var result = new Point
            {
                X = (pt.X - xmin) * chartCanvas.Width * 0.95 / (xmax - xmin),
                Y = chartCanvas.Height - (pt.Y - ymin) * chartCanvas.Height * 0.80
                    / (ymax - ymin)
            };
            return result;
        }

        private void DrawScale()
        {
            for (var i = 0; i < 101; i++)
            {
                var x_scale = new Line
                {
                    StrokeEndLineCap = PenLineCap.Triangle,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    X1 = 0 + i * 4.5
                };

                x_scale.X2 = x_scale.X1;

                x_scale.Y1 = 352;
                if (i % 5 == 0 && i != 0)
                {
                    x_scale.StrokeThickness = 2;
                    x_scale.Y2 = x_scale.Y1 - 8;
                }
                else
                {
                    x_scale.Y2 = x_scale.Y1 - 4;
                }

                Dispatcher.Invoke(() => { chartCanvas.Children.Add(x_scale); });
            }
        }

        private static int[] GetSvtCurveData(object TypeID)
        {
            var TempData = new int[101];
            foreach (var mstSvtExptmp in GlobalPathsAndDatas.mstSvtExpArray)
            {
                if (((JObject) mstSvtExptmp)["type"].ToString() != TypeID.ToString()) continue;
                TempData[Convert.ToInt32(((JObject) mstSvtExptmp)["lv"])] =
                    Convert.ToInt32(((JObject) mstSvtExptmp)["curve"].ToString());
            }

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
            var TempSplit1 = HttpRequest.GetList(IndividualListLinkA, IndividualListLinkB).Replace("\r\n", "")
                .Split('|');
            var IndividualityCommons = new string[TempSplit1.Length][];
            for (var i = 0; i < TempSplit1.Length; i++)
            {
                var TempSplit2 = TempSplit1[i].Split('+');
                IndividualityCommons[i] = new string[TempSplit2.Length];
                for (var j = 0; j < TempSplit2.Length; j++) IndividualityCommons[i][j] = TempSplit2[j];
            }

            var Outputs = "";
            foreach (var Cases in IndividualityStringArray)
            {
                if (Cases.Length >= 6) continue;
                if (Cases == "5010" || Cases == "5000") continue;
                for (var k = 0; k < IndividualityCommons.Length; k++)
                {
                    if (Cases == IndividualityCommons[k][0])
                    {
                        Outputs += IndividualityCommons[k][1] + ",";
                        break;
                    }

                    if (k == IndividualityCommons.Length - 1 && Cases != IndividualityCommons[k][0])
                        Outputs += "未知特性(" + Cases + "),";
                }
            }

            Outputs = Outputs.Substring(0, Outputs.Length - 1);
            svtIndividuality.Dispatcher.Invoke(() => { svtIndividuality.Text = Outputs; });
        }

        private void EasternEggSvt()
        {
            Dispatcher.Invoke(() =>
            {
                Svtname.Text = "ACPudding";
                SvtBattlename.Text = "ACPudding";
                svtIndividuality.Text = "男性,Foreigner,人之力,秩序,善,从者,人形,被EA特攻,现今生存的人类,人科从者";
                svtclass.Text = "Foreigner";
                rarity.Text = "5 ☆";
                gendle.Text = "男性";
                hiddenattri.Text = "人";
                cv.Text = "-";
                illust.Text = "-";
                ssvtstarrate.Text = "14.9%";
                ssvtdeathrate.Text = "6.8%";
                jixing.Text = "153";
                notrealnprate.Text = "0.57%";
                nprate.Text = "Quick: " + "0.77%" + "   Arts: " +
                              "0.77%" + "   Buster: " +
                              "0.77%" + "\r\nExtra: " +
                              "0.77%" + "   宝具: " + "0.77%" +
                              "   受击: " + "4.00%";
                classskill.Text = "单独行动(自宅) EX, 阵地建造(异) B+, 领域外生命 C, 道具作成(Code) B";
                basichp.Text = "2132";
                basicatk.Text = "1789";
                maxhp.Text = "14535";
                maxatk.Text = "11593";
                sixwei.Text = "筋力: " + "B+" + "    耐久: " + "A" +
                              "    敏捷: " +
                              "C+" +
                              "    魔力: " + "B+" + "    幸运: " + "A" +
                              "    宝具: " +
                              "C";
                cards.Text = "[Q,Q,A,A,B]";
                bustercard.Text = "4 hit [10,20,30,40]";
                artscard.Text = "3 hit [16,33,51]";
                quickcard.Text = "5 hit [6,13,20,26,35]";
                extracard.Text = "4 hit [10,20,30,40]";
                treasuredevicescard.Text = "4 hit [10,20,30,40]";
                npcardtype.Text = "Arts";
                nptype.Text = "全体宝具";
                nprank.Text = "C (对人宝具)";
                npruby.Text = "トゥシタ·ヘブンズフィールド (Tushita Heaven's Field)";
                npname.Text = "兜率天·極楽曼荼羅";
                npdetail.Text =
                    "解除敌方全体的攻击强化状态 + 自身攻击力与防御力提升(3回合)<Over Charge时效果提升> + 对敌方全体发动强大的无视防御力攻击[lv.1-lv.5] + 自身NP获得量下降(1回合)【负面效果】";
                skill1name.Text = "迷乱代码 B";
                skill1details.Text =
                    "解除敌方全体的防御强化状态 + 当位于〔屋内〕场景时赋予防御力大幅下降的状态(3回合)[lv.1-lv.10] + 自身的Arts指令卡性能提升(5次・3回合)[lv.1-lv.10]";
                skill1cdlv1.Text = "7";
                skill1cdlv6.Text = "6";
                skill1cdlv10.Text = "5";
                Skill1FuncList.Items.Add(new SkillListSval("防御强化解除", "1000", "1000", "1000"));
                Skill1FuncList.Items.Add(new SkillListSval("防御力下降", "1000,3,-1,400", "1000,3,-1,500", "1000,3,-1,600"));
                Skill1FuncList.Items.Add(new SkillListSval("Arts性能提升", "1000,3,5,200", "1000,3,5,300", "1000,3,5,400"));
                skill2name.Text = "危险代码注入 EX";
                skill2details.Text = "己方单体的NP增加[lv.1-lv.10] + 宝具威力超绝提升(1次·1回合)[lv.1-lv.10] + 付与〔1回合后必定即死〕效果";
                skill2cdlv1.Text = "12";
                skill2cdlv6.Text = "11";
                skill2cdlv10.Text = "10";
                Skill2FuncList.Items.Add(new SkillListSval("NP增加", "1000,8000", "1000,9000", "1000,10000"));
                Skill2FuncList.Items.Add(new SkillListSval("宝具威力提升", "1000,1,1,600", "1000,1,1,700", "1000,1,1,800"));
                Skill2FuncList.Items.Add(new SkillListSval("〔1回合后必定即死〕效果", "1000,3,-1", "1000,3,-1", "1000,3,-1"));
                skill3name.Text = "共享安乐 B+";
                skill3details.Text =
                    "敌方全体攻击力下降(3回合)[lv.1-lv.10] + 除自身以外的己方攻击力下降(3回合)【负面效果】+ 己方全体强化解除耐性提升(1次·3回合)[lv.1-lv.10] + 己方全体每回合最大HP提升状态(3回合)[lv.1-lv.10] + 自身暴击威力提升(3次·3回合)[lv.1-lv.10] + 己方随机单体获得弱体无效状态(1次·3回合)";
                skill3cdlv1.Text = "8";
                skill3cdlv6.Text = "7";
                skill3cdlv10.Text = "6";
                Skill3FuncList.Items.Add(new SkillListSval("攻击力下降", "1000,3,-1,100", "1000,3,-1,200", "1000,3,-1,300"));
                Skill3FuncList.Items.Add(new SkillListSval("攻击力下降", "1000,3,-1,300", "1000,3,-1,300", "1000,3,-1,300"));
                Skill3FuncList.Items.Add(new SkillListSval("强化解除耐性提升", "1000,3,1,600", "1000,3,1,800",
                    "1000,3,1,1000"));
                Skill3FuncList.Items.Add(new SkillListSval("最大HP提升", "1000,3,-1,1000", "1000,3,-1,1500",
                    "1000,3,-1,2000"));
                Skill3FuncList.Items.Add(new SkillListSval("暴击威力提升", "1000,3,3,200", "1000,3,3,250", "1000,3,3,300"));
                Skill3FuncList.Items.Add(new SkillListSval("弱化无效状态", "1000,3,1", "1000,3,1", "1000,3,1"));
                TDFuncList.Items.Add(new TDlistSval("攻击强化解除", "1000", "1000", "1000", "1000", "1000"));
                TDFuncList.Items.Add(new TDlistSval("攻击力提升", "1000,3,-1,100", "1000,3,-1,150", "1000,3,-1,200",
                    "1000,3,-1,250", "1000,3,-1,300"));
                TDFuncList.Items.Add(new TDlistSval("防御力提升", "1000,3,-1,100", "1000,3,-1,150", "1000,3,-1,200",
                    "1000,3,-1,250", "1000,3,-1,300"));
                TDFuncList.Items.Add(new TDlistSval("防御无视攻击", "1000,6000", "1000,7500", "1000,8250", "1000,8625",
                    "1000,9000"));
                TDFuncList.Items.Add(new TDlistSval("NP获得量下降", "1000,1,-1,400", "1000,1,-1,400", "1000,1,-1,400",
                    "1000,1,-1,400", "1000,1,-1,400"));
                Dispatcher.Invoke(() => { Growl.Info("注意,您触发了彩蛋,此处显示的为软件作者自己捏造的自制从者.并非游戏内实际内容!"); });
                jibantext1.Text = "一名初学C#的辣鸡,这个是我随便瞎诌的自设orz.\r\n有很多地方根本就不合理233.";
                jibantext2.Text = "〇 迷乱代码 B：\r\n因为自己写的代码不仅自己看着麻烦给我朋友看也觉得极其繁琐和复杂.";
                jibantext3.Text = "〇 危险代码注入 EX：\r\n为了规避掉一些奇怪的问题就写了很多奇怪的代码(我自己也看不太懂,直接百度233).";
                jibantext4.Text = "〇 共享安乐 B+：\r\n这个就没啥好解释了((((\r\n胡诌的技能.";
                jibantext5.Text = "兜率天·極楽曼荼羅 (トゥシタ·ヘブンズフィールド):\r\n自己比较喜欢杀生院的宝具名,于是查了Wiki随便按样式编了一个名字.(好中二啊orz)";
                jibantext6.Text = "为什么选择Foreigner是因为感觉逼格比较高(WinForm版程序的一设是Caster).\r\n或许可以给自己设置一个3破改变宝具名称x.";
                jibantext7.Text = "希望有缘人能够重写本烂程序.\r\n                           ---作者记";
            });
        }

        private void Button_Click_Event(object sender, RoutedEventArgs e)
        {
            ButtonEvent.IsEnabled = false;
            var LEL = new Task(LoadEventList);
            LEL.Start();
        }

        private async void Button_DecryptBinFile(object sender, RoutedEventArgs e)
        {
            var inputdialog = new CommonOpenFileDialog {Title = "选择cpk文件"};
            var resultinput = inputdialog.ShowDialog();
            var inputfile = "";
            if (resultinput == CommonFileDialogResult.Ok) inputfile = inputdialog.FileName;
            if (inputfile == "") return;
            var file = new FileInfo(inputfile);
            var outputfolder = file.DirectoryName;
            var output = new DirectoryInfo(outputfolder);
            ButtonCpkSelect.IsEnabled = false;
            ButtonBinSelectFolder.IsEnabled = false;
            await Task.Run(async () => { await DecryptCpkFile(file, output); });
            ButtonCpkSelect.IsEnabled = true;
            ButtonBinSelectFolder.IsEnabled = true;
            Thread.Sleep(1000);
            decryptstatus.Content = "";
        }

        private async Task DecryptCpkFile(FileInfo file, DirectoryInfo outputfolder)
        {
            var RemindLog = "解包音频: " + file.Name;
            Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
            var acbFilename = "";
            await Task.Run(() =>
            {
                try
                {
                    acbFilename = FGOAudioDecoder.UnpackCpkFiles(file, outputfolder);
                }
                catch (Exception)
                {
                    //ignore
                }
            });
            if (acbFilename == "")
            {
                Dispatcher.Invoke(() =>
                {
                    decryptstatus.Content = "该文件不是cpk类型文件.请重试.";
                    MessageBox.Error("该文件不是cpk类型文件.", "错误");
                });
                return;
            }

            await Task.Run(() => { FGOAudioDecoder.DecodeAcbFiles(new FileInfo(acbFilename), outputfolder); });
            Dispatcher.Invoke(() => { decryptstatus.Content = "完成."; });
            Thread.Sleep(1500);
            Process.Start(outputfolder.FullName + @"\DecodedWavs");
        }

        private async void Button_DecryptBinFolder(object sender, RoutedEventArgs e)
        {
            var inputdialog = new CommonOpenFileDialog {IsFolderPicker = true, Title = "需要解密的资源文件目录"};
            var resultinput = inputdialog.ShowDialog();
            var inputfolder = "";
            if (resultinput == CommonFileDialogResult.Ok) inputfolder = inputdialog.FileName;
            if (inputfolder == "") return;
            var outputfolder = inputfolder + @"\DecryptedResources";
            var isDeleteFile = false;
            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                Application.Current.MainWindow,
                "是否删除源文件夹内的所有文件?(全部移动至输出文件夹)",
                "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                isDeleteFile = true;
            var input = new DirectoryInfo(inputfolder);
            var output = new DirectoryInfo(outputfolder);
            ButtonBinSelectFolder.IsEnabled = false;
            ButtonCpkSelect.IsEnabled = false;
            decryptprogress.Visibility = Visibility.Visible;
            decryptprogress.Value = 0;
            await Task.Run(async () => { await DecryptBinFileFolderAsync(input, output, isDeleteFile); });
            ButtonBinSelectFolder.IsEnabled = true;
            ButtonCpkSelect.IsEnabled = true;
            decryptstatus.Content = "";
            decryptprogress.Visibility = Visibility.Hidden;
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
                    assetName = tmp[tmp.Length - 1].Replace('/', '@');
                    fileName = CatAndMouseGame.GetMD5String(assetName);
                    AudioArray.Add(new JObject(new JProperty("audioName", assetName),
                        new JProperty("fileName", fileName)));
                }
                else if (tmp[4].Contains("Movie"))
                {
                    assetName = tmp[tmp.Length - 1].Replace('/', '@');
                    fileName = CatAndMouseGame.GetMD5String(assetName);
                    MovieArray.Add(new JObject(new JProperty("movieName", assetName),
                        new JProperty("fileName", fileName)));
                }
                else if (!tmp[4].Contains("Movie"))
                {
                    assetName = tmp[tmp.Length - 1].Replace('/', '@') + ".unity3d";
                    fileName = CatAndMouseGame.GetShaName(assetName);
                    AssetArray.Add(new JObject(new JProperty("assetName", assetName),
                        new JProperty("fileName", fileName)));
                }
            }

            var serializerSettings = new JsonSerializerSettings();
            var AudioArrayConverter = JsonConvert.SerializeObject(AudioArray, serializerSettings);
            var MovieArrayConverter = JsonConvert.SerializeObject(MovieArray, serializerSettings);
            var AssetArrayConverter = JsonConvert.SerializeObject(AssetArray, serializerSettings);
            File.WriteAllText(decrypt.FullName + @"\AudioName.json", AudioArrayConverter);
            File.WriteAllText(decrypt.FullName + @"\MovieName.json", MovieArrayConverter);
            File.WriteAllText(decrypt.FullName + @"\AssetName.json", AssetArrayConverter);
        }

        private string GetSvtName(string svtID, int Option)
        {
            var svtName = "";
            foreach (var svtIDtmp in GlobalPathsAndDatas.mstSvtArray)
            {
                if (((JObject) svtIDtmp)["id"].ToString() != svtID) continue;
                var mstSvtobjtmp = JObject.Parse(svtIDtmp.ToString());
                svtName = Option == 1 ? mstSvtobjtmp["battleName"].ToString() : mstSvtobjtmp["name"].ToString();
                return svtName;
            }

            return "???";
        }

        private async Task DecryptBinFileFolderAsync(DirectoryInfo inputdest, DirectoryInfo outputdest,
            bool isDeleteFile)
        {
            var folder = inputdest;
            var decrypt = outputdest;
            if (!Directory.Exists(decrypt.FullName))
                Directory.CreateDirectory(decrypt.FullName);
            var renamedAudio = new DirectoryInfo(outputdest.FullName + @"\Audio\");
            var renamedMovie = new DirectoryInfo(outputdest.FullName + @"\Movie\");
            var renamedAssets = new DirectoryInfo(outputdest.FullName + @"\Assets\");
            byte[] raw;
            byte[] output;
            if (File.Exists(folder.FullName + @"\cfb1d36393fd67385e046b084b7cf7ed"))
            {
                if (File.Exists(decrypt.FullName + @"\AssetStorage.txt"))
                    File.Delete(decrypt.FullName + @"\AssetStorage.txt");
                if (isDeleteFile)
                    File.Move(folder.FullName + @"\cfb1d36393fd67385e046b084b7cf7ed",
                        decrypt.FullName + @"\AssetStorage.txt");
                else
                    File.Copy(folder.FullName + @"\cfb1d36393fd67385e046b084b7cf7ed",
                        decrypt.FullName + @"\AssetStorage.txt");
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Error(
                        "AssetStorage.txt文件不存在\r\n请检查输入文件夹内是否存在\"cfb1d36393fd67385e046b084b7cf7ed\"\r\n或者\"AssetStorage.txt\"文件.",
                        "错误");
                });
                return;
            }

            if (File.Exists(folder.FullName + @"\4fb2705e743f2eed610a17b9eaba5541"))
            {
                if (File.Exists(decrypt.FullName + @"\AssetStorageBack.txt"))
                    File.Delete(decrypt.FullName + @"\AssetStorageBack.txt");
                if (isDeleteFile)
                    File.Move(folder.FullName + @"\4fb2705e743f2eed610a17b9eaba5541",
                        decrypt.FullName + @"\AssetStorageBack.txt");
                else
                    File.Copy(folder.FullName + @"\4fb2705e743f2eed610a17b9eaba5541",
                        decrypt.FullName + @"\AssetStorageBack.txt");
            }

            var data = File.ReadAllText(decrypt.FullName + @"\AssetStorage.txt");
            var loadData = CatAndMouseGame.MouseGame8(data);
            File.WriteAllText(decrypt.FullName + @"\AssetStorage_dec.txt", loadData);
            var RemindLog = "写入: " + decrypt.FullName + @"\AssetStorage_dec.txt";
            Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
            var assetStore = File.ReadAllLines(decrypt.FullName + @"\AssetStorage_dec.txt");
            var Sub1 = new Task(() => { DecryptBinfileSub(assetStore, decrypt); });
            Sub1.Start();
            Thread.Sleep(1500);
            Dispatcher.Invoke(() => { decryptstatus.Content = "开始解密bin."; });
            var fileCountbin = Directory.GetFiles(folder.FullName, "*.bin").Length;
            var fileCountall = Directory.GetFiles(folder.FullName).Length;
            var progressValuebin = Convert.ToDouble(100000 / fileCountbin);
            var progressValueall = Convert.ToDouble(100000 / fileCountall);
            Parallel.ForEach(folder.GetFiles("*.bin"), file =>
            {
                try
                {
                    RemindLog = "解密: " + file.FullName;
                    Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
                    raw = File.ReadAllBytes(file.FullName);
                    output = CatAndMouseGame.MouseGame4(raw);
                    if (!Directory.Exists(renamedAssets.FullName))
                        Directory.CreateDirectory(renamedAssets.FullName);
                    File.WriteAllBytes(renamedAssets.FullName + @"\" + file.Name, output);
                    Dispatcher.Invoke(() => { decryptprogress.Value += progressValuebin; });
                    if (isDeleteFile)
                        File.Delete(file.FullName);
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() => { MessageBox.Error("解密时遇到错误.\r\n", "错误"); });
                }
            });
            Dispatcher.Invoke(() =>
            {
                decryptprogress.Value = decryptprogress.Maximum;
                decryptstatus.Content = "解密完成.现在开始重命名所有资源文件.\r\n读取json中...";
            });
            Thread.Sleep(1500);
            Task.WaitAll(Sub1);
            Dispatcher.Invoke(() => { decryptprogress.Value = 0; });
            var AssetJsonName = File.ReadAllText(decrypt.FullName + @"\AssetName.json");
            var AssetJsonNameArray = (JArray) JsonConvert.DeserializeObject(AssetJsonName);
            Parallel.ForEach(renamedAssets.GetFiles("*.bin"), file =>
            {
                Parallel.ForEach(AssetJsonNameArray, FileNametmp =>
                {
                    if (((JObject) FileNametmp)["fileName"].ToString() != file.Name) return;
                    var FileNameObjtmp = JObject.Parse(FileNametmp.ToString());
                    var FileAssetNametmp = FileNameObjtmp["assetName"].ToString();
                    RemindLog = "重命名: " + file.Name + " → \r\n" + FileAssetNametmp + "\n";
                    Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
                    if (File.Exists(renamedAssets.FullName + @"\" + FileAssetNametmp))
                        File.Delete(renamedAssets.FullName + @"\" + FileAssetNametmp);
                    File.Move(renamedAssets.FullName + @"\" + file.Name,
                        renamedAssets.FullName + @"\" + FileAssetNametmp);
                    Dispatcher.Invoke(() => { decryptprogress.Value += progressValueall; });
                    Thread.Sleep(10);
                });
            });
            var AudioAssetJsonName = File.ReadAllText(decrypt.FullName + @"\AudioName.json");
            var AudioAssetJsonNameArray = (JArray) JsonConvert.DeserializeObject(AudioAssetJsonName);
            Parallel.ForEach(folder.GetFiles("*."), file =>
            {
                Parallel.ForEach(AudioAssetJsonNameArray, FileNametmp2 =>
                {
                    if (((JObject) FileNametmp2)["fileName"].ToString() != file.Name) return;
                    var FileNameObjtmp2 = JObject.Parse(FileNametmp2.ToString());
                    var FileAssetNametmp2 = FileNameObjtmp2["audioName"].ToString();
                    RemindLog = "重命名: " + file.Name + " → \r\n" + FileAssetNametmp2 + "\n";
                    Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
                    if (!Directory.Exists(renamedAudio.FullName))
                        Directory.CreateDirectory(renamedAudio.FullName);
                    if (File.Exists(renamedAudio.FullName + @"\" + FileAssetNametmp2))
                        File.Delete(renamedAudio.FullName + @"\" + FileAssetNametmp2);
                    if (isDeleteFile)
                        File.Move(folder.FullName + @"\" + file.Name, renamedAudio.FullName + @"\" + FileAssetNametmp2);
                    else
                        File.Copy(folder.FullName + @"\" + file.Name, renamedAudio.FullName + @"\" + FileAssetNametmp2);
                    Dispatcher.Invoke(() => { decryptprogress.Value += progressValueall; });
                    Thread.Sleep(10);
                });
            });
            var MovieAssetJsonName = File.ReadAllText(decrypt.FullName + @"\MovieName.json");
            var MovieAssetJsonNameArray = (JArray) JsonConvert.DeserializeObject(MovieAssetJsonName);
            Parallel.ForEach(folder.GetFiles("*."), file =>
            {
                Parallel.ForEach(MovieAssetJsonNameArray, FileNametmp3 =>
                {
                    if (((JObject) FileNametmp3)["fileName"].ToString() != file.Name) return;
                    var FileNameObjtmp3 = JObject.Parse(FileNametmp3.ToString());
                    var FileAssetNametmp3 = FileNameObjtmp3["movieName"].ToString();
                    RemindLog = "重命名: " + file.Name + " → \r\n" + FileAssetNametmp3 + "\n";
                    Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
                    if (!Directory.Exists(renamedMovie.FullName))
                        Directory.CreateDirectory(renamedMovie.FullName);
                    if (File.Exists(renamedMovie.FullName + @"\" + FileAssetNametmp3))
                        File.Delete(renamedMovie.FullName + @"\" + FileAssetNametmp3);
                    if (isDeleteFile)
                        File.Move(folder.FullName + @"\" + file.Name, renamedMovie.FullName + @"\" + FileAssetNametmp3);
                    else
                        File.Copy(folder.FullName + @"\" + file.Name, renamedMovie.FullName + @"\" + FileAssetNametmp3);
                    Dispatcher.Invoke(() => { decryptprogress.Value += progressValueall; });
                    Thread.Sleep(10);
                });
            });
            if (Directory.Exists(renamedAudio.FullName))
                try
                {
                    var AudioFileQuantityCheck = Directory.GetFiles(renamedAudio.FullName).Length;
                    var acbFilename = "";
                    if (AudioFileQuantityCheck > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            GlobalPathsAndDatas.SuperMsgBoxRes = MessageBox.Show(
                                Application.Current.MainWindow,
                                "解密完成,是否需要尝试解包音频文件?",
                                "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                        });
                        if (GlobalPathsAndDatas.SuperMsgBoxRes == MessageBoxResult.OK)
                        {
                            Dispatcher.Invoke(() => { decryptprogress.Value = 0; });
                            var AudioDecodeStatusVision =
                                (double) 100000 / Directory.GetFiles(renamedAudio.FullName).Length;
                            foreach (var file in renamedAudio.GetFiles("*.cpk.bytes"))
                            {
                                RemindLog = "解包音频: " + file.Name;
                                Dispatcher.Invoke(() => { decryptstatus.Content = RemindLog; });
                                await Task.Run(() =>
                                {
                                    acbFilename = FGOAudioDecoder.UnpackCpkFiles(file, renamedAudio);
                                });
                                await Task.Run(() =>
                                {
                                    FGOAudioDecoder.DecodeAcbFiles(new FileInfo(acbFilename), renamedAudio);
                                });
                                Dispatcher.Invoke(() => { decryptprogress.Value += AudioDecodeStatusVision; });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => { MessageBox.Show("解密时遇到错误.\r\n" + ex, "错误"); });
                }

            Dispatcher.Invoke(() =>
            {
                decryptprogress.Value = decryptprogress.Maximum;
                decryptstatus.Content = "完成.";
            });
            Thread.Sleep(2000);
            Process.Start(decrypt.FullName);
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
                if (((JObject) svtFiltertmp)["name"].ToString() != "次回イベント対象") continue;
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

        private struct SkillListSval
        {
            public string SkillName { get; }
            public string SkillSvallv1 { get; }
            public string SkillSvallv6 { get; }
            public string SkillSvallv10 { get; }

            public SkillListSval(string v1, string v2, string v3, string v4) : this()
            {
                SkillName = v1;
                SkillSvallv1 = v2;
                SkillSvallv6 = v3;
                SkillSvallv10 = v4;
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

        private struct TDlistSval
        {
            public string TDFuncName { get; }
            public string TDSvalOC1lv1 { get; }
            public string TDSvalOC2lv2 { get; }
            public string TDSvalOC3lv3 { get; }
            public string TDSvalOC4lv4 { get; }
            public string TDSvalOC5lv5 { get; }

            public TDlistSval(string v1, string v2, string v3, string v4, string v5, string v6) : this()
            {
                TDFuncName = v1;
                TDSvalOC1lv1 = v2;
                TDSvalOC2lv2 = v3;
                TDSvalOC3lv3 = v4;
                TDSvalOC4lv4 = v5;
                TDSvalOC5lv5 = v6;
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
            public string WeakClass { get; }
            public string ResistClass { get; }

            public ClassRelationList(string v1, string v2, string v3) : this()
            {
                ClassName = v1;
                WeakClass = v2;
                ResistClass = v3;
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