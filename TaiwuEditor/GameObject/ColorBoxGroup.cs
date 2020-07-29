using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using Image = UnityEngine.UI.Image;

namespace TaiwuEditor.GameObject
{
    public class ColorBoxGroup : BoxGridGameObject
    {
        private static readonly Image _backgroundImage = null;
        static ColorBoxGroup()
        {
            _backgroundImage = Resources.Load<UnityEngine.GameObject>("prefabs/ui/views/ui_systemsetting").transform.Find("SystemSetting/SetBackup/BackupIntervalSlider").Find("Background").GetComponent<CImage>() as Image;
        }
        public virtual Image Res_BackgroundImage => _backgroundImage;

        public List<Color> Colors = new List<Color>();
        internal List<ColorBox> ColorBoxes = new List<ColorBox>();

        /// <summary>
        /// 返回 Index
        /// </summary>
        public Action<int, ColorBoxGroup> onValueChanged = delegate { };

        public int GetSelectedColorIndex
        {
            get
            {
                for (int i = 0; i < ColorBoxes.Count; i++)
                    if (ColorBoxes[i].isOn)
                        return i;
                ColorBoxes[0].isOn = true;
                return 0;
            }
        }
        public Color GetSelectedColor
        {
            get => Colors[GetSelectedColorIndex];
        }

        public override void Create(bool active)
        {
            Grid.Spacing = new Vector2(1,1);
            Grid.CellSize = new Vector2(30, 20);
            Grid.ConstraintCount = 10;
            base.Create(active);

            if (Res_BackgroundImage != null)
            {
                var bg = Get<Image>();
                bg.type = Res_BackgroundImage.type;
                bg.sprite = Res_BackgroundImage.sprite;
                bg.color = Res_BackgroundImage.color;
            }

            Get<UnityEngine.UI.LayoutElement>().preferredHeight = (Colors.Count / 10) * 20 + 50;
            Get<UnityEngine.UI.LayoutElement>().preferredWidth = (Colors.Count / 10f) >= 1 ? 340 : (Colors.Count * 30 + 40);


            for (int i = 0; i < Colors.Count; i++)
            {
                var colorBox = new ColorBox()
                {
                    Name = i.ToString(),
                    Color = Colors[i],
                    Element =
                    {
                        PreferredSize = { 30 , 20 }
                    },
                    onValueChanged = (bool value, Toggle tg) =>
                    {
                        if (value && tg.Parent is ColorBoxGroup)
                            (tg.Parent as ColorBoxGroup).onValueChanged?.Invoke(int.Parse(tg.Name), (tg.Parent as ColorBoxGroup));
                    }
                };
                colorBox.SetParent(this);
                colorBox.Get<UnityEngine.UI.Toggle>().group = Get<UnityEngine.UI.ToggleGroup>();
                ColorBoxes.Add(colorBox);
            }
        }

        public void ChoseIndex(int Index)
        {
            if (Index < 0)
                Index = 0;
            if (Index >= ColorBoxes.Count)
                Index = ColorBoxes.Count - 1;
            ColorBoxes[Index].isOn = true;
        }
    }
}
