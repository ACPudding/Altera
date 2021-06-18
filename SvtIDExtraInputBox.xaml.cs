using System;
using System.Windows.Input;
using Altera.Properties;
using HandyControl.Controls;

namespace Altera
{
    /// <summary>
    ///     SvtIDExtraInputBox.xaml 的交互逻辑
    /// </summary>
    public partial class SvtIDExtraInputBox
    {
        public string svtidreturnS;

        public SvtIDExtraInputBox()
        {
            InitializeComponent();
        }

        public string svtidreturn => svtidreturnS;

        private void Load(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() => { SvtList.Items.Clear(); });
            try
            {
                var svtList = GlobalPathsAndDatas.svtIDListStr.Split('|');
                for (var i = 0; i <= svtList.Length - 1; i++)
                {
                    var tmp = svtList[i].Split('%');
                    try
                    {
                        var ListAddValue = new SvtGetList(tmp[0], tmp[1]);
                        Dispatcher.Invoke(() => { SvtList.Items.Add(ListAddValue); });
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Error("加载从者列表失败，请确定游戏数据是否完整.\r\n\r\n" + ex, "错误");
                    Close();
                });
            }
        }

        private void Double_Click(object sender, MouseButtonEventArgs e)
        {
            if (SvtList.SelectedItem == null) return;
            var GetSelectedItem = (SvtGetList) SvtList.SelectedItem;
            svtidreturnS = GetSelectedItem.SVTID;
            Close();
        }

        private struct SvtGetList
        {
            public string SVTID { get; }
            public string SVTNME { get; }

            public SvtGetList(string v1, string v2) : this()
            {
                SVTID = v1;
                SVTNME = v2;
            }
        }
    }
}