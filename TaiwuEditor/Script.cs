using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor
{
    public class EditorBoxMore : MonoBehaviour
    {
        private BaseScroll instance;

        public TaiwuLabel NameValue;
        public TaiwuInputField ActorID_InputField;

        public bool NeedUpdate = false;
        public List<BoxGridGameObject> DataFields = new List<BoxGridGameObject>();

        void OnGUI()
        {
            if (DateFile.instance == null || DateFile.instance.mianActorId == 0)
            {
                if(!instance.ContentChildren["未载入存档"].IsActive)
                    foreach(var i in instance.ContentChildren)
                    {
                        if (i.Key != "未载入存档")
                            i.Value.SetActive(false);
                        else
                            i.Value.SetActive(true);
                    }
                return;
            }
            if (instance.ContentChildren["未载入存档"].IsActive)
            {
                foreach (var i in instance.ContentChildren)
                {
                    if (i.Key != "未载入存档")
                        i.Value.SetActive(true);
                    else
                        i.Value.SetActive(false);
                }
                return;
            }
            if (NameValue != null)
            {
                NameValue.Text = $"<color=#FFA500>{DateFile.instance.GetActorName(RuntimeConfig.UI_Config.ActorId)}</color>";
            }
            if (ActorID_InputField != null && ActorID_InputField.ReadOnly)
            {
                ActorID_InputField.Text = RuntimeConfig.UI_Config.ActorId.ToString();
            }
            if (NeedUpdate)
            {
                foreach(var tab in DataFields)
                {
                    if (tab.Name == "太吾专属")
                    {
                        if (tab.IsActive && RuntimeConfig.UI_Config.ActorId != DateFile.instance.mianActorId)
                        {
                            tab.SetActive(false);
                            tab.Parent.Children[2].SetActive(false);
                        }
                        else if (RuntimeConfig.UI_Config.ActorId != DateFile.instance.mianActorId)
                            continue;
                    }
                    foreach (var field in tab.Children)
                    {
                        (field.Children[1] as TaiwuInputField).Text = FetchFieldValueHelper(int.Parse(field.Name));
                    }
                }
                var evilValue = Helper.LifeDateHelper(DateFile.instance, RuntimeConfig.UI_Config.ActorId, 501);
                (instance.ContentChildren[$"Char-Field:{"入邪值"}"].Children[1].Children[1] as TaiwuLabel)
                    .Text = evilValue == -1 ? "无" : evilValue.ToString();
                NeedUpdate = false;
            }
        }


        public void SetInstance(BaseScroll instance)
        {
            this.instance = instance;
        }


        /// <summary>
        /// 获取游戏属性数据到修改框数值缓存
        /// </summary>
        /// <param name="resid">字段</param>
        /// <param name="ActorID">角色ID</param>
        /// <returns></returns>
        public static string FetchFieldValueHelper(int resid,int? ActorID = null)
        {
            string result = "";
            int currentActorId = ActorID ?? RuntimeConfig.UI_Config.ActorId;

            switch (resid)
            {
                case -1:
                    result = DateFile.instance.gongFaExperienceP.ToString();
                    break;
                case 12: // 获取健康数据
                    result = DateFile.instance.Health(currentActorId).ToString();
                    break;
                default:
                    var value = GameData.Characters.GetCharProperty(currentActorId, resid);
                    if (value == null)
                    {
                        int id;
                        if (GameData.Characters.HasChar(currentActorId))
                        {
                            string charProperty = GameData.Characters.GetCharProperty(currentActorId, 997);
                            id = (charProperty != null) ? int.Parse(charProperty) : currentActorId;
                        }
                        else
                        {
                            id = currentActorId;
                        }
                        Dictionary<int, string> tmpDict = null;  // 无意义，只为了省略泛型函数的参数
                        if (!DateFile.instance.presetActorDate.TryGetValue(id, resid, out value, tmpDict))
                        {
                            value = "0";
                        }
                    }
                    result = value;
                    break;
            }

            return result;
        }
    }

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

    public class KeyboardMonitor : MonoBehaviour
    {
        public bool Monitored = false;
        public FieldInfo HotkeyField;

        public Dictionary<KeyCode,bool> KeyCodes = new Dictionary<KeyCode, bool>();

        public KeyCode FinalKey;
        public TaiwuLabel ValueLabel;
        public UnityUIKit.GameObjects.Button Button;

        void Update()
        {
            if(Monitored)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
                {
                    Monitored = false;
                    KeyCodes = new Dictionary<KeyCode, bool>();
                    FinalKey = KeyCode.None;
                    Button.Text = "修改快捷键";
                }
                else
                    foreach (var i in KeyboardShortcut.AllKeyCodes)
                    {
                        if (Input.GetKeyDown(i))
                        {
                            KeyCodes[i] = true;
                            FinalKey = i;
                        }
                        if (Input.GetKeyUp(i))
                        {
                            KeyCodes[i] = false;
                            if (FinalKey == i)
                            {
                                Monitored = false;
                                SetHotkey();
                                break;
                            }
                        }
                    }
            }
        }

        public void SetHotkey()
        {
            var filed = HotkeyField.GetValue(TaiwuEditor.settings.Hotkey) as ConfigEntry<KeyboardShortcut>;
            List<KeyCode> Modifiers = new List<KeyCode>();
            KeyCodes[FinalKey] = false;
            foreach (var i in KeyCodes)
            {
                if(i.Value)
                    Modifiers.Add(i.Key);
            }
            filed.Value = new KeyboardShortcut(FinalKey, Modifiers.ToArray());

            ValueLabel.Text = Core.UI.EditorUI.HotkeyUI.Hotkey_ToString(filed.Value);
            Button.Text = "修改快捷键";
            KeyCodes = new Dictionary<KeyCode, bool>();
        }

        public void Monitoring(FieldInfo ConfigField, TaiwuLabel Label, UnityUIKit.GameObjects.Button bt)
        {
            Monitored = !Monitored;
            if (Monitored)
            {
                HotkeyField = ConfigField;
                ValueLabel = Label;
                Button = bt;
                bt.Text = "请按下按键";
            }
            else
            {
                Monitored = false;
                KeyCodes = new Dictionary<KeyCode, bool>();
                FinalKey = KeyCode.None;
                Button.Text = "修改快捷键";
            }
        }
    }
}
