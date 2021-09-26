using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Altera.Properties
{
    internal static class GlobalPathsAndDatas
    {
        public static string path = Directory.GetCurrentDirectory();
        public static DirectoryInfo gamedata = new DirectoryInfo(path + @"\Android\masterdata\");
        public static DirectoryInfo folder = new DirectoryInfo(path + @"\Android\");
        public static DirectoryInfo outputdir = new DirectoryInfo(path + @"\Output\");
        public static JArray mstSvtExpArray = null;
        public static JArray mstSvtFilterArray = null;
        public static JArray mstGiftArray = null;
        public static JArray mstClassRelationArray = null;
        public static JArray mstClassArray = null;
        public static JArray mstEventArray = null;
        public static JArray npcSvtFollowerArray = null;
        public static JArray mstQuestArray = null;
        public static JArray mstItemArray = null;
        public static JArray mstQuestPickupArray = null;
        public static JArray mstCombineLimitArray = null;
        public static JArray mstCombineSkillArray = null;
        public static JArray mstSkillLvArray = null;
        public static JArray mstSvtCommentArray = null;
        public static JArray mstSvtArray = null;
        public static JArray mstSvtLimitArray = null;
        public static JArray mstCvArray = null;
        public static JArray mstIllustratorArray = null;
        public static JArray mstSvtCardArray = null;
        public static JArray mstSvtTreasureDevicedArray = null;
        public static JArray mstTreasureDevicedArray = null;
        public static JArray mstTreasureDeviceDetailArray = null;
        public static JArray mstSkillArray = null;
        public static JArray mstSvtSkillArray = null;
        public static JArray mstSkillDetailArray = null;
        public static JArray mstFuncArray = null;
        public static JArray mstGachaArray = null;
        public static JArray mstTreasureDeviceLvArray = null;
        public static JArray mstSvtAppendPassiveSkillArray = null;
        public static int svtArtsCardhit;
        public static bool askxlsx = true;
        public static string ExeUpdateUrl;
        public static DateTime StartTime;
        public static string NewerVersion;
        public static MessageBoxResult SuperMsgBoxRes;
        public static string TranslationList;
        public static string[] TranslationListArray = null;
        public static int basichp;
        public static int basicatk;
        public static int maxhp;
        public static int maxatk;
        public static string CurveType;
        public static string svtIDListStr;
        public static string IDListStr;
        public static double notrealnprate;
        public static int LvExpCurveLvCount;
        public static int[] CurveBaseData;
        public static double ymax;
        public static int classid;
    }
}