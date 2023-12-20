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
        public bool isModal;
        public string uiWindowName;
        public string uiWindowType;

        public void Register(UIWindowBase ui)
        {
            id = ui.ID;
            groupType = ui.GroupType;
            isModal = ui.IsModal;
            uiWindowType = ui.GetType().ToString();
            uiWindowName = ui.GetType().Name;
        }


    }
}
