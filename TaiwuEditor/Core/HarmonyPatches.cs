using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using GameData;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Reflection.Emit;
using TaiwuEditor.Script;
using YanLib;

namespace TaiwuEditor
{
    public static class HarmonyPatches
    {
        public static readonly Harmony harmony = new Harmony(TaiwuEditor.GUID);
        public static readonly Type PatchesType = typeof(HarmonyPatches);
        public static readonly Dictionary<string, YanLib.PatchHandler> PatchHandles = new Dictionary<string, PatchHandler>
        {
            { "无限特质点", new PatchHandler
                {
                    TargetType = typeof(NewGame),
                    TargetMethonName = "GetAbilityP",
                    Postfix = AccessTools.Method(PatchesType,"NewGame_UpdateAbility_Postfix")
                }},
            { "时钟", new PatchHandler
            {
                TargetType = typeof(ui_TopBar),
                TargetMethonName = "OnInit",
                Postfix = AccessTools.Method(PatchesType,"ui_TopBar_OnInit_Postfix")
            }},
            { "蛐蛐作弊", new PatchHandler
            {
                TargetType = typeof(GetQuquWindow),
                TargetMethonName = "GetQuquButton",
                Prefix = AccessTools.Method(PatchesType,"GetQuquButton_Prefix")
            }},
            { "战斗距离", new PatchHandler
            {
                TargetType = typeof(BattleSystem),
                TargetMethonName = "Update",
                Postfix = AccessTools.Method(PatchesType,"BattleSystem_Update_Postfix")
            }},
            { "人力无限制", new PatchHandler
            {
                TargetType = typeof(UIDate),
                TargetMethonName = "GetMaxManpower",
                Prefix = AccessTools.Method(PatchesType,"UIDate_GetMaxManpower_Prefix")
            }},
            { "无消耗人力", new PatchHandler
            {
                TargetType = typeof(UIDate),
                TargetMethonName = "GetUseManPower",
                Prefix = AccessTools.Method(PatchesType,"UIDate_GetUseManPower_Prefix")
            }},
            { "建筑修改", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "LoadGameConfigs",
                Postfix = AccessTools.Method(PatchesType,"DataFile_LoadGameConfigs_Postfix")
            }},
            { "建筑最大运营", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "GetBuildingLevelPct",
                Transpiler = AccessTools.Method(PatchesType,"DataFile_GetBuildingLevelPct_Transpiler")
            }},
            { "最大好感和最大印象", new PatchHandler
            {
                TargetType = typeof(ui_MessageWindow),
                TargetMethonName = "SetMassageWindow",
                Prefix = AccessTools.Method(PatchesType,"ui_MessageWindow_SetMassageWindow_Prefix")
            }},
            { "锁定戒心", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "GetActorWariness",
                Postfix = AccessTools.Method(PatchesType,"DateFile_GetActorWariness_Postfix")
            }},
            { "快速修习", new PatchHandler
            {
                TargetType = typeof(BuildingWindow),
                TargetMethonName = "StudySkillUp",
                Prefix = AccessTools.Method(PatchesType,"BuildingWindow_StudySkillUp_Prefix")
            }},
            { "快速阅读", new PatchHandler
            {
                TargetType = typeof(BuildingWindow),
                TargetMethonName = "StartReadBook",
                Prefix = AccessTools.Method(PatchesType,"BuildingWindow_StartReadBook_Prefix")
            }},
            { "无限负重", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "GetMaxItemSize",
                Postfix = AccessTools.Method(PatchesType,"DateFile_GetMaxItemSize_Postfix")
            }},
            { "奇遇直接到达目的地", new PatchHandler
            {
                TargetType = typeof(StorySystem),
                TargetMethonName = "OpenStory",
                Prefix = AccessTools.Method(PatchesType,"StorySystem_OpenStory_Prefix")
            }},
            { "门派恩义", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "GetGangPartValue",
                Prefix = AccessTools.Method(PatchesType,"DateFile_GetGangPartValue_Prefix")
            }},
            { "地区恩义", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "GetBasePartValue",
                Prefix = AccessTools.Method(PatchesType,"DateFile_GetBasePartValue_Prefix")
            }},
            { "地区恩义无消耗", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "SetGangValue",
                Prefix = AccessTools.Method(PatchesType,"DateFile_SetGangValue_Prefix")
            }},
            { "战斗距离修改", new PatchHandler
            {
                TargetType = typeof(BattleSystem),
                TargetMethonName = "Initialize",
                Postfix = AccessTools.Method(PatchesType,"BattleSystem_Initialize_Postfix")
            }},
            { "加载书本数据", new PatchHandler
            {
                TargetType = typeof(DateFile),
                TargetMethonName = "LoadDate",
                Prefix = AccessTools.Method(typeof(SwitchBook),"DateFile_LoadDate_Prefix")
            }},
            { "人物包裹中的功法书籍", new PatchHandler
            {
                TargetType = typeof(SetItem),
                TargetMethonName = "SetActorMenuItemIcon",
                Postfix = AccessTools.Method(typeof(SwitchBook),"SetItem_SetActorMenuItemIcon_Postfix")
            }},
            { "人物装备中的功法书籍", new PatchHandler
            {
                TargetType = typeof(SetItem),
                TargetMethonName = "SetActorEquipIcon",
                Postfix = AccessTools.Method(typeof(SwitchBook),"SetItem_SetActorEquipIcon_Postfix")
            }}
        };

        public static void Init()
        {
            foreach(var patch in PatchHandles)
            {
                try
                {
                    patch.Value.Patch(harmony);
                }
                catch(Exception ex)
                {
                    TaiwuEditor.Logger.LogError($"{ patch.Key } Patch Failed");
                    TaiwuEditor.Logger.LogError(ex);
                }
            }    
        }


        /// <summary>
        /// 蛐蛐修改
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool GetQuquButton_Prefix(GetQuquWindow __instance,int index, ref bool ___startGetQuqu, ref bool ___startFirstTime, ref bool ___getQuquEnd)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.GetQuquNoMiss.Value)
                return true;

            if (___startGetQuqu || ___startFirstTime)
            {
                ___startGetQuqu = false;

                if (RuntimeConfig.DebugMode)
                {
                    int num = 10 + __instance.cricketDate[index][6] - Mathf.Min(__instance.cricketDate[index][3] * 5, 40);
                    TaiwuEditor.Logger.LogInfo(index + ":" + __instance.cricketDate[index][6] + "|" + num);
                }

                if(TaiwuEditor.settings.GetAllQuqu.Value)
                    GetAllQuqu(__instance);
                else
                    AccessTools.Method(__instance.GetType(), "GetQuqu").Invoke(__instance,new object[] { index });

                ___getQuquEnd = true;
            }
            return false;
        }
        private static void GetAllQuqu(GetQuquWindow __instance)
        {
            int MainActorID = DateFile.instance.MianActorID();
            List<int[]> QuquList = new List<int[]>();
            for (int i = 0; i < __instance.placeImage.Length; i++)
            {
                int newThing = DateFile.instance.MakeNewItem(int.Parse(DateFile.instance.cricketPlaceDate[__instance.cricketDate[i][0]][102]));
                int colorId = __instance.cricketDate[i][1];
                int partId = __instance.cricketDate[i][2];
                __instance.MakeQuqu(newThing, colorId, partId);
                int ququDate = __instance.GetQuquDate(newThing, 93, true);
                DateFile.instance.getQuquTrun += ququDate;
                DateFile.instance.AddActorScore(501, 100 + Mathf.Abs(ququDate) * 5);
                int num3 = int.Parse(DateFile.instance.GetItemDate(newThing, 8, true));
                if (Random.Range(0, 100) < num3 * 2)
                {
                    DateFile.instance.ChangeItemHp(MainActorID, newThing, -1, 0, true);
                    __instance.QuquAddInjurys(newThing);
                    QuquList.Add(new int[2]
                    {
                        newThing,
                        1
                    });
                    QuquList.Add(new int[2]
                    {
                        96,
                        Random.Range(1, num3)
                    });
                }
                else if (Random.Range(0, 100) < 10)
                {
                    int num4 = DateFile.instance.MakRandQuqu((num3 - 1) * 3);
                    DateFile.instance.ChangeItemHp(MainActorID, newThing, -(int.Parse(DateFile.instance.GetItemDate(newThing, 901, true)) / 2), 0, true);
                    DateFile.instance.ChangeItemHp(MainActorID, num4, -(int.Parse(DateFile.instance.GetItemDate(num4, 901, true)) / 2), 0, true);
                    QuquList.Add(new int[2]
                    {
                        newThing,
                        1
                    });
                    QuquList.Add(new int[2]
                    {
                        num4,
                        1
                    });
                }
                else
                {
                    QuquList.Add(new int[2]
                    {
                        newThing,
                        1
                    });
                }
            }

            var NewQuquList = new List<int[]>();
            foreach (var i in QuquList)
            {
                if(int.Parse(DateFile.instance.GetItemDate(i[0], 8)) < (10 - TaiwuEditor.settings.CustomLockValue.Value[3]))
                {
                    var value = int.Parse(Characters.GetCharProperty(MainActorID, 406)) + int.Parse(DateFile.instance.GetItemDate(i[0], 905));
                    Characters.SetCharProperty(MainActorID, 406, value.ToString());
                    Items.RemoveItem(i[0]);
                }
                else
                {
                    NewQuquList.Add(i);
                }
            }
            
            DateFile.instance.GetItem(DateFile.instance.MianActorID(), NewQuquList, newItem: false, 0);
        }


        /// <summary>
        /// 战斗必胜修改
        /// </summary>
        /// <param name="__instance"></param>
        private static void BattleSystem_Update_Postfix(BattleSystem __instance)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.BattleMustWin.Value)
                return;
            if(TaiwuEditor.settings.Hotkey.BattleMustWin.Value.IsDown())
            {
                __instance.SetRealDamage(isActor: false, 0, 15, 1000000, __instance.ActorId(isActor: false), __instance.largeSize, getWin: true);
            }
        }


        /// <summary>
        /// 人力无上限
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        private static bool UIDate_GetMaxManpower_Prefix(UIDate __instance, ref int __result)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.ManPowerNoLimit.Value)
                return true;
            int value = __instance.GetBaseMaxManpower() + __instance.GetHomeManpower();
            __result = value < 5 ? 5 : value;
            return false;
        }


        /// <summary>
        /// 无限人力
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        private static bool UIDate_GetUseManPower_Prefix(UIDate __instance, ref int __result)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.InfManPower.Value)
                return true;
            __result = __instance.GetMaxManpower();
            return false;
        }


        /// <summary>
        /// 无限特性点数
        /// </summary>
        /// <param name="__result"></param>
        private static void NewGame_UpdateAbility_Postfix(ref int __result)
        {
            if (TaiwuEditor.enabled && TaiwuEditor.settings.InfAbilityP.Value)
            {
                __result = 10;
            }
        }


        /// <summary>
        /// 时钟用的
        /// </summary>
        /// <param name="__instance"></param>
        private static void ui_TopBar_OnInit_Postfix(ui_TopBar __instance)
        {
            if (TaiwuEditor.enabled)
            {
                if (__instance.transform.Find("MissionBack/MissionNameBack/MissionNameText").gameObject.GetComponent<Clock_Text>() == null)
                    __instance.transform.Find("MissionBack/MissionNameBack/MissionNameText").gameObject.AddComponent<Clock_Text>();
            }
        }


        /// <summary>
        /// 建筑上限修改
        /// 以后直接改成全部的建筑
        /// </summary>
        private static void DataFile_LoadGameConfigs_Postfix()
        {
            if (TaiwuEditor.enabled)
            {
                if(TaiwuEditor.settings.BuildingMaxLevelCheat.Value)
                    BuildingMaxLevelChangeApply();
            }
        }
        public static void BuildingMaxLevelChangeApply()
        {
            if (DateFile.instance)
            {
                DateFile.instance.basehomePlaceDate[1001][1] = "60";
                DateFile.instance.basehomePlaceDate[1004][1] = "999";
                DateFile.instance.basehomePlaceDate[1005][1] = "999";
            }
        }
        public static void BuildingMaxLevelChangeCancel()
        {
            if(DateFile.instance)
            {
                DateFile.instance.basehomePlaceDate[1001][1] = "20";
                DateFile.instance.basehomePlaceDate[1004][1] = "20";
                DateFile.instance.basehomePlaceDate[1005][1] = "20";
            }
        }


        /// <summary>
        /// 建筑工作上限
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        private static IEnumerable<CodeInstruction> DataFile_GetBuildingLevelPct_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var MathfClamp = AccessTools.Method(typeof(Mathf), "Clamp",new Type[] { typeof(int), typeof(int), typeof(int) });
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == MathfClamp)
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(PatchesType, "DataFile_GetBuildingLevelPct_Clamp"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        private static int DataFile_GetBuildingLevelPct_Clamp(int value, int min, int max)
        {
            if(!TaiwuEditor.enabled || !TaiwuEditor.settings.BuildingLevelPctNoLimit.Value)
            {
                return Mathf.Clamp(value, min, max);
            }
            return Mathf.Clamp(value, min, Mathf.Max(TaiwuEditor.settings.CustomLockValue.Value[4] * 5, max));
        }


        /// <summary>
        /// 最大好感和最大印象
        /// </summary>
        private static bool ui_MessageWindow_SetMassageWindow_Prefix(int[] baseEventDate)
        {
            /// <summary>最大好感</summary>
            if (TaiwuEditor.enabled && TaiwuEditor.settings.MaxRelationship.Value)
            {
                MessageEventManager.Instance.MainEventData = (int[])baseEventDate.Clone();

                // 主事件ID
                int mainEventId = baseEventDate[2];
                // 事件类型
                int eventType = int.Parse(DateFile.instance.eventDate[mainEventId][2]);
                //int num2 = DateFile.instance.MianActorID();
                //int num3 = (num != 0) ? ((num != -1) ? num : num2) : baseEventDate[1];

                if (eventType == 0) // 事件类型是与NPC互动
                {
                    // baseEventDate[1]是互动的NPC的ID
                    int npcId = baseEventDate[1];
                    if (Characters.HasChar(npcId))
                    {
                        //try catch 目前用于跳过个别时候载入游戏时触发过月后npc互动会报错的问题
                        try
                        {
                            // 改变好感
                            DateFile.instance.ChangeFavor(npcId, 60000, true, false);
                        }
                        catch (Exception e)
                        {
                            TaiwuEditor.Logger.LogError("好感修改失败");
                            TaiwuEditor.Logger.LogError(e);
                        }
                    }
                }
            }

            /// <summary>最大印象</summary>
            /// <see cref="DateFile.ChangeActorLifeFace"/>
            if (TaiwuEditor.enabled && TaiwuEditor.settings.MaxImpression.Value)
            {
                int mainEventId = baseEventDate[2];
                int eventType = int.Parse(DateFile.instance.eventDate[mainEventId][2]);
                int mainActorId = DateFile.instance.MianActorID();
                //int num6 = (eventType != 0) ? ((eventType != -1) ? eventType : num5) : baseEventDate[1];
                if (eventType == 0)
                {
                    int npcId = baseEventDate[1];
                    if (Characters.HasChar(npcId))
                    {
                        //try catch 目前用于跳过个别特殊npc本身无印象数据会报错的问题
                        try
                        {
                            // 改变印象
                            // 时装ID
                            int fashionDress = int.Parse(DateFile.instance.GetActorDate(mainActorId, 305));
                            if (fashionDress > 0)
                            {
                                // 时装身份ID
                                int faceId = int.Parse(DateFile.instance.GetItemDate(fashionDress, 15));
                                /// <seealso cref="DateFile.ResetActorLifeFace"/>
                                if (faceId != 0)
                                {
                                    DateFile.instance.actorLife[npcId].Remove(1001);
                                    // 添加新印象,100%
                                    DateFile.instance.actorLife[npcId].Add(1001, new List<int>
                                        {
                                            faceId,
                                            100
                                        });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            TaiwuEditor.Logger.LogError("印象修改失败");
                            TaiwuEditor.Logger.LogError(e);
                        }
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// 锁定戒心为0
        /// </summary>
        private static void DateFile_GetActorWariness_Postfix(ref int __result)
        {
            if (TaiwuEditor.enabled && TaiwuEditor.settings.VigilanceCheat.Value)
            {
                __result = 0;
            }
        }


        /// <summary>
        /// 快速修习
        /// </summary>
        private static bool BuildingWindow_StudySkillUp_Prefix(int ___studySkillId, int ___studySkillTyp, ref BuildingWindow __instance)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.PracticeMax.Value || ___studySkillId <= 0 || ___studySkillTyp <= 0 || ___studySkillTyp > 17)
                return true;
            else
            {
                int mainActorId = DateFile.instance.MianActorID();
                if (___studySkillTyp == 17)
                {
                    if (DateFile.instance.GetGongFaLevel(mainActorId, ___studySkillId, 0) >= 100)
                    {
                        return false;
                    }
                    // 基础遗惠
                    int scoreGain = int.Parse(DateFile.instance.gongFaDate[___studySkillId][2]);
                    // 清零因为实战而获得的突破成功率加成
                    DateFile.instance.addGongFaStudyValue = 0;
                    DateFile.instance.ChangeActorGongFa(mainActorId, ___studySkillId, 100, 0, 0, false);
                    // 突破成功一次增加的遗惠
                    DateFile.instance.AddActorScore(302, scoreGain * 100);
                    if (DateFile.instance.GetGongFaLevel(mainActorId, ___studySkillId, 0) >= 100 && DateFile.instance.GetGongFaFLevel(mainActorId, ___studySkillId) >= 10)
                    {
                        // 修习到100%时增加的遗惠
                        DateFile.instance.AddActorScore(304, scoreGain * 100);
                    }
                }
                else
                {
                    if (DateFile.instance.GetSkillLevel(___studySkillId) >= 100)
                    {
                        return false;
                    }
                    int scoreGain = int.Parse(DateFile.instance.skillDate[___studySkillId][2]);
                    // 清零因为较艺而获得的突破成功率加成
                    DateFile.instance.addSkillStudyValue = 0;
                    DateFile.instance.ChangeMianSkill(___studySkillId, 100, 0, false);
                    //DateFile.instance.actorSkills[___studySkillId][0] = 100;
                    // 突破成功一次增加的遗惠
                    DateFile.instance.AddActorScore(202, scoreGain * 100);
                    if (DateFile.instance.GetSkillLevel(___studySkillId) >= 100 && DateFile.instance.GetSkillFLevel(___studySkillId) >= 10)
                    {
                        // 修习到100%时增加的遗惠
                        DateFile.instance.AddActorScore(204, scoreGain * 100);
                    }
                }
                __instance.UpdateStudySkillWindow();
                __instance.UpdateLevelUPSkillWindow();
                __instance.UpdateReadBookWindow();
                return false;
            }
        }


        /// <summary>
        /// 奇遇直接到终点
        /// </summary>
        private static bool StorySystem_OpenStory_Prefix(ref StorySystem __instance, ref bool ___keepHiding)
        {
            if (RuntimeConfig.DebugMode)
                TaiwuEditor.Logger.LogInfo($"OpenStory: StoryId: {__instance.storySystemStoryId}");

            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.StoryCheat.Value)
            {
                return true;
            }

            /// 借鉴<see cref="StorySystem.StorySystemOpend"/>。<see cref="StorySystem.StoryToBattle"/>会将此值设为true
            /// 用这个值判断是否本次调用<see cref="StorySystem.OpenStory"/>是否由<see cref="StorySystem.StoryToBattle"/>
            /// 返回的，防止奇遇终点是战斗时，无限进入战斗。
            if (___keepHiding)
            {
                return true;
            }

            int storyId = __instance.storySystemStoryId;
            if (storyId > 0)
            {
                bool storyIdExist = false;
                for (int i = 0; i < TaiwuEditor.settings.includedStoryTyps.Value.Length; i++)
                {
                    if (TaiwuEditor.settings.includedStoryTyps.Value[i])
                    {
                        if (TaiwuEditor.settings.GetStoryTyp(i).IsContainStoryId(storyId))
                        {
                            storyIdExist = true;
                            break;
                        }
                    }
                }
                if (!storyIdExist)
                {
                    return true;
                }
                // 关闭奇遇窗口
                ToStoryMenu.Instance.CloseToStoryMenu();
                // 终点的事件ID
                int storyEndEventId = int.Parse(DateFile.instance.baseStoryDate[storyId][302]);
                if (RuntimeConfig.DebugMode)
                {
                    TaiwuEditor.Logger.LogInfo($"OpenStory: storyEndEventId: {storyEndEventId}");
                    TaiwuEditor.Logger.LogInfo($"OpenStory: StoryPlayer: {StorySystem.instance.storyPlayer.name}");
                    TaiwuEditor.Logger.LogInfo($"OpenStory: StoryPlayer_placeid: {StorySystem.instance.storyPlayer.parent.parent.parent.name}");
                }

                if (!Helper.EventSetup(StorySystem.StoryBaseManager, storyEndEventId,
                                          __instance.storySystemPartId, __instance.storySystemPlaceId,
                                          __instance.storySystemStoryId))
                {
                    TaiwuEditor.Logger.LogInfo($"OpenStory has been removed due to storyEndEventId is 0");
                    __instance.StoryEnd();
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// 背包最大载重
        /// </summary>
        private static void DateFile_GetMaxItemSize_Postfix(ref int key, ref int __result)
        {
            if (TaiwuEditor.enabled && TaiwuEditor.settings.InfWeightBearing.Value && DateFile.instance.mianActorId == key)
            {
                __result = 1000000000;
            }
        }


        /// <summary>
        /// 快速读书
        /// </summary>
        private static bool BuildingWindow_StartReadBook_Prefix(int ___readBookId, int ___studySkillTyp, BuildingWindow __instance)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.ReadBookCheat.Value || ___studySkillTyp < 1 || ___studySkillTyp > 17 || ___readBookId < 1)
                return true;
            else
            {
                Helper.EasyReadV2(___readBookId, ___studySkillTyp, TaiwuEditor.settings.PagesPerFastRead.Value);
                __instance.UpdateReadBookWindow();
                return false;
            }
        }


        /// <summary>
        /// 锁定门派支持度
        /// </summary>
        private static bool DateFile_GetGangPartValue_Prefix(int gangId, ref int __result)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.LockGangPartValue.Value)
                return true;

            // 太吾村没有支持度
            __result = (gangId == 16) ? 0 : (TaiwuEditor.settings.CustomLockValue.Value[0] == 0) ? DateFile.instance.GetMaxWorldValue() : TaiwuEditor.settings.CustomLockValue.Value[0] * 10;
            return false;
        }


        /// <summary>
        /// 锁定地区恩义
        /// </summary>
        private static bool DateFile_GetBasePartValue_Prefix(ref int __result)
        {
            if (!TaiwuEditor.enabled || !TaiwuEditor.settings.LockBasePartValue.Value)
                return true;

            __result = (TaiwuEditor.settings.CustomLockValue.Value[1] == 0) ? DateFile.instance.GetMaxWorldValue() : TaiwuEditor.settings.CustomLockValue.Value[1] * 10;
            return false;
        }


        /// <summary>
        /// 阻止地区恩义减少
        /// </summary>
        private static bool DateFile_SetGangValue_Prefix(ref int value)
        {
            if (TaiwuEditor.enabled && TaiwuEditor.settings.LockBasePartValue.Value && value < 0)
                value = 0;

            return true;
        }


        /// <summary>
        /// 设置默认战斗距离
        /// </summary>
        private static void BattleSystem_Initialize_Postfix(BattleSystem __instance)
        {
            if (TaiwuEditor.enabled && TaiwuEditor.settings.ChangeDefalutCombatRange.Value)
            {
                RuntimeConfig.SetNeedRange.Invoke(__instance, new object[3]
                    {
                            true,
                            TaiwuEditor.settings.CustomLockValue.Value[2],
                            false
                    });

                if (TaiwuEditor.settings.ChangeCombatRange.Value)
                {
                    __instance.UpdateBattleRange(TaiwuEditor.settings.CustomLockValue.Value[2], true, 1, -1);
                }
            }
        }


    }
}
