using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuEditor.MGO;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.Core
{
    public static class EditorUI
    {
        public static readonly int wideOfLabel = 200;

        public static void ReadBookCheatUI(BaseScroll Func_Base_Scroll,Settings settings)
        {
            Func_Base_Scroll.Add("每次阅读", new ToggleSliderBar
            {
                Name = "每次阅读",
                MinValue = 1,
                MaxValue = 10,
                ValueFormat = "n0",
                Value = settings.PagesPerFastRead.Value,
                Title = "阅读页数",
                isOn = settings.ReadBookCheat.Value,
                OnToggleValueChanged = (bool value, TaiwuToggle toggle, ToggleSliderBar tsb) =>
                {
                    toggle.Text = value ? "开" : "关";
                    settings.ReadBookCheat.Value = value;
                    tsb.Slider.Interactable = value;
                },
                OnSliderValueChanged = (float value, TaiwuSlider silder, ToggleSliderBar tsb) =>
                {
                    settings.PagesPerFastRead.Value = (int)value;
                },
                Toggle_TipTitle = "快速阅读",
                Toggle_TipContant = "开启或关闭快速阅读",
                Slider_TipTitle = "设置快速阅读页数",
                Slider_TipContant = "每次阅读指定篇数(只对功法类书籍有效，技艺类书籍会全部读完)",
            });
        }

        public static void StoryCheatUI(BaseScroll Func_Base_Scroll, Settings settings)
        {
            BoxGridGameObject storyTyps = new BoxGridGameObject
            {
                Name = "StoryTypes",
                Grid =
                        {
                            StartAxis = Direction.Horizontal,
                            Constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount,
                            ConstraintCount = 5,
                            CellSize = new Vector2(0 , 50),
                            AutoWidth = true
                        },
                SizeFitter =
                        {
                            VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                        }
            };
            for (int index = 0; index < settings.StoryTypsList.Count; index++)
            {
                var storyType = settings.GetStoryTyp(index);
                storyTyps.Children.Add(new Container
                {
                    Name = $"Type-{storyType.Name}",
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
                                    Name = $"Text-{storyType.Name}",
                                    Text = storyType.Name,
                                    UseBoldFont = true,
                                    UseOutline = true
                                },
                                new TaiwuToggle
                                {
                                    Name = $"{index}",
                                    Text = settings.includedStoryTyps.Value[index] ? "开" : "关",
                                    UseBoldFont = true,
                                    isOn = settings.includedStoryTyps.Value[index],
                                    onValueChanged = (bool value , Toggle tg) =>
                                    {
                                        settings.includedStoryTyps.Value[int.Parse(tg.Name)] = value;
                                        tg.Text = value ? "开" : "关";
                                    },
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    }
                                }
                            }
                });
            }
            TaiwuButton allChoose = new TaiwuButton
            {
                Name = "Button",
                Text = "全选",
                FontColor = Color.white,
                OnClick = (Button _this) =>
                {
                    if (_this.Text == "全选")
                    {
                        _this.Text = "全取消";
                        foreach (var child in storyTyps.Children)
                            (child.Children[1] as Toggle).isOn = true;
                    }
                    else
                    {
                        _this.Text = "全选";
                        foreach (var child in storyTyps.Children)
                            (child.Children[1] as Toggle).isOn = false;
                    }
                }
            };
            Func_Base_Scroll.Add("奇遇直接到达目的地", (new BoxAutoSizeModelGameObject
            {
                Name = "Box_奇遇直接到达目的地",
                Group =
                {
                    Spacing = 2,
                    Direction = Direction.Vertical,
                    ForceExpandChildWidth = true
                },
                SizeFitter =
                {
                    VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                },
                Children =
                {
                    new Container
                    {
                        Name = "Main",
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
                                Name = "Text",
                                Text = "奇遇到达目的地",
                                Element =
                                {
                                    PreferredSize = { wideOfLabel , 0 }
                                },
                                UseBoldFont = true
                            },
                            new TaiwuToggle
                            {
                                Name = "Toggle",
                                Text = settings.StoryCheat.Value ? "开" : "关",
                                onValueChanged = (bool value , Toggle toggle) =>
                                {
                                    storyTyps.SetActive(value);
                                    settings.StoryCheat.Value = value;
                                    toggle.Text = value ? "开" : "关";
                                    allChoose.Interactable = true;
                                },
                                PreferredSize = { 50 , 50 },
                                isOn = settings.StoryCheat.Value,
                                TipTitle = "奇遇直接到达目的地",
                                TipContant = "开启后，指定的奇遇将会直接略过过程，直达目的地。"
                            },
                            allChoose,
                        }
                    },
                    storyTyps
                }
            }));
            storyTyps.SetActive(settings.StoryCheat.Value);
        }

        public static void BaseFuncToggle(BaseScroll Func_Base_Scroll, Settings settings)
        {
            Func_Base_Scroll.Add("行动不减+修习单击全满+背包无限负重+见面关系全满", new Container()
            {
                Name = "Box_行动-修习-负重-关系",
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
                                Text = "行动力不减",
                                Element =
                                {
                                    PreferredSize = { 0 , 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.DayTimeMax.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.DayTimeMax.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.DayTimeMax.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "锁定一月行动不减",
                                TipContant = "开启或关闭锁定行动力。"
                            }
                        }
                    },
                    new Container()
                    {
                        Name = "修习单击全满",
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
                                Text = "修习全满",
                                Element =
                                {
                                    PreferredSize = { 0 , 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.PracticeMax.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.PracticeMax.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.PracticeMax.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "修习单击全满",
                                TipContant = "开启或关闭修习单击全满。"
                            }
                        }
                    },
                    new Container()
                    {
                        Name = "无限负重",
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
                                Text = "无限负重",
                                Element =
                                {
                                    PreferredSize = { 0, 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.InfWeightBearing.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.InfWeightBearing.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.InfWeightBearing.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "无限负重",
                                TipContant = "开启后则负重上限会变成1000000000。"
                            }
                        }
                    },
                    new Container()
                    {
                        Name = "见面关系全满",
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
                                Text = "见面关系全满",
                                Element =
                                {
                                    PreferredSize = { 0, 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.MaxRelationship.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.MaxRelationship.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.MaxRelationship.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "见面关系全满",
                                TipContant = "开启后则打开对话窗口关系会改成最大值（可能需要打开两次对话窗口）。"
                            }
                        }
                    },
                }
            });
            Func_Base_Scroll.Add("印象见面全满+戒心全无+无限特性点数", new Container()
            {
                Name = "Box_印象-戒心-特性",
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
                        Name = "见面印象最深",
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
                                Text = "印象最深",
                                Element =
                                {
                                    PreferredSize = { 0 , 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.MaxImpression.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.MaxImpression.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.MaxImpression.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "见面印象最深",
                                TipContant = "开启后，见面将会把印象加到最大值，换衣服后会重置印象。"
                            }
                        }
                    },
                    new Container()
                    {
                        Name = "锁定戒心为零",
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
                                Text = "戒心为零",
                                Element =
                                {
                                    PreferredSize = { 0, 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.VigilanceCheat.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.VigilanceCheat.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.VigilanceCheat.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "锁定戒心为零",
                                TipContant = "开启后戒心会锁定为 0 。"
                            }
                        }
                    },
                    new Container()
                    {
                        Name = "无限特性点数",
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
                                Text = "特性点数无限",
                                Element =
                                {
                                    PreferredSize = { 0, 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "Toggle",
                                Text = settings.InfAbilityP.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.InfAbilityP.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.InfAbilityP.Value = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "特性点数无限",
                                TipContant = "开启后则在新建游戏时，特性不消耗点数。"
                            }
                        }
                    },
                }
            });
        }

        public static void LockGangPartValueUI(BaseScroll Func_Base_Scroll, Settings settings)
        {
            Func_Base_Scroll.Add("门派支持度", new ToggleSliderBar
            {
                Name = "门派支持度",
                Title = "门派支持度",
                MinValue = 0,
                MaxValue = 100,
                ValueFormat = "n0",
                Value = settings.CustomLockValue.Value[0],
                isOn = settings.LockGangPartValue.Value,
                OnToggleValueChanged = (bool value, TaiwuToggle toggle, ToggleSliderBar tsb) =>
                {
                    toggle.Text = value ? "开" : "关";
                    settings.LockGangPartValue.Value = value;
                    tsb.Slider.Interactable = value;
                },
                OnSliderValueChanged = (float value, TaiwuSlider silder, ToggleSliderBar tsb) =>
                {
                    settings.CustomLockValue.Value[0] = (int)value;
                },
                Toggle_TipTitle = "锁定门派支持度",
                Toggle_TipContant = "开启或关闭锁定门派支持度。\n<color=#F28234>设置为0则根据剑冢世界进度自动设定最大门派支持度。</color>",
                Slider_TipTitle = "设置门派支持度",
                Slider_TipContant = "门派支持度将会锁定为指定值（如果开启作弊）\n<color=#F28234>设置为0则根据剑冢世界进度自动设定最大门派支持度。</color>",
            });
        }

        public static void LockBasePartValueUI(BaseScroll Func_Base_Scroll, Settings settings)
        {
            Func_Base_Scroll.Add("地区恩义", new ToggleSliderBar
            {
                Name = "地区恩义",
                Title = "地区恩义",
                MinValue = 0,
                MaxValue = 100,
                ValueFormat = "n0",
                Value = settings.CustomLockValue.Value[1],
                isOn = settings.LockBasePartValue.Value,
                OnToggleValueChanged = (bool value, TaiwuToggle toggle, ToggleSliderBar tsb) =>
                {
                    toggle.Text = value ? "开" : "关";
                    settings.LockBasePartValue.Value = value;
                    tsb.Slider.Interactable = value;
                },
                OnSliderValueChanged = (float value, TaiwuSlider silder, ToggleSliderBar tsb) =>
                {
                    settings.CustomLockValue.Value[1] = (int)value;
                },
                Toggle_TipTitle = "锁定地区恩义",
                Toggle_TipContant = "开启或关闭锁定地区恩义。\n<color=#F28234>设置为0则根据剑冢世界进度自动设定最大地区恩义。</color>",
                Slider_TipTitle = "设置地区恩义",
                Slider_TipContant = "地区恩义将会锁定为指定值（如果开启作弊）\n<color=#F28234>设置为0则根据剑冢世界进度自动设定地区恩义。</color>",
            });
        }

        public static void ChangeDefalutCombatRangeUI(BaseScroll Func_Base_Scroll, Settings settings)
        {
            TaiwuLabel CombatRangeValueLabel;
            TaiwuSlider CombatRangeValueSlider = null;
            TaiwuToggle CombatRangeValueCheat = null;
            Func_Base_Scroll.Add("战斗距离", new Container()
            {
                Name = "Box_默认战斗距离",
                Element =
                {
                    PreferredSize = { 0 , 50 }
                },
                Group =
                {
                    Spacing = 2,
                    Direction = Direction.Horizontal
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = "Label",
                        Text = "默认战斗距离",
                        Element =
                        {
                            PreferredSize = { wideOfLabel, 0 }
                        },
                        UseBoldFont = true,
                        UseOutline = true
                    },
                    new TaiwuToggle()
                    {
                        Name = "Toggle",
                        Text = settings.ChangeDefalutCombatRange.Value ? "开" : "关",
                        Element =
                        {
                            PreferredSize = { 50 , 50 }
                        },
                        isOn = settings.ChangeDefalutCombatRange.Value,
                        onValueChanged = (bool value,Toggle toggle) =>
                        {
                            toggle.Text = value ? "开" : "关";
                            settings.ChangeDefalutCombatRange.Value = value;
                            CombatRangeValueSlider.Interactable = value;
                            CombatRangeValueCheat.SetActive(value);
                        },
                        UseBoldFont = true,
                        UseOutline = true,
                        TipTitle = "锁定默认战斗距离",
                        TipContant = "开启或关闭锁定默认战斗距离。"
                    },
                    (CombatRangeValueCheat = new TaiwuToggle()
                    {
                        Name = "Toggle-Cheat",
                        Text = settings.ChangeCombatRange.Value ? "修改：开启" : "修改：关闭",
                        Element =
                        {
                            PreferredSize = { 150 , 50 }
                        },
                        isOn = settings.ChangeCombatRange.Value,
                        onValueChanged = (bool value,Toggle toggle) =>
                        {
                            toggle.Text = value ? "修改：开启" : "修改：关闭";
                            settings.ChangeCombatRange.Value = value;
                        },
                        UseBoldFont = true,
                        UseOutline = true,
                        TipTitle = "锁定战斗距离",
                        TipContant = "锁定战斗距离，指的是开局就处在的距离。"
                    }),
                    (CombatRangeValueLabel = new TaiwuLabel()
                    {
                        Name = "ValueLabel",
                        Text = $"{settings.CustomLockValue.Value[2] / 10f:.0}",
                        Element =
                        {
                            PreferredSize = { 70 , 0 }
                        },
                        BackgroundStyle = TaiwuLabel.Style.Value
                    }),
                    (CombatRangeValueSlider = new TaiwuSlider()
                    {
                        Name = "ValueSlider",
                        MinValue = 20,
                        MaxValue = 90,
                        Value = settings.CustomLockValue.Value[2],
                        OnValueChanged = (float value,Slider silder) =>
                        {
                            settings.CustomLockValue.Value[2] = (int)value;
                            CombatRangeValueLabel.Text = $"{settings.CustomLockValue.Value[2] / 10f:.0}";
                        },
                        TipTitle = "设置默认战斗距离",
                        TipContant = "默认战斗距离将会为指定值（如果开启功能）",
                    })
                }
            });
            CombatRangeValueSlider.Interactable = settings.ChangeDefalutCombatRange.Value;
            CombatRangeValueCheat.SetActive(settings.ChangeDefalutCombatRange.Value);
        }
    }
}
