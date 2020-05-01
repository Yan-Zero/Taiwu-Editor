using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.MGO
{
    public class ToggleSliderBar : Container
    {
        public TaiwuSlider Slider = new TaiwuSlider()
        {
            Name = "Slider",
            WholeNumber = true,
        };
        public TaiwuToggle Toggle = new TaiwuToggle
        {
            Name = "Toggle",
            Element =
            {
                PreferredSize = { 50, 50 }
            },
            UseBoldFont = true,
            UseOutline = true,
        };
        public TaiwuLabel TitleLabel = new TaiwuLabel
        {
            Name = "Label",
            Element =
            {
                PreferredSize = { 200, 0 }
            },
            UseBoldFont = true,
            UseOutline = true
        };
        public TaiwuLabel ValueLabel = new TaiwuLabel()
        {
            Name = "ValueLabel",
            Element =
            {
                PreferredSize = { 70 , 0 }
            },
            BackgroundStyle = TaiwuLabel.Style.Value
        };

        public bool isOn
        {
            get => Toggle.isOn;
            set => Toggle.isOn = value;
        }
        public string Title
        {
            get => TitleLabel.Text;
            set => TitleLabel.Text = value;
        }
        public string Toggle_TipTitle
        {
            get => Toggle.TipTitle;
            set => Toggle.TipTitle = value;
        }
        public string Toggle_TipContant
        {
            get => Toggle.TipContant;
            set => Toggle.TipContant = value;
        }

        public float MaxValue
        {
            get => Slider.MaxValue;
            set => Slider.MaxValue = value;
        }
        public float MinValue
        {
            get => Slider.MinValue;
            set => Slider.MinValue = value;
        }
        public float Value
        {
            get => Slider.Value;
            set => Slider.Value = value;
        }
        public bool WholeNumber
        {
            get => Slider.WholeNumber;
            set => Slider.WholeNumber = value;
        }
        public string Slider_TipTitle
        {
            get => Slider.TipTitle;
            set => Slider.TipTitle = value;
        }
        public string Slider_TipContant
        {
            get => Slider.TipContant;
            set => Slider.TipContant = value;
        }

        public string ValueFormat = "";

        public Action<bool, TaiwuToggle, ToggleSliderBar> OnToggleValueChanged;
        public Action<float, TaiwuSlider, ToggleSliderBar> OnSliderValueChanged;


        public override void Create(bool active)
        {
            Element.PreferredSize = new List<float> { 0, 50 };
            Group.Spacing = 2;
            Group.Direction = UnityUIKit.Core.Direction.Horizontal;

            Toggle.Text = Toggle.isOn ? "开" : "关";
            Toggle.onValueChanged = OnToggleValueChange;

            Slider.Interactable = isOn;
            Slider.OnValueChanged = OnSliderValueChange;
            ValueLabel.Text = Value.ToString(ValueFormat);

            Children.Add(TitleLabel);
            Children.Add(Toggle);
            Children.Add(ValueLabel);
            Children.Add(Slider);

            base.Create(active);
        }

        public void OnToggleValueChange(bool value,Toggle toggle)
        {
            OnToggleValueChanged.Invoke(value, toggle as TaiwuToggle,this);
        }

        public void OnSliderValueChange(float value, Slider slider)
        {
            ValueLabel.Text = value.ToString(ValueFormat);
            OnSliderValueChanged.Invoke(value, slider as TaiwuSlider, this);
        }
    }
}
