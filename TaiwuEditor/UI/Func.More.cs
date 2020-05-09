using System;
using System.Collections.Generic;
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
        public static class MoreUI
        {
            public static void Init(BaseScroll Func_More_Scroll)
            {
                var i = RuntimeConfig.UI_Tab_Instance.Func_More_Scroll.AddComponent<TabFuncMore>();
                i.SetInstance(RuntimeConfig.UI_Tab_Instance.Func_More_Scroll);

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

                TopOfFuncMoreScroll(Func_More_Scroll);
                DisplayDataFields(Func_More_Scroll, 61, 66, "基本属性");
                DisplayDataFields(Func_More_Scroll, 401, 407, "资源");
                DisplayDataFields(Func_More_Scroll, 501, 516, "技艺资质");
                DisplayDataFields(Func_More_Scroll, 601, 614, "功法资质");
                TaiwuField(Func_More_Scroll);
                DisplayHealthAge(Func_More_Scroll);
                DisplayXXField(Func_More_Scroll);

                RuntimeConfig.UI_Tab_Instance.Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
            }

            //属性修改
            public static void TopOfFuncMoreScroll(BaseScroll Func_More_Scroll)
            {
                Func_More_Scroll.Get<TabFuncMore>().ActorID_InputField = new TaiwuInputField
                {
                    Name = "ActorIdInputField",
                    Text = DateFile.instance.mianActorId.ToString(),
                    Placeholder = "请输入人物 ID",
                    ReadOnly = RuntimeConfig.UI_Config.PropertyChoose != 2,
                    InputType = UnityEngine.UI.InputField.InputType.AutoCorrect,
                    ContentType = UnityEngine.UI.InputField.ContentType.IntegerNumber,
                    OnEndEdit = (string Value, UnityUIKit.GameObjects.InputField IF) =>
                    {
                        RuntimeConfig.UI_Config.ActorId = int.Parse(Value) > -1 ? int.Parse(Value) : DateFile.instance.mianActorId;
                        Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                    }
                };

                Func_More_Scroll.Add("修改人物", new ToggleGroup
                {
                    Name = "修改人物",
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
                            Name = "Label",
                            Text = "<color=#87CEEB>修改人物</color>",
                            UseBoldFont = true,
                            UseOutline = true,
                            Element =
                            {
                                PreferredSize = { wideOfLabel , 0 }
                            }
                        },
                        new TaiwuToggle
                        {
                            Name = "太吾本人",
                            Text = "太吾本人",
                            isOn = RuntimeConfig.UI_Config.PropertyChoose == 0,
                            onValueChanged = (bool value, Toggle tg) =>
                            {
                                if(value)
                                {
                                    RuntimeConfig.UI_Config.PropertyChoose = 0;
                                    RuntimeConfig.UI_Config.ActorId = DateFile.instance.mianActorId;
                                    Func_More_Scroll.Get<TabFuncMore>().ActorID_InputField.ReadOnly = true;
                                    Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                                }
                            },
                            UseBoldFont = true
                        },
                        new TaiwuToggle
                        {
                            Name = "最近打开人物菜单的人物",
                            Text = "最近打开人物菜单的人物",
                            isOn = RuntimeConfig.UI_Config.PropertyChoose == 1,
                            onValueChanged = (bool value, Toggle tg) =>
                            {
                                if(value)
                                {
                                    RuntimeConfig.UI_Config.PropertyChoose = 1;
                                    RuntimeConfig.UI_Config.ActorId = (ActorMenu.instance.actorId == 0) ? DateFile.instance.mianActorId : ActorMenu.instance.actorId;
                                    Func_More_Scroll.Get<TabFuncMore>().ActorID_InputField.ReadOnly = true;
                                    Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                                }
                            },
                            UseBoldFont = true
                        },
                        new TaiwuToggle
                        {
                            Name = "自定义人物",
                            Text = "自定义人物",
                            isOn = RuntimeConfig.UI_Config.PropertyChoose == 2,
                            onValueChanged = (bool value, Toggle tg) =>
                            {
                                if(value)
                                {
                                    RuntimeConfig.UI_Config.PropertyChoose = 2;
                                    Func_More_Scroll.Get<TabFuncMore>().ActorID_InputField.ReadOnly = false;
                                    Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                                }
                            },
                            UseBoldFont = true
                        }
                    }
                });

                Func_More_Scroll.Add("Tip", new BaseText
                {
                    Name = "Tip",
                    Text = "<color=#87CEEB>修改完成后数值不发生变化是游戏界面没有刷新的原因，并不是修改不成功。"
                    + "\n属性资质修改需重新进入人物菜单才会刷新结果而资源和威望修改需发生对应变化的行为后才会更新。"
                    + "\n所有资质属性均为基础值，不含特性、装备、早熟晚熟以及年龄加成</color>"
                });

                Func_More_Scroll.Add("人物ID", new Container
                {
                    Name = "人物ID-物品修改按钮",
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
                        new Container
                        {
                            Name = "人物ID",
                            Group =
                            {
                                Direction = Direction.Horizontal,
                                Spacing = 2
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            },
                            Children =
                            {
                                new TaiwuLabel
                                {
                                    Name = "Label",
                                    UseBoldFont = true,
                                    Text = "<color=#87CEEB>人物ID</color>",
                                    UseOutline = true,
                                    Element =
                                    {
                                        PreferredSize = { wideOfLabel , 0 }
                                    }
                                },
                                Func_More_Scroll.Get<TabFuncMore>().ActorID_InputField
                            }
                        },
                        new Container
                        {
                            Name = "名字",
                            Group =
                            {
                                Direction = Direction.Horizontal,
                                Spacing = 2
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            },
                            Children =
                            {
                                new TaiwuLabel
                                {
                                    Name = "Label",
                                    UseBoldFont = true,
                                    Text = "名字",
                                    UseOutline = true,
                                    Element =
                                    {
                                        PreferredSize = { wideOfLabel , 0 }
                                    }
                                },
                                (Func_More_Scroll.Get<TabFuncMore>().NameValue = new TaiwuLabel
                                {
                                    Name = "NameValue",
                                    UseBoldFont = true,
                                    UseOutline = true,
                                    BackgroundStyle = TaiwuLabel.Style.Value
                                })
                            }
                        }
                    }
                });

                Func_More_Scroll.Add("Tip_2", new BaseText
                {
                    Name = "Tip",
                    Text = "<color=#F28234>点击按钮修改对应类型属性</color>",
                    UseBoldFont = true,
                    UseOutline = true
                });
            }

            public static void DisplayDataFields(BaseScroll Func_More_Scroll, int residBegin, int residEnd, string buttonName)
            {
                if (residBegin >= residEnd)
                {
                    return;
                }
                BoxGridGameObject actorFields = new BoxGridGameObject
                {
                    Name = $"Field:{residBegin}-{residEnd}",
                    Grid =
                    {
                        StartAxis = Direction.Horizontal,
                        Constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount,
                        ConstraintCount = 3,
                        CellSize = new Vector2(0 , 50),
                        AutoWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    }
                };
                for (int resid = residBegin; resid <= residEnd; resid++)
                {
                    actorFields.Children.Add(new Container
                    {
                        Name = $"{resid}",
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
                                Name = $"Text-{Helper.ActorFieldNames[resid]}",
                                Text = Helper.ActorFieldNames[resid],
                                UseBoldFont = true,
                                UseOutline = true,
                                Element =
                                {
                                    PreferredSize = { 125 , 0 }
                                }
                            },
                            new TaiwuInputField
                            {
                                Name = $"InputField-{Helper.ActorFieldNames[resid]}",
                                Placeholder = null,
                                InputType = UnityEngine.UI.InputField.InputType.AutoCorrect,
                                ContentType = UnityEngine.UI.InputField.ContentType.IntegerNumber,
                            },
                            new TaiwuButton
                            {
                                Name = $"Button-{Helper.ActorFieldNames[resid]}",
                                Text = "修改",
                                UseBoldFont = true,
                                OnClick = (Button button) =>
                                {
                                    Helper.ActorSetFieldValue(int.Parse(button.Parent.Name),(button.Parent.Children[1] as TaiwuInputField).Text);
                                },
                                Element =
                                {
                                    PreferredSize = { 75, 0 }
                                }
                            }
                        }
                    });
                }
                Func_More_Scroll.Add($"Char-Field:{buttonName}", new BoxAutoSizeModelGameObject
                {
                    Name = $"Char-Field:{buttonName}",
                    Group =
                    {
                        Spacing = 3,
                        Direction = Direction.Vertical,
                        ForceExpandChildWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    },
                    Children =
                    {
                        new TaiwuButton
                        {
                            Name = "Button-Show",
                            Text = buttonName,
                            UseBoldFont = true,
                            UseOutline = true,
                            OnClick = (Button button) =>
                            {
                                button.Parent.Children[1].SetActive(!button.Parent.Children[1].IsActive);
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            }
                        },
                        actorFields
                    }
                });
                Func_More_Scroll.Get<TabFuncMore>().DataFields.Add(actorFields);
                actorFields.SetActive(false);
            }

            public static void TaiwuField(BaseScroll Func_More_Scroll)
            {
                BoxGridGameObject actorFields = new BoxGridGameObject
                {
                    Name = $"太吾专属",
                    Grid =
                    {
                        StartAxis = Direction.Horizontal,
                        Constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount,
                        ConstraintCount = 2,
                        CellSize = new Vector2(0 , 50),
                        AutoWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    }
                };
                foreach (int resid in new int[] { -1, 706 })
                {
                    actorFields.Children.Add(new Container
                    {
                        Name = $"{resid}",
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
                                Name = $"Text-{Helper.ActorFieldNames[resid]}",
                                Text = Helper.ActorFieldNames[resid],
                                UseBoldFont = true,
                                UseOutline = true,
                                Element =
                                {
                                    PreferredSize = { 125 , 0 }
                                }
                            },
                            new TaiwuInputField
                            {
                                Name = $"InputField-{Helper.ActorFieldNames[resid]}",
                                Placeholder = null,
                                InputType = UnityEngine.UI.InputField.InputType.AutoCorrect,
                                ContentType = UnityEngine.UI.InputField.ContentType.IntegerNumber,
                            },
                            new TaiwuButton
                            {
                                Name = $"Button-{Helper.ActorFieldNames[resid]}",
                                Text = "修改",
                                UseBoldFont = true,
                                OnClick = (Button button) =>
                                {
                                    Helper.ActorSetFieldValue(int.Parse(button.Parent.Name),(button.Parent.Children[1] as TaiwuInputField).Text);
                                },
                                Element =
                                {
                                    PreferredSize = { 75, 0 }
                                }
                            }
                        }
                    });
                }
                Func_More_Scroll.Add($"Char-Field:{"太吾专属"}", new BoxAutoSizeModelGameObject
                {
                    Name = $"Char-Field:{"太吾专属"}",
                    Group =
                    {
                        Spacing = 3,
                        Direction = Direction.Vertical,
                        ForceExpandChildWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    },
                    Children =
                    {
                        new TaiwuButton
                        {
                            Name = "Button-Show",
                            Text = "太吾专属",
                            UseBoldFont = true,
                            UseOutline = true,
                            OnClick = (Button button) =>
                            {
                                if(RuntimeConfig.UI_Config.ActorId == DateFile.instance.mianActorId)
                                {
                                    button.Parent.Children[1].SetActive(!button.Parent.Children[1].IsActive);
                                    button.Parent.Children[2].SetActive(!button.Parent.Children[2].IsActive);
                                }
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            }
                        },
                        actorFields,
                        new BaseText
                        {
                            Name = "Tip",
                            UseBoldFont = true,
                            Text = "每10点无属性内力增加1点真气"
                        }
                    }
                });
                Func_More_Scroll.Get<TabFuncMore>().DataFields.Add(actorFields);
                Func_More_Scroll.ContentChildren[$"Char-Field:{"太吾专属"}"].Children[2].SetActive(false);
                actorFields.SetActive(false);
            }

            public static void DisplayHealthAge(BaseScroll Func_More_Scroll)
            {
                BoxGridGameObject actorFields = new BoxGridGameObject
                {
                    Name = $"健康",
                    Grid =
                    {
                        StartAxis = Direction.Horizontal,
                        Constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount,
                        ConstraintCount = 3,
                        CellSize = new Vector2(0 , 50),
                        AutoWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    }
                };
                foreach (int resid in new int[] { 11, 13, 12 })
                {
                    actorFields.Children.Add(new Container
                    {
                        Name = $"{resid}",
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
                                Name = $"Text-{Helper.ActorFieldNames[resid]}",
                                Text = Helper.ActorFieldNames[resid],
                                UseBoldFont = true,
                                UseOutline = true,
                                Element =
                                {
                                    PreferredSize = { 125 , 0 }
                                }
                            },
                            new TaiwuInputField
                            {
                                Name = $"InputField-{Helper.ActorFieldNames[resid]}",
                                Placeholder = null,
                                InputType = UnityEngine.UI.InputField.InputType.AutoCorrect,
                                ContentType = UnityEngine.UI.InputField.ContentType.IntegerNumber,
                            },
                            new TaiwuButton
                            {
                                Name = $"Button-{Helper.ActorFieldNames[resid]}",
                                Text = "修改",
                                UseBoldFont = true,
                                OnClick = (Button button) =>
                                {
                                    Helper.ActorSetFieldValue(int.Parse(button.Parent.Name),(button.Parent.Children[1] as TaiwuInputField).Text);
                                },
                                Element =
                                {
                                    PreferredSize = { 75, 0 }
                                }
                            }
                        }
                    });
                }

                Func_More_Scroll.Add($"Char-Field:{"健康年龄"}", new BoxAutoSizeModelGameObject
                {
                    Name = $"Char-Field:{"健康年龄"}",
                    Group =
                    {
                        Spacing = 3,
                        Direction = Direction.Vertical,
                        ForceExpandChildWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    },
                    Children =
                    {
                        new TaiwuButton
                        {
                            Name = "Button-Show",
                            Text = "健康、寿命",
                            UseBoldFont = true,
                            UseOutline = true,
                            OnClick = (Button button) =>
                            {
                                for(int i = 1;i<button.Parent.Children.Count;i++)
                                {
                                    button.Parent.Children[i].SetActive(!button.Parent.Children[i].IsActive);
                                }
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            }
                        },
                        new BaseText
                        {
                            Name = "Tip",
                            UseBoldFont = true,
                            Text = "<color=#F28234>注意：\n1.基础寿命为不含人物特性加成的寿命\n2.人物健康修改为0后，过月就会死亡</color>"
                        },
                        actorFields,
                        new Container
                        {
                            Name = "功能按钮",
                            Group =
                            {
                                Spacing = 3,
                                Direction = Direction.Horizontal
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            },
                            Children =
                            {
                                new TaiwuButton
                                {
                                    Name = "我全部都要",
                                    Text = "我全部都要",
                                    UseBoldFont = true,
                                    OnClick = delegate
                                    {
                                        Helper.CureHelper(0);
                                        Helper.CureHelper(1);
                                        Helper.CureHelper(2);
                                    }
                                },
                                new TaiwuButton
                                {
                                    Name = "一键疗伤",
                                    Text = "一键疗伤",
                                    UseBoldFont = true,
                                    OnClick = delegate { Helper.CureHelper(0); }
                                },
                                new TaiwuButton
                                {
                                    Name = "一键祛毒",
                                    Text = "一键祛毒",
                                    UseBoldFont = true,
                                    OnClick = delegate { Helper.CureHelper(1); }
                                },
                                new TaiwuButton
                                {
                                    Name = "一键调理内息",
                                    Text = "一键调理内息",
                                    UseBoldFont = true,
                                    OnClick = delegate { Helper.CureHelper(2); }
                                }
                            }
                        }
                    }
                });
                Func_More_Scroll.Get<TabFuncMore>().DataFields.Add(actorFields);
                actorFields.SetActive(false);
                var temp = Func_More_Scroll.ContentChildren[$"Char-Field:{"健康年龄"}"];
                for (int i = 1; i < temp.Children.Count; i++)
                {
                    temp.Children[i].SetActive(false);
                }
            }

            public static void DisplayXXField(BaseScroll Func_More_Scroll)
            {
                // 入邪值
                int evilValue = Helper.LifeDateHelper(DateFile.instance, RuntimeConfig.UI_Config.ActorId, 501);
                var actorFields = new BoxAutoSizeModelGameObject
                {
                    Name = "入邪值",
                    Group =
                    {
                        Spacing = 2,
                        Direction = Direction.Horizontal
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    },
                    Children =
                    {
                        new TaiwuLabel
                        {
                            Text = "当前入邪值",
                            Name = "evilLabel",
                            UseBoldFont = true,
                            Element =
                            {
                                PreferredSize = { 125 , 50 }
                            }
                        },
                        new TaiwuLabel
                        {
                            Text = evilValue == -1 ? "无" : evilValue.ToString(),
                            Name = "evilLabel",
                            UseBoldFont = true,
                            Element =
                            {
                                PreferredSize = { 100 , 50 }
                            }
                        },
                        new TaiwuButton
                        {
                            Name = "恢复正常",
                            Text = "恢复正常",
                            UseBoldFont = true,
                            OnClick = delegate
                            {
                                Helper.SetActorXXValue(DateFile.instance, RuntimeConfig.UI_Config.ActorId, 0);
                                Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                            }
                        },
                        new TaiwuButton
                        {
                            Name = "相枢入邪",
                            Text = "相枢入邪",
                            UseBoldFont = true,
                            OnClick = delegate
                            {
                                Helper.SetActorXXValue(DateFile.instance, RuntimeConfig.UI_Config.ActorId, 100);
                                Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                            }
                        },
                        new TaiwuButton
                        {
                            Name = "相枢化魔",
                            Text = "相枢化魔",
                            UseBoldFont = true,
                            OnClick = delegate
                            {
                                Helper.SetActorXXValue(DateFile.instance, RuntimeConfig.UI_Config.ActorId, 200);
                                Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
                            }
                        }
                    }
                };

                Func_More_Scroll.Add($"Char-Field:{"入邪值"}", new BoxAutoSizeModelGameObject
                {
                    Name = $"Char-Field:{"入邪值"}",
                    Group =
                    {
                        Spacing = 3,
                        Direction = Direction.Vertical,
                        ForceExpandChildWidth = true
                    },
                    SizeFitter =
                    {
                        VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize
                    },
                    Children =
                    {
                        new TaiwuButton
                        {
                            Name = "Button-Show",
                            Text = "入邪值",
                            UseBoldFont = true,
                            UseOutline = true,
                            OnClick = (Button button) =>
                            {
                                for(int i = 1;i<button.Parent.Children.Count;i++)
                                {
                                    button.Parent.Children[i].SetActive(!button.Parent.Children[i].IsActive);
                                }
                            },
                            Element =
                            {
                                PreferredSize = { 0 , 50 }
                            }
                        },
                        actorFields
                    }
                });
                actorFields.SetActive(false);
            }
        }
    }
}
