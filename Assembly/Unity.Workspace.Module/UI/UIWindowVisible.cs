using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ZFramework
{
    internal class UIWindowVisible : MonoBehaviour
    {
        public int id;
        public UIGroupType groupType;
        public UIPopType popType;
        public string uiWindowName;
        public string uiWindowType;

        public void Register(UIWindowBase ui)
        {
            id = ui.ID;
            groupType = ui.GroupType;
            popType = ui.PopType;
            uiWindowType = ui.GetType().ToString();
            uiWindowName = ui.GetType().Name;
        }


    }
}
