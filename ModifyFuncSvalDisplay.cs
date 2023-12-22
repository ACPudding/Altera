using System;
using System.Collections.Generic;
using System.Linq;
using Altera.Properties;
using Newtonsoft.Json.Linq;

namespace Altera
{
    //对所有从者数值进行规范化显示(可能会有误显示的问题)
    internal class ModifyFuncSvalDisplay
    {
        public static string[] IndividualListStringTemp;

        public static string ModifyFuncStr(string Funcname, string Funcsval, bool ignoreEnemyFunc)
        {
            var output = Funcsval;
            string[] Tempsval;
            var ArrayNum = ReturnArrayNum(Funcname);
            switch (ArrayNum)
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
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 26:
                case 27:
                case 29:
                case 30:
                case 35:
                case 36:
                case 37:
                case 38:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 58:
                case 59:
                case 60:
                case 61:
                case 71:
                case 72:
                case 75:
                case 77:
                case 83:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 91:
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                case 101:
                case 102:
                case 103:
                case 108:
                case 113:
                case 114:
                case 117:
                case 118:
                case 124:
                case 125:
                case 126:
                case 127:
                case 128:
                case 132:
                case 133:
                case 134:
                case 139:
                case 140:
                case 141:
                case 149:
                case 163:
                case 165:
                case 169:
                case 170:
                case 171:
                case 173:
                case 174:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                    {
                        if ((Funcname.Contains("延长") || Funcname.Contains("延長")) && Tempsval[3].Contains("TargetList"))
                            try
                            {
                                output =
                                    $"延长{Tempsval[1]}回合\r\n作用目标:{SearchIndividualality(Tempsval[3].Replace("TargetList:", ""))}" +
                                    (Tempsval[0] == "1000"
                                        ? ""
                                        : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("ShowQuestNoEffect") ||
                            Tempsval[4].Contains("IncludePassiveIndividuality") ||
                            Tempsval[4].Contains("CheckDuplicate"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("RatioHPLow"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "% + \r\n" +
                                         Convert.ToDouble(Tempsval[4].Replace("RatioHPLow:", "")) / 10 +
                                         "% * (1 - HP / MaxHP)\r\n" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("IgnoreIndivUnreleaseable"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("UseRate"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "% (" +
                                         Convert.ToDouble(Tempsval[4].Replace("UseRate:", "")) / 10 + "%概率生效)" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("ShowState"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("StarHigher"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + " ( 暴击星 >= " +
                                         Tempsval[4].Replace("StarHigher:", "") + " )";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("ActSelectIndex"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + " (可选释放)";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("Hide"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("OnField"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[3].Length == 6 && Tempsval[3][0] == '9')
                        {
                            var Lv = "1";
                            if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                            var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                            output = "\r\n" + Clockval + " " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                    }
                    else if (Tempsval.Length > 5)
                    {
                        if (Tempsval.Length == 7)
                        {
                            if (Tempsval[4].Contains("ShowState") && (Tempsval[5].Contains("Individualty") ||
                                                                      Tempsval[6].Contains("Individualty")))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[4].Contains("ShowState") && (Tempsval[5].Contains("SameBuff") ||
                                                                      Tempsval[6].Contains("SameBuff")))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" + " * N | N ≤ " +
                                             Tempsval[6].Replace("SameBuffLimitNum:", "") + " |" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }
                        }

                        if (Tempsval.Length == 8)
                        {
                            if (Tempsval[4].Contains("Param"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "% + " +
                                             Convert.ToDouble(Tempsval[6].Replace("ParamAddValue:", "")) / 10 +
                                             "% * N " + " ( N ≤ " +
                                             Convert.ToInt32(Tempsval[5].Replace("ParamAddMaxCount:", "")) +
                                             " ) \r\n" + "关联Buff: " +
                                             Tempsval[4].Replace("ParamAddOpIndividuality:", "") + " \r\n" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[4].Contains("RatioHPLow") && Tempsval[5].Contains("RatioHPHigh"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[5].Replace("RatioHPHigh:", "")) / 10 +
                                             "% ~ " +
                                             Convert.ToDouble(Tempsval[4].Replace("RatioHPLow:", "")) / 10 +
                                             "%" + " (HP越少数值越高)" +
                                             "\r\n生效HP范围: " +
                                             Convert.ToDouble(Tempsval[6].Replace("RatioHPRangeLow:", "")) / 10 +
                                             "% ~ " +
                                             Convert.ToDouble(Tempsval[7].Replace("RatioHPRangeHigh:", "")) / 10 +
                                             "%\r\n" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }
                        }

                        if (Tempsval.Length == 6)
                        {
                            if (Tempsval[4].Contains("Hide") || Tempsval[4].Contains("ShowState"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[4].Contains("TriggeredFuncPosition") &&
                                Tempsval[5].Contains("DisplayLastFuncInvalidType"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[4].Contains("ParamAddSelfIndividuality"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "% + " +
                                             Convert.ToDouble(Tempsval[5].Replace("ParamAddValue:", "")) / 10 +
                                             "% * N \r\n" + "关联Buff: " +
                                             Tempsval[4].Replace("ParamAddSelfIndividuality:", "") + " \r\n" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[4].Contains("ParamAdd") && Tempsval[5].Contains("ParamMax"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" + " + " +
                                             Convert.ToDouble(Tempsval[4].Replace("ParamAdd:", "")) / 10 +
                                             "% * (T-1) " + "\r\n最大值:" +
                                             Convert.ToDouble(Tempsval[5].Replace("ParamMax:", "")) / 10 +
                                             "% \r\n注: T为持续第T回合\r\n" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[5].Contains("ShowState"))
                            {
                                if (Tempsval[3].Length == 6 && Tempsval[3][0] == '9')
                                {
                                    var Lv = "1";
                                    if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                                    var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                                    output = "\r\n" + Clockval + " " +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }

                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }
                            }

                            if (Tempsval[5].Contains("Hide") || Tempsval[5].Contains("OnField"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[4].Contains("Act") && Tempsval[5].Contains("Act"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + "\r\n动作Set:" +
                                             Tempsval[4].Replace("ActSet:", "") + " - 发动概率: " +
                                             Convert.ToInt32(Tempsval[5].Replace("ActSetWeight:", "")) + "%";
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }
                        }

                        if (Tempsval.Length == 9)
                        {
                            if (Tempsval[4].Contains("ShowState") && Tempsval[6].Contains("Linkage"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[7].Contains("HideMiss") && Tempsval[8].Contains("HideNoEffect"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }
                        }


                        var tmpstr = "";
                        for (var Q = 3; Q < Tempsval.Length; Q++) tmpstr += Tempsval[Q] + ",";
                        tmpstr = tmpstr.Substring(0, tmpstr.Length - 1);
                        output = "[" + tmpstr + "] " +
                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                     ? ""
                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                 (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                 (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        break;
                    }

                    output = Funcsval;
                    break;
                case 112:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 6)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3]) + "倍" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 161:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 5)
                        try
                        {
                            output = "指令卡变更: " +
                                     Tempsval[3].Replace("1", "Arts").Replace("2", "Buster").Replace("3", "Quick") +
                                     " (动作变更:" + Tempsval[4].Replace("MotionChange:", "") + ")" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 143:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4 || (Tempsval.Length == 5 && Tempsval[4].Contains("ShowState")))
                        try
                        {
                            output = "场地特性附加: " + Convert.ToDouble(Tempsval[3]) + "\r\n" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 172:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4 || (Tempsval.Length == 5 && Tempsval[4].Contains("ShowState")))
                        try
                        {
                            output = "场地特性打消: " + Tempsval[3].Replace("TargetList:", "") + "\r\n" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 31:
                case 32:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3]) / 100 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                    {
                        if (Tempsval[4].Contains("ShowQuestNoEffect"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) / 100 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        output = Funcsval;
                        break;
                    }

                    output = Funcsval;
                    break;
                case 55:
                case 56:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 2)
                        try
                        {
                            output = Tempsval[1] + "格" + (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Tempsval[1] + "格" + (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") + " 动作Set:" +
                                     Tempsval[2].Replace("ActSet:", "") + " - 发动概率: " +
                                     Convert.ToInt32(Tempsval[3].Replace("ActSetWeight:", "")) + "%";
                            ;
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 3 && Tempsval[2].Contains("IncludePassiveIndividuality"))
                        try
                        {
                            output = Tempsval[1] + "格" + (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 160:
                    Tempsval = Funcsval.Split(',');
                    if (Funcname.Contains("毎ターン確率で"))
                    {
                        if (Tempsval.Length > 4)
                        {
                            if (Tempsval[3].Length == 6 && Tempsval[3][0] == '9')
                            {
                                var Lv = "1";
                                if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                                var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                                if (Tempsval.Length >= 6)
                                    if (Tempsval[5].Contains("UseRate"))
                                    {
                                        output = "\r\n" + Clockval + " " +
                                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                     ? ""
                                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                                 (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                                 (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") +
                                                 " Buff成功率:" + (Tempsval[5].Replace("UseRate:", "") == "1000"
                                                     ? ""
                                                     : Convert.ToDouble(Tempsval[5].Replace("UseRate:", "")) / 10 +
                                                       "%");
                                        break;
                                    }

                                output = "\r\n" + Clockval + " " +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }

                            var tmpstr = "";
                            for (var Q = 3; Q < Tempsval.Length; Q++) tmpstr += Tempsval[Q] + ",";
                            tmpstr = tmpstr.Substring(0, tmpstr.Length - 1);
                            output = "[" + tmpstr + "] " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }

                        output = Funcsval;
                        break;
                    }

                    if (Tempsval.Length == 4)
                        try
                        {
                            var PossibleCount = (Tempsval[2] + "," + Tempsval[3]).Replace("DependFuncVals1:", "")
                                .Replace("]", "").Split(',');
                            output = "最大 " + PossibleCount[1] + " 格/单体" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                    {
                        if (!Funcsval.Contains("DependFunc"))
                        {
                            var tmpstr = "";
                            for (var Q = 3; Q < Tempsval.Length; Q++) tmpstr += Tempsval[Q] + ",";
                            tmpstr = tmpstr.Substring(0, tmpstr.Length - 1);
                            output = "[" + tmpstr + "] " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }

                        try
                        {
                            var PossibleCount = (Tempsval[2] + "," + Tempsval[3] + "," + Tempsval[4])
                                .Replace("DependFuncVals1:", "").Replace("]", "").Replace("Value2:", "").Split(',');
                            output = "减少敌方最大 " + Convert.ToDouble(PossibleCount[1]) / 100 + "% NP/单体, \r\n增加 " +
                                     Convert.ToDouble(PossibleCount[2]) / 100 + "格 充能(按成功数叠加)" + (Tempsval[0] == "1000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }
                    }

                    output = Funcsval;
                    break;
                case 69:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 3)
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 25:
                case 28:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Tempsval[3] + "HP" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                    {
                        if (Tempsval[4].Contains("ShowQuestNoEffect"))
                            try
                            {
                                output = Tempsval[3] + "HP" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("UseRate"))
                            try
                            {
                                output = Tempsval[3] + "HP" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") +
                                         " Buff成功率:" + (Tempsval[4].Replace("UseRate:", "") == "1000"
                                             ? ""
                                             : Convert.ToDouble(Tempsval[4].Replace("UseRate:", "")) / 10 + "%");
                                ;
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[3].Length == 6 && Tempsval[3][0] == '9')
                        {
                            var Lv = "1";
                            if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                            var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                            if (Tempsval.Length >= 6)
                                if (Tempsval[5].Contains("UseRate"))
                                {
                                    output = "\r\n" + Clockval + " " +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") +
                                             " Buff成功率:" + (Tempsval[5].Replace("UseRate:", "") == "1000"
                                                 ? ""
                                                 : Convert.ToDouble(Tempsval[5].Replace("UseRate:", "")) / 10 +
                                                   "%");
                                    break;
                                }

                            output = "\r\n" + Clockval + " " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }

                        output = Funcsval;
                        break;
                    }

                    output = Funcsval;
                    break;
                case 23:
                case 24:
                case 34:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Tempsval[3] +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                    {
                        if (Tempsval[4].Contains("ShowQuestNoEffect"))
                            try
                            {
                                output = Tempsval[3] +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        output = Funcsval;
                        break;
                    }

                    output = Funcsval;
                    break;
                case 74:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Tempsval[3] + "OC" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 153:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = "〔罗马〕(ID:" + Tempsval[3] + ")" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case 62:
                {
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 2)
                        try
                        {
                            output = $"减少{Tempsval[1]}回合" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 3)
                        try
                        {
                            output = $"减少{Tempsval[1]}回合 (技能{Tempsval[2]})" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                }
                case 33:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                    {
                        if (Tempsval[2].Contains("Act") && Tempsval[3].Contains("Act"))
                            try
                            {
                                output = Tempsval[1] + "个" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") + "\r\n动作Set:" +
                                         Tempsval[2].Replace("ActSet:", "") + " - 发动概率: " +
                                         Convert.ToInt32(Tempsval[3].Replace("ActSetWeight:", "")) + "%";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[2].Contains("TriggeredFuncPositionAll"))
                            try
                            {
                                output = Tempsval[1] + "个" +
                                         (Tempsval[0] == "1000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[3].Contains("MultipleGainStar"))
                            try
                            {
                                output = Tempsval[1] + "个/满足条件的对象" + (Tempsval[0] == "1000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        try
                        {
                            output = Tempsval[3] + "个" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }
                    }

                    if (Tempsval.Length == 2)
                        try
                        {
                            output = Tempsval[1] + "个" + (Tempsval[0] == "1000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 3)
                    {
                        if (Tempsval[2].Contains("MultipleGainStar"))
                            try
                            {
                                output = Tempsval[1] + "个/满足条件的对象" + (Tempsval[0] == "1000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        output = Funcsval;
                        break;
                    }

                    if (Tempsval.Length == 5)
                    {
                        if (Tempsval[4].Contains("ShowQuestNoEffect"))
                            try
                            {
                                output = Tempsval[3] + "个" +
                                         (Tempsval[0] == "1000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[4].Contains("UseRate"))
                            try
                            {
                                output = Tempsval[3] + "个 (" +
                                         Convert.ToDouble(Tempsval[4].Replace("UseRate:", "")) / 10 + "%概率生效)" +
                                         (Tempsval[0] == "1000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        output = Funcsval;
                        break;
                    }

                    if (Tempsval.Length == 6)
                        if (Tempsval[4].Contains("TriggeredFuncPosition"))
                            try
                            {
                                output = Tempsval[3] + "个" +
                                         (Tempsval[0] == "1000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                    output = Funcsval;
                    break;
                case 57:
                case 115:
                    Tempsval = Funcsval.Split(',');
                    switch (Tempsval.Length)
                    {
                        case 2:
                            try
                            {
                                output = Tempsval[1] + "个" + (Tempsval[0] == "1000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }
                        case 3:
                            if (Tempsval[2].Contains("MultipleGainStar"))
                                try
                                {
                                    output = Tempsval[1] + "个/满足条件的对象" +
                                             (Tempsval[0] == "1000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[2].Contains("ActSelectIndex"))
                                try
                                {
                                    output = Tempsval[1] + "个" + (Tempsval[0] == "1000"
                                        ? ""
                                        : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)" + " (可选释放)");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            output = Funcsval;
                            break;
                        case 4:
                            if (Tempsval[2].Contains("TriggeredFuncPositionAll"))
                                try
                                {
                                    output = Tempsval[1] + "个" +
                                             (Tempsval[0] == "1000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            if (Tempsval[3].Contains("MultipleGainStar"))
                                try
                                {
                                    output = Tempsval[1] + "个/满足条件的对象" +
                                             (Tempsval[0] == "1000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                            output = Funcsval;
                            break;
                        default:
                            output = Funcsval;
                            break;
                    }

                    break;
                case 164:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 8)
                        try
                        {
                            var CounterTDID = Tempsval[3].Replace("CounterId:", "");
                            var CounterTDLv = Tempsval[4].Replace("CounterLv:", "");
                            var result =
                                ServantTreasureDeviceSvalCheckForCounter(CounterTDID, CounterTDLv, ignoreEnemyFunc);
                            if (result != "false")
                            {
                                output = "\r\n<" + result + ">" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                ;
                            }
                            else
                            {
                                output = Funcsval;
                            }
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                    break;
                case 166:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 7)
                        try
                        {
                            output = "∅\r\n场景特性: " + Tempsval[3].Replace("FieldIndividuality:", "") +
                                     (Tempsval[0] == "1000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            ;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                    break;
                case 167:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = "∅\r\n充能减少: " + Tempsval[3] + "格" + (Tempsval[0] == "1000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            ;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                    break;
                case 168:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 3)
                        try
                        {
                            output = "∅" + (Tempsval[0] == "1000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                    break;
                case 175:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 5)
                        try
                        {
                            output =
                                $"延长{Tempsval[1]}回合\r\n作用目标:{SearchIndividualality(Tempsval[3].Replace("TargetList:", ""))}" +
                                (Tempsval[0] == "1000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                    break;
                case 176:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 5)
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                    break;
                default:
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 3)
                    {
                        if (Funcname.Contains("NP"))
                            if (Tempsval[2].Contains("IncludePassiveIndividuality") ||
                                Tempsval[2].Contains("ShowQuestNoEffect"))
                                try
                                {
                                    try
                                    {
                                        output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                     ? ""
                                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                    }
                                    catch (Exception)
                                    {
                                        output = Funcsval;
                                    }

                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }
                    }

                    if (Tempsval.Length == 2)
                        try
                        {
                            output = Tempsval[1] + (Tempsval[0] == "1000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                        if (Tempsval[1] == "205000")
                            try
                            {
                                output = "第" + Tempsval[3] + "再临\r\n宝具变化";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                    if (Tempsval.Length == 4)
                    {
                        if (Tempsval[1] == "304800")
                            try
                            {
                                output = "第" + Tempsval[3] + "再临\r\n宝具换装";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Tempsval[3].Contains("StarHigher"))
                            try
                            {
                                output = "∅" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + " ( 暴击星 >= " +
                                         Tempsval[3].Replace("StarHigher:", "") + " )";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Funcname.Contains("必中") && Tempsval[3].Contains("ShowQuestNoEffect"))
                            try
                            {
                                output = "∅" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Funcname.Contains("〔毒〕回復化") && Tempsval[3].Contains("HpReduce"))
                            try
                            {
                                output = "∅" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Funcname.Contains("必中") && Tempsval[3].Contains("ActSelectIndex"))
                            try
                            {
                                output = "∅" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + " (可选释放)";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        if (Funcname.Contains("回避") && Tempsval[3].Contains("ActSelectIndex"))
                            try
                            {
                                output = "∅" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + " (可选释放)";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                        try
                        {
                            output = "[" + Tempsval[3] + "] " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }
                    }

                    if (Tempsval.Length == 1)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[0]) / 10 + "%";
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 9)
                    {
                        if (Tempsval[4].Contains("SkillID"))
                        {
                            var Lv = "1";
                            if (Tempsval[5].Contains("SkillLV")) Lv = Tempsval[5].Replace("SkillLV:", "");
                            var Clockval = FindClockBuff(Tempsval[4].Replace("SkillID:", ""), Lv, ignoreEnemyFunc);
                            output = "\r\n" + Clockval + " " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }

                        var tmpstr = "";
                        for (var Q = 3; Q < Tempsval.Length; Q++) tmpstr += Tempsval[Q] + ",";
                        tmpstr = tmpstr.Substring(0, tmpstr.Length - 1);
                        output = "[" + tmpstr + "] " +
                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                     ? ""
                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                 (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                 (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        break;
                    }

                    if (Tempsval.Length > 4)
                    {
                        if (Tempsval[3].Length == 6 && Tempsval[3][0] == '9')
                        {
                            var Lv = "1";
                            if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                            var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                            if (Tempsval.Length >= 6)
                                if (Tempsval[5].Contains("UseRate"))
                                {
                                    output = "\r\n" + Clockval + " " +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                             (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") +
                                             " Buff成功率:" + (Tempsval[5].Replace("UseRate:", "") == "1000"
                                                 ? ""
                                                 : Convert.ToDouble(Tempsval[5].Replace("UseRate:", "")) / 10 +
                                                   "%");
                                    break;
                                }

                            output = "\r\n" + Clockval + " " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }

                        var tmpstr = "";
                        for (var Q = 3; Q < Tempsval.Length; Q++) tmpstr += Tempsval[Q] + ",";
                        tmpstr = tmpstr.Substring(0, tmpstr.Length - 1);
                        output = "[" + tmpstr + "] " +
                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                     ? ""
                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                 (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                 (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        break;
                    }

                    output = Funcsval;
                    break;
            }

            switch (Funcname)
            {
                case "NP増加":
                case "NP減少":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        if (Tempsval[2].Contains("Act") && Tempsval[3].Contains("Act"))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         "\r\n动作Set:" + Tempsval[2].Replace("ActSet:", "") + " - 发动概率: " +
                                         Convert.ToInt32(Tempsval[3].Replace("ActSetWeight:", "")) + "%";
                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                    if (Tempsval.Length == 3)
                        if (Tempsval[2].Contains("IncludePassiveIndividuality") ||
                            Tempsval[2].Contains("ShowQuestNoEffect"))
                            try
                            {
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                }

                                break;
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                                break;
                            }

                    try
                    {
                        output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                     ? ""
                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }

                    break;
                case "诅咒吸収":
                case "原稿完成解除":
                    try
                    {
                        output =
                            "∅" +
                            (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }

                    break;
                case "无特效特殊(快速)即死":
                    try
                    {
                        output =
                            "∅" +
                            (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }

                    break;
                case "灼伤無効":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 3 || Funcsval.Contains("Individualty"))
                        try
                        {
                            output = "∅" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }
                    else
                        output = Funcsval;

                    break;
                case "NP吸収":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            var PossibleCount = (Tempsval[2] + "," + Tempsval[3]).Replace("DependFuncVals1:", "")
                                .Replace("]", "").Split(',');
                            output = "最大 " + Convert.ToDouble(PossibleCount[1]) / 100 + "% NP/单体" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    if (Tempsval.Length == 5)
                        try
                        {
                            var PossibleCount = (Tempsval[2] + "," + Tempsval[3] + "," + Tempsval[4])
                                .Replace("DependFuncVals1:", "").Replace("]", "").Replace("Value2:", "").Split(',');
                            output = "减少敌方最大 " + PossibleCount[1] + "格充能/单体, \r\n增加 " +
                                     Convert.ToDouble(PossibleCount[2]) / 100 + "% NP(按成功数叠加)" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case "HP減少":
                case "HP回復":
                case "HP减少":
                case "HP回复":
                    Tempsval = Funcsval.Split(',');
                    try
                    {
                        output = Tempsval[1] + (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                            ? "HP"
                            : "HP(" + Convert.ToInt64(Tempsval[0]) / 10 + "%成功率)");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }

                    break;
                case "HP吸収":
                    Tempsval = Funcsval.Split(',');
                    try
                    {
                        output = Tempsval[3].Replace("]", "") + (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                            ? "HP"
                            : "HP(" + Convert.ToInt64(Tempsval[0]) / 10 + "%成功率)");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }

                    break;
                case "毒":
                case "やけど":
                case "灼伤":
                case "呪い":
                case "诅咒":
                case "每回合回复HP":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4 || (Tempsval.Length == 6 && Tempsval[4].Contains("Individualty")))
                        try
                        {
                            output = Tempsval[3] + "HP" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case "幻惑":
                case "呪厄":
                case "蝕毒":
                case "延焼":
                case "死の淵":
                case "NiceShot!":
                case "祭神の怒り":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case "ハッピーハロウィン":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 5)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                case "増殖":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 7)
                    {
                        if (Tempsval[4].Contains("ShowState") && (Tempsval[5].Contains("SameBuff") ||
                                                                  Tempsval[6].Contains("SameBuff")))
                            try
                            {
                                output = Convert.ToDouble(Tempsval[3]) + "HP" + " * N | N ≤ " +
                                         Tempsval[6].Replace("SameBuffLimitNum:", "") + " |" +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                            }

                        break;
                    }

                    output = Funcsval;
                    break;
                case "概率回避":
                    Tempsval = Funcsval.Split(',');
                    if (Tempsval.Length == 4)
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3].Replace("UseRate:", "")) / 10 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            break;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                            break;
                        }

                    output = Funcsval;
                    break;
                default:
                    if (Funcname.Contains("NP") && !Funcname.Contains("獲得") && !Funcname.Contains("获得") &&
                        !Funcname.Contains("每回合") &&
                        !Funcname.Contains("每ターン") && !Funcname.Contains("発動") && !Funcname.Contains("发动"))
                    {
                        Tempsval = Funcsval.Split(',');
                        if (Tempsval.Length == 4)
                            if (Tempsval[2].Contains("Act") && Tempsval[3].Contains("Act"))
                                try
                                {
                                    output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                             (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                 ? ""
                                                 : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                             "\r\n动作Set:" + Tempsval[2].Replace("ActSet:", "") + " - 发动概率: " +
                                             Convert.ToInt32(Tempsval[3].Replace("ActSetWeight:", "")) + "%";
                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                        if (Tempsval.Length == 3)
                            if (Tempsval[2].Contains("IncludePassiveIndividuality") ||
                                Tempsval[2].Contains("ShowQuestNoEffect"))
                                try
                                {
                                    try
                                    {
                                        output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                                 (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                                     ? ""
                                                     : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                                    }
                                    catch (Exception)
                                    {
                                        output = Funcsval;
                                    }

                                    break;
                                }
                                catch (Exception)
                                {
                                    output = Funcsval;
                                    break;
                                }

                        try
                        {
                            output = Convert.ToDouble(Tempsval[1]) / 100 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }
                    }

                    break;
            }

            if (Funcname == "∅" && Funcsval.Contains("ShowQuestNoEffect"))
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length == 5)
                    try
                    {
                        output =
                            $"场地{Tempsval[3]}" +
                            (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                            (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                            (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }
                else
                    output = Funcsval;
            }

            if (Funcname == "∅")
            {
                Tempsval = Funcsval.Split(',');
                switch (Tempsval.Length)
                {
                    case 3:
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    case 6 when Tempsval[4].Contains("ShowState"):
                        try
                        {
                            output =
                                $"场地{Tempsval[3]}" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    default:
                        output = Funcsval;
                        break;
                }
            }

            if (Funcname.Contains("追加効果") || Funcname.Contains("攻击时发动"))
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length > 4)
                {
                    if (Tempsval[3].Length == 6 && Tempsval[3][0] == '9')
                    {
                        var Lv = "1";
                        if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                        var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                        if (Tempsval.Length >= 6)
                        {
                            if (Tempsval[5].Contains("UseRate"))
                                output = "\r\n" + Clockval + " " +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次") + "\r\nBuff成功率:" +
                                         (Tempsval[5].Replace("UseRate:", "") == "1000"
                                             ? ""
                                             : Convert.ToDouble(Tempsval[5].Replace("UseRate:", "")) / 10 + "%");
                            else
                                output = "\r\n" + Clockval + " " +
                                         (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                             ? ""
                                             : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                         (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                         (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                        else
                        {
                            output = "\r\n" + Clockval + " " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                    }
                    else
                    {
                        if (Tempsval[3][0] == '9')
                        {
                            var Lv = "1";
                            if (Tempsval[4].Contains("Value2")) Lv = Tempsval[4].Replace("Value2:", "");
                            var Clockval = FindClockBuff(Tempsval[3], Lv, ignoreEnemyFunc);
                            output = "\r\n" + Clockval + " " +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                        else
                        {
                            var tmpstr = "";
                            for (var Q = 3; Q < Tempsval.Length; Q++) tmpstr += Tempsval[Q] + ",";
                            tmpstr = tmpstr.Substring(0, tmpstr.Length - 1);
                            output = "[" + tmpstr + "]\r\n" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                     (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                     (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                    }
                }
            }

            if (Funcname.Contains("强力攻击") || Funcname.Contains("防御无视攻击") || Funcname.Contains("被伤害反射"))
            {
                Tempsval = Funcsval.Split(',');
                try
                {
                    output = Convert.ToDouble(Tempsval[1]) / 10 + "%";
                }
                catch (Exception)
                {
                    output = Funcsval;
                }
            }

            if (Funcname == "宝具封印" || Funcname == "魅了" || Funcname == "魅惑")
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length == 3)
                {
                    try
                    {
                        output =
                            "∅" +
                            (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                ? ""
                                : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                            (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                            (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }
                }
                else
                {
                    if (Tempsval.Length == 4)
                    {
                        if (Tempsval[3].Contains("IncludePassiveIndividuality"))
                            try
                            {
                                output =
                                    "∅" +
                                    (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                        ? ""
                                        : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                    (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                    (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                            }
                            catch (Exception)
                            {
                                output = Funcsval;
                            }
                    }
                    else
                    {
                        output = Funcsval;
                    }
                }
            }

            if (Funcname == "永久睡眠")
            {
                Tempsval = Funcsval.Split(',');
                switch (Tempsval.Length)
                {
                    case 3:
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    case 5:
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    case 6:
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    default:
                        output = Funcsval;
                        break;
                }
            }

            if (Funcname == "鎮国武成王")
            {
                Tempsval = Funcsval.Split(',');
                switch (Tempsval.Length)
                {
                    case 7:
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                (Tempsval[1] == "-1" ? "" : " - " + Tempsval[1] + "回合") +
                                (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "次");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    default:
                        output = Funcsval;
                        break;
                }
            }

            if (Funcname == "強化解除" || Funcname == "防御強化解除" || Funcname == "攻撃強化解除" || Funcname == "攻撃弱体解除" ||
                Funcname == "防御弱体解除" || Funcname == "弱体解除" || Funcname == "必中解除" || Funcname == "回避状態解除" ||
                Funcname == "ガッツ解除" || Funcname == "毅力解除" || Funcname == "从者位置变更" || Funcname == "活祭" ||
                Funcname == "诅咒解除" || Funcname == "诅咒無効" || Funcname == "毒＆呪い無効" || Funcname == "毒＆诅咒無効" ||
                Funcname == "毒＆やけど無効" || Funcname == "毒＆灼伤無効" || Funcname == "弱体耐性無視" || Funcname == "やけど＆延焼解除" ||
                Funcname == "灼傷＆延焼解除" ||
                Funcname == "やけど＆延焼無効" || Funcname == "灼伤＆延焼無効")

            {
                Tempsval = Funcsval.Split(',');
                switch (Tempsval.Length)
                {
                    case 1:
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    case 3 when Funcsval.Contains("TargetIndiv"):
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    case 3:
                    case 4 when Tempsval[3].Contains("Hide"):
                        try
                        {
                            output =
                                "∅" +
                                (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                    ? ""
                                    : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)") +
                                (Tempsval[2] == "-1" ? "" : " · " + Tempsval[2] + "Buff/次");
                            ;
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                    case 4 when Funcname == "弱体耐性無視":
                        try
                        {
                            output = Convert.ToDouble(Tempsval[3]) / 10 + "%" +
                                     (Tempsval[0] == "1000" || Tempsval[0] == "-5000"
                                         ? ""
                                         : "(" + Convert.ToDouble(Tempsval[0]) / 10 + "%成功率)");
                        }
                        catch (Exception)
                        {
                            output = Funcsval;
                        }

                        break;
                }
            }

            if (Funcname.Contains("特攻攻击") && !Funcname.Contains("特殊"))
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length == 4 || Tempsval.Length == 5)
                    try
                    {
                        output =
                            $"\r\n基础倍率: {Convert.ToDouble(Tempsval[1]) / 10}% 特攻倍率: {Convert.ToDouble(Tempsval[3]) / 10}% 特攻对象(ID):〔{SearchIndividualality(Tempsval[2])}〕";
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }
                else
                    output = Funcsval;
            }

            if (Funcname.Contains("HP越少威力"))
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length == 3)
                    try
                    {
                        output = "\r\n" + Convert.ToDouble(Tempsval[1]) / 10 + "% + " +
                                 Convert.ToDouble(Tempsval[2]) / 10 +
                                 "% * (1 - HP / MaxHP)";
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }
                else
                    output = Funcsval;
            }

            if (Funcname.Contains("稀有度"))
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length == 5)
                    try
                    {
                        output =
                            $"\r\n基础倍率: {Convert.ToDouble(Tempsval[1]) / 10}% 特攻倍率: {Convert.ToDouble(Tempsval[3]) / 10}% 特攻稀有度: {Tempsval[4].Replace("TargetRarityList:", "").Replace("/", ",")}";
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }
                else
                    output = Funcsval;
            }

            if (Funcname.Contains("特殊)特攻攻击"))
            {
                Tempsval = Funcsval.Split(',');
                if (Tempsval.Length == 7 || Tempsval.Length == 8)
                    try
                    {
                        var N_count = Convert.ToInt32(Tempsval[5].Replace("ParamAddMaxCount:", ""));
                        var N_Value = Convert.ToDecimal(Tempsval[3]) / 10;
                        if (N_count == 0 && N_Value == 0.0M)
                            output =
                                $"\r\n基础倍率: {Convert.ToDouble(Tempsval[1]) / 10}% 特攻对象(ID):〔{SearchIndividualality(Tempsval[4].Replace("TargetList:", ""))}〕\r\n" +
                                $"特攻倍率: {Convert.ToDouble(Tempsval[6].Replace("Value2:", "")) / 10}%";
                        else
                            output =
                                $"\r\n基础倍率: {Convert.ToDouble(Tempsval[1]) / 10}% 特攻对象(ID):〔{SearchIndividualality(Tempsval[4].Replace("TargetList:", ""))}〕\r\n" +
                                $"特攻倍率: {Convert.ToDouble(Tempsval[6].Replace("Value2:", "")) / 10}% + {N_Value}% * N (N≤{N_count})";
                    }
                    catch (Exception)
                    {
                        output = Funcsval;
                    }
                else
                    output = Funcsval;
            }

            if (Funcname == "人格交换" || Funcname.Contains("暫無翻譯")) output = Funcsval;
            if (Funcname != "即死") return output;
            output = Convert.ToDouble(Funcsval) / 10 + "%";
            IndividualListStringTemp = null;
            return output;
        }

        public static string TDStrForExcel(string funcid, string lv1, string lv5, string TDStrName)
        {
            var lv1spl = lv1.Split(',');
            var lv5spl = lv5.Split(',');
            string output;
            switch (Convert.ToInt32(funcid))
            {
                case 12:
                case 13:
                case 14:
                case 15:
                    try
                    {
                        output = TDStrName + " 【{" + (Convert.ToDouble(lv1spl[1]) == Convert.ToDouble(lv5spl[1])
                            ? Convert.ToDouble(lv1spl[1]) / 10 + "%}】\r\n"
                            : Convert.ToDouble(lv1spl[1]) / 10 + "% - " + Convert.ToDouble(lv5spl[1]) / 10 +
                              "%}】\r\n");
                    }
                    catch (Exception)
                    {
                        output = "false";
                    }

                    break;
                case 16:
                case 17:
                case 24:
                case 25:
                    if (lv1spl.Length == 4 || lv1spl.Length == 5)
                        try
                        {
                            output = TDStrName + " 【{基础倍率: " +
                                     (Convert.ToDouble(lv1spl[1]) == Convert.ToDouble(lv5spl[1])
                                         ? Convert.ToDouble(lv1spl[1]) / 10 + "% "
                                         : Convert.ToDouble(lv1spl[1]) / 10 + "% - " +
                                           Convert.ToDouble(lv5spl[1]) / 10 + "% ")
                                     + " 特攻倍率: " +
                                     (Convert.ToDouble(lv1spl[3]) == Convert.ToDouble(lv5spl[3])
                                         ? Convert.ToDouble(lv1spl[3]) / 10 + "% "
                                         : Convert.ToDouble(lv1spl[3]) / 10 + "% - " +
                                           Convert.ToDouble(lv5spl[3]) / 10 + "% ") + "特攻对象:〔" +
                                     SearchIndividualalityTDExcel(lv1spl[2]) + "〕}】\r\n";
                        }
                        catch (Exception)
                        {
                            output = "false";
                        }
                    else
                        output = "false";

                    break;
                case 3853:
                case 3854:
                    if (lv1spl.Length == 5)
                        try
                        {
                            output = TDStrName + " 【{基础倍率: " +
                                     (Convert.ToDouble(lv1spl[1]) == Convert.ToDouble(lv5spl[1])
                                         ? Convert.ToDouble(lv1spl[1]) / 10 + "% "
                                         : Convert.ToDouble(lv1spl[1]) / 10 + "% - " +
                                           Convert.ToDouble(lv5spl[1]) / 10 + "% ") + "特攻倍率: " +
                                     (Convert.ToDouble(lv1spl[3]) == Convert.ToDouble(lv5spl[3])
                                         ? Convert.ToDouble(lv1spl[3]) / 10 + "% "
                                         : Convert.ToDouble(lv1spl[3]) / 10 + "% - " +
                                           Convert.ToDouble(lv5spl[3]) / 10 + "% ") + "特攻稀有度: " +
                                     lv1spl[4].Replace("TargetRarityList:", "").Replace("/", ",") + "}】\r\n";
                        }
                        catch (Exception)
                        {
                            output = "false";
                        }
                    else
                        output = "false";

                    break;
                case 5336:
                case 5337:
                    if (lv1spl.Length == 7 || lv1spl.Length == 8)
                        try
                        {
                            output = TDStrName + " 【{基础倍率: " +
                                     (Convert.ToDouble(lv1spl[1]) == Convert.ToDouble(lv5spl[1])
                                         ? Convert.ToDouble(lv1spl[1]) / 10 + "% "
                                         : Convert.ToDouble(lv1spl[1]) / 10 + "% - " +
                                           Convert.ToDouble(lv5spl[1]) / 10 + "% ")
                                     + "特攻倍率: " +
                                     (Convert.ToDouble(lv1spl[6].Replace("Value2:", "")) ==
                                      Convert.ToDouble(lv5spl[6].Replace("Value2:", "")) &&
                                      Convert.ToDouble(lv1spl[3]) == Convert.ToDouble(lv5spl[3])
                                         ? Convert.ToDouble(lv1spl[6].Replace("Value2:", "")) / 10 + "% + " +
                                           Convert.ToDouble(lv1spl[3]) / 10 + "% * N " + "(N≤" +
                                           lv1spl[5].Replace("ParamAddMaxCount:", "") + ") "
                                         : "<" + Convert.ToDouble(lv1spl[6].Replace("Value2:", "")) / 10 + "% + " +
                                           Convert.ToDouble(lv1spl[3]) / 10 + "% * N > - <" +
                                           Convert.ToDouble(lv5spl[6].Replace("Value2:", "")) / 10 + "% + " +
                                           Convert.ToDouble(lv5spl[3]) / 10 + "% * N >" + "(N≤" +
                                           lv1spl[5].Replace("ParamAddMaxCount:", "") + ")") +
                                     "特攻对象:〔" +
                                     SearchIndividualalityTDExcel(lv1spl[4].Replace("TargetList:", "")) +
                                     "〕" + ")}】\r\n";
                        }
                        catch (Exception)
                        {
                            output = "false";
                        }
                    else
                        output = "false";

                    break;
                default:
                    output = "false";
                    break;
            }

            return output;
        }

        public static string FindClockBuff(string id, string lv, bool ignoreEnemyFunc)
        {
            var FuncSval = "";
            var FuncID = "";
            foreach (var SKLTMP in GlobalPathsAndDatas.mstSkillLvArray)
            {
                if (((JObject)SKLTMP)["skillId"].ToString() != id ||
                    ((JObject)SKLTMP)["lv"].ToString() != lv) continue;
                var SKLobjtmp = JObject.Parse(SKLTMP.ToString());
                FuncSval = SKLobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                    .Replace("[", "").Replace("]\"", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                FuncSval = FuncSval.Substring(0, FuncSval.Length - 2);
                FuncID = SKLobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                    .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                break;
            }

            var FuncIDList = new List<string>(FuncID.Split(','));
            var FuncIDArray = FuncIDList.ToArray();
            var FuncList = new List<string>();
            var applyTargetList = new List<string>();
            var targetRawList = new List<string>();
            var funcTypeList = new List<string>();
            FuncList.AddRange(from skfuncidtmp in FuncIDArray
                from functmp in GlobalPathsAndDatas.mstFuncArray
                where ((JObject)functmp)["id"].ToString() == skfuncidtmp
                select JObject.Parse(functmp.ToString())
                into mstFuncobjtmp
                select MainWindow.TranslateBuff(mstFuncobjtmp["popupText"].ToString()));
            applyTargetList.AddRange(from skfuncidtmp in FuncIDArray
                from functmp in GlobalPathsAndDatas.mstFuncArray
                where ((JObject)functmp)["id"].ToString() == skfuncidtmp
                select JObject.Parse(functmp.ToString())
                into mstFuncobjtmp
                select MainWindow.TranslateBuff(mstFuncobjtmp["applyTarget"].ToString()));
            targetRawList.AddRange(from skfuncidtmp in FuncIDArray
                from functmp in GlobalPathsAndDatas.mstFuncArray
                where ((JObject)functmp)["id"].ToString() == skfuncidtmp
                select JObject.Parse(functmp.ToString())
                into mstFuncobjtmp
                select MainWindow.TranslateBuff(mstFuncobjtmp["targetType"].ToString()));
            funcTypeList.AddRange(from skfuncidtmp in FuncIDArray
                from functmp in GlobalPathsAndDatas.mstFuncArray
                where ((JObject)functmp)["id"].ToString() == skfuncidtmp
                select JObject.Parse(functmp.ToString())
                into mstFuncobjtmp
                select MainWindow.TranslateBuff(mstFuncobjtmp["funcType"].ToString()));
            var applyTargetListArray = applyTargetList.ToArray();
            var FuncListArray = FuncList.ToArray();
            var FuncSvalArray = FuncSval.Split('|');
            var targetrawArray = targetRawList.ToArray();
            var funcTypeArray = funcTypeList.ToArray();
            var result = "<";
            for (var i = 0; i < FuncListArray.Length; i++)
            {
                if (FuncIDList[i] == "21039") continue;
                if (FuncListArray[i] == "" &&
                    FuncSvalArray[i].Count(c => c == ',') == 1 &&
                    !FuncSvalArray[i].Contains("Hide"))
                    FuncListArray[i] = "HP回复";
                if ((FuncListArray[i] == "なし" || (FuncListArray[i] == "" &&
                                                  FuncSvalArray[i].Contains("Hide"))) &&
                    FuncSvalArray[i].Count(c => c == ',') > 0)
                    FuncListArray[i] = TranslateTDAttackNameForClock(FuncIDArray[i]);
                if (FuncListArray[i] == "生贄")
                    FuncListArray[i] = "活祭";
                if (FuncSvalArray[i].Contains("5000,-1,-1,ShowState:-1,HideMiss:1,HideNoEffect:1")) continue;
                if (FuncSvalArray[i].Contains("5000,1,1,ShowState:-1,HideNoEffect:1")) continue;
                if (FuncListArray[i] == "" && funcTypeArray[i] == "2") continue;
                if (FuncIDList[i] == "0" && id == "970409")
                {
                    FuncListArray[i] = "自身撤退为替补";
                    FuncSvalArray[i] = "∅";
                }

                if (ignoreEnemyFunc)
                {
                    if (applyTargetListArray[i] == "2")
                    {
                        if (FuncListArray[i].Contains("チャージ増加") || FuncListArray[i].Contains("充能增加") ||
                            FuncListArray[i].Contains("クリティカル発生") || FuncListArray[i].Contains("暴击发生率"))
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
                                    continue;
                            }
                        else
                            continue;
                    }

                    if (applyTargetListArray[i] == "1")
                        if (FuncListArray[i].Contains("NP増加") || FuncListArray[i].Contains("スター発生") ||
                            FuncListArray[i].Contains("暴击星掉落率"))
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

                result += FuncListArray[i] + "(" +
                          ModifyFuncStr(FuncListArray[i], FuncSvalArray[i], ignoreEnemyFunc) + ") + ";
            }

            try
            {
                result = result.Substring(0, result.Length - 3) + " ";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            result += "> ";
            return result;
        }

        private string FindFuncTypeNameDebugger(string funcTypeId)
        {
            foreach (var funcListObjectDebugger in GlobalPathsAndDatas.funcListDebuggerArray)
                if (((JObject)funcListObjectDebugger)["funcTypeId"].ToString() == funcTypeId)
                    return funcListObjectDebugger["typeName"].ToString();
            return funcTypeId + "(未知)";
        }

        private static string TranslateTDAttackNameForClock(string TDFuncID)
        {
            try
            {
                foreach (var TdAttackNmeTmp in GlobalPathsAndDatas.TDAttackNameTranslation)
                {
                    if (((JObject)TdAttackNmeTmp)["id"].ToString() != TDFuncID) continue;
                    var tdFuncName = JObject.Parse(TdAttackNmeTmp.ToString());
                    return tdFuncName["tdFuncname"].ToString();
                }

                foreach (var functmp in GlobalPathsAndDatas.mstFuncArray)
                {
                    if (((JObject)functmp)["id"].ToString() != TDFuncID) continue;
                    var mstFuncobjtmp = JObject.Parse(functmp.ToString());
                    var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                        .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                    foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                    {
                        if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                        return ((JObject)Bufftmp)["name"].ToString();
                    }
                }

                return "未知Func";
            }
            catch (Exception)
            {
                return "FuncID: " + TDFuncID;
            }
        }

        private static int ReturnArrayNum(string str)
        {
            foreach (var buffNmeTmp in GlobalPathsAndDatas.TranslationList)
            {
                if (!((JObject)buffNmeTmp)["buffStr"].ToString().Contains(str) &&
                    !((JObject)buffNmeTmp)["buffTrans"].ToString().Contains(str)) continue;
                var buffNameObj = JObject.Parse(buffNmeTmp.ToString());
                return Convert.ToInt32(buffNameObj["id"].ToString()) + 1;
            }

            return (from buffNmeTmp in GlobalPathsAndDatas.TranslationList
                where str.Contains(((JObject)buffNmeTmp)["buffStr"].ToString()) ||
                      str.Contains(((JObject)buffNmeTmp)["buffTrans"].ToString())
                select JObject.Parse(buffNmeTmp.ToString())
                into buffNameObj
                select Convert.ToInt32(buffNameObj["id"].ToString()) + 1).FirstOrDefault();
        }

        public static string SearchIndividualality(string Input)
        {
            if (Input.Contains(@"/"))
            {
                var InputArray = Input.Split('/');
                var output = "";
                var trigger1 = true;
                foreach (var item in InputArray)
                {
                    if (item.Length >= 6) output += item + "/";
                    if (item == "5010" || item == "5000") continue;
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                    {
                        if (((JObject)svtInditmp)["id"].ToString() != item) continue;
                        var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                        output += svtIndiObj["individualityName"] + "(" + item + ")/";
                        trigger1 = false;
                        break;
                    }

                    if (trigger1) output += "特性" + item + "/";
                }

                if (output.Length > 1) output = output.Substring(0, output.Length - 1);
                return output;
            }

            if (Input.Length >= 6) return Input;
            if (Input == "5010" || Input == "5000") return Input;
            foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
            {
                if (((JObject)svtInditmp)["id"].ToString() != Input) continue;
                var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                return svtIndiObj["individualityName"] + " ( " + Input + " ) ";
            }

            return Input;
        }

        public static string SearchIndividualalityTDExcel(string Input)
        {
            if (Input.Contains(@"/"))
            {
                var InputArray = Input.Split('/');
                var output = "";
                var trigger1 = true;
                foreach (var item in InputArray)
                {
                    if (item.Length >= 6) output += item + "/";
                    if (item == "5010" || item == "5000") continue;
                    foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
                    {
                        if (((JObject)svtInditmp)["id"].ToString() != item) continue;
                        var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                        output += svtIndiObj["individualityName"] + "/";
                        trigger1 = false;
                        break;
                    }

                    if (trigger1) output += item + "/";
                }

                if (output.Length > 1) output = output.Substring(0, output.Length - 1);
                return output;
            }

            if (Input.Length >= 6) return Input;
            if (Input == "5010" || Input == "5000") return Input;
            foreach (var svtInditmp in GlobalPathsAndDatas.SvtIndividualityTranslation)
            {
                if (((JObject)svtInditmp)["id"].ToString() != Input) continue;
                var svtIndiObj = JObject.Parse(svtInditmp.ToString());
                return svtIndiObj["individualityName"].ToString();
            }

            return Input;
        }

        public static string TranslateBuff(string buffname)
        {
            try
            {
                foreach (var buffNmeTmp in GlobalPathsAndDatas.TranslationList)
                {
                    if (((JObject)buffNmeTmp)["buffStr"].ToString() != buffname) continue;
                    var buffNameObj = JObject.Parse(buffNmeTmp.ToString());
                    return buffNameObj["buffTrans"].ToString();
                }

                return buffname;
            }
            catch (Exception)
            {
                return buffname;
            }
        }

        private static string ServantTreasureDeviceSvalCheckForCounter(string svtTDID, string Lv, bool ignoreEnemyFunc)
        {
            string svtTreasureDeviceFuncID;
            string[] svtTreasureDeviceFuncIDArray = null;
            string[] svtTreasureDeviceFuncArray;
            string[] TDFuncstrArray = null;
            string[] TDlv1OC1strArray = null;
            var output = "";
            foreach (var TDLVtmp in GlobalPathsAndDatas.mstTreasureDeviceLvArray)
            {
                if (((JObject)TDLVtmp)["treaureDeviceId"].ToString() != svtTDID ||
                    ((JObject)TDLVtmp)["lv"].ToString() != Lv) continue;
                var TDLVobjtmp = JObject.Parse(TDLVtmp.ToString());
                var NPval1 = TDLVobjtmp["svals"].ToString().Replace("\n", "").Replace("\r", "")
                    .Replace("[", "").Replace("]", "*").Replace("\"", "").Replace(" ", "").Replace("*,", "|");
                if (NPval1.Length >= 2) NPval1 = NPval1.Substring(0, NPval1.Length - 2);
                TDlv1OC1strArray = NPval1.Split('|');
                svtTreasureDeviceFuncID = TDLVobjtmp["funcId"].ToString().Replace("\n", "").Replace("\t", "")
                    .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                svtTreasureDeviceFuncIDArray = svtTreasureDeviceFuncID.Split(',');
            }

            try
            {
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
                        if (funcnametmp != "") continue;
                        var BuffVal = mstFuncobjtmp["vals"].ToString().Replace("\n", "").Replace("\t", "")
                            .Replace("\r", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                        foreach (var Bufftmp in GlobalPathsAndDatas.mstBuffArray)
                        {
                            if (((JObject)Bufftmp)["id"].ToString() != BuffVal) continue;
                            funcnametmp = ((JObject)Bufftmp)["name"].ToString();
                            break;
                        }
                    }

                    TmpList.Add(TranslateBuff(funcnametmp));
                }

                svtTreasureDeviceFuncArray = TmpList.ToArray();
                TDFuncstrArray = svtTreasureDeviceFuncArray;
                for (var i = 0; i <= TDFuncstrArray.Length - 1; i++)
                {
                    if ((TDFuncstrArray[i] == "なし" || (TDFuncstrArray[i] == "" &&
                                                       TDlv1OC1strArray[i].Contains("Hide"))) &&
                        TDlv1OC1strArray[i].Count(c => c == ',') > 0)
                        TDFuncstrArray[i] = TranslateTDAttackName(svtTreasureDeviceFuncIDArray[i]);

                    if (TDFuncstrArray[i] == "生贄" && svtTreasureDeviceFuncIDArray[i] == "3851")
                        TDFuncstrArray[i] = "活祭";

                    if (TDFuncstrArray[i] == "" && svtTreasureDeviceFuncIDArray[i] == "7011")
                        TDFuncstrArray[i] = "从者位置变更";

                    if (TDFuncstrArray[i] == "" && TDlv1OC1strArray[i].Count(c => c == ',') == 1 &&
                        !TDlv1OC1strArray[i].Contains("Hide")) TDFuncstrArray[i] = "HP回复";
                    TDlv1OC1strArray[i] = ModifyFuncStr(TDFuncstrArray[i],
                        TDlv1OC1strArray[i], ignoreEnemyFunc);
                    output += TDFuncstrArray[i] + "(" + TDlv1OC1strArray[i] + ") ";
                }

                GC.Collect();
                return output;
            }
            catch (Exception)
            {
                return "false";
            }
        }

        private static string TranslateTDAttackName(string TDFuncID)
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
            catch (Exception)
            {
                return "FuncID: " + TDFuncID;
            }
        }
    }
}