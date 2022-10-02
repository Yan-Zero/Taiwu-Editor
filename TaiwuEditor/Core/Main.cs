using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using TaiwuModdingLib.Core.Plugin;
using TaiwuEditor.UI;
using UnityEngine;
using YanLib.ModHelper;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;
using UnityUIKit.Core.GameObjects;
using UnityEngine.UI;

namespace TaiwuEditor
{
    [PluginConfig("TaiwuEditor", "Yan", TaiwuEditor.Version)]
    public class TaiwuEditor : TaiwuRemakePlugin
    {
        /// <summary>版本</summary>
        public const string Version = "1.0";
        /// <summary>GUID</summary>
        public const string GUID = "0.Yan.TaiwuEditor";

        /// <summary>日志</summary>
        public static class Logger
        {
            public static void LogInfo(object context)
            {
                GLog.Log("[TaiwuEditor] Info: " + context.ToString());
            }
            public static void LogError(object context)
            {
                GLog.Error("[TaiwuEditor]Error: " + context.ToString());
            }
        }

        private static bool ToggleUI = false;
        /// <summary>Mod 是否开启</summary>
        public static bool enabled = false;
        /// <summary>太吾修改器的参数</summary>
        internal static Settings settings = new Settings();
        /// <summary>
        /// Yan.Lib 用的
        /// </summary>
        public static ModHelper Mod;

        /// <summary>用于锁定每月行动点数的计时器</summary>
        private static Timer timer;

        public new string GetGuid()
        {
            return GUID;
        }

        public override void Initialize()
        {
            if (enabled)
                return;
            //Logger = base.Logger;
            //RuntimeConfig.TaiwuEditor = this;
#if DEBUG
            TaiwuUIKit.Resources.Others.test();
#endif
            HarmonyPatches.Init();
            RuntimeConfig.Init();

            Mod = new ModHelper(GUID, "太吾修改器")
            {
                OnUpdate = Update
            };
            settings.Init(Mod.Config);
            Mod.SettingUI = new BaseFrame()
            {
                Name = "GUID",
                Group =
                {
                    Padding = { 10 },
                    Spacing = 10,
                },
                Element = { PreferredSize = { 0, 50 } },
                Children =
                {
                    new Container()
                    {
                        Name = "1",
                        Group =
                        {
                            Direction = UnityUIKit.Core.Direction.Horizontal,
                            Spacing = 3
                        },
                        Element = { PreferredSize = { 0, 50 } },

                        Children =
                        {
                            new TaiwuLabel()
                            {
                                Name = "Title",
                                Text = "打开 UI 快捷键",
                                Element =
                                {
                                    PreferredSize = { 200 , 50 }
                                }
                            },
                            new TaiwuLabel()
                            {
                                Name = "Value",
                                Text = settings.Hotkey.OpenUI.Value.ToString(),
                                Element =
                                {
                                    PreferredSize = { 0 , 50 }
                                }
                            }
                        }
                    }
                }
            };


            //// 用于锁定每月行动点数（每秒重置一次行动点数）
            //timer = new Timer(500);
            //timer.Elapsed += DayTimeLock;
            //timer.Start();

            enabled = true;
        }

        private void Update()
        {
            // ESC 键
            if (Input.GetKeyDown(KeyCode.Escape))
                if (RuntimeConfig.UI_Config.overlay != null && RuntimeConfig.UI_Config.overlay.Created && RuntimeConfig.UI_Config.overlay.IsActive)
                    ToggleUI = true;

            // UI Hotkey
            if (settings.Hotkey.OpenUI.Value.IsDown() || ToggleUI)
            {
                ToggleUI = false;
                if (RuntimeConfig.UI_Config.overlay != null && RuntimeConfig.UI_Config.overlay.Created)
                {
                    RuntimeConfig.UI_Config.overlay.RectTransform.SetAsLastSibling();
                    if (RuntimeConfig.UI_Config.overlay.IsActive)
                        RuntimeConfig.UI_Config.overlay.SetActive(false);
                    else
                        RuntimeConfig.UI_Config.overlay.SetActive(true);
                }
                else
                {
                    EditorUI.PrepareGUI();

                    var parent = GameObject.Find("Camera_UIRoot/Canvas/LayerPopUp").transform;
                    RuntimeConfig.UI_Config.overlay.SetParent(parent);
                    RuntimeConfig.UI_Config.overlay.GameObject.layer = 5;
                    RuntimeConfig.UI_Config.overlay.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    RuntimeConfig.UI_Config.overlay.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    RuntimeConfig.UI_Config.overlay.RectTransform.anchoredPosition = Vector2.zero;

                    try
                    {
                        EditorUI.TryInit(settings);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);

                        //基础功能 UI
                        EditorUI.BaseUI.Init(RuntimeConfig.UI_Tab_Instance.Func_Base_Scroll, settings);

                        //属性修改
                        EditorUI.MoreUI.Init(RuntimeConfig.UI_Tab_Instance.Func_More_Scroll);

                        //图鉴添加物品
                        EditorUI.AddItemUI.Init(RuntimeConfig.UI_Tab_Instance.Func_AddItem_Container, settings);

                        //快捷键窗口
                        EditorUI.HotkeyUI.Init(RuntimeConfig.UI_Tab_Instance.Func_Hotkey_Scroll, settings);
                    }

                    //var onTitleClick = RuntimeConfig.UI_Config.windows.TaiwuTitle.Get<ClickHelper>();
                    //onTitleClick.OnClick = (ClickHelper ch) =>
                    //{
                    //    if (ch.ClickCount % 5 == 0)
                    //    {
                    //        RuntimeConfig.DebugMode = !RuntimeConfig.DebugMode;
                    //        Logger.LogInfo("Debug Mode : " + (RuntimeConfig.DebugMode ? "On" : "Off"));
                    //    }

                    //    if (ch.ClickCount > 10000)
                    //        ch.ClickCount = 0;
                    //};
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
