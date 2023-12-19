using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class UISettingAttribute : BaseAttribute
    {
        public string Path { get; }
        public UIGroupType GroupType { get; }
        public UIPopType UIPopType { get; }
        private UISettingAttribute() { }
        public UISettingAttribute(string path, UIGroupType groupType, UIPopType uiPopType = UIPopType.Pop)
        {
            this.Path = path;
            this.GroupType = groupType;
            this.UIPopType = uiPopType;
        }
    }
}
