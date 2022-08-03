using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/Boot File", order = 1)]
    public class BootFile : ScriptableObject
    {
        public int projectName;
    }
}
