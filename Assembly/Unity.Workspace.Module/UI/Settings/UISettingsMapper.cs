using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    internal class UISettingsMapper
    {
        private Dictionary<Type, UISettingAttribute> uiSettingMap;

        internal void Load()
        {
            uiSettingMap = new Dictionary<Type, UISettingAttribute>();
            foreach (var item in Game.GetTypesByAttribute<UISettingAttribute>())
            {
                UISettingAttribute uiSetting = item.GetCustomAttribute<UISettingAttribute>();
                uiSettingMap.Add(item, uiSetting);
            }
        }

        internal UISettingAttribute GetSettings(Type uiType)
        {
            if (!uiSettingMap.TryGetValue(uiType, out UISettingAttribute uiSetting))
                throw new Exception($"UISettingAttribute不存在，请在UI{uiType}上标记特性");
            return uiSetting;
        }
    }
}
