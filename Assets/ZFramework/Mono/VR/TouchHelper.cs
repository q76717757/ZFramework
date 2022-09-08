#if UNITY_VR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;

namespace ZFramework
{
	//public enum AttachmentFlags
	//{
	//	SnapOnAttach = 1 << 0, // 抓取的瞬间，调整抓取物位置，以适应手柄状态  该对象应该吸附到手上指定的连接点的位置。
	//	DetachOthers = 1 << 1, // 附在这只手上的其他物体将被分离。
	//	DetachFromOtherHand = 1 << 2, // 一只手抓取时，另一只手也要抓取同一个物体时，原手自动松开。
	//	ParentToHand = 1 << 3, // 该对象将成为手的父对象
	//	VelocityMovement = 1 << 4, // 物体将尝试移动以匹配手的位置和旋转。
	//	TurnOnKinematic = 1 << 5, // 这个物体不会对外部物理反应。
	//	TurnOffGravity = 1 << 6, // 这个物体不会对外部物理反应。
	//	AllowSidegrade = 1 << 7, // 该物体能够从捏抓器切换到抓握器。既减少了投掷成功的可能性，也减少了意外掉落的可能性
	//};

	[RequireComponent(typeof(Interactable))]
    public class TouchHelper : MonoBehaviour
    {
		private Interactable interactable;

		//private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);
		//拾取的物体
		public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;//车门
		//-------------------------------------------------
		void Awake()
		{
			interactable = this.GetComponent<Interactable>();
		}

		public Action<Hand> OnEnter;
        // 当手开始悬停在该对象上时调用
        private void OnHandHoverBegin(Hand hand)
		{
			OnEnter?.Invoke(hand);
			Debug.Log("OnHandHoverBegin ->" + hand.name);
		}

		// 当手停止悬停在该物体上时调用//joint1_M
		public Action<Hand> OnExit;
		private void OnHandHoverEnd(Hand hand)
		{
			OnExit?.Invoke(hand);
			Debug.Log("OnHandHoverEnd ->" + hand.name);
		}

		public Action<Hand> OnClient;
		// 当一个手悬停在这个对象上时调用every Update()
		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes startingGrabType = hand.GetGrabStarting();
			if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
			{
				// 调用这个来继续接收HandHoverUpdate消息，
				// 并防止手悬停在任何其他东西上
				//hand.HoverLock(interactable);

				// 把这个物体系在手上
				//hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
				OnClient?.Invoke(hand);
			}
            //else if (hand.IsGrabEnding(this.gameObject))
            //{
            //    // 从手上取下这个物体
            //    hand.DetachObject(gameObject);

            //    // 调用该函数来撤销HoverLock
            //    hand.HoverUnlock(interactable);
            //}
        }

		// 当GameObject连接到手上时调用
		private void OnAttachedToHand(Hand hand)
		{
			Debug.Log("OnAttachedToHand ->" + hand.name);
		}

		// 当GameObject从手部分离时调用
		private void OnDetachedFromHand(Hand hand)
		{
			Debug.Log("OnDetachedFromHand ->" + hand.name);
		}

		// 当GameObject附在手上时调用每个Update()
		private void HandAttachedUpdate(Hand hand)
		{

		}

		// 当这个附加的游戏对象成为主要附加对象时调用
		private void OnHandFocusAcquired(Hand hand)
		{
			Debug.Log("OnHandFocusAcquired ->" + hand.name);
		}

		// 当另一个附加的游戏对象成为主要附加对象时调用
		private void OnHandFocusLost(Hand hand)
		{
			Debug.Log("OnHandFocusLost ->" + hand.name);
		}







		//public void AnimateHandWithController()
		//{
		//	for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
		//	{
		//		Hand hand = Player.instance.hands[handIndex];
		//		if (hand != null)
		//		{
		//			hand.SetSkeletonRangeOfMotion(Valve.VR.EVRSkeletalMotionRange.WithController);
		//		}
		//	}
		//}
		//public void AnimateHandWithoutController()
		//{
		//	for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
		//	{
		//		Hand hand = Player.instance.hands[handIndex];
		//		if (hand != null)
		//		{
		//			hand.SetSkeletonRangeOfMotion(Valve.VR.EVRSkeletalMotionRange.WithoutController);
		//		}
		//	}
		//}
		public void ShowController()
		{
			for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
			{
				Hand hand = Player.instance.hands[handIndex];
				if (hand != null)
				{
					hand.ShowController(true);
				}
			}
		}
		public void HideController()
		{
			for (int handIndex = 0; handIndex < Player.instance.hands.Length; handIndex++)
			{
				Hand hand = Player.instance.hands[handIndex];
				if (hand != null)
				{
					hand.HideController(true);
				}
			}
		}
	}
}
#endif