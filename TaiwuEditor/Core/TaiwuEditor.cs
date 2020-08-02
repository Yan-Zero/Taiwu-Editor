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
using TaiwuEditor.Script;
using TaiwuEditor.UI;
using YanLib.ModHelper;

namespace TaiwuEditor
{
    [BepInPlugin(TaiwuEditor.GUID, "TaiwuEditor", TaiwuEditor.version)]
    [BepInProcess("The Scroll Of Taiwu Alpha V1.0.exe")]
    [BepInDependency("0.0Yan.Lib")]
    public class TaiwuEditor : BaseUnityPlugin
    {
        /// <summary>版本</summary>
        public const string version = "1.6.2.0";

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

        public static ModHelper Mod;


        private void Awake()
        {
            DontDestroyOnLoad(this);

            settings.Init(Config);
            Logger = base.Logger;
            RuntimeConfig.TaiwuEditor = this;

            HarmonyPatches.Init();
            RuntimeConfig.Init();

            Mod = new ModHelper(GUID, "太吾修改器");

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

        public static bool ToggleUI = false;

        private void Update()
        {
            // ESC 键
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (RuntimeConfig.UI_Config.overlay != null && RuntimeConfig.UI_Config.overlay.Created && RuntimeConfig.UI_Config.overlay.IsActive)
                    ToggleUI = true;
            }

            // UI Hotkey
            if (settings.Hotkey.OpenUI.Value.IsDown() || ToggleUI)
            {
                ToggleUI = false;
                if (RuntimeConfig.UI_Config.overlay != null && RuntimeConfig.UI_Config.overlay.Created)
                {
                    RuntimeConfig.UI_Config.overlay.RectTransform.SetAsLastSibling();
                    if (RuntimeConfig.UI_Config.overlay.IsActive)
                    {
                        RuntimeConfig.UI_Config.overlay.SetActive(false);
                        settings.Save();
                    }
                    else
                    {
                        RuntimeConfig.UI_Config.overlay.SetActive(true);
                        AudioManager.instance.PlaySE("SE_BUTTONDEFAULT");
                    }
                }
                else
                {
                    EditorUI.PrepareGUI();

                    var parent =  UnityEngine.GameObject.Find("/UIRoot/Canvas/UIPopup").transform;
                    RuntimeConfig.UI_Config.overlay.SetParent(parent);
                    RuntimeConfig.UI_Config.overlay.GameObject.layer = 5;
                    RuntimeConfig.UI_Config.overlay.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    RuntimeConfig.UI_Config.overlay.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    RuntimeConfig.UI_Config.overlay.RectTransform.anchoredPosition = Vector2.zero;

                    try
                    {
                        EditorUI.TryInit(settings);
                    }
                    catch(Exception ex)
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

                    var onTitleClick = RuntimeConfig.UI_Config.windows.TaiwuTitle.Get<ClickHelper>();
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
                    RuntimeConfig.UI_Config.windows.CloseButton.OnClick = delegate
                    {
                        ToggleUI = true;
                    };
                }
            }
        }

        ~TaiwuEditor()
        {
            settings.Save();
        }
    }

}
