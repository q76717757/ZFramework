using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;
using UnityEngine.UI;

namespace ZFramework
{
    [RequireComponent(typeof(Interactable))]
    public class TouchUGUI : MonoBehaviour
    {
		protected Hand currentHand;
		protected virtual void Awake()
		{

		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			currentHand = hand;
			InputModule.instance.HoverBegin(gameObject);
			//ControllerButtonHints.ShowButtonHint(hand, hand.uiInteractAction);
			Debug.Log("1");

			//Valve.VR.SteamVR_Fade.View(Color.yellow,5);
        }

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			InputModule.instance.HoverEnd(gameObject);
			//ControllerButtonHints.HideButtonHint(hand, hand.uiInteractAction);
			currentHand = null;
			Debug.Log("2");
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			if (hand.uiInteractAction != null && hand.uiInteractAction.GetStateDown(hand.handType))
			{
				InputModule.instance.Submit(gameObject);
				//ControllerButtonHints.HideButtonHint(hand, hand.uiInteractAction);
				Debug.Log("3");
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
