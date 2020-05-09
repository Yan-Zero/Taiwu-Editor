using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuEditor.MGO;
using TaiwuEditor.Script;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.UI
{
    public static partial class EditorUI
    {
        public static class HotkeyUI
        {
            public static void Init(BaseScroll Func_Hotkey_Scroll, Settings settings)
            {
                Hotkey_UI(Func_Hotkey_Scroll, settings.Hotkey);
            }

            //快捷键
            public static void Hotkey_UI(BaseScroll Func_Hotkey_Scroll, Settings.HotkeyConfig HotkeyConfig)
            {
                foreach (var i in AccessTools.GetFieldNames(HotkeyConfig))
                {
                    var Field = AccessTools.Field(HotkeyConfig.GetType(), i);
                    if (Field.FieldType == typeof(ConfigEntry<KeyboardShortcut>))
                    {
                        var Container = new Container
                        {
                            Name = i,
                            Group =
                            {
                                Spacing = 2,
                                Direction = Direction.Horizontal
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            },
                            Children =
                            {
                                new TaiwuLabel
                                {
                                    Name = "TitleLabel",
                                    Text = (Field.GetValue(HotkeyConfig) as ConfigEntry<KeyboardShortcut>).Description.Description,
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    Element =
                                    {
                                        PreferredSize = { 200 , 0}
                                    }
                                },
                                new TaiwuLabel
                                {
                                    Name = "ValueLabel",
                                    Text = Hotkey_ToString((Field.GetValue(HotkeyConfig) as ConfigEntry<KeyboardShortcut>).Value),
                                    BackgroundStyle = TaiwuLabel.Style.Value
                                },
                                new TaiwuButton
                                {
                                    Name = i,
                                    Text = "修改快捷键",
                                    Element =
                                    {
                                        PreferredSize = { 200 , 0 }
                                    },
                                    UseBoldFont = true,
                                    OnClick = (Button bt) =>
                                    {
                                        var KeyboardMonitor = Func_Hotkey_Scroll.Get<KeyboardMonitor>();
                                        KeyboardMonitor.Monitoring(AccessTools.Field(HotkeyConfig.GetType(), bt.Name),bt.Parent.Children[1] as TaiwuLabel, bt);
                                    }
                                }
                            }
                        };

                        Func_Hotkey_Scroll.Add(i, Container);
                    }
                }
            }

            public static string Hotkey_ToString(KeyboardShortcut KeyboardShortcut)
            {
                string result = "";
                if (KeyboardShortcut.Modifiers.Count() > 0)
                {
                    result = string.Join(" + ", KeyboardShortcut.Modifiers.Select((KeyCode c) => c.ToString()).ToArray());
                    result += $" + {KeyboardShortcut.MainKey}";
                }
                else
                    result = KeyboardShortcut.MainKey.ToString();

                return result;
            }
        }
    }
}