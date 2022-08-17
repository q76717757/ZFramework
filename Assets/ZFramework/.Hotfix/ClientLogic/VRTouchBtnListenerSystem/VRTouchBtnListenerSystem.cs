using KVR_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;

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
            entity.snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft");
            entity.snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight");

            var player = Player.instance;
            if (player != null)
            {
                entity.snapTurn = player.GetComponentInChildren<SnapTurn>();

                //VRUI
                var defEventSystem = InputModule.instance;
                if (defEventSystem != null)
                {
                    entity.kvr_InputModule = defEventSystem.gameObject.AddComponent<Kvr_InputModule>();
                }

                var leftHand = player.leftHand;
                if (leftHand != null)
                {
                    entity.lineLeft =  leftHand.GetComponent<SteamVR_LaserPointer>();
                    if (entity.lineLeft == null)
                    {
                        entity.lineLeft = leftHand.gameObject.AddComponent<SteamVR_LaserPointer>();
                        entity.lineLeft.color = Color.green;
                    }
                    entity.uiPointerLeft = leftHand.gameObject.AddComponent<Kvr_UIPointer>();
                }

                var rightHand = player.rightHand;
                if (rightHand != null)
                {
                    entity.lineRight = rightHand.GetComponent<SteamVR_LaserPointer>();
                    if (entity.lineRight == null)
                    {
                        entity.lineRight = rightHand.gameObject.AddComponent<SteamVR_LaserPointer>();
                        entity.lineRight.color = Color.green;
                    }
                    entity.uiPointerRight = rightHand.gameObject.AddComponent<Kvr_UIPointer>();
                }

            }

        }
    }

    public class VRTouchBtnListenerUpdate : UpdateSystem<VRTouchBtnListenerComponent>
    {
        public override void OnUpdate(VRTouchBtnListenerComponent entity)
        {
            if (entity.btnA != null && entity.btnA.activeBinding && entity.btnA.GetStateDown(SteamVR_Input_Sources.Any))
            {
                entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
            }
            if (entity.btnB != null && entity.btnB.activeBinding && entity.btnB.GetStateDown(SteamVR_Input_Sources.Any))
            {
                entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
            }
            if (entity.btnX != null && entity.btnX.activeBinding && entity.btnX.GetStateDown(SteamVR_Input_Sources.Any))
            {
                entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
            }
            if (entity.btnY != null && entity.btnY.activeBinding && entity.btnY.GetStateDown(SteamVR_Input_Sources.Any))
            {
                entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
            }

            if (entity.snapLeftAction != null && entity.snapRightAction != null && entity.snapLeftAction.activeBinding && entity.snapRightAction.activeBinding)
            {
                //bool left = entity.snapLeftAction.GetStateDown(SteamVR_Input_Sources.LeftHand) || entity.snapLeftAction.GetStateDown(SteamVR_Input_Sources.RightHand);
                //bool right = entity.snapRightAction.GetStateDown(SteamVR_Input_Sources.LeftHand) || entity.snapRightAction.GetStateDown(SteamVR_Input_Sources.RightHand);
                //if (left)
                //{
                //    Log.Info("Left");
                //}
                //if (right)
                //{
                //    Log.Info("Right");
                //}

                bool holdLeft = entity.snapLeftAction.GetState(SteamVR_Input_Sources.Any);
                bool holdRight = entity.snapRightAction.GetState(SteamVR_Input_Sources.Any);
                if (holdLeft)
                {
                    Log.Info("holdLeft");
                }
                if (holdRight)
                {
                    Log.Info("holdRight");
                }
            }

        }
    }

    public static class VRTouchBtnListenerSystem
    {
        //private void Pulse()//手柄震动
        //{
        //    if (hand && (hand.isActive) && (hand.GetBestGrabbingType() != GrabTypes.None))
        //    {
        //        ushort duration = (ushort)Random.Range(minimumPulseDuration, maximumPulseDuration + 1);
        //        hand.TriggerHapticPulse(duration);

        //        onPulse.Invoke();
        //    }
        //}



    }

}
