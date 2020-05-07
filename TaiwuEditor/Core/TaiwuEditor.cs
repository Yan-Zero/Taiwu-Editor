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
using UnityUIKit.GameObjects;
using TaiwuUIKit.GameObjects;
using UnityUIKit.Core;
using System.IO;
using System.Linq;
using UnityUIKit.Components;
using UnityUIKit.Core.GameObjects;
using TaiwuEditor.Core;
using TaiwuEditor.Core.UI;

namespace TaiwuEditor
{
    [BepInPlugin(TaiwuEditor.GUID, "TaiwuEditor", TaiwuEditor.version)]
    [BepInProcess("The Scroll Of Taiwu Alpha V1.0.exe")]
    public class TaiwuEditor : BaseUnityPlugin
    {
        /// <summary>版本</summary>
        public const string version = "1.4.1.0";

        /// <summary>GUID</summary>
        public const string GUID = "0.Yan.TaiwuEditor";

        /// <summary>日志</summary>
        public static new ManualLogSource Logger;

        /// <summary>Mod 是否开启</summary>
        public static new bool enabled = true;

        /// <summary>太吾修改器的参数</summary>
        internal static Settings settings = new Settings();

        /// <summary>用于锁定每月行动点数的计时器</summary>
        private static Timer timer;


        private void Awake()
        {
            DontDestroyOnLoad(this);
            TypeConverterSupporter.Init();

            settings.Init(Config);
            Logger = base.Logger;
            RuntimeConfig.TaiwuEditor = this;

            HarmonyPatches.Init();
            RuntimeConfig.Init();

            // 用于锁定每月行动点数（每秒重置一次行动点数）
            timer = new Timer(500);
            timer.Elapsed += DayTimeLock;
            timer.Start();
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

        private static Container.CanvasContainer overlay = null;
        private static TaiwuWindows windows;
        private static BaseScroll Func_Base_Scroll;
        private static BaseScroll Func_More_Scroll;
        private static BaseScroll Func_Hotkey_Scroll;

        public static bool ToggleUI = false;

        /// <summary>
        /// 初始化UI
        /// </summary>
        private static void PrepareGUI()
        {
            Func_Base_Scroll = new BaseScroll()
            {
                Name = "Func_Base_Scroll",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 15,
                    Padding = { 10 },
                    ForceExpandChildWidth = true
                }
            };
            Func_More_Scroll = new BaseScroll()
            {
                Name = "Func_More_Scroll",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 15,
                    Padding = { 10 },
                    ForceExpandChildWidth = true
                },
                DefaultActive = false
            };
            Func_Hotkey_Scroll = new BaseScroll()
            {
                Name = "Func_Hotkey_Scroll",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 15,
                    Padding = { 10 },
                    ForceExpandChildWidth = true
                },
                DefaultActive = false
            };
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
                                        onValueChanged = (bool value,Toggle Toggle) => Func_Base_Scroll.SetActive(value),
                                        isOn = true
                                    },
                                    new TaiwuToggle()
                                    {
                                        Name = "More.Func",
                                        Text = "属性修改",
                                        UseBoldFont = true,
                                        UseOutline = true,
                                        onValueChanged = (bool value,Toggle Toggle) => Func_More_Scroll.SetActive(value),
                                    },
                                    new TaiwuToggle()
                                    {
                                        Name = "Hotkey.Func",
                                        Text = "快捷键管理",
                                        UseBoldFont = true,
                                        UseOutline = true,
                                        onValueChanged = (bool value,Toggle Toggle) => Func_Hotkey_Scroll.SetActive(value),
                                    }
                                }
                            },
                            Func_Base_Scroll,
                            Func_More_Scroll,
                            Func_Hotkey_Scroll
                        },
                        Element =
                        {
                            PreferredSize = { 1400, 1000 }
                        },
                    }),
                }
            };
        }

        private void Update()
        {
            // UI Hotkey
            if (settings.Hotkey.OpenUI.Value.IsDown() || ToggleUI)
            {
                ToggleUI = false;
                if (overlay != null && overlay.Created)
                {
                    overlay.RectTransform.SetAsLastSibling();
                    if (overlay.IsActive)
                    {
                        overlay.SetActive(false);
                        settings.Save();
                    }
                    else
                    {
                        overlay.SetActive(true);
                        AudioManager.instance.PlaySE("SE_BUTTONDEFAULT");
                    }
                }
                else
                {
                    PrepareGUI();

                    var parent = GameObject.Find("/UIRoot/Canvas/UIPopup").transform;
                    overlay.SetParent(parent);
                    overlay.GameObject.layer = 5;
                    overlay.RectTransform.anchorMax = new Vector2(0.5f,0.5f);
                    overlay.RectTransform.anchorMin = new Vector2(0.5f,0.5f);
                    overlay.RectTransform.anchoredPosition = Vector2.zero;

                    //基础功能 UI
                    EditorUI.BaseUI.BaseFuncToggle(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.StoryCheatUI(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.ReadBookCheatUI(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.GetAllQuquUI(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.LockGangPartValueUI(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.LockBasePartValueUI(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.ChangeDefalutCombatRangeUI(Func_Base_Scroll, settings);
                    EditorUI.BaseUI.BuildingLevelPctLimitUI(Func_Base_Scroll, settings);

                    var onTitleClick = windows.TaiwuTitle.Get<ClickHelper>();
                    onTitleClick.OnClick = (ClickHelper ch) =>
                    {
                        if (ch.ClickCount % 5 == 0)
                        {
                            RuntimeConfig.DebugMode = !RuntimeConfig.DebugMode;
                            Logger.LogInfo("Debug Mode : " + (RuntimeConfig.DebugMode ? "On" : "Off"));
                        }

                        if (ch.ClickCount > 10000)
                            ch.ClickCount = 0;
                    };
                    windows.CloseButton.OnClick = delegate
                    {
                        ToggleUI = true;
                    };

                    //属性修改
                    var i = Func_More_Scroll.AddComponent<EditorBoxMore>();
                    i.SetInstance(Func_More_Scroll);

                    Func_More_Scroll.Add("未载入存档", new BaseFrame()
                    {
                        Name = "未载入存档",
                        Children =
                        {
                            new BaseText()
                            {
                                Name = "Text",
                                Text = "未载入存档"
                            }
                        },
                        DefaultActive = false
                    });
                    EditorUI.MoreUI.TopOfFuncMoreScroll(Func_More_Scroll);
                    EditorUI.MoreUI.DisplayDataFields(Func_More_Scroll, 61, 66, "基本属性");
                    EditorUI.MoreUI.DisplayDataFields(Func_More_Scroll, 401, 407, "资源");
                    EditorUI.MoreUI.DisplayDataFields(Func_More_Scroll, 501, 516, "技艺资质");
                    EditorUI.MoreUI.DisplayDataFields(Func_More_Scroll, 601, 614, "功法资质");
                    EditorUI.MoreUI.TaiwuField(Func_More_Scroll);
                    EditorUI.MoreUI.DisplayHealthAge(Func_More_Scroll);
                    EditorUI.MoreUI.DisplayXXField(Func_More_Scroll);


                    //快捷键窗口
                    EditorUI.HotkeyUI.Hotkey_UI(Func_Hotkey_Scroll, settings.Hotkey);

                    Func_More_Scroll.Get<EditorBoxMore>().NeedUpdate = true;
                }
            }
        }


        ~TaiwuEditor()
        {
            settings.Save();
        }
    }

}
