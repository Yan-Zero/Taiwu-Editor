using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;
using YanLib;

namespace TaiwuEditor
{
    public static class RuntimeConfig
    {
        public static TaiwuEditor TaiwuEditor;
        public static MethodInfo SetNeedRange;
        public static bool DebugMode = false;

        public static void Init()
        {
            SetNeedRange = AccessTools.Method(typeof(BattleSystem), "SetNeedRange");
        }

        public static class UI_Config
        {
            /// <summary>选择修改哪个人物的属性，0太吾，1上一个打开菜单的人物，2自定义人物</summary>
            public static int PropertyChoose = 0;

            /// <summary>想要修改属性的NPC ID</summary>
            public static int ActorId = -1;

            /// <summary>
            /// Canvas
            /// </summary>
            public static Container.CanvasContainer overlay = null;

            /// <summary>
            /// Main Windows
            /// </summary>
            public static TaiwuWindows windows;

        }

        public static class UI_Tab_Instance
        {
            [MGOInfo(Name = "基础功能", Order = 1, InitTypeName = "BaseUI")]
            public static BaseScroll Func_Base_Scroll;
            [MGOInfo(Name = "属性修改", Order = 2, InitTypeName = "MoreUI")]
            public static BaseScroll Func_More_Scroll;
            [MGOInfo(Name = "添加物品", Order = 3, InitTypeName = "AddItemUI")]
            public static Container Func_AddItem_Container;
            [MGOInfo(Name = "快捷键修改", Order = 4, InitTypeName = "HotkeyUI")]
            public static BaseScroll Func_Hotkey_Scroll;
        }
    }
}
