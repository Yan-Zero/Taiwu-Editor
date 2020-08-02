using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuEditor.Script;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.UI
{
    public static partial class EditorUI
    {
        public static class AddItemUI
        {
            public static void Init(Container Func_AddItem_Container, Settings settings)
            {
                var TabFuncAddItem = Func_AddItem_Container.Get<TabFuncAddItem>();
                TabFuncAddItem.SetInstance(Func_AddItem_Container);

                TopOfFuncAddItemContainer(Func_AddItem_Container);
                BaseScroll baseScroll;
                (baseScroll = new BaseScroll
                {
                    Name = "ItemShowerHandle",
                    Group =
                    {
                        Direction = Direction.Vertical,
                        Spacing = 0,
                        Padding = { 10 },
                        ForceExpandChildWidth = true
                    }
                }).SetParent(Func_AddItem_Container);
                TabFuncAddItem.valueHander = baseScroll;
            }

            //图鉴添加物品
            public static void TopOfFuncAddItemContainer(Container Func_AddItem_Container)
            {
                new Container
                {
                    Name = "TopBar",
                    Group =
                    {
                        Spacing = 5,
                        Direction = UnityUIKit.Core.Direction.Horizontal
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
                            Element =
                            {
                                PreferredSize = { 100 , 0 }
                            },
                            UseBoldFont = true,
                            Text = "类型"
                        },
                        new ToggleGroup
                        {
                            Name = "TypeOfItem",
                            Group =
                            {
                                Spacing = 2,
                                Direction = UnityUIKit.Core.Direction.Horizontal
                            },
                            Children =
                            {
                                new TaiwuToggle
                                {
                                    Name = "All",
                                    Text = "全部",
                                    UseBoldFont = true,
                                    onValueChanged = (bool value,Toggle tg) => Func_AddItem_Container.Get<TabFuncAddItem>().ItemType = TabFuncAddItem.ItemTypes.All,
                                }
                            }
                        }
                    }
                }.SetParent(Func_AddItem_Container);
                new Container
                {
                    Name = "SeacherBar",
                    Group =
                    {
                        Spacing = 5,
                        Direction = UnityUIKit.Core.Direction.Horizontal
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
                            Element =
                            {
                                PreferredSize = { wideOfLabel , 0 }
                            },
                            UseBoldFont = true,
                            Text = "名字"
                        },
                        new TaiwuInputField
                        {
                            Name = "SeacherInput",
                            Placeholder = "请输入物品名称或 ID"
                        },
                        new TaiwuButton
                        {
                            Name = "Search",
                            Text = "搜索",
                            Element =
                            {
                                PreferredSize = { wideOfLabel , 0 }
                            },
                            UseBoldFont = true,
                            FontColor = Color.white,
                            OnClick = (Button bt) =>
                            {
                                var i = bt.Parent.Children[1] as InputField;
                                Func_AddItem_Container.Get<TabFuncAddItem>().SearchItem(i.Text);
                            }
                        }
                    }
                }.SetParent(Func_AddItem_Container);
            }
        }
    }
}
