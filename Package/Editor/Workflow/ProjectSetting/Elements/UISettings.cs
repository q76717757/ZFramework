using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class UISettings : ZFrameworkSettingsProviderElement<UISettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();
    }
}
