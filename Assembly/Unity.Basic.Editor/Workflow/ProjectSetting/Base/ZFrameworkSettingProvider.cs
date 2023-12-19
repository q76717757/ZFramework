using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ZFrameworkSettingProvider : SettingsProvider
    {
        public static string Path => "Project/ZFramework";
        public ZFrameworkSettingProvider() : base(Path, SettingsScope.Project) { }
        [SettingsProvider] public static SettingsProvider Register() => new ZFrameworkSettingProvider();
        public static void OpenSettings() => SettingsService.OpenProjectSettings(Path);

        public override void OnGUI(string searchContext)
        {
        }
    }

}
