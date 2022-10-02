using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Image = UnityEngine.UI.Image;
using TaiwuEditor.Script;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using System.Runtime.Remoting.Messaging;

namespace TaiwuEditor.UI
{
    public static partial class EditorUI
    {
        public static class MoreUI
        {
            static MoreUI()
            {
            }

            public static void Init(BaseScroll Func_More_Scroll)
            {
                RuntimeConfig.UI_Tab_Instance.Func_More_Scroll.AddComponent<TabFuncMore>().SetInstance(RuntimeConfig.UI_Tab_Instance.Func_More_Scroll);

                Func_More_Scroll.Add("未载入存档", new BaseFrame()
                {
                    Name = "未载入存档",
                    Children =
                            {
                                new BaseText()
                                {
                                    Name = "Text",
                                    Text = "本界面未实装"
                                }
                            },
                    DefaultActive = true
                });
                RuntimeConfig.UI_Tab_Instance.Func_More_Scroll.Get<TabFuncMore>().NeedUpdate = true;
            }

        }
    }
}
