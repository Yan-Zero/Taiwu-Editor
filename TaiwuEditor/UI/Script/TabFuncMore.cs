using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using TaiwuEditor.UI;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace TaiwuEditor.Script
{
    public class TabFuncMore : MonoBehaviour
    {
        private BaseScroll instance;
        public bool NeedUpdate = false;

        void OnGUI()
        {
        }

        public void SetInstance(BaseScroll instance)
        {
            this.instance = instance;
        }
    }
}
