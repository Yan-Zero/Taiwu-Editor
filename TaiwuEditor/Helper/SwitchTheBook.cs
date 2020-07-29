using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YanLib;

namespace TaiwuEditor
{
    public class SwitchBook
    {
        /// <summary>
        /// 进行具体的切换行为
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="itemId"></param>
        static void DoSwitch(SetItem __instance, int itemId)
        {
            bool actionFlag = false;

            //Main.Logger.Log("Lets start to switch for " + __instance.itemNumber.text);

            //确认该物品为图纸或图书
            if (int.Parse(DateFile.instance.GetItemDate(itemId, 4, false)) == 5)
                //确认该物品为图书
                if (int.Parse(DateFile.instance.GetItemDate(itemId, 5, false)) == 21)
                    //确认该物品为功法书
                    if (int.Parse(DateFile.instance.GetItemDate(itemId, 506, false)) >= 20)
                        //通过处理许可
                        actionFlag = true;

            if (!actionFlag)
                return;

            //添加相应处理Component,注入参数
            int actorId = DateFile.instance.mianActorId;
            var iconobj = __instance.gameObject;
            var clickActions = iconobj.GetComponents<ClickAction>();
            if (clickActions.Length >= 1)//避免重复添加
            {
                clickActions[0].SetParam(__instance.itemNumber, itemId, actorId);
            }
            else
            {
                var actionstub = iconobj.AddComponent<ClickAction>();
                actionstub.SetParam(__instance.itemNumber, itemId, actorId);
            }
            // var actionstub = iconobj.AddComponent<ClickAction>();
            // actionstub.setParam(__instance.itemNumber, itemId);
        }

        /// <summary>
        /// 将人物包裹中的功法书籍通过点击进行真传和手抄的切换
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="actorId"></param>
        /// <param name="itemId"></param>
        /// <param name="actorFavor"></param>
        /// <param name="injuryTyp"></param>
        private static void SetItem_SetActorMenuItemIcon_Postfix(SetItem __instance, int actorId, int itemId, int actorFavor, int injuryTyp)
        {
            if (!TaiwuEditor.enabled)
                return;

            if (!GongFaDict.IsDictLoaded)
                return;

            if (actorId != DateFile.instance.mianActorId)
                return;


            DoSwitch(__instance, itemId);

        }

        /// <summary>
        /// 将人物装备中的功法书籍通过点击进行真传和手抄的切换
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="itemId"></param>
        private static void SetItem_SetActorEquipIcon_Postfix(SetItem __instance, int itemId)
        {
            if (!TaiwuEditor.enabled)
                return;

            //Dictionary<int, int> gg = new Dictionary<int, int>();
            //int cc = gg[1];

            //Main.Logger.Log("We get in for " + __instance.itemNumber.text);

            if (!GongFaDict.IsDictLoaded)
                return;
            if (ActorMenu.instance.actorId != DateFile.instance.mianActorId)
                return;
            DoSwitch(__instance, itemId);
        }

        /// <summary>
        /// 加载功法数据
        /// </summary>
        private static void DateFile_LoadDate_Prefix()
        {
            if (TaiwuEditor.enabled && TaiwuEditor.settings.SwitchTheBook.Value)
            {
                if (!GongFaDict.IsDictLoaded)
                {
                    if (DateFile.instance != null)
                    {
                        //建立功法ID的索引
                        int iterNum = 0;
                        foreach (KeyValuePair<int, Dictionary<int, string>> item in DateFile.instance.presetitemDate)
                        {
                            iterNum += 1;
                            //确认该物品为图纸或图书
                            if (int.Parse(item.Value[4]) == 5)
                                //确认该物品为图书
                                if (int.Parse(item.Value[5]) == 21)
                                    //确认该物品为功法书
                                    if (int.Parse(item.Value[506]) >= 20)
                                        //将其加入功法词典
                                        GongFaDict.Add_new_GongFa(int.Parse(item.Value[32]),
                                            int.Parse(item.Value[35]), int.Parse(item.Value[999]));
                        }
                        GongFaDict.IsDictLoaded = true;
                        TaiwuEditor.Logger.LogInfo("GongFaDict Loaded with size: " + GongFaDict.Size().ToString());
                        TaiwuEditor.Logger.LogInfo("Totally find itemsDate with size: " + iterNum);
                        TaiwuEditor.Logger.LogInfo("Dict of itemsDate with size: " + DateFile.instance.presetitemDate.Count().ToString());
                    }
                    else
                        TaiwuEditor.Logger.LogError("GongFaDict already Loaded?");
                }
                else
                    TaiwuEditor.Logger.LogError("We fail to load the GongFaDict!");
            }
        }
    }

    /// <summary>
    /// 功法数据
    /// </summary>
    internal static class GongFaDict
    {
        public static bool IsDictLoaded = false;
        private static Dictionary<int, int> gongfaDict = new Dictionary<int, int>();

        private static int _gongfa_index(int gongfaid, int ispirate)
        {
            return gongfaid * 2 + ispirate;
        }

        public static int Size()
        {
            return gongfaDict.Count();
        }

        public static void Add_new_GongFa(int gongfaid, int ispirate, int itemtrueid)
        {
            int indexKey = _gongfa_index(gongfaid, ispirate);
            if (gongfaDict.ContainsKey(indexKey))
                TaiwuEditor.Logger.LogInfo("We already have this Gongfa in GongFaDict: " + indexKey.ToString());
            else
                gongfaDict.Add(_gongfa_index(gongfaid, ispirate), itemtrueid);
        }

        public static int Get_GongFa_ID(int gongfaid, int ispirate)
        {
            int itemtrueid = -1;
            int indexKey = _gongfa_index(gongfaid, ispirate);
            if (!gongfaDict.ContainsKey(indexKey))
                TaiwuEditor.Logger.LogInfo("Fail to get Gongfa TrueID with index: " + indexKey.ToString());
            else
                itemtrueid = gongfaDict[indexKey];
            return itemtrueid;
        }
    }

    /// <summary>
    ///  鼠标事件处理
    /// </summary>
    public class ClickAction : MonoBehaviour, IPointerClickHandler
    {
        Text _text;
        int _itemid;
        int _actorid;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!TaiwuEditor.settings.SwitchTheBook.Value)
                return;

            //将该书籍的真传和手抄属性进行切换
            int gongFaID = int.Parse(DateFile.instance.GetItemDate(_itemid, 32, false));
            int isPirate = int.Parse(DateFile.instance.GetItemDate(_itemid, 35, false));
            //int itemTrueID = int.Parse(DateFile.instance.GetItemDate(_itemid, 999, false));
            int newItemTrueID = GongFaDict.Get_GongFa_ID(gongFaID, 1 - isPirate);
            if (newItemTrueID == -1)
                TaiwuEditor.Logger.LogError("We fail to get the GongFa ID:" + _text.text);
            else
                // DateFile.instance.itemsDate[_itemid][999] = newItemTrueID.ToString();
                GameData.Items.SetItemProperty(_itemid, 999, newItemTrueID.ToString());
            // DateFile.instance.ChangItemDate(_itemid, 999, newItemTrueID, true);

            DateFile.instance.GetItem(_actorid, _itemid, 1, false, 0, 0);
            // GameData.Items.GetItem(_itemid);

            TaiwuEditor.Logger.LogInfo("We may complete the book switch.");

            // ActorMenu.instance.UpdateItemInformation(_itemid);
            //if (isPirate > 0)
            //    DateFile.instance.itemsDate[_itemid][999] = (itemTrueID - 200000).ToString();
            //else
            //    DateFile.instance.itemsDate[_itemid][999] = (itemTrueID + 200000).ToString();
        }

        public void SetParam(Text text, int itemid, int actorid)
        {
            _text = text;
            _itemid = itemid;
            _actorid = actorid;
        }
    }

}
