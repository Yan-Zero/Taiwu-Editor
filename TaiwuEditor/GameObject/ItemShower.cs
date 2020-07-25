using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.MGO
{
    public class ItemShower : Container
    {
        public Dictionary<int, string> Item;

        public override void Create(bool active)
        {
            Group.Direction = UnityUIKit.Core.Direction.Horizontal;
            Element.PreferredSize = new List<float>
            {
                0 , 75
            };

            var lable = new TaiwuLabel
            {
                Name = $"Label,{Item[999]}",
                Text = "<color=#B97D4BFF>" + DateFile.instance.GetItemDate(int.Parse(Item[999]), 0) + "</color>",
                UseBoldFont = true,
                Element =
                {
                    PreferredSize = { 0 , 50 }
                }
            };

            Children.Add(lable);

            Children.Add(new TaiwuButton
            {
                Name = "Add",
                Text = "添加",
                UseBoldFont = true,
                UseOutline = true,
                Element =
                {
                    PreferredSize = { 75 , 50 }
                },
                FontColor = Color.white,
                OnClick = AddItem
            });

            base.Create(active);

            lable.GameObject.tag = "ActorItem";
            lable.Get<PointerEnter>();
        }

        public void AddItem(Button button)
        {
            DateFile.instance.GetItem(DateFile.instance.MianActorID(), int.Parse(Item[999]), 1, true, 0);
        }
    }

}
