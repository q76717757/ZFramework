using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace ZFramework
{
    public class RemoteControl : MonoBehaviour
    {
        public Transform Joystick;
        public float joyMove = 0.1f;

        public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move");
        public SteamVR_Action_Boolean jumpAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("platformer", "Jump");

        public Renderer jumpHighlight;

        private Vector3 movement;
        private float glow;
        private SteamVR_Input_Sources hand;
        private Interactable interactable;



        public Transform snapTo;
        private Rigidbody body;
        public float snapTime = 2;

        private float dropTimer;


        private void Start()
        {
            interactable = GetComponent<Interactable>();
            body = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (interactable.attachedToHand)
            {
                hand = interactable.attachedToHand.handType;
                Vector2 m = moveAction[hand].axis;
                movement = new Vector3(m.x, 0, m.y);

                glow = Mathf.Lerp(glow, jumpAction[hand].state ? 1.5f : 1.0f, Time.deltaTime * 20);
            }
            else
            {
                movement = Vector2.zero;
                glow = 0;
            }

            Joystick.localPosition = movement * joyMove;

            float rot = transform.eulerAngles.y;

            movement = Quaternion.AngleAxis(rot, Vector3.up) * movement;

            jumpHighlight.sharedMaterial.SetColor("_EmissionColor", Color.white * glow);

            bool used = false;
            if (interactable != null)
                used = interactable.attachedToHand;

            if (used)
            {
                body.isKinematic = false;
                dropTimer = -1;
            }
            else
            {
                dropTimer += Time.deltaTime / (snapTime / 2);

                body.isKinematic = dropTimer > 1;

                if (dropTimer > 1)
                {
                    //transform.parent = snapTo;
                    transform.position = snapTo.position;
                    transform.rotation = snapTo.rotation;
                }
                else
                {
                    float t = Mathf.Pow(35, dropTimer);

                    body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, Time.fixedDeltaTime * 4);
                    if (body.useGravity)
                        body.AddForce(-Physics.gravity);

                    transform.position = Vector3.Lerp(transform.position, snapTo.position, Time.fixedDeltaTime * t * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, snapTo.rotation, Time.fixedDeltaTime * t * 2);
                }
            }
        }
    }
}
