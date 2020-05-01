using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TaiwuEditor
{
    /// <summary>
    /// Mod设置类
    /// </summary>
    public class Settings
    {
        private ConfigFile Config;

        /// <summary>
        /// 奇遇类型
        /// </summary>
        private static readonly StoryTyp[] storyTyps =
        {
            new StoryTyp(new HashSet<int>{101,102,103,104,105,106,107,108,109,110,111,112}, "外道巢穴"),
            new StoryTyp(new HashSet<int>{1,10001,10005,10006},"促织高鸣"),
            new StoryTyp(new HashSet<int>{2,3,},"静谧竹庐/深谷出口"),
            new StoryTyp(new HashSet<int>{4,5},"英雄猴杰/古墓仙人"),
            new StoryTyp(new HashSet<int>{6,7,8},"大片血迹"),
            new StoryTyp(new HashSet<int>{11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014},"奇书"),
            new StoryTyp(new HashSet<int>{3007,3014,3107,3114,3207,3214,3307,3314,3407,3414,3421,3428,4004,4008,4012,4016,4020,
                4024,4028,4032,4036,4040,4044,4048,4052,4056,4060,4064,4068,4072,4076,4080,4084,4088,4092,4096,4207,4214,4221,
                4228,4235,4242},"天材地宝"),
            //new StoryTyp(new HashSet<int>{5001,5002,5003,5004,5005},"门派争端"),
            new StoryTyp(new HashSet<int>{20001,20002,20003,20004,20005,20006,20007,20008,20009},"剑冢")
        };

        /// <summary>
        /// 锁定值名称
        /// </summary>
        private static readonly string[] lockValueName =
        {
            "门派支持度",
            "地区恩义",
            "默认战斗距离"
        };

        /// <summary>
        /// 检查Mod设置类中的成员是否初始化，若没有初始化则初始化
        /// </summary>
        /// <param name="storyTyps"></param>
        public void Init(ConfigFile config)
        {
            Config = config;
            Hotkey = Config.Bind("Hotkey", "OpenUI", new KeyboardShortcut(KeyCode.F6, new KeyCode[] { KeyCode.LeftControl }), "打开窗口的快捷键");
            CustomLockValue = Config.Bind<int[]>("Cheat", "customLockValue", null, "自定义锁定值");
            includedStoryTyps = Config.Bind<bool[]>("Cheat", "includedStoryTyps", null, "需要直达终点的奇遇的类型");
            PagesPerFastRead = Config.Bind<int>("Cheat", "PagesPerFastRead", 10, "快速读书每次篇数");


            DayTimeMax = Config.Bind<bool>("Cheat", "DayTimeMax", false, "行动力锁定");
            ReadBookCheat = Config.Bind<bool>("Cheat", "ReadBookCheat", false, "快速读书（对残缺篇章有效）");
            PracticeMax = Config.Bind<bool>("Cheat", "PracticeMax", false, "修习单击全满");
            StoryCheat = Config.Bind<bool>("Cheat", "StoryCheat", false, "奇遇直接到达目的地");
            InfWeightBearing = Config.Bind<bool>("Cheat", "InfWeightBearing", false, "无限负重");
            MaxRelationship = Config.Bind<bool>("Cheat", "MaxRelationship", false, "见面关系全满");
            MaxImpression = Config.Bind<bool>("Cheat", "MaxImpression", false, "见面印象全满");
            VigilanceCheat = Config.Bind<bool>("Cheat", "VigilanceCheat", false, "戒心锁定为 0");
            LockGangPartValue = Config.Bind<bool>("Cheat", "LockGangPartValue", false, "锁定门派支持度");
            LockBasePartValue = Config.Bind<bool>("Cheat", "LockBasePartValue", false, "锁定地区恩义");
            ChangeCombatRange = Config.Bind<bool>("Cheat", "ChangeCombatRange", false, "锁定战斗距离");
            ChangeDefalutCombatRange = Config.Bind<bool>("Cheat", "LockCombatRange", false, "默认战斗距离");
            InfAbilityP = Config.Bind<bool>("Cheat", "InfAbilityP", false, "无限特性点数");

            Config.SaveOnConfigSet = true;

            // 初始化直接到终点的奇遇的ID清单
            if (includedStoryTyps.Value == null || includedStoryTyps.Value.Length != storyTyps.Length)
            {
                includedStoryTyps.Value = new bool[storyTyps.Length];
            }

            // 初始化锁定值
            if (CustomLockValue.Value == null || CustomLockValue.Value.Length != lockValueName.Length)
            {
                CustomLockValue.Value = new int[lockValueName.Length];
                CustomLockValue.Value[2] = 20;
            }

        }

        /// <summary>
        /// 获取奇遇类型
        /// </summary>
        /// <param name="index">奇遇类型ID</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StoryTyp GetStoryTyp(int index) => index < storyTyps.Length ? storyTyps[index] : null;

        /// <summary>
        /// 奇遇类型列表
        /// </summary>
        public List<StoryTyp> StoryTypsList => storyTyps.ToList();

        /// <summary>
        /// 快速读书每次篇数
        /// </summary>
        public ConfigEntry<int> PagesPerFastRead;

        /// <summary>
        /// 需要直达终点的奇遇的类型
        /// </summary>
        public ConfigEntry<bool[]> includedStoryTyps;

        /// <summary>
        /// 自定义锁定值
        /// (index:0)门派支持度值
        /// (index:1)地区恩义值
        /// (index:2)默认战斗距离
        /// </summary>
        public ConfigEntry<int[]> CustomLockValue;

        /// <summary>
        /// 打开修改器窗口的快捷键
        /// </summary>
        public ConfigEntry<KeyboardShortcut> Hotkey;

        /// <summary>
        /// 行动力设定
        /// </summary>
        public ConfigEntry<bool> DayTimeMax;

        /// <summary>
        /// 读书修改
        /// </summary>
        public ConfigEntry<bool> ReadBookCheat;

        /// <summary>
        /// 修习单击全满
        /// </summary>
        public ConfigEntry<bool> PracticeMax;

        /// <summary>
        /// 无限负重
        /// </summary>
        public ConfigEntry<bool> InfWeightBearing;

        /// <summary>
        /// 机遇到达目的地
        /// </summary>
        public ConfigEntry<bool> StoryCheat;

        /// <summary>
        /// 关系全满
        /// </summary>
        public ConfigEntry<bool> MaxRelationship;

        /// <summary>
        /// 印象最大
        /// </summary>
        public ConfigEntry<bool> MaxImpression;

        /// <summary>
        /// 锁定门派支持度
        /// </summary>
        public ConfigEntry<bool> LockGangPartValue;

        /// <summary>
        /// 锁定地区恩义
        /// </summary>
        public ConfigEntry<bool> LockBasePartValue;

        /// <summary>
        /// 戒心锁定为 0 
        /// </summary>
        public ConfigEntry<bool> VigilanceCheat;

        /// <summary>
        /// 默认战斗距离修改
        /// </summary>
        public ConfigEntry<bool> ChangeDefalutCombatRange;

        /// <summary>
        /// 战斗距离修改
        /// </summary>
        public ConfigEntry<bool> ChangeCombatRange;

        /// <summary>
        /// 无限特性点数
        /// </summary>
        public ConfigEntry<bool> InfAbilityP;

        public void Save()
        {
            Config.Save();
        }
    }

    public static class RuntimeConfig
    {
        public static TaiwuEditor TaiwuEditor;
        public static MethodInfo SetNeedRange;
        public static bool DebugMode = false;
        public static int CountClickTitle = 0;

        public static void Init()
        {
            SetNeedRange = AccessTools.Method(typeof(BattleSystem), "SetNeedRange");
        }
    }

    /// <summary>
    /// 奇遇种类的类
    /// </summary>
    public class StoryTyp
    {
        // 该类奇遇包含的奇遇Id
        private readonly HashSet<int> storyIds;
        /// <summary>
        /// 奇遇种类的名字
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 奇遇种类
        /// </summary>
        /// <param name="storyIds">包含的奇遇ID</param>
        /// <param name="name">奇遇种类的名称</param>
        public StoryTyp(HashSet<int> storyIds, string name)
        {
            this.storyIds = storyIds;
            Name = name;
        }
        /// <summary>
        /// 该种类奇遇是够包含某奇遇
        /// </summary>
        /// <param name="storyId">要检查的奇遇ID</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsContainStoryId(int storyId) => storyIds.Contains(storyId);
    }
}
