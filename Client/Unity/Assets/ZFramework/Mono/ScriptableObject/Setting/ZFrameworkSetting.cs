using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [CreateAssetMenu(fileName = "NewAudioLibrary", menuName = "ZFramework/TestProjectSetting", order = 1)]
    public class ZFrameworkSetting: ScriptableObject
    {
        public int aa;
        public int bb;
        public string cc;
    }
}
