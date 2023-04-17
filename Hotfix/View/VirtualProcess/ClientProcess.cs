using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.Profiling;
using Cysharp.Threading.Tasks;

namespace ZFramework
{
    public class ClientProcess : VirtualProcess
    {
        public override void Start()
        {
            Application.targetFrameRate = 60;

            //GameObject CAR = GameObject.Find("T2X (1)");
            //Root.AddChild().AddComponent<Car>().Awake(CAR);
        }
    }
}
