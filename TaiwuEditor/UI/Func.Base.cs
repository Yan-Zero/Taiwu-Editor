using GameData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using YanLib;

namespace TaiwuEditor.UI
{
    public static partial class EditorUI
    {
        public static class BaseUI
        {
            public static void Init(BaseScroll Func_Base_Scroll, Settings settings)
            {
                BaseFuncToggle(Func_Base_Scroll, settings);
            }

            //基础功能
            public static void BaseFuncToggle(BaseScroll Func_Base_Scroll, Settings settings)
            {
                Func_Base_Scroll.Add("行动不减+修习单击全满+背包无限负重+见面关系全满", new Container()
                {
                    Name = "无限特性点",
                    Element =
                    {
                        PreferredSize = { 0 , 50 }
                    },
                    Group =
                    {
                        Spacing = 5,
                        Direction = Direction.Horizontal
                    },
                    Children =
                    {
                        new Container()
                        {
                            Name = "行动力不减",
                            Group =
                            {
                                Spacing = 2,
                                Direction = Direction.Horizontal
                            },
                            Children =
                            {
                                new TaiwuLabel()
                                {
                                    Name = "Text",
                                    Text = "无限特性点",
                                    Element =
                                    {
                                        PreferredSize = { 0 , 0 }
                                    },
                                    UseOutline = true,
                                    LableStyle = TaiwuLabel.Style.Subtile
                                },
                                new TaiwuToggle()
                                {
                                    Name = "Toggle",
                                    Text = settings.InfAbilityP.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    IsOn = settings.InfAbilityP.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        (toggle as TaiwuToggle).Text = value ? "开" : "关";
                                        settings.InfAbilityP.Value = value;
                                    },
                                    TipTitle = "无限特性点",
                                    TipContent = "虽然可能依旧显示特性点数量为0，但是确实是无限的。"
                                }
                            }
                        },
                    }
                });
            }

        }
    }
}
