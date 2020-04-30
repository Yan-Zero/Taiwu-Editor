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

namespace TaiwuEditor
{
    [BepInPlugin("Yan.TaiwuEditor", "TaiwuEditor", TaiwuEditor.version)]
    public class TaiwuEditor : BaseUnityPlugin
    {
        /// <summary>版本</summary>
        public const string version = "1.1.0.0";

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

            RuntimeCongfig.TaiwuEditor = this;
            RuntimeCongfig.Init();
            PrepareGUI();

            if (!uiIsShow && EditorUIOld.Load(settings))
            {
                uiIsShow = true;
                Patches.Init();

                // 用于锁定每月行动点数（每秒重置一次行动点数）
                timer = new Timer(1000);
                timer.Elapsed += DayTimeLock;
                timer.Start();
            }
            enabled = uiIsShow;
        }

        /// <summary>
        /// 游戏中锁定行动点数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DayTimeLock(object sender, ElapsedEventArgs e)
        {
            if (enabled && DateFile.instance != null && settings.DayTimeMax.Value)
            {
                DateFile.instance.dayTime = DateFile.instance.GetMaxDayTime();
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

                    Func_Base_Scroll.Add("印象见面全满+戒心全无+盗窃必定成功", new Container()
                    {
                        Name = "Box_印象-戒心-盗窃",
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
                            //new Container()
                            //{
                            //    Name = "见面关系全满",
                            //    Group =
                            //    {
                            //        Spacing = 2,
                            //        Direction = Direction.Horizontal
                            //    },
                            //    Children =
                            //    {
                            //        new TaiwuLabel()
                            //        {
                            //            Name = "Text",
                            //            Text = "见面关系全满",
                            //            Element =
                            //            {
                            //                PreferredSize = { 0, 0 }
                            //            },
                            //            UseBoldFont = true,
                            //            UseOutline = true
                            //        },
                            //        new TaiwuToggle()
                            //        {
                            //            Name = "Toggle",
                            //            Text = settings.MaxRelationship.Value ? "开" : "关",
                            //            Element =
                            //            {
                            //                PreferredSize = { 50 , 50 }
                            //            },
                            //            isOn = settings.MaxRelationship.Value,
                            //            onValueChanged = (bool value,Toggle toggle) =>
                            //            {
                            //                toggle.Text = value ? "开" : "关";
                            //                settings.MaxRelationship.Value = value;
                            //            },
                            //            UseBoldFont = true,
                            //            UseOutline = true,
                            //            TipTitle = "见面关系全满",
                            //            TipContant = "开启后则打开对话窗口关系会改成最大值（可能需要打开两次对话窗口）。"
                            //        }
                            //    }
                            //},
                        }
                    });

                    TaiwuLabel PagesValue;
                    TaiwuSlider PagesSlider = null;
                    Func_Base_Scroll.Add("每次阅读", new Container()
                    {
                        Name = "Box_每次阅读",
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
                                Name = "ReadBookLabel",
                                Text = "阅读页数",
                                Element =
                                {
                                    PreferredSize = { wideOfLabel, 0 }
                                },
                                UseBoldFont = true,
                                UseOutline = true
                            },
                            new TaiwuToggle()
                            {
                                Name = "ReadBookToggle",
                                Text = settings.ReadBookCheat.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.ReadBookCheat.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.ReadBookCheat.Value = value;
                                    PagesSlider.Interactable = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "快速阅读",
                                TipContant = "开启或关闭快速阅读"
                            },
                            (PagesValue = new TaiwuLabel()
                            {
                                Name = "PagesValue",
                                Text = settings.PagesPerFastRead.Value.ToString(),
                                Element =
                                {
                                    PreferredSize = { 70 , 0 }
                                },
                                BackgroundStyle = TaiwuLabel.Style.Value
                            }),
                            (PagesSlider = new TaiwuSlider()
                            {
                                Name = "ReadPages",
                                MinValue = 1,
                                MaxValue = 10,
                                Value = settings.PagesPerFastRead.Value,
                                WholeNumber = true,
                                OnValueChanged = (float value,Slider silder) =>
                                {
                                    PagesValue.Text = ((int)value).ToString();
                                    settings.PagesPerFastRead.Value = (int)value;
                                },
                                TipTitle = "设置快速阅读页数",
                                TipContant = $"每次阅读指定篇数(只对功法类书籍有效，技艺类书籍会全部读完)",
                            })
                        }
                    });
                    PagesSlider.Interactable = settings.ReadBookCheat.Value;

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
                    for(int index = 0;index < settings.StoryTypsList.Count;index ++)
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
                            if(_this.Text == "全选")
                            {
                                _this.Text = "全取消";
                                foreach(var child in storyTyps.Children)
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
                    Func_Base_Scroll.Add("奇遇直接到达目的地",(new BoxAutoSizeModelGameObject
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

                    TaiwuLabel GangPartValueLabel;
                    TaiwuSlider GangPartValueSlider = null;
                    Func_Base_Scroll.Add("门派支持度", new Container()
                    {
                        Name = "Box_门派支持度",
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
                                Text = "门派支持度",
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
                                Text = settings.LockGangPartValue.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.LockGangPartValue.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.LockGangPartValue.Value = value;
                                    GangPartValueSlider.Interactable = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "锁定门派支持度",
                                TipContant = "开启或关闭锁定门派支持度。\n<color=#F28234>设置为0则根据剑冢世界进度自动设定最大门派支持度。</color>"
                            },
                            (GangPartValueLabel = new TaiwuLabel()
                            {
                                Name = "ValueLabel",
                                Text = settings.CustomLockValue.Value[0].ToString(),
                                Element =
                                {
                                    PreferredSize = { 70 , 0 }
                                },
                                BackgroundStyle = TaiwuLabel.Style.Value
                            }),
                            (GangPartValueSlider = new TaiwuSlider()
                            {
                                Name = "GangPartValueSlider",
                                MinValue = 0,
                                MaxValue = 100,
                                Value = settings.CustomLockValue.Value[0],
                                WholeNumber = true,
                                OnValueChanged = (float value,Slider silder) =>
                                {
                                    GangPartValueLabel.Text = ((int)value).ToString();
                                    settings.CustomLockValue.Value[0] = (int)value;
                                },
                                TipTitle = "设置门派支持度",
                                TipContant = "门派支持度将会锁定为指定值（如果开启作弊）",
                            })
                        }
                    });
                    GangPartValueSlider.Interactable = settings.LockGangPartValue.Value;

                    TaiwuLabel BasePartValueLabel;
                    TaiwuSlider BasePartValueSlider = null;
                    Func_Base_Scroll.Add("地区恩义", new Container()
                    {
                        Name = "Box_地区恩义",
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
                                Text = "地区恩义",
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
                                Text = settings.LockBasePartValue.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.LockBasePartValue.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.LockBasePartValue.Value = value;
                                    BasePartValueSlider.Interactable = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "锁定地区恩义",
                                TipContant = "开启或关闭锁定地区恩义。\n<color=#F28234>设置为0则根据剑冢世界进度自动设定最大地区恩义。</color>"
                            },
                            (BasePartValueLabel = new TaiwuLabel()
                            {
                                Name = "ValueLabel",
                                Text = settings.CustomLockValue.Value[1].ToString(),
                                Element =
                                {
                                    PreferredSize = { 70 , 0 }
                                },
                                BackgroundStyle = TaiwuLabel.Style.Value
                            }),
                            (BasePartValueSlider = new TaiwuSlider()
                            {
                                Name = "ValueSlider",
                                MinValue = 0,
                                MaxValue = 100,
                                Value = settings.CustomLockValue.Value[1],
                                WholeNumber = true,
                                OnValueChanged = (float value,Slider silder) =>
                                {
                                    BasePartValueLabel.Text = ((int)value).ToString();
                                    settings.CustomLockValue.Value[1] = (int)value;
                                },
                                TipTitle = "设置地区恩义",
                                TipContant = "地区恩义将会锁定为指定值（如果开启作弊）",
                            })
                        }
                    });
                    BasePartValueSlider.Interactable = settings.LockBasePartValue.Value;

                    TaiwuLabel CombatRangeValueLabel;
                    TaiwuSlider CombatRangeValueSlider = null;
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
                                Text = settings.LockCombatRange.Value ? "开" : "关",
                                Element =
                                {
                                    PreferredSize = { 50 , 50 }
                                },
                                isOn = settings.LockCombatRange.Value,
                                onValueChanged = (bool value,Toggle toggle) =>
                                {
                                    toggle.Text = value ? "开" : "关";
                                    settings.LockCombatRange.Value = value;
                                    CombatRangeValueSlider.Interactable = value;
                                },
                                UseBoldFont = true,
                                UseOutline = true,
                                TipTitle = "锁定默认战斗距离",
                                TipContant = "开启或关闭锁定默认战斗距离。"
                            },
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
                                TipContant = "默认战斗距离将会锁定为指定值（如果开启作弊）",
                            })
                        }
                    });
                    CombatRangeValueSlider.Interactable = settings.LockCombatRange.Value;

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
                }
            }
        }


        ~TaiwuEditor()
        {
            settings.Save();
        }
    }

    /// <summary>
    /// Mod设置类
    /// </summary>
    public class Settings
    {
        private ConfigFile Config;

        /// <summary>
        /// 奇遇类型
        /// </summary>
        private static readonly StoryTyp[] storyTyps =
        {
            new StoryTyp(new HashSet<int>{101,102,103,104,105,106,107,108,109,110,111,112}, "外道巢穴"),
            new StoryTyp(new HashSet<int>{1,10001,10005,10006},"促织高鸣"),
            new StoryTyp(new HashSet<int>{2,3,},"静谧竹庐/深谷出口"),
            new StoryTyp(new HashSet<int>{4,5},"英雄猴杰/古墓仙人"),
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
            "地区恩义",
            "默认战斗距离"
        };

        /// <summary>
        /// 检查Mod设置类中的成员是否初始化，若没有初始化则初始化
        /// </summary>
        /// <param name="storyTyps"></param>
        public void Init(ConfigFile config)
        {
            Config = config;
            Hotkey = Config.Bind("Hotkey", "OpenUI", new KeyboardShortcut(KeyCode.F6, new KeyCode[] { KeyCode.LeftControl }),"打开窗口的快捷键");
            CustomLockValue = Config.Bind<int[]>("Cheat", "customLockValue", null, "自定义锁定值");
            includedStoryTyps = Config.Bind<bool[]>("Cheat", "includedStoryTyps", null, "需要直达终点的奇遇的类型");
            PagesPerFastRead = Config.Bind<int>("Cheat", "PagesPerFastRead", 10, "快速读书每次篇数");


            DayTimeMax = Config.Bind<bool>("Cheat", "DayTimeMax", false, "行动力锁定");
            ReadBookCheat = Config.Bind<bool>("Cheat", "ReadBookCheat", false, "快速读书（对残缺篇章有效）");
            PracticeMax = Config.Bind<bool>("Cheat", "PracticeMax", false, "修习单击全满");
            StoryCheat = Config.Bind<bool>("Cheat", "StoryCheat", false, "奇遇直接到达目的地");
            InfWeightBearing = Config.Bind<bool>("Cheat", "InfWeightBearing", false, "无限负重");
            MaxRelationship = Config.Bind<bool>("Cheat", "MaxRelationship", false, "见面关系全满");
            MaxImpression = Config.Bind<bool>("Cheat", "MaxImpression", false, "见面印象全满");
            VigilanceCheat = Config.Bind<bool>("Cheat", "VigilanceCheat", false, "戒心锁定为 0");
            LockGangPartValue = Config.Bind<bool>("Cheat", "LockGangPartValue", false, "锁定门派支持度");
            LockBasePartValue = Config.Bind<bool>("Cheat", "LockBasePartValue", false, "锁定地区恩义");
            LockCombatRange = Config.Bind<bool>("Cheat", "LockCombatRange", false, "锁定战斗距离");

            Config.SaveOnConfigSet = true;

            // 初始化直接到终点的奇遇的ID清单
            if (includedStoryTyps.Value == null || includedStoryTyps.Value.Length != storyTyps.Length)
            {
                includedStoryTyps.Value = new bool[storyTyps.Length];
            }

            // 初始化锁定值
            if (CustomLockValue.Value == null || CustomLockValue.Value.Length != lockValueName.Length)
            {
                CustomLockValue.Value = new int[lockValueName.Length];
                CustomLockValue.Value[2] = 20;
            }

        }

        /// <summary>
        /// 获取奇遇类型
        /// </summary>
        /// <param name="index">奇遇类型ID</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StoryTyp GetStoryTyp(int index) => index < storyTyps.Length ? storyTyps[index] : null;

        /// <summary>
        /// 奇遇类型列表
        /// </summary>
        public List<StoryTyp> StoryTypsList => storyTyps.ToList();

        /// <summary>
        /// 快速读书每次篇数
        /// </summary>
        public ConfigEntry<int> PagesPerFastRead;

        /// <summary>
        /// 需要直达终点的奇遇的类型
        /// </summary>
        public ConfigEntry<bool[]> includedStoryTyps;

        /// <summary>
        /// 自定义锁定值
        /// (index:0)门派支持度值
        /// (index:1)地区恩义值
        /// (index:2)默认战斗距离
        /// </summary>
        public ConfigEntry<int[]> CustomLockValue;

        /// <summary>
        /// 打开修改器窗口的快捷键
        /// </summary>
        public ConfigEntry<KeyboardShortcut> Hotkey;

        /// <summary>
        /// 行动力设定
        /// </summary>
        public ConfigEntry<bool> DayTimeMax;

        /// <summary>
        /// 读书修改
        /// </summary>
        public ConfigEntry<bool> ReadBookCheat;

        /// <summary>
        /// 修习单击全满
        /// </summary>
        public ConfigEntry<bool> PracticeMax;

        /// <summary>
        /// 无限负重
        /// </summary>
        public ConfigEntry<bool> InfWeightBearing;

        /// <summary>
        /// 机遇到达目的地
        /// </summary>
        public ConfigEntry<bool> StoryCheat;

        /// <summary>
        /// 关系全满
        /// </summary>
        public ConfigEntry<bool> MaxRelationship;

        /// <summary>
        /// 印象最大
        /// </summary>
        public ConfigEntry<bool> MaxImpression;

        /// <summary>
        /// 锁定门派支持度
        /// </summary>
        public ConfigEntry<bool> LockGangPartValue;

        /// <summary>
        /// 锁定地区恩义
        /// </summary>
        public ConfigEntry<bool> LockBasePartValue;

        /// <summary>
        /// 戒心锁定为 0 
        /// </summary>
        public ConfigEntry<bool> VigilanceCheat;

        /// <summary>
        /// 默认战斗距离锁定
        /// </summary>
        public ConfigEntry<bool> LockCombatRange;

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
