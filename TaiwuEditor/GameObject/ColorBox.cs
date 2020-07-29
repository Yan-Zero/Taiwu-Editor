using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.GameObject
{
    internal class ColorBox : Toggle
    {
        private static readonly PointerClick _pointerClick;
        private static readonly UnityEngine.UI.Image _BackgroundImage;
        private static readonly UnityEngine.UI.Image _image;
        private static readonly UnityEngine.UI.ColorBlock _colors;
        private static readonly UnityEngine.UI.Selectable.Transition _transition;


        public override UnityEngine.UI.Image Res_Image => _BackgroundImage;
        public virtual PointerClick Res_PointerClick => _pointerClick;
        public override UnityEngine.UI.ColorBlock Res_Colors => _colors;
        public virtual UnityEngine.UI.Selectable.Transition Res_Transition => _transition;


        static ColorBox()
        {
            var ColorBox = Resources.Load<UnityEngine.GameObject>("OldScenePrefabs/NewGameMenu").transform.Find("NewGameBack/FaceView/FaceBack/FaceSettingHolder/NoseNameBack/ColorNameBack/FaceColorHolder/ColorBox");
            _pointerClick = ColorBox.GetComponent<PointerClick>();
            _BackgroundImage = ColorBox.GetComponent<UnityEngine.UI.Toggle>().graphic as UnityEngine.UI.Image;
            _image = ColorBox.GetComponent<UnityEngine.UI.Image>();
            _colors = ColorBox.GetComponent<UnityEngine.UI.Toggle>().colors;
            _transition = ColorBox.GetComponent<UnityEngine.UI.Toggle>().transition;
        }

        public Color Color;
        public override void Create(bool active)
        { 
            base.Create(active);

            BoxModelGameObject BackgroundContainer;
            (BackgroundContainer = new BoxModelGameObject()
            {
                Name = "Label"
            }).SetParent(this);

            var bgOn = BackgroundContainer.Get<UnityEngine.UI.Image>();
            bgOn.type = Res_Image.type;
            bgOn.sprite = Res_Image.sprite;
            bgOn.color = Res_Image.color;

            BackgroundContainer.RectTransform.sizeDelta = Vector2.zero;
            BackgroundContainer.RectTransform.anchoredPosition = Vector2.zero;
            BackgroundContainer.RectTransform.pivot = new Vector2(0.5f,0.5f);
            BackgroundContainer.RectTransform.anchorMin = Vector2.zero;
            BackgroundContainer.RectTransform.anchorMax = Vector2.one;
            BackgroundContainer.RectTransform.offsetMin = new Vector2(-1,-1);
            BackgroundContainer.RectTransform.offsetMax = new Vector2(0.5f, 0.5f);
            BackgroundContainer.RectTransform.SetAsFirstSibling();

            BackgroundContainer.Get<UnityEngine.UI.LayoutElement>().ignoreLayout = true;
            Get<UnityEngine.UI.Toggle>().graphic = bgOn;
            Get<UnityEngine.UI.Toggle>().colors = Res_Colors;
            Get<UnityEngine.UI.Toggle>().transition = Res_Transition;

            if (Res_PointerClick != null)
            {
                var pc = Get<PointerClick>();
                pc.playSE = Res_PointerClick.playSE;
                pc.SEKey = Res_PointerClick.SEKey;
            }

            Get<UnityEngine.UI.Image>().type = _image.type;
            Get<UnityEngine.UI.Image>().sprite = _image.sprite;
            Get<UnityEngine.UI.Image>().color = Color;
        }
    }
}
