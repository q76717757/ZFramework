using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public enum Platform
    {
        Win,
        Mac,
        Andorid,
        IOS,
        WebGL,
        UWP
    }
    public enum Network
    {
        None,
        Exist,
        Unknow,
    }

    public class GlobalConfig : ScriptableObject
    {
        public Platform platform;
        public Network network;

        public bool allowOffline;
    }
}
