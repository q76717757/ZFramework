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
    public class VRHelperAwake : AwakeSystem<VRHelperComponent>
    {
        public override void OnAwake(VRHelperComponent entity)
        {
            entity.btnA = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnA");
            entity.btnB = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnB");
            entity.btnX = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnX");
            entity.btnY = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BtnY");
            entity.snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft");
            entity.snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight");

            entity.player = Player.instance;
            if (entity.player != null)
            {
                entity.snapTurn = entity.player.GetComponentInChildren<SnapTurn>();

                //VRUI
                var defEventSystem = InputModule.instance;
                var leftHand = entity.player.leftHand;
                var rightHand = entity.player.rightHand;

                if (leftHand != null && rightHand != null && defEventSystem != null)
                {
                    if (leftHand != null && rightHand != null)
                    {
                        entity.kvr_InputModule = defEventSystem.gameObject.AddComponent<Kvr_InputModule>();

                        entity.lineLeft = leftHand.GetComponent<SteamVR_LaserPointer>();
                        entity.hasLineLeft = entity.lineLeft != null;
                        if (!entity.hasLineLeft)
                        {
                            entity.lineLeft = leftHand.gameObject.AddComponent<SteamVR_LaserPointer>();
                            entity.lineLeft.color = Color.green;
                        }
                        entity.uiPointerLeft = leftHand.gameObject.AddComponent<Kvr_UIPointer>();

                        entity.lineRight = rightHand.GetComponent<SteamVR_LaserPointer>();
                        entity.hasLineRight = entity.lineRight != null;
                        if (!entity.hasLineRight)
                        {
                            entity.lineRight = rightHand.gameObject.AddComponent<SteamVR_LaserPointer>();
                            entity.lineRight.color = Color.green;
                        }
                        entity.uiPointerRight = rightHand.gameObject.AddComponent<Kvr_UIPointer>();
                    }
                }
                entity.LineIsOn = true;
                entity.SetUILine(false);
            }

        }
    }

    public class VRTouchBtnListenerUpdate : UpdateSystem<VRHelperComponent>
    {
        public override void OnUpdate(VRHelperComponent entity)
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

    public static class VRHelperSystem
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

        public static void SetRota(this VRHelperComponent component,bool canRota)//旋转功能的开关  上车不给旋转身为 打开小地图也不给旋转身为  用左右键来转小地图
        {
            if (component.snapTurn != null)
            {
                component.snapTurn.enabled = canRota;
            }
        }

        public static void SetUILine(this VRHelperComponent component, bool isOn)//需要的时候打开UI射线 用完关闭
        {
            if (component.LineIsOn == isOn) return;
            component.LineIsOn = isOn;

            component.uiPointerLeft.enabled = isOn;
            component.uiPointerRight.enabled = isOn;

            component.lineLeft.enabled = isOn;
            if (component.lineLeft.holder)
            {
                component.lineLeft.holder.SetActive(isOn);
            }
            component.lineRight.enabled = isOn;
            if (component.lineRight.holder)
            {
                component.lineRight.holder.SetActive(isOn);
            }
        }

        public static void Move(this VRHelperComponent component, Vector3 pos)
        {
            if (component.player != null)
            {
                component.player.transform.position = pos;
            }
        }

        public static void Move(this VRHelperComponent component, Vector3 pos, Quaternion rota)
        {
            if (component.player != null)
            {
                Vector3 playerFeetOffset = component.player.trackingOriginTransform.position - component.player.feetPositionGuess;
                component.player.trackingOriginTransform.position = pos + playerFeetOffset;

                if (component.player.leftHand.currentAttachedObjectInfo.HasValue)
                    component.player.leftHand.ResetAttachedTransform(component.player.leftHand.currentAttachedObjectInfo.Value);
                if (component.player.rightHand.currentAttachedObjectInfo.HasValue)
                    component.player.rightHand.ResetAttachedTransform(component.player.rightHand.currentAttachedObjectInfo.Value);
            }
        }
    }

}
