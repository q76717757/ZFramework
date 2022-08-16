using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace ZFramework
{
    /// <summary>
    /// 用来监听VR的一些自定按钮  X Y A B 菜单键等
    /// </summary>
    public class TouchBtnHandler : MonoBehaviour
    {
        SteamVR_Action_Boolean btnA;
        SteamVR_Action_Boolean btnB;
        SteamVR_Action_Boolean btnX;
        SteamVR_Action_Boolean btnY;

        private void Start()
        {
            btnA = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnA");
            btnB = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnB");
            btnX = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnX");
            btnY = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnY");
        }

        private void Update()
        {
            if (btnA.GetStateDown( SteamVR_Input_Sources.Any))
            {
                Debug.Log("A");
            }
            if (btnB.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log("B");
            }
            if (btnX.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log("X");
            }
            if (btnY.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log("Y");
            }

        }

    }
}
