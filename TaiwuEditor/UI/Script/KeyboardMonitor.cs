using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaiwuUIKit.GameObjects;
using UnityEngine;

namespace TaiwuEditor.Script
{
    public class KeyboardMonitor : MonoBehaviour
    {
        public bool Monitored = false;
        public FieldInfo HotkeyField;

        public Dictionary<KeyCode, bool> KeyCodes = new Dictionary<KeyCode, bool>();

        public KeyCode FinalKey;
        public TaiwuLabel ValueLabel;
        public UnityUIKit.GameObjects.Button Button;

        void Update()
        {
            if (Monitored)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse0))
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
                if (i.Value)
                    Modifiers.Add(i.Key);
            }
            filed.Value = new KeyboardShortcut(FinalKey, Modifiers.ToArray());

            ValueLabel.Text = UI.EditorUI.HotkeyUI.Hotkey_ToString(filed.Value);
            Button.Text = "修改快捷键";
            FinalKey = KeyCode.None;
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
