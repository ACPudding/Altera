using System;
using System.IO;
using System.Threading.Tasks;
using Altera.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Altera
{
    internal class LoadorRenewCommonDatas
    {
        public static async Task ReloadData()
        {
            var mstSvt =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvt");
            var mstSvtLimit =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtLimit");
            var mstCv =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstCv");
            var mstIllustrator =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstIllustrator");
            var mstSvtCard =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtCard");
            var mstSvtTreasureDevice =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                 "mstSvtTreasureDevice");
            var mstTreasureDevice =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstTreasureDevice");
            var mstTreasureDeviceDetail =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                 "mstTreasureDeviceDetail");
            var mstSkill =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSkill");
            var mstSvtSkill =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtSkill");
            var mstSkillDetail =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSkillDetail");
            var mstFunc =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstFunc");
            var mstTreasureDeviceLv =
                File.ReadAllText(
                    GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstTreasureDeviceLv");
            var mstSvtComment =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtComment");
            var mstCombineLimit =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstCombineLimit");
            var mstCombineSkill =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstCombineSkill");
            var mstSkillLv =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSkillLv");
            var mstQuest =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstQuest");
            var mstItem =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstItem");
            var mstQuestPickup =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstQuestPickup");
            var npcSvtFollower =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "npcSvtFollower");
            var mstEvent =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstEvent");
            var mstClass =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstClass");
            var mstClassRelation =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstClassRelation");
            var mstGift =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstGift");
            var mstSvtExp =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtExp");
            var mstGacha =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstGacha");
            var mstSvtFilter =
                File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtFilter");
            GlobalPathsAndDatas.mstSvtFilterArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtFilter);
            GlobalPathsAndDatas.mstSvtExpArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtExp);
            GlobalPathsAndDatas.mstGachaArray =
                (JArray) JsonConvert.DeserializeObject(mstGacha);
            GlobalPathsAndDatas.mstGiftArray =
                (JArray) JsonConvert.DeserializeObject(mstGift);
            GlobalPathsAndDatas.mstClassArray =
                (JArray) JsonConvert.DeserializeObject(mstClass);
            GlobalPathsAndDatas.mstClassRelationArray =
                (JArray) JsonConvert.DeserializeObject(mstClassRelation);
            GlobalPathsAndDatas.mstEventArray =
                (JArray) JsonConvert.DeserializeObject(mstEvent);
            GlobalPathsAndDatas.npcSvtFollowerArray =
                (JArray) JsonConvert.DeserializeObject(npcSvtFollower);
            GlobalPathsAndDatas.mstQuestArray = (JArray) JsonConvert.DeserializeObject(mstQuest);
            GlobalPathsAndDatas.mstItemArray = (JArray) JsonConvert.DeserializeObject(mstItem);
            GlobalPathsAndDatas.mstQuestPickupArray =
                (JArray) JsonConvert.DeserializeObject(mstQuestPickup);
            GlobalPathsAndDatas.mstCombineLimitArray =
                (JArray) JsonConvert.DeserializeObject(mstCombineLimit);
            GlobalPathsAndDatas.mstCombineSkillArray =
                (JArray) JsonConvert.DeserializeObject(mstCombineSkill);
            GlobalPathsAndDatas.mstSkillLvArray =
                (JArray) JsonConvert.DeserializeObject(mstSkillLv);
            GlobalPathsAndDatas.mstSvtCommentArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtComment);
            GlobalPathsAndDatas.mstSvtArray = (JArray) JsonConvert.DeserializeObject(mstSvt);
            GlobalPathsAndDatas.mstSvtLimitArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtLimit);
            GlobalPathsAndDatas.mstCvArray = (JArray) JsonConvert.DeserializeObject(mstCv);
            GlobalPathsAndDatas.mstIllustratorArray =
                (JArray) JsonConvert.DeserializeObject(mstIllustrator);
            GlobalPathsAndDatas.mstSvtCardArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtCard);
            GlobalPathsAndDatas.mstSvtTreasureDevicedArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtTreasureDevice);
            GlobalPathsAndDatas.mstTreasureDevicedArray =
                (JArray) JsonConvert.DeserializeObject(mstTreasureDevice);
            GlobalPathsAndDatas.mstTreasureDeviceDetailArray =
                (JArray) JsonConvert.DeserializeObject(mstTreasureDeviceDetail);
            GlobalPathsAndDatas.mstSkillArray = (JArray) JsonConvert.DeserializeObject(mstSkill);
            GlobalPathsAndDatas.mstSvtSkillArray =
                (JArray) JsonConvert.DeserializeObject(mstSvtSkill);
            GlobalPathsAndDatas.mstSkillDetailArray =
                (JArray) JsonConvert.DeserializeObject(mstSkillDetail);
            GlobalPathsAndDatas.mstFuncArray = (JArray) JsonConvert.DeserializeObject(mstFunc);
            GlobalPathsAndDatas.mstTreasureDeviceLvArray =
                (JArray) JsonConvert.DeserializeObject(mstTreasureDeviceLv);
            GC.Collect();
        }
    }
}