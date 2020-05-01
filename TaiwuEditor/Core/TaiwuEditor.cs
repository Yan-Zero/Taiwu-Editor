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
using UnityUIKit.GameObjects;
using TaiwuUIKit.GameObjects;
using UnityUIKit.Core;
using System.IO;
using System.Linq;
using UnityUIKit.Components;
using UnityUIKit.Core.GameObjects;
using TaiwuEditor.Core;

namespace TaiwuEditor
{
    [BepInPlugin("Yan.TaiwuEditor", "TaiwuEditor", TaiwuEditor.version)]
    public class TaiwuEditor : BaseUnityPlugin
    {
        /// <summary>版本</summary>
        public const string version = "1.1.1.0";

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

        private void Awake()
        {
            DontDestroyOnLoad(this);

            TypeConverterSupporter.Init();
            settings.Init(Config);
            Logger = base.Logger;

            RuntimeConfig.TaiwuEditor = this;
            RuntimeConfig.Init();
            PrepareGUI();

            if (!uiIsShow && EditorUIOld.Load(settings))
            {
                uiIsShow = true;
                Patches.Init();

                // 用于锁定每月行动点数（每秒重置一次行动点数）
                timer = new Timer(500);
                timer.Elapsed += DayTimeLock;
                timer.Start();
            }
            enabled = uiIsShow;
        }

        /// <summary>
        /// 游戏中锁定行动点数
        /// 修改时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DayTimeLock(object sender, ElapsedEventArgs e)
        {
            if(enabled)
            {
                if (DateFile.instance != null && settings.DayTimeMax.Value)
                {
                    DateFile.instance.dayTime = DateFile.instance.GetMaxDayTime();
                }
            }
        }

        private Container.CanvasContainer overlay;
        private TaiwuWindows windows;
        private BaseScroll Func_Base_Scroll;
        private BaseScroll Func_More_Scroll;

        /// <summary>
        /// 时代之泪
        /// 实现属性修改的功能后记得删掉
        /// </summary>
        public bool ToggleUI = false;

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void PrepareGUI()
        {
            overlay = new Container.CanvasContainer()
            {
                Name = "TaiwuEditor.Canvas",
                Group =
                {
                    Padding = { 0 },
                },
                Children =
                {
                    (windows = new TaiwuWindows()
                    {
                        Name = "TaiwuEditor.Windows",
                        Title = $"太吾修改器 {TaiwuEditor.version}",
                        Direction = Direction.Vertical,
                        Spacing = 10,
                        Group =
                        {
                            ChildrenAlignment = TextAnchor.UpperCenter,
                        },
                        Children =
                        {
                            new ToggleGroup()
                            {
                                Name = "Func.Choose",
                                Group =
                                {
                                    Direction = Direction.Horizontal,
                                    Spacing = 5
                                },
                                Element =
                                {
                                    PreferredSize = { 0 , 50 }
                                },
                                Children =
                                {
                                    new TaiwuLabel()
                                    {
                                        Name = "Text",
                                        Text = "功能选择",
                                        Element =
                                        {
                                            PreferredSize = { 150 , 0 }
                                        },
                                        UseOutline = true,
                                        UseBoldFont = true
                                    },
                                    new TaiwuToggle()
                                    {
                                        Name = "Base.Func",
                                        Text = "基础功能",
                                        UseBoldFont = true,
                                        UseOutline = true,
                                        onValueChanged = (bool value,Toggle Toggle) =>
                                        {
                                            Func_Base_Scroll.SetActive(value);
                                        },
                                        isOn = true
                                    },
                                    new TaiwuToggle()
                                    {
                                        Name = "More.Func",
                                        Text = "属性修改",
                                        UseBoldFont = true,
                                        UseOutline = true,
                                        onValueChanged = (bool value,Toggle Toggle) =>
                                        {
                                            //Func_More_Scroll.SetActive(value);
                                            if(value)
                                            {
                                                try
                                                {
                                                    ToggleUI = true;
                                                    EditorUIOld.Instance.ToggleWindow(true);
                                                    (Toggle?.Parent.Children[1] as Toggle).isOn = true;
                                                }
                                                catch(Exception ex)
                                                {
                                                    Logger.LogError(ex);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            (Func_Base_Scroll = new BaseScroll()
                            {
                                Name = "Func_Base_Scroll",
                                Group=
                                {
                                    Direction = Direction.Vertical,
                                    Spacing = 15,
                                    Padding = { 10 },
                                    ForceExpandChildWidth = true
                                }
                            }),
                            (Func_More_Scroll = new BaseScroll()
                            {
                                Name = "Func_More_Scroll",
                                Group=
                                {
                                    Direction = Direction.Vertical,
                                    Spacing = 15,
                                    Padding = { 10 },
                                    ForceExpandChildWidth = true
                                },
                                DefaultActive = false
                            })
                        },
                        Element =
                        {
                            PreferredSize = { 1400, 1000 }
                        },
                    }),
                }
            };
        }

        private const int wideOfLabel = 200;

        private void Update()
        {
            // UI Hotkey
            if (settings.Hotkey.Value.IsDown() || ToggleUI)
            {
                ToggleUI = false;
                if (overlay.Created)
                {
                    overlay.RectTransform.SetAsLastSibling();
                    if (overlay.IsActive)
                        windows.CloseButton.Click();
                    else
                    {
                        overlay.SetActive(true);
                        AudioManager.instance.PlaySE("SE_BUTTONDEFAULT");
                    }
                }
                else
                {
                    var parent = GameObject.Find("/UIRoot/Canvas/UIPopup").transform;
                    overlay.SetParent(parent);
                    overlay.GameObject.layer = 5;
                    overlay.RectTransform.anchorMax = new Vector2(0.5f,0.5f);
                    overlay.RectTransform.anchorMin = new Vector2(0.5f,0.5f);
                    overlay.RectTransform.anchoredPosition = Vector2.zero;


                    //基础功能 UI
                    EditorUI.BaseFuncToggle(Func_Base_Scroll, settings);
                    EditorUI.StoryCheatUI(Func_Base_Scroll, settings);
                    EditorUI.ReadBookCheatUI(Func_Base_Scroll, settings);
                    EditorUI.LockGangPartValueUI(Func_Base_Scroll, settings);
                    EditorUI.LockBasePartValueUI(Func_Base_Scroll, settings);


                    //属性修改
                    var i = Func_More_Scroll.AddComponent<EditorBoxMore>();
                    i.SetInstance(Func_More_Scroll);

                    Func_More_Scroll.Add("未载入存档", new BaseFrame()
                    {
                        Name = "Box_未载入存档",
                        Children =
                        {
                            new BaseText()
                            {
                                Name = "Text 未载入存档",
                                Text = "未载入存档"
                            }
                        }
                    });

                    var onTitleClick = (windows.Children[0] as TaiwuTitle).Get<ClickHelper>();
                    onTitleClick.OnClick = delegate
                    {
                        RuntimeConfig.CountClickTitle ++;
                        if(RuntimeConfig.CountClickTitle == 5)
                        {
                            RuntimeConfig.DebugMode = !RuntimeConfig.DebugMode;
                            RuntimeConfig.CountClickTitle = 0;
                            Logger.LogInfo("Debug Mode : " + (RuntimeConfig.DebugMode ? "On" : "Off"));
                        }
                    };
                }
            }
        }


        ~TaiwuEditor()
        {
            settings.Save();
        }
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
