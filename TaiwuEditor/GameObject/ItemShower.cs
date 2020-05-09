using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.MGO
{
    public class ItemShower : Container
    {
        public Dictionary<int, string> Item;

        public override void Create(bool active)
        {
            Element.PreferredSize = new List<float>
            {
                0 , 50
            };
            Children.Add(new TaiwuLabel
            {
                Name = "Label",
                Text = "<color=#B97D4BFF>" + DateFile.instance.GetItemDate(int.Parse(Item[999]), 0) + "</color>",
                UseBoldFont = true,
                Element =
                {
                    PreferredSize = { 0 , 0 }
                }
            });

            base.Create(active);
        }
    }
}
