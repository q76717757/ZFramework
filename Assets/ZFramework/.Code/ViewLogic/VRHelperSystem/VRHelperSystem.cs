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
            entity.remoteControl = RefHelper.References.Get<RemoteControl>("RemoteControl");
            entity.remoteControl.onAttached += entity.OnHoldRemoteControl;

            entity.player = Player.instance;
            var defInputModule = InputModule.instance;

            var snapTurn = entity.player.GetComponentInChildren<SnapTurn>();//把自带的转头去掉 用自定义的
            if (snapTurn != null)
            {
                snapTurn.gameObject.SetActive(false);
            }

            //VRUI
            var leftHand = entity.player.leftHand;
            var rightHand = entity.player.rightHand;

            if (leftHand != null && rightHand != null && defInputModule != null)
            {
                entity.kvr_InputModule = defInputModule.gameObject.AddComponent<Kvr_InputModule>();

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
            entity.SetUILine(false);
        }
    }

    public class VRTouchBtnListenerUpdate : UpdateSystem<VRHelperComponent>
    {
        public override void OnUpdate(VRHelperComponent entity)
        {
            //菜单
            if (entity.player.leftHand.currentAttachedObject == null && entity.player.rightHand.currentAttachedObject == null)
            {
                if (entity.btnA != null && entity.btnA.activeBinding && entity.btnA.GetStateDown(SteamVR_Input_Sources.Any))//地图
                {
                    entity.Process.GetComponent<VRMapComponent>().Show();
                }
                if (entity.btnB != null && entity.btnB.activeBinding && entity.btnB.GetStateDown(SteamVR_Input_Sources.Any))//菜单
                {
                    entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
                }
                if (entity.btnX != null && entity.btnX.activeBinding && entity.btnX.GetStateDown(SteamVR_Input_Sources.Any))//地图
                {
                    entity.Process.GetComponent<VRMapComponent>().Show();
                }
                if (entity.btnY != null && entity.btnY.activeBinding && entity.btnY.GetStateDown(SteamVR_Input_Sources.Any))//菜单
                {
                    entity.Process.GetComponent<UIComponent>().Show(UIType.Canvas_Menu);
                }
            }
            //摇杆
            if (entity.snapLeftAction != null && entity.snapRightAction != null && entity.snapLeftAction.activeBinding && entity.snapRightAction.activeBinding)
            {
                //左手
                if (entity.player.leftHand.currentAttachedObject == null)
                {
                    if (entity.snapLeftAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
                    {
                        entity.RotatePlayer(-60);
                    }
                    if (entity.snapRightAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
                    {
                        entity.RotatePlayer(60);
                    }
                }
                else
                {
                    if (entity.remoteControl.attached && entity.remoteControl.hand == SteamVR_Input_Sources.LeftHand)
                    {
                        var videoCom = entity.Process.GetComponent<UIComponent>().Get(UIType.Canvas_Video) as Canvas_VideoComponent;
                        if (entity.snapLeftAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
                        {
                            videoCom.CallLeft();
                        }
                        if (entity.snapRightAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
                        {
                            videoCom.CallRight();
                        }
                    }
                }
                //右手
                if (entity.player.rightHand.currentAttachedObject == null)
                {
                    if (entity.snapLeftAction.GetStateDown(SteamVR_Input_Sources.RightHand))
                    {
                        entity.RotatePlayer(-60);
                    }
                    if (entity.snapRightAction.GetStateDown(SteamVR_Input_Sources.RightHand))
                    {
                        entity.RotatePlayer(60);
                    }
                }
                else
                {
                    if (entity.remoteControl.attached && entity.remoteControl.hand == SteamVR_Input_Sources.RightHand)
                    {
                        var videoCom = entity.Process.GetComponent<UIComponent>().Get(UIType.Canvas_Video) as Canvas_VideoComponent;
                        if (entity.snapLeftAction.GetStateDown(SteamVR_Input_Sources.RightHand))
                        {
                            videoCom.CallLeft();
                        }
                        if (entity.snapRightAction.GetStateDown(SteamVR_Input_Sources.RightHand))
                        {
                            videoCom.CallRight();
                        }
                    }
                }

                //地图旋转
                bool holdLeft = entity.snapLeftAction.GetState(SteamVR_Input_Sources.Any);
                bool holdRight = entity.snapRightAction.GetState(SteamVR_Input_Sources.Any);
                if (holdLeft && entity.player.leftHand.currentAttachedObject == null && entity.player.rightHand.currentAttachedObject == null)
                {
                    entity.Process.GetComponent<VRMapComponent>().Rotation(45);
                }
                if (holdRight && entity.player.leftHand.currentAttachedObject == null && entity.player.rightHand.currentAttachedObject == null)
                {
                    entity.Process.GetComponent<VRMapComponent>().Rotation(-45);
                }
            }
        }
    }

    public static class VRHelperSystem
    {
        public static void SetUILine(this VRHelperComponent component, bool isOn)//需要的时候打开UI射线 用完关闭
        {
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
        public static void SetUILine(this VRHelperComponent component, SteamVR_Input_Sources handType, bool isOn)
        {
            if (handType == SteamVR_Input_Sources.LeftHand)
            {
                component.uiPointerLeft.enabled = isOn;
                component.lineLeft.enabled = isOn;
                if (component.lineLeft.holder)
                {
                    component.lineLeft.holder.SetActive(isOn);
                }
            }
            if (handType == SteamVR_Input_Sources.RightHand)
            {
                component.uiPointerRight.enabled = isOn;
                component.lineRight.enabled = isOn;
                if (component.lineRight.holder)
                {
                    component.lineRight.holder.SetActive(isOn);
                }
            }
        }

        public static void Move(this VRHelperComponent component, Vector3 pos)
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

        public static void OnHoldRemoteControl(this VRHelperComponent component, SteamVR_Input_Sources handType , bool isHold)
        {
            if (isHold)
            {
                component.Process.GetComponent<UIComponent>().Hide(UIType.Canvas_Menu);
                component.Process.GetComponent<VRMapComponent>().Hide();
                component.SetUILine(SteamVR_Input_Sources.LeftHand, handType == SteamVR_Input_Sources.LeftHand);
                component.SetUILine(SteamVR_Input_Sources.RightHand, handType == SteamVR_Input_Sources.RightHand);
            }
            else
            {
                component.SetUILine(false);
            }
        }

        public static void RotatePlayer(this VRHelperComponent component, float angle)
        {
            Vector3 playerFeetOffset = component.player.trackingOriginTransform.position - component.player.feetPositionGuess;
            component.player.trackingOriginTransform.position -= playerFeetOffset;
            component.player.transform.Rotate(Vector3.up, angle);
            playerFeetOffset = Quaternion.Euler(0.0f, angle, 0.0f) * playerFeetOffset;
            component.player.trackingOriginTransform.position += playerFeetOffset;
        }

    }

}
