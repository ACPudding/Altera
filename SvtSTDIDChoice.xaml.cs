using System;
using System.Windows.Input;
using Altera.Properties;
using HandyControl.Controls;
using Newtonsoft.Json.Linq;

namespace Altera
{
    /// <summary>
    ///     SvtIDExtraInputBox.xaml 的交互逻辑
    /// </summary>
    public partial class SvtSTDIDChoice
    {
        public string idreturnS;

        public SvtSTDIDChoice()
        {
            InitializeComponent();
        }

        public string idreturn => idreturnS;

        private void Load(object sender, EventArgs eventArgs)
        {
            var Nametmp = "";
            Dispatcher.Invoke(() => { IDList.Items.Clear(); });
            try
            {
                var idListS = GlobalPathsAndDatas.IDListStr.Split('^');
                var idList = idListS[0].Split('*');
                idreturnS = idList[0];
                switch (idListS[1])
                {
                    case "TD":
                        for (var i = 0; i <= idList.Length - 1; i++)
                            foreach (var TreasureDevicestmp in GlobalPathsAndDatas.mstTreasureDevicedArray)
                            {
                                if (((JObject) TreasureDevicestmp)["id"].ToString() != idList[i]) continue;
                                Nametmp = ((JObject) TreasureDevicestmp)["ruby"] + "\r\n" +
                                          ((JObject) TreasureDevicestmp)["name"];
                                try
                                {
                                    var ListAddValue = new SvtGetList(idList[i], "宝具", Nametmp);
                                    Dispatcher.Invoke(() => { IDList.Items.Add(ListAddValue); });
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                        break;
                    case "SK":
                        for (var i = 0; i <= idList.Length - 1; i++)
                            foreach (var skilltmp in GlobalPathsAndDatas.mstSkillArray)
                            {
                                if (((JObject) skilltmp)["id"].ToString() != idList[i]) continue;
                                Nametmp = ((JObject) skilltmp)["name"].ToString();
                                try
                                {
                                    var ListAddValue = new SvtGetList(idList[i], "技能", Nametmp);
                                    Dispatcher.Invoke(() => { IDList.Items.Add(ListAddValue); });
                                }
                                catch (Exception)
                                {
                                    //ignore
                                }
                            }

                        break;
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Error("加载列表失败，请确定游戏数据是否完整.\r\n\r\n" + ex, "错误");
                    Close();
                });
            }
        }

        private void Double_Click(object sender, MouseButtonEventArgs e)
        {
            if (IDList.SelectedItem == null) return;
            var GetSelectedItem = (SvtGetList) IDList.SelectedItem;
            idreturnS = GetSelectedItem.ID;
            Close();
        }

        private struct SvtGetList
        {
            public string ID { get; }
            public string TYPE { get; }
            public string NME { get; }

            public SvtGetList(string v1, string v2, string v3) : this()
            {
                ID = v1;
                TYPE = v2;
                NME = v3;
            }
        }
    }
}