using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using TaiwuEditor.MGO;
using UnityUIKit.Core.GameObjects;

namespace TaiwuEditor.Script
{
    public class TabFuncAddItem : MonoBehaviour
    {
        public enum ItemTypes
        {
            All = 0b111111,
        }

        private Container instance;
        public BaseScroll valueHander;
        private ItemTypes itemType = ItemTypes.All;
        public ItemTypes ItemType
        {
            get => itemType;
            set
            {
                itemType = value;
                NeedUpdate = true;
            }
        }
        public bool NeedUpdate = false;


        void OnGUI()
        {

        }


        public void SetInstance(Container instance)
        {
            this.instance = instance;
        }

        public void SearchItem(string NameOrID)
        {
            UpdateValue(searchItem(NameOrID));
        }

        private void UpdateValue(List<Dictionary<int, string>> value)
        {
            var SiblingIndex = valueHander.RectTransform.GetSiblingIndex();
            valueHander.Destroy(true);

            valueHander = new BaseScroll
            {
                Name = "ItemShowerHandle",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 0,
                    Padding = { 10 },
                    ForceExpandChildWidth = true
                }
            };
            valueHander.SetParent(instance);
            valueHander.RectTransform.SetSiblingIndex(SiblingIndex);

            var ItemShowerHandle = new BoxGridGameObject
            {
                Name = "ItemShowerHandle",
                Grid =
                {
                    CellSize = { x = 0, y = 75 },
                    Spacing = { x = 5 , y = 5 },
                    AutoWidth = true,
                    Constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount,
                    ConstraintCount = 4,
                },
                SizeFitter =
                {
                    VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize,
                }
            };

            foreach(var i in value)
            {
                ItemShowerHandle.Children.Add(new ItemShower
                {
                    Name = $"ItemID-{i[999]}",
                    Item = i
                });
            }
            valueHander.Add(ItemShowerHandle.Name, ItemShowerHandle);
        }

        private List<Dictionary<int, string>> searchItem(string NameOrID)
        {
            var result = new Dictionary<int,Dictionary<int, string>>();

            if (int.TryParse(NameOrID, out int Id))
            {
                if (DateFile.instance.presetitemDate.TryGetValue(Id, out var i))
                {
                    if (i[4] != "0")
                        result[Id] = i;
                }
            }

            foreach (var i in DateFile.instance.presetitemDate.Keys)
            {
                if(DateFile.instance.presetitemDate[i][0].Contains(NameOrID))
                {
                    if(DateFile.instance.presetitemDate[i][4] != "0")
                        result[i] = DateFile.instance.presetitemDate[i];
                }
            }

            return result.Values.ToList();
        }
    }
}
