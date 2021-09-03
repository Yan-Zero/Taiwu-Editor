using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaiwuEditor.GameObject;
using TaiwuEditor.UI;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.Script
{
    public class TabFuncMore : MonoBehaviour
    {
        public static readonly int[] map_to_name_id = { 5, 0, 30 };

        private BaseScroll instance;

        public TaiwuLabel NameValue;
        public TaiwuInputField ActorID_InputField;

        public bool NeedUpdate = false;
        public List<BoxGridGameObject> DataFields = new List<BoxGridGameObject>();

        public TaiwuActorFace TaiwuActorFace = null;

        public TaiwuToggle GenderChange = null;
        public TaiwuToggle Male = null;
        public TaiwuToggle Female = null;
        public ColorBoxGroup[] ColorBoxGroups = new ColorBoxGroup[8];
        public TaiwuLabel[] AppValueLable = new TaiwuLabel[8];

        public TaiwuInputField[] NameChange = new TaiwuInputField[3];

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
                RuntimeConfig.UI_Config.ActorId = DateFile.instance.mianActorId;
                return;
            }
            if(!GameData.Characters.HasChar(RuntimeConfig.UI_Config.ActorId))
            {
                RuntimeConfig.UI_Config.ActorId = DateFile.instance.mianActorId;
                ActorID_InputField.Text = RuntimeConfig.UI_Config.ActorId.ToString();
            }
            if (NameValue != null)
                NameValue.Text = $"<color=#FFA500>{DateFile.instance.GetActorName(RuntimeConfig.UI_Config.ActorId)}</color>";
            if (ActorID_InputField != null && ActorID_InputField.ReadOnly)
                ActorID_InputField.Text = RuntimeConfig.UI_Config.ActorId.ToString();
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

                UpdateActorFace();

                if (TaiwuActorFace.Gender == 1)
                    Male.isOn = true;
                else
                    Female.isOn = true;
                if (TaiwuActorFace.GenderChange == 0)
                    GenderChange.isOn = false;
                else
                    GenderChange.isOn = true;

                string actorDate = DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, 0, applyBonus: false);
                TaiwuButton tb_cn = (TaiwuButton)NameChange[0].Parent.Children.Find((x) => { return x.Name == "Save"; });
                tb_cn.UnityButton.enabled = actorDate != "";
                if (tb_cn.UnityButton.enabled)
                    for (int i = 0; i < 3; ++i)
                    {
                        NameChange[i].ReadOnly = false;
                        NameChange[i].Text = DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, map_to_name_id[i], applyBonus: false);
                    }
                else
                    foreach (var i in NameChange)
                    {
                        i.ReadOnly = true;
                        i.Text = "这家伙的改不了";
                    }

                NeedUpdate = false;
            }
        }

        public void SetInstance(BaseScroll instance)
        {
            this.instance = instance;
        }

        public void UpdateActorFace()
        {
            var baseFace = (DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, 995) ?? "0").Split('|');
            if (baseFace.Length == 1)
            {
                instance.ContentChildren["AppChange"].SetActive(false);
                return;
            }
            else
                instance.ContentChildren["AppChange"].SetActive(true);
            TaiwuActorFace.Age = DateFile.instance.GetActorFaceAge(RuntimeConfig.UI_Config.ActorId);
            TaiwuActorFace.Gender = int.Parse(DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, 14, false) ?? "1");
            TaiwuActorFace.GenderChange = int.Parse((DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, 17)) ?? "0");
            TaiwuActorFace.FaceData = new int[baseFace.Length];
            for (int i = 0; i < TaiwuActorFace.FaceData.Length; i++)
                TaiwuActorFace.FaceData[i] = int.Parse(baseFace[i] ?? "0");
            var baseColor = (DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, 996) ?? "0").Split('|');
            TaiwuActorFace.FaceColor = new int[baseColor.Length];
            for (int i = 0; i < TaiwuActorFace.FaceColor.Length; i++)
                TaiwuActorFace.FaceColor[i] = int.Parse(baseColor[i] ?? "0");
            int clothes = int.Parse(DateFile.instance.GetActorDate(RuntimeConfig.UI_Config.ActorId, 305) ?? "0");
            if (clothes <= 0)
                clothes = 0;
            else
                clothes = int.Parse(DateFile.instance.presetitemDate[int.Parse(GameData.Items.GetItemProperty(clothes, 999) ?? "0")][15] ?? "0");
            TaiwuActorFace.ClothesIndex = clothes;

            for(int i = 0;i< 8;i++)
            {
                AppValueLable[i].Text = $"{TaiwuActorFace.FaceData[i] + 1} / {SingletonObject.getInstance<DynamicSetSprite>().GetGroupLength("actorFace", TaiwuActorFace.AppGender - 1, 0, EditorUI.MoreUI.KeyTable[i])}";
                ColorBoxGroups[i].ChoseIndex(TaiwuActorFace.FaceColor[i]);
            }
        }

        /// <summary>
        /// 获取游戏属性数据到修改框
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
}
