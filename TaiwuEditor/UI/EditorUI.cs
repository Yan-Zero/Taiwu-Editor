using HarmonyLib;
using System;
using System.Linq;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.GameObjects;
using YanLib;

namespace TaiwuEditor.UI
{
    public static partial class EditorUI
    {
        public static readonly int wideOfLabel = 200;

        /// <summary>
        /// 初始化UI
        /// </summary>
        public static Container.CanvasContainer PrepareGUI()
        {
            var toggleGroup = new ToggleGroup()
            {
                Name = "Func.Choose",
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 5
                },
                Element =
                {
                    PreferredSize = { 0, 50 }
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = "Text",
                        Text = "功能选择",
                        Element =
                        {
                            PreferredSize = { 150, 0 }
                        },
                        UseOutline = true,
                        UseBoldFont = true
                    }
                }
            };
            RuntimeConfig.UI_Config.overlay = new Container.CanvasContainer()
            {
                Name = "TaiwuEditor.Canvas",
                Group =
                {
                    Padding = { 0 },
                },
                Children =
                {
                    (RuntimeConfig.UI_Config.windows = new TaiwuWindows()
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
                            toggleGroup
                        },
                        Element =
                        {
                            PreferredSize = { 1400, 1000 }
                        },
                    }),
                }
            };


            foreach (var fieldName in AccessTools.GetFieldNames(typeof(RuntimeConfig.UI_Tab_Instance)))
            {
                var field = AccessTools.Field(typeof(RuntimeConfig.UI_Tab_Instance), fieldName);
                if (field.FieldType == typeof(BaseScroll))
                {
                    field.SetValue(null, new BaseScroll()
                    {
                        Name = fieldName,
                        Group =
                        {
                            Direction = Direction.Vertical,
                            Spacing = 15,
                            Padding = { 10 },
                            ForceExpandChildWidth = true
                        },
                        DefaultActive = false
                    });
                }
                else if (field.FieldType == typeof(Container))
                {
                    field.SetValue(null, new Container()
                    {
                        Name = fieldName,
                        Group =
                        {
                            Direction = Direction.Vertical,
                            Spacing = 5,
                            Padding = { 10 },
                            ForceExpandChildWidth = true
                        },
                        DefaultActive = false
                    });
                }
                else
                {
                    throw new TypeLoadException("暂时不支持的 Type");
                }

                MGOInfoAttribute info = Attribute.GetCustomAttribute(field, typeof(MGOInfoAttribute)) as MGOInfoAttribute;
                if (info == null)
                {
                    info = new MGOInfoAttribute()
                    {
                        Name = fieldName,
                        Order = 1
                    };
                }

                if (info.Order <= 0)
                    info.Order = 1;

                if (info.Order <= toggleGroup.Children.Count)
                    toggleGroup.Children.Insert(info.Order, new TaiwuToggle()
                    {
                        Name = fieldName,
                        Text = info.Name ?? fieldName,
                        UseBoldFont = true,
                        UseOutline = true,
                        onValueChanged = (bool value, Toggle Toggle) =>
                        {
                            var i = AccessTools.Field(typeof(RuntimeConfig.UI_Tab_Instance), Toggle.Name).GetValue(null) as ManagedGameObject;
                            i.SetActive(value);
                        },
                    });
                else
                    toggleGroup.Children.Add(new TaiwuToggle()
                    {
                        Name = fieldName,
                        Text = info.Name ?? fieldName,
                        UseBoldFont = true,
                        UseOutline = true,
                        onValueChanged = (bool value, Toggle Toggle) =>
                        {
                            var i = AccessTools.Field(typeof(RuntimeConfig.UI_Config), Toggle.Name).GetValue(null) as ManagedGameObject;
                            i.SetActive(value);
                        },
                    });

                RuntimeConfig.UI_Config.windows.Children.Add(field.GetValue(null) as ManagedGameObject);
            }

            return RuntimeConfig.UI_Config.overlay;
        }

        public static void TryInit(Settings settings)
        {
            foreach(var mgo in RuntimeConfig.UI_Config.windows.Children[0].Children)
            {
                var toggle = mgo as Toggle;
                if (toggle != null)
                {
                    var field = AccessTools.Field(typeof(RuntimeConfig.UI_Tab_Instance), toggle.Name);
                    MGOInfoAttribute info = Attribute.GetCustomAttribute(field, typeof(MGOInfoAttribute)) as MGOInfoAttribute;
                    if (info == null)
                        info = new MGOInfoAttribute();

                    Type InitType = null;

                    if(info.InitType != null)
                        InitType = info.InitType;
                    else if (!string.IsNullOrEmpty(info.InitTypeName))
                        InitType = Type.GetType(typeof(EditorUI).FullName + "+" + info.InitTypeName);

                    var init = AccessTools.Method(InitType, "Init");

                    object[] parm = null;
                    if (init.GetParameters().Count() == 1)
                    {
                        parm = new object[] { field };
                    }
                    else
                    {
                        parm = new object[] { field, settings };
                    }

                    init.Invoke(null, parm);
                }
            }
        }
    }
}