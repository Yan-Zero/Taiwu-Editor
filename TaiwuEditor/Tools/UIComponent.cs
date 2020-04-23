using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaiwuEditor.UI
{
    public class UIComponent
    {
        public class Text : UnityEngine.UI.Text
        {
            private RectTransform mRectTransform;

            public RectTransform RectTransform => mRectTransform;

            protected Text()
            {
                mRectTransform = gameObject.GetComponent<RectTransform>();
            }
        }

        public class Button : UnityEngine.UI.Button
        {
            private RectTransform mRectTransform;
            private Image mImage;
            private HotkeyButtonCaller mHotkeyButtonCaller;

            private Text mText;
            private UnityEngine.EventSystems.EventTrigger mEventTrigger;

            public RectTransform RectTransform => mRectTransform;
            public Image Image => mImage;
            public Text Text => mText;
            public UnityEngine.EventSystems.EventTrigger EventTrigger => mEventTrigger;

            private class HotkeyButtonCaller : MonoBehaviour
            {
                public KeyboardShortcut Hotkey;
                public UnityEngine.UI.Button Button;

                void Update()
                {
                    if (Hotkey.IsDown())
                        Button.onClick.Invoke();
                }
            }

            public KeyboardShortcut? Hotkey
            {
                get
                {
                    if (mHotkeyButtonCaller == null)
                        return null;
                    else
                        return mHotkeyButtonCaller.Hotkey;
                }
                set
                {
                    if (mHotkeyButtonCaller == null)
                    {
                        if (value == null)
                            return;
                        mHotkeyButtonCaller = gameObject.AddComponent<HotkeyButtonCaller>();
                        mHotkeyButtonCaller.Hotkey = (KeyboardShortcut)value;
                        mHotkeyButtonCaller.Button = this;
                    }
                    else
                    {
                        if (value == null)
                            Destroy(mHotkeyButtonCaller);
                        else
                            mHotkeyButtonCaller.Hotkey = (KeyboardShortcut)value;
                    }
                }
            }

            protected Button()
            {
                mRectTransform = gameObject.GetComponent<RectTransform>();
                if (mRectTransform == null)
                    mRectTransform = gameObject.AddComponent<RectTransform>();
                mImage = gameObject.AddComponent<Image>();
                mEventTrigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

                mText = new GameObject("Text").AddComponent<Text>();
                mText.RectTransform.SetParent(gameObject.transform);
                mText.RectTransform.anchorMin = new Vector2(0, 0);
                mText.RectTransform.anchorMax = new Vector2(1, 1);

                mText.alignment = TextAnchor.MiddleCenter;
            }

            public void SetImage(Image image)
            {
                Misc.Copy(mImage, image);
            }
        }
    }

    public static class Misc
    {
        public static void Copy(Image _this, Image source)
        {
            _this.color = source.color;
            _this.material = source.material;
            _this.sprite = source.sprite;
            _this.fillMethod = source.fillMethod;
            _this.preserveAspect = source.preserveAspect;
            _this.fillCenter = source.fillCenter;
            _this.alphaHitTestMinimumThreshold = source.alphaHitTestMinimumThreshold;
            _this.useSpriteMesh = source.useSpriteMesh;
            _this.fillOrigin = source.fillOrigin;
            _this.fillClockwise = source.fillClockwise;
            _this.type = source.type;
            _this.overrideSprite = source.overrideSprite;
            _this.fillAmount = source.fillAmount;
        }
    }
}
