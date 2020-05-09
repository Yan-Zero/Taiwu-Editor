using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaiwuEditor.Script
{
    public class ClickHelper : MonoBehaviour, IPointerClickHandler
    {
        public Action<ClickHelper> OnClick;
        public int ClickCount = 0;
        public void OnPointerClick(PointerEventData eventData)
        {
            ClickCount++;
            OnClick.Invoke(this);
        }
    }

    public class Clock_Text : MonoBehaviour
    {
        private Text Text;

        void Awake()
        {
            Text = gameObject.GetComponent<Text>();
        }

        void OnGUI()
        {
            Text.text = $"手记({DateTime.Now.ToString("HH:mm:ss")})";
        }
    }
}
