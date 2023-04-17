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
            Log.Info("Start -->");
            Application.targetFrameRate = 60;
            //Game.AddSingleComponent<ZEventTemp>();
            //Game.AddSingleComponent<EventSystem>();


            //var uiCam = BootStrap.instance.GetComponent<References>().Get<Camera>("UICamera");
            //Root.AddComponent<UIComponent, Camera>(uiCam);
        }
    }
}