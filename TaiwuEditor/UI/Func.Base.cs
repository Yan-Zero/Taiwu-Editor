using GameData;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TaiwuEditor.MGO;
using TaiwuEditor.Script;
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
                StoryCheatUI(Func_Base_Scroll, settings);
                ReadBookCheatUI(Func_Base_Scroll, settings);
                GetAllQuquUI(Func_Base_Scroll, settings);
                LockGangPartValueUI(Func_Base_Scroll, settings);
                LockBasePartValueUI(Func_Base_Scroll, settings);
                ChangeDefalutCombatRangeUI(Func_Base_Scroll, settings);
                BuildingLevelPctLimitUI(Func_Base_Scroll, settings);
                MoveOrDelNPC(Func_Base_Scroll, settings);
            }

            //基础功能
            public static void ReadBookCheatUI(BaseScroll Func_Base_Scroll, Settings settings)
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
                Func_Base_Scroll.Add("奇遇直接到达目的地", new BoxAutoSizeModelGameObject
                {
                    Name = "奇遇直接到达目的地",
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
                                            PreferredSize = { EditorUI.wideOfLabel, 0 }
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
                });
                storyTyps.SetActive(settings.StoryCheat.Value);
            }

            public static void BaseFuncToggle(BaseScroll Func_Base_Scroll, Settings settings)
            {
                Func_Base_Scroll.Add("行动不减+修习单击全满+背包无限负重+见面关系全满", new Container()
                {
                    Name = "行动-修习-负重-关系",
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
                Func_Base_Scroll.Add("印象见面全满+戒心全无+无限特性点数+蛐蛐必定捕捉", new Container()
                {
                    Name = "印象-戒心-特性-必捕",
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
                        new Container()
                        {
                            Name = "抓蛐蛐不会失手",
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
                                    Text = "抓蛐蛐不会失手",
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
                                    Text = settings.GetQuquNoMiss.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.GetQuquNoMiss.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.GetQuquNoMiss.Value = value;
                                        Func_Base_Scroll.ContentChildren["蛐蛐全捕捉"]?.SetActive(value);
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "蛐蛐捕捉不会失手",
                                    TipContant = "开启后则在必定捕捉蛐蛐，不需要等待漫长的前摇（大雾）。"
                                }
                            }
                        },
                    }
                });
                Func_Base_Scroll.Add("战斗必胜+解除人力上限+无人力消耗+建筑等级上限修改", new Container()
                {
                    Name = "必胜-人力-人力-建筑",
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
                            Name = "战斗必胜",
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
                                    Text = "战斗必胜",
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
                                    Text = settings.BattleMustWin.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.BattleMustWin.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.BattleMustWin.Value = value;
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "战斗必胜",
                                    TipContant = "开启后，在战斗中按下快捷键，将会秒杀对方。"
                                }
                            }
                        },
                        new Container()
                        {
                            Name = "解除人力上限",
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
                                    Text = "解除人力上限",
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
                                    Text = settings.ManPowerNoLimit.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.ManPowerNoLimit.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.ManPowerNoLimit.Value = value;
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "解除人力上限",
                                    TipContant = "开启后，人力将无上限（即没有最大值，同时最小值也成了 5 ）。"
                                }
                            }
                        },
                        new Container()
                        {
                            Name = "无人力消耗",
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
                                    Text = "无人力消耗",
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
                                    Text = settings.InfManPower.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.InfManPower.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.InfManPower.Value = value;
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "无人力消耗",
                                    TipContant = "开启后，人力将一直是最大值。"
                                }
                            }
                        },
                        new Container()
                        {
                            Name = "建筑等级上限修改",
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
                                    Text = "建筑等级上限修改",
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
                                    Text = settings.BuildingMaxLevelCheat.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.BuildingMaxLevelCheat.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.BuildingMaxLevelCheat.Value = value;
                                        if(value)
                                        {
                                            HarmonyPatches.BuildingMaxLevelChangeApply();
                                        }
                                        else
                                        {
                                            HarmonyPatches.BuildingMaxLevelChangeCancel();
                                        }
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "建筑等级上限修改",
                                    TipContant = "开启后，部分建筑的等级上限将变大。"
                                }
                            }
                        },
                    }
                });
                Func_Base_Scroll.Add("单击切换功法正逆", new Container()
                {
                    Name = "单击切换功法正逆",
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
                            Name = "单击切换功法正逆",
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
                                    Text = "单击切换功法书本正逆",
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
                                    Text = settings.SwitchTheBook.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.SwitchTheBook.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.SwitchTheBook.Value = value;
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "单击切换功法书本正逆",
                                    TipContant = "开启后，点击功法书，将会切换手抄真传。"
                                }
                            }
                        },
                        /*
                        new Container()
                        {
                            Name = "解除人力上限",
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
                                    Text = "解除人力上限",
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
                                    Text = settings.ManPowerNoLimit.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.ManPowerNoLimit.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.ManPowerNoLimit.Value = value;
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "解除人力上限",
                                    TipContant = "开启后，人力将无上限（即没有最大值，同时最小值也成了 5 ）。"
                                }
                            }
                        },
                        new Container()
                        {
                            Name = "无人力消耗",
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
                                    Text = "无人力消耗",
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
                                    Text = settings.InfManPower.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.InfManPower.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.InfManPower.Value = value;
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "无人力消耗",
                                    TipContant = "开启后，人力将一直是最大值。"
                                }
                            }
                        },
                        new Container()
                        {
                            Name = "建筑等级上限修改",
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
                                    Text = "建筑等级上限修改",
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
                                    Text = settings.BuildingMaxLevelCheat.Value ? "开" : "关",
                                    Element =
                                    {
                                        PreferredSize = { 50 , 50 }
                                    },
                                    isOn = settings.BuildingMaxLevelCheat.Value,
                                    onValueChanged = (bool value,Toggle toggle) =>
                                    {
                                        toggle.Text = value ? "开" : "关";
                                        settings.BuildingMaxLevelCheat.Value = value;
                                        if(value)
                                        {
                                            HarmonyPatches.BuildingMaxLevelChangeApply();
                                        }
                                        else
                                        {
                                            HarmonyPatches.BuildingMaxLevelChangeCancel();
                                        }
                                    },
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    TipTitle = "建筑等级上限修改",
                                    TipContant = "开启后，部分建筑的等级上限将变大。"
                                }
                            }
                        },
                        */
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
                                PreferredSize = { EditorUI.wideOfLabel , 0 }
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

            public static void GetAllQuquUI(BaseScroll Func_Base_Scroll, Settings settings)
            {
                Func_Base_Scroll.Add("蛐蛐全捕捉", new ToggleSliderBar
                {
                    Name = "蛐蛐一网打尽",
                    Title = "蛐蛐一网打尽",
                    MinValue = 1,
                    MaxValue = 9,
                    ValueFormat = "n0",
                    Value = settings.CustomLockValue.Value[3],
                    isOn = settings.GetAllQuqu.Value,
                    OnToggleValueChanged = (bool value, TaiwuToggle toggle, ToggleSliderBar tsb) =>
                    {
                        toggle.Text = value ? "开" : "关";
                        settings.GetAllQuqu.Value = value;
                        tsb.Slider.Interactable = value;
                    },
                    OnSliderValueChanged = (float value, TaiwuSlider silder, ToggleSliderBar tsb) =>
                    {
                        settings.CustomLockValue.Value[3] = (int)value;
                    },
                    Toggle_TipTitle = "蛐蛐一网打尽",
                    Toggle_TipContant = "开启后，蛐蛐捕捉会把全部都捕捉了，在指定品级下的会自动换成银钱。",
                    Slider_TipTitle = "设置蛐蛐品级下限",
                    Slider_TipContant = "在品级之下的，都会自动换成银钱。",
                });

                (Func_Base_Scroll.ContentChildren["蛐蛐全捕捉"] as ToggleSliderBar).Slider.UnitySlider.direction = UnityEngine.UI.Slider.Direction.RightToLeft;
                Func_Base_Scroll.ContentChildren["蛐蛐全捕捉"].SetActive(settings.GetQuquNoMiss.Value);
            }

            public static void BuildingLevelPctLimitUI(BaseScroll Func_Base_Scroll, Settings settings)
            {
                Func_Base_Scroll.Add("建筑工作效率上限", new ToggleSliderBar
                {
                    Name = "建筑工作效率上限",
                    Title = "建筑工作效率上限",
                    MinValue = 40,
                    MaxValue = 100,
                    ValueFormat = "n0",
                    Value = settings.CustomLockValue.Value[4],
                    isOn = settings.BuildingLevelPctNoLimit.Value,
                    OnToggleValueChanged = (bool value, TaiwuToggle toggle, ToggleSliderBar tsb) =>
                    {
                        toggle.Text = value ? "开" : "关";
                        settings.BuildingLevelPctNoLimit.Value = value;
                        tsb.Slider.Interactable = value;
                    },
                    OnSliderValueChanged = (float value, TaiwuSlider silder, ToggleSliderBar tsb) =>
                    {
                        settings.CustomLockValue.Value[4] = (int)value;
                    },
                    Toggle_TipTitle = "建筑工作效率上限设置",
                    Toggle_TipContant = "将上限设定为指定值。",
                    Slider_TipTitle = "工作效率上限",
                    Slider_TipContant = "地区恩义将会锁定为指定值（如果开启作弊）。",
                });
            }

            public static void MoveOrDelNPC(BaseScroll Func_Base_Scroll, Settings settings)
            {
                Func_Base_Scroll.Add("移动 NPC",new Container()
                {
                    Name = "Move Or Del NPC",
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
                        new TaiwuLabel
                        {
                            Name = "Label",
                            Element =
                                {
                                    PreferredSize = { wideOfLabel , 0 }
                                },
                            UseBoldFont = true,
                            Text = "人物名字/ID"
                        },
                        new TaiwuInputField
                        {
                            Name = "Input",
                            Placeholder = "请输入人物名称或 ID",
                            OnValueChanged = (string Text,InputField IF) =>
                            {
                                if(string.IsNullOrWhiteSpace(Text))
                                {
                                    (IF.Parent.Children[2] as Button).UnityButton.interactable = false;
                                    (IF.Parent.Children[3] as Button).UnityButton.interactable = false;
                                }
                                else
                                {
                                    (IF.Parent.Children[2] as Button).UnityButton.interactable = true;
                                    (IF.Parent.Children[3] as Button).UnityButton.interactable = true;
                                }
                            },
                            Text = "10001"
                        },
                        new TaiwuButton
                        {
                            Name = "Move",
                            Text = "移动",
                            Element =
                            {
                                PreferredSize = { 100 , 0 }
                            },
                            UseBoldFont = true,
                            FontColor = Color.white,
                            OnClick = (Button bt) =>
                            {
                                var name_ID = bt.Parent.Children[1] as InputField;
                                List<int> ActorIDs = new List<int>();
                                name_ID.Text = name_ID.Text.Replace(" ","");
                                if(int.TryParse(name_ID.Text,out int id))
                                    ActorIDs.Add(id);
                                else
                                    foreach(int ActorID in Characters.GetAllCharIds())
                                        if(DateFile.instance.GetActorName(ActorID) == name_ID.Text)
                                            ActorIDs.Add(ActorID);

                                var place = DateFile.instance.GetActorAtPlace(DateFile.instance.mianActorId);
                                foreach(var ActorID in ActorIDs)
                                    DateFile.instance.MoveToPlace(place[0],place[1],ActorID,true);

                            }
                        }
                    }
                });
            }
        }
    }
}
