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
            await Task.Run(() =>
            {
                var mstSvt =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvt.json");
                var mstSvtLimit =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtLimit.json");
                var mstCv =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstCv.json");
                var mstIllustrator =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstIllustrator.json");
                var mstSvtCard =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSvtCard.json");
                var mstSvtTreasureDevice =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtTreasureDevice.json");
                var mstTreasureDevice =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstTreasureDevice.json");
                var mstTreasureDeviceDetail =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstTreasureDeviceDetail.json");
                var mstSkill =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSkill.json");
                var mstSvtSkill =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtSkill.json");
                var mstSkillDetail =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSkillDetail.json");
                var mstFunc =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstFunc.json");
                var mstTreasureDeviceLv =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstTreasureDeviceLv.json");
                var mstSvtComment =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtComment.json");
                var mstCombineLimit =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstCombineLimit.json");
                var mstCombineSkill =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstCombineSkill.json");
                var mstSkillLv =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstSkillLv.json");
                var mstQuest =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstQuest.json");
                var mstItem =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstItem.json");
                var mstQuestPickup =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstQuestPickup.json");
                var npcSvtFollower =
                    File.ReadAllText(
                        GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "npcSvtFollower.json");
                var mstEvent =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstEvent.json");
                var mstClass =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstClass.json");
                var mstClassRelation =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstClassRelation.json");
                var mstGift =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstGift.json");
                var mstSvtExp =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtExp.json");
                var mstGacha =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" + "mstGacha.json");
                var mstSvtFilter =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtFilter.json");
                var mstSvtAppendPassiveSkill =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtAppendPassiveSkill.json");
                var mstSvtLimitAdd =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtLimitAdd.json");
                var mstSvtIndividuality =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtIndividuality.json");
                var mstBuff =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstBuff.json");
                var mstSkillAdd =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSkillAdd.json");
                var mstSvtPassiveSkill =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstSvtPassiveSkill.json");
                var mstCombineAppendPassiveSkill =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "decrypted_masterdata/" +
                                     "mstCombineAppendPassiveSkill.json");
                var funcListDebugger =
                    File.ReadAllText(GlobalPathsAndDatas.gamedata.FullName + "FuncList.json");
                GlobalPathsAndDatas.funcListDebuggerArray =
                    (JArray)JsonConvert.DeserializeObject(funcListDebugger);
                GlobalPathsAndDatas.mstCombineAppendPassiveSkillArray =
                    (JArray)JsonConvert.DeserializeObject(mstCombineAppendPassiveSkill);
                GlobalPathsAndDatas.mstSvtPassiveSkillArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtPassiveSkill);
                GlobalPathsAndDatas.mstSkillAddArray =
                    (JArray)JsonConvert.DeserializeObject(mstSkillAdd);
                GlobalPathsAndDatas.mstBuffArray =
                    (JArray)JsonConvert.DeserializeObject(mstBuff);
                GlobalPathsAndDatas.mstSvtLimitAddArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtLimitAdd);
                GlobalPathsAndDatas.mstSvtIndividualityArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtIndividuality);
                GlobalPathsAndDatas.mstSvtAppendPassiveSkillArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtAppendPassiveSkill);
                GlobalPathsAndDatas.mstSvtFilterArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtFilter);
                GlobalPathsAndDatas.mstSvtExpArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtExp);
                GlobalPathsAndDatas.mstGachaArray =
                    (JArray)JsonConvert.DeserializeObject(mstGacha);
                GlobalPathsAndDatas.mstGiftArray =
                    (JArray)JsonConvert.DeserializeObject(mstGift);
                GlobalPathsAndDatas.mstClassArray =
                    (JArray)JsonConvert.DeserializeObject(mstClass);
                GlobalPathsAndDatas.mstClassRelationArray =
                    (JArray)JsonConvert.DeserializeObject(mstClassRelation);
                GlobalPathsAndDatas.mstEventArray =
                    (JArray)JsonConvert.DeserializeObject(mstEvent);
                GlobalPathsAndDatas.npcSvtFollowerArray =
                    (JArray)JsonConvert.DeserializeObject(npcSvtFollower);
                GlobalPathsAndDatas.mstQuestArray = (JArray)JsonConvert.DeserializeObject(mstQuest);
                GlobalPathsAndDatas.mstItemArray = (JArray)JsonConvert.DeserializeObject(mstItem);
                GlobalPathsAndDatas.mstQuestPickupArray =
                    (JArray)JsonConvert.DeserializeObject(mstQuestPickup);
                GlobalPathsAndDatas.mstCombineLimitArray =
                    (JArray)JsonConvert.DeserializeObject(mstCombineLimit);
                GlobalPathsAndDatas.mstCombineSkillArray =
                    (JArray)JsonConvert.DeserializeObject(mstCombineSkill);
                GlobalPathsAndDatas.mstSkillLvArray =
                    (JArray)JsonConvert.DeserializeObject(mstSkillLv);
                GlobalPathsAndDatas.mstSvtCommentArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtComment);
                GlobalPathsAndDatas.mstSvtArray = (JArray)JsonConvert.DeserializeObject(mstSvt);
                GlobalPathsAndDatas.mstSvtLimitArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtLimit);
                GlobalPathsAndDatas.mstCvArray = (JArray)JsonConvert.DeserializeObject(mstCv);
                GlobalPathsAndDatas.mstIllustratorArray =
                    (JArray)JsonConvert.DeserializeObject(mstIllustrator);
                GlobalPathsAndDatas.mstSvtCardArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtCard);
                GlobalPathsAndDatas.mstSvtTreasureDevicedArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtTreasureDevice);
                GlobalPathsAndDatas.mstTreasureDevicedArray =
                    (JArray)JsonConvert.DeserializeObject(mstTreasureDevice);
                GlobalPathsAndDatas.mstTreasureDeviceDetailArray =
                    (JArray)JsonConvert.DeserializeObject(mstTreasureDeviceDetail);
                GlobalPathsAndDatas.mstSkillArray = (JArray)JsonConvert.DeserializeObject(mstSkill);
                GlobalPathsAndDatas.mstSvtSkillArray =
                    (JArray)JsonConvert.DeserializeObject(mstSvtSkill);
                GlobalPathsAndDatas.mstSkillDetailArray =
                    (JArray)JsonConvert.DeserializeObject(mstSkillDetail);
                GlobalPathsAndDatas.mstFuncArray = (JArray)JsonConvert.DeserializeObject(mstFunc);
                GlobalPathsAndDatas.mstTreasureDeviceLvArray =
                    (JArray)JsonConvert.DeserializeObject(mstTreasureDeviceLv);
                GlobalPathsAndDatas.svtIDListStr = "";
                foreach (var SvtItem in GlobalPathsAndDatas.mstSvtArray)
                    switch (((JObject)SvtItem)["id"].ToString().Length)
                    {
                        case 6:
                        case 7 when ((JObject)SvtItem)["id"].ToString().Substring(0, 1) != "9" ||
                                    ((JObject)SvtItem)["id"].ToString().Substring(0, 2) == "99":
                            GlobalPathsAndDatas.svtIDListStr +=
                                ((JObject)SvtItem)["id"] + "%" + ((JObject)SvtItem)["name"] + "|";
                            break;
                    }
            });
            GC.Collect();
        }

        public static async Task ReloadGivenData(string name, string data)
        {
            await Task.Run(() =>
            {
                switch (name)
                {
                    case "mstCombineAppendPassiveSkill":
                        GlobalPathsAndDatas.mstCombineAppendPassiveSkillArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtPassiveSkill":
                        GlobalPathsAndDatas.mstSvtPassiveSkillArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstBuff":
                        GlobalPathsAndDatas.mstBuffArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSkillAdd":
                        GlobalPathsAndDatas.mstSkillAddArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtIndividuality":
                        GlobalPathsAndDatas.mstSvtIndividualityArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtLimitAdd":
                        GlobalPathsAndDatas.mstSvtLimitAddArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtAppendPassiveSkill":
                        GlobalPathsAndDatas.mstSvtAppendPassiveSkillArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtFilter":
                        GlobalPathsAndDatas.mstSvtFilterArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtExp":
                        GlobalPathsAndDatas.mstSvtExpArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstGacha":
                        GlobalPathsAndDatas.mstGachaArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstGift":
                        GlobalPathsAndDatas.mstGiftArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstClass":
                        GlobalPathsAndDatas.mstClassArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstClassRelation":
                        GlobalPathsAndDatas.mstClassRelationArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstEvent":
                        GlobalPathsAndDatas.mstEventArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "npcSvtFollower":
                        GlobalPathsAndDatas.npcSvtFollowerArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstQuest":
                        GlobalPathsAndDatas.mstQuestArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstItem":
                        GlobalPathsAndDatas.mstItemArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstQuestPickup":
                        GlobalPathsAndDatas.mstQuestPickupArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstCombineLimit":
                        GlobalPathsAndDatas.mstCombineLimitArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstCombineSkill":
                        GlobalPathsAndDatas.mstCombineSkillArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSkillLv":
                        GlobalPathsAndDatas.mstSkillLvArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtComment":
                        GlobalPathsAndDatas.mstSvtCommentArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvt":
                        GlobalPathsAndDatas.mstSvtArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        GlobalPathsAndDatas.svtIDListStr = "";
                        foreach (var SvtItem in GlobalPathsAndDatas.mstSvtArray)
                            switch (((JObject)SvtItem)["id"].ToString().Length)
                            {
                                case 6:
                                case 7 when ((JObject)SvtItem)["id"].ToString().Substring(0, 1) != "9" ||
                                            ((JObject)SvtItem)["id"].ToString().Substring(0, 2) == "99":
                                    GlobalPathsAndDatas.svtIDListStr +=
                                        ((JObject)SvtItem)["id"] + "%" + ((JObject)SvtItem)["name"] + "|";
                                    break;
                            }

                        break;
                    case "mstSvtLimit":
                        GlobalPathsAndDatas.mstSvtLimitArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstCv":
                        GlobalPathsAndDatas.mstCvArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstIllustrator":
                        GlobalPathsAndDatas.mstIllustratorArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtCard":
                        GlobalPathsAndDatas.mstSvtCardArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtTreasureDevice":
                        GlobalPathsAndDatas.mstSvtTreasureDevicedArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstTreasureDevice":
                        GlobalPathsAndDatas.mstTreasureDevicedArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstTreasureDeviceDetail":
                        GlobalPathsAndDatas.mstTreasureDeviceDetailArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSkill":
                        GlobalPathsAndDatas.mstSkillArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSvtSkill":
                        GlobalPathsAndDatas.mstSvtSkillArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstSkillDetail":
                        GlobalPathsAndDatas.mstSkillDetailArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstFunc":
                        GlobalPathsAndDatas.mstFuncArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                    case "mstTreasureDeviceLv":
                        GlobalPathsAndDatas.mstTreasureDeviceLvArray =
                            (JArray)JsonConvert.DeserializeObject(data);
                        break;
                }
            });
        }
    }
}