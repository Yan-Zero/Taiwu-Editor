using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using HarmonyLib;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using System.Runtime.InteropServices;
using System;
using Newtonsoft.Json;

namespace TaiwuEditor
{
    [BepInPlugin("Yan.TaiwuEditor", "TaiwuEditor", TaiwuEditor.version)]
    public class TaiwuEditor : BaseUnityPlugin
    {
        /// <summary>版本</summary>
        public const string version = "1.0.10.7";

        /// <summary>日志</summary>
        public static new ManualLogSource Logger;

        /// <summary>Mod 是否开启</summary>
        public static new bool enabled = true;

        /// <summary>太吾修改器的参数</summary>
        internal static Settings settings = new Settings();

        /// <summary>用于锁定每月行动点数的计时器</summary>
        private static Timer timer;

        /// <summary>UI类是否已经创建</summary>
        private static bool uiIsShow = false;

        public void Awake()
        {
            DontDestroyOnLoad(this);

            TypeConverterSupporter.Init();
            settings.Init(Config);
            Logger = base.Logger;

            if (!uiIsShow && EditorUI.Load(settings))
            {
                uiIsShow = true;

                Patches.Init();

                // 用于锁定每月行动点数（每秒重置一次行动点数）
                timer = new Timer(1000);
                timer.Elapsed += DayTimeLock;
                timer.Start();
            }
            enabled = uiIsShow;
            //return uiIsShow;
        }

        /// <summary>
        /// 游戏中锁定行动点数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DayTimeLock(object sender, ElapsedEventArgs e)
        {
            if (enabled && DateFile.instance != null && settings.basicUISettings.Value[0])
            {
                DateFile.instance.dayTime = DateFile.instance.GetMaxDayTime();
            }
        }
    }

    /// <summary>
    /// Mod设置类
    /// </summary>
    public class Settings
    {
        private ConfigFile Config;

        // 基本功能页面设置
        private static readonly string[] basicUISettingNames =
        {
            "锁定一月行动不减",  //0
            "快速读书（对残缺篇章有效）", //1
            "修习单击全满", //2
            "奇遇直接到达目的地",  //3
            "身上物品永不超重（仓库无效）", //4
            "见面关系全满", //5
            "见面印象最深(换衣服会重置印象)", //6
            "锁定戒心为零", //7
            "锁定门派支持度", //8
            "锁定地区恩义" //9
        };

        /// <summary>
        /// 奇遇类型
        /// </summary>
        private static readonly StoryTyp[] storyTyps =
        {
            new StoryTyp(new HashSet<int>{101,102,103,104,105,106,107,108,109,110,111,112}, "外道巢穴"),
            new StoryTyp(new HashSet<int>{1,10001,10005,10006},"促织高鸣"),
            new StoryTyp(new HashSet<int>{2,3,4,5},"静谧竹庐/深谷出口/英雄猴杰/古墓仙人"),
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
            "地区恩义"
        };

        /// <summary>
        /// 检查Mod设置类中的成员是否初始化，若没有初始化则初始化
        /// </summary>
        /// <param name="storyTyps"></param>
        public void Init(ConfigFile config)
        {
            Config = config;
            hotKey = Config.Bind("Hotkey", "OpenUI", new KeyboardShortcut(KeyCode.F6, new KeyCode[] { KeyCode.LeftControl }),"打开窗口的快捷键");
            customLockValue = Config.Bind<int[]>("Cheat", "customLockValue", null, "自定义锁定值");
            includedStoryTyps = Config.Bind<bool[]>("Cheat", "includedStoryTyps", null, "需要直达终点的奇遇的类型");
            pagesPerFastRead = Config.Bind<int>("Cheat", "pagesPerFastRead", 10, "快速读书每次篇数");
            basicUISettings = Config.Bind<bool[]>("Cheat", "basicUISettings", null, "基本功能页面设置");

            Config.SaveOnConfigSet = true;

            // 初始化基本功能的设置
            if (basicUISettings.Value == null || basicUISettings.Value.Length < basicUISettingNames.Length)
            {
                basicUISettings.Value = new bool[basicUISettingNames.Length];
            }

            // 初始化直接到终点的奇遇的ID清单
            if (includedStoryTyps.Value == null || includedStoryTyps.Value.Length != storyTyps.Length)
            {
                includedStoryTyps.Value = new bool[storyTyps.Length];
            }

            // 初始化锁定值
            if (customLockValue.Value == null || customLockValue.Value.Length != lockValueName.Length)
            {
                customLockValue.Value = new int[lockValueName.Length];
            }

        }

        /// <summary>
        /// 获取基本功能的名称
        /// </summary>
        /// <param name="index">基本功能ID</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetBasicSettingName(int index) => index < basicUISettingNames.Length ? basicUISettingNames[index] : "";

        /// <summary>
        /// 获取奇遇类型
        /// </summary>
        /// <param name="index">奇遇类型ID</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StoryTyp GetStoryTyp(int index) => index < storyTyps.Length ? storyTyps[index] : null;

        /// <summary>
        /// 获取自定义锁定值的名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetLockValueName(int index) => index < lockValueName.Length ? lockValueName[index] : null;

        /// <summary>
        /// 基本功能页面设置
        /// </summary>
        public ConfigEntry<bool[]> basicUISettings;
        
        /// <summary>
        /// 快速读书每次篇数
        /// </summary>
        public ConfigEntry<int> pagesPerFastRead;

        /// <summary>
        /// 需要直达终点的奇遇的类型
        /// </summary>
        public ConfigEntry<bool[]> includedStoryTyps;

        /// <summary>
        /// 自定义锁定值，(index:0)门派支持度值/(index:1)地区恩义值
        /// </summary>
        public ConfigEntry<int[]> customLockValue;

        /// <summary>
        /// 打开修改器窗口的快捷键
        /// </summary>
        public ConfigEntry<KeyboardShortcut> hotKey;

        public void Save()
        {
            Config.Save();
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

    /// <summary>
    /// 转换器支持
    /// </summary>
    public static class TypeConverterSupporter
    {
        public static void Init()
        {
            TypeConverter converter = new TypeConverter
            {
                ConvertToString = ((object obj, Type type) => JsonConvert.SerializeObject(obj)),
                ConvertToObject = ((string str, Type type) => JsonConvert.DeserializeObject(str, type))

            };
            TomlTypeConverter.AddConverter(typeof(int[]), converter);
            TomlTypeConverter.AddConverter(typeof(bool[]), converter);
        }

    }
}
