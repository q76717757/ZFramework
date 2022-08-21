using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

namespace ZFramework
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Rigidbody))]
    public class RemoteControl : MonoBehaviour
    {

        public Transform Joystick;
        public float joyMove = 0.1f;
        public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move");
        public Renderer jumpHighlight;

        private Vector3 movement;
        private float glow;
        private SteamVR_Input_Sources hand;
        private Interactable interactable;
        private Vector3 brithPos;
        private Quaternion brithQua;
        private Rigidbody body;
        private float dropTimer;


        public System.Action<SteamVR_Input_Sources,bool> onAttached;

        private void Start()
        {
            body = GetComponent<Rigidbody>();
            body.maxAngularVelocity = 50.0f;

            interactable = GetComponent<Interactable>();
            interactable.hideHandOnAttach = true;

            brithPos = transform.position;
            brithQua = transform.rotation;
        }

        private void Update()
        {
            if (interactable.attachedToHand)
            {
                hand = interactable.attachedToHand.handType;
                Vector2 m = moveAction[hand].axis;
                movement = new Vector3(m.x, 0, m.y);
                glow = 1;
            }
            else
            {
                movement = Vector2.zero;
                glow = 0;
            }

            Joystick.localPosition = movement * joyMove;
            jumpHighlight.sharedMaterial.SetColor("_EmissionColor", Color.white * glow);

        }

        private void FixedUpdate()
        {
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
                dropTimer += Time.deltaTime;

                body.isKinematic = dropTimer > 1;

                if (dropTimer > 1)
                {
                    transform.position = brithPos;
                    transform.rotation = brithQua;
                }
                else
                {
                    float t = Mathf.Pow(35, dropTimer);

                    body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, Time.fixedDeltaTime * 4);
                    if (body.useGravity)
                        body.AddForce(-Physics.gravity);

                    transform.position = Vector3.Lerp(transform.position, brithPos, Time.fixedDeltaTime * t * 3);
                    transform.rotation = Quaternion.Slerp(transform.rotation, brithQua, Time.fixedDeltaTime * t * 2);
                }
            }
        }


        [EnumFlags]
        [Tooltip("The flags used to attach this object to the hand.")]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
        public Transform attachmentOffset;

        public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;

        [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
        public float releaseVelocityTimeOffset = -0.011f;

        public float scaleReleaseVelocity = 1.1f;

        [Tooltip("The release velocity magnitude representing the end of the scale release velocity curve. (-1 to disable)")]
        public float scaleReleaseVelocityThreshold = -1.0f;
        [Tooltip("Use this curve to ease into the scaled release velocity based on the magnitude of the measured release velocity. This allows greater differentiation between a drop, toss, and throw.")]
        public AnimationCurve scaleReleaseVelocityCurve = AnimationCurve.EaseInOut(0.0f, 0.1f, 1.0f, 1.0f);

        [Tooltip("When detaching the object, should it return to its original parent?")]
        public bool restoreOriginalParent = false;

        public bool attached = false;
        protected float attachTime;
        protected Vector3 attachPosition;
        protected Quaternion attachRotation;
        protected Transform attachEaseInTransform;

        protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        protected virtual void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                if (hand.currentAttachedObject == null)
                {
                    hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
                }
            }
        }

        protected virtual void OnAttachedToHand(Hand hand)
        {
            hadInterpolation = body.interpolation;
            attached = true;
            onAttached?.Invoke(hand.handType, true);

            //onPickUp.Invoke();

            hand.HoverLock(null);

            body.interpolation = RigidbodyInterpolation.None;

            attachTime = Time.time;
            attachPosition = transform.position;
            attachRotation = transform.rotation;

        }
        protected virtual void HandAttachedUpdate(Hand hand)
        {
            if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetStateDown(hand.handType))
            {
                hand.DetachObject(hand.currentAttachedObject, restoreOriginalParent);
            }

            //if (hand.IsGrabEnding(this.gameObject))
            //{
            //    hand.DetachObject(gameObject, restoreOriginalParent);

            //    // Uncomment to detach ourselves late in the frame.
            //    // This is so that any vehicles the player is attached to
            //    // have a chance to finish updating themselves.
            //    // If we detach now, our position could be behind what it
            //    // will be at the end of the frame, and the object may appear
            //    // to teleport behind the hand when the player releases it.
            //    //StartCoroutine( LateDetach( hand ) );
            //}

            //if (onHeldUpdate != null)
            //    onHeldUpdate.Invoke(hand);
        }
        protected virtual void OnDetachedFromHand(Hand hand)
        {
            attached = false;
            onAttached?.Invoke(hand.handType, false);
            //onDetachFromHand.Invoke();

            hand.HoverUnlock(null);

            body.interpolation = hadInterpolation;

            Vector3 velocity;
            Vector3 angularVelocity;

            GetReleaseVelocities(hand, out velocity, out angularVelocity);

            body.velocity = velocity;
            body.angularVelocity = angularVelocity;

        }


        public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
                releaseVelocityStyle = ReleaseStyle.ShortEstimation; // only type that works with fallback hand is short estimation.

            switch (releaseVelocityStyle)
            {
                case ReleaseStyle.ShortEstimation:
                    Debug.LogWarning("[SteamVR Interaction System] Throwable: No Velocity Estimator component on object but release style set to short estimation. Please add one or change the release style.");

                    velocity = body.velocity;
                    angularVelocity = body.angularVelocity;
                    break;
                case ReleaseStyle.AdvancedEstimation:
                    hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                    break;
                case ReleaseStyle.GetFromHand:
                    velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                    angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                    break;
                default:
                case ReleaseStyle.NoChange:
                    velocity = body.velocity;
                    angularVelocity = body.angularVelocity;
                    break;
            }

            if (releaseVelocityStyle != ReleaseStyle.NoChange)
            {
                float scaleFactor = 1.0f;
                if (scaleReleaseVelocityThreshold > 0)
                {
                    scaleFactor = Mathf.Clamp01(scaleReleaseVelocityCurve.Evaluate(velocity.magnitude / scaleReleaseVelocityThreshold));
                }

                velocity *= (scaleFactor * scaleReleaseVelocity);
            }
        }
        protected virtual IEnumerator LateDetach(Hand hand)
        {
            yield return new WaitForEndOfFrame();

            hand.DetachObject(gameObject, restoreOriginalParent);
        }

    }
}
