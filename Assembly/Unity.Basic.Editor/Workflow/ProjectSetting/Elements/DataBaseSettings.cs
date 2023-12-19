using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class DataBaseSettings : ZFrameworkSettingsProviderElement<DataBaseSettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();
    }
}
