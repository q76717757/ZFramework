using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace ZFramework
{
    [RequireComponent(typeof(Interactable))]
    public class TouchUGUI : MonoBehaviour
    {
		public int btnIndex;
		public bool useTween = true;
		protected Hand currentHand;

		public UnityEngine.Events.UnityEvent<int> callback;

		protected virtual void Awake()
		{
			BulidBoxCollider();
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			currentHand = hand;
			InputModule.instance.HoverBegin(gameObject);
            //ControllerButtonHints.ShowButtonHint(hand, hand.uiInteractAction);

            //Valve.VR.SteamVR_Fade.View(Color.yellow,5);

            if (useTween)
            {
				transform.DOKill();
				transform.DOScale(0.95f, 0.1f);
			}
			
		}

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			InputModule.instance.HoverEnd(gameObject);
			//ControllerButtonHints.HideButtonHint(hand, hand.uiInteractAction);
			currentHand = null;

            if (useTween)
            {
				transform.DOKill();
				transform.DOScale(1f, 0.1f);
			}

			callback.Invoke(btnIndex);
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			if (hand.uiInteractAction != null && hand.uiInteractAction.GetStateDown(hand.handType))
			{
				InputModule.instance.Submit(gameObject);
				//ControllerButtonHints.HideButtonHint(hand, hand.uiInteractAction);
			}
		}

		public void BulidBoxCollider()
		{
			var box = GetComponent<BoxCollider>();
			if (box == null)
			{ 
				box= gameObject.AddComponent<BoxCollider>();
			}
			var rect = transform as RectTransform;
			if (rect != null)
			{
				box.size = new Vector3(rect.rect.width, rect.rect.height, 20);
				box.isTrigger = true;
			}
			
		}
	}
}
