using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace ZFramework
{
    public class VRTouchBtnListenerAwake : AwakeSystem<VRTouchBtnListenerComponent>
    {
        public override void OnAwake(VRTouchBtnListenerComponent entity)
        {
            entity.btnA = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnA");
            entity.btnB = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnB");
            entity.btnX = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnX");
            entity.btnY = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnY");
        }
    }

    public class VRTouchBtnListenerUpdate : UpdateSystem<VRTouchBtnListenerComponent>
    {
        public override void OnUpdate(VRTouchBtnListenerComponent entity)
        {
            if (entity.btnA.GetStateDown(SteamVR_Input_Sources.Any) ||
                entity.btnB.GetStateDown(SteamVR_Input_Sources.Any) ||
                entity.btnX.GetStateDown(SteamVR_Input_Sources.Any) ||
                entity.btnY.GetStateDown(SteamVR_Input_Sources.Any))
            {
                entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
                //Debug.Log("A");
            }
        }
    }

    public static class VRTouchBtnListenerSystem
    {

    }

}
