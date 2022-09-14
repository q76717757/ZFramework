#if VR
using UnityEngine;
using Valve.VR;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

namespace KVR_UI
{
    public struct UIPointerEventArgs
    {
        public bool isActive;
        public GameObject currentTarget;
        public GameObject previousTarget;
        public RaycastResult raycastResult;
    }

    public delegate void UIPointerEventHandler(object sender, UIPointerEventArgs e);

    public class Kvr_UIPointer : MonoBehaviour
    {
        public enum ClickMethods
        {
            ClickOnButtonUp,
            ClickOnButtonDown
        }

        internal ClickMethods clickMethod = ClickMethods.ClickOnButtonUp;

        [HideInInspector]
        public bool collisionClick = false;


        [HideInInspector]
        public GameObject autoActivatingCanvas = null;

        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        [HideInInspector]
        public float hoverDurationTimer = 0f;
        [HideInInspector]
        public bool canClickOnHover = false;

        internal Transform pointerOriginTransform = null;

        protected bool pointerClicked = false;
        protected bool beamEnabledState = false;
        protected bool lastPointerPressState = false;
        protected bool lastPointerClickState = false;

        protected GameObject currentTarget;

        protected EventSystem cachedEventSystem;
        protected Kvr_InputModule cachedVRInputModule;
        protected Transform originalPointerOriginTransform;

        public event UIPointerEventHandler UIPointerElementEnter;

        public event UIPointerEventHandler UIPointerElementExit;

        public event UIPointerEventHandler UIPointerElementClick;

        public event UIPointerEventHandler UIPointerElementDragStart;

        public event UIPointerEventHandler UIPointerElementDragEnd;

        public static bool TouchBtnValue = false;

        //[Header("UI交互动作"), SerializeField]
        private SteamVR_Action_Boolean action = SteamVR_Actions.default_InteractUI;

        //[Header("显示激光"), SerializeField]
        private bool enableLaserPointer = true;

        private SteamVR_LaserPointer laserPointer;

        public virtual bool PointerActive() { return true; }

        public virtual bool IsSelectionButtonPressed() { return action.state; }

        public virtual Vector3 GetOriginPosition() { return (pointerOriginTransform ? pointerOriginTransform.position : transform.position); }

        public virtual Vector3 GetOriginForward() { return pointerOriginTransform ? pointerOriginTransform.forward : transform.forward; }

        public virtual bool ValidClick(bool checkLastClick, bool lastClickState = false)
        {
            var controllerClicked = (collisionClick ? collisionClick : IsSelectionButtonPressed());
            var result = (checkLastClick ? controllerClicked : controllerClicked);
            return result;
        }

        protected virtual void ResetHoverTimer()
        {
            hoverDurationTimer = 0f;
            canClickOnHover = false;
        }

        public virtual void OnUIPointerElementEnter(UIPointerEventArgs e)
        {
            if (e.currentTarget != currentTarget)
            {
                ResetHoverTimer();
            }

            currentTarget = e.currentTarget;
            if (UIPointerElementEnter != null)
            {
                UIPointerElementEnter(this, e);
            }
        }

        public virtual void OnUIPointerElementExit(UIPointerEventArgs e)
        {
            if (e.previousTarget == currentTarget)
            {
                ResetHoverTimer();
            }
            if (UIPointerElementExit != null)
            {
                UIPointerElementExit(this, e);
            }
        }

        public virtual void OnUIPointerElementClick(UIPointerEventArgs e)
        {
            if (e.currentTarget == currentTarget)
            {
                ResetHoverTimer();
            }

            if (UIPointerElementClick != null)
            {
                UIPointerElementClick(this, e);
            }
        }

        public virtual void OnUIPointerElementDragStart(UIPointerEventArgs e)
        {
            if (UIPointerElementDragStart != null)
            {
                UIPointerElementDragStart(this, e);
            }
        }

        public virtual void OnUIPointerElementDragEnd(UIPointerEventArgs e)
        {
            if (UIPointerElementDragEnd != null)
            {
                UIPointerElementDragEnd(this, e);
            }
        }

        public virtual UIPointerEventArgs SetUIPointerEvent(RaycastResult currentRaycastResult, GameObject currentTarget, GameObject lastTarget = null)
        {
            UIPointerEventArgs e;
            e.isActive = PointerActive();
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            e.raycastResult = currentRaycastResult;
            return e;
        }

        protected virtual void Awake()
        {
            originalPointerOriginTransform = pointerOriginTransform;
            laserPointer = GetComponent<SteamVR_LaserPointer>();
        }

        protected virtual void OnEnable()
        {
            pointerOriginTransform = originalPointerOriginTransform;

            ConfigureEventSystem();
            pointerClicked = false;
            lastPointerPressState = false;
            lastPointerClickState = false;
            beamEnabledState = false;

            if (!enableLaserPointer)
            {
                laserPointer.enabled = false;
                action = null;
                return;
            }
        }

        protected virtual void OnDisable()
        {
            Kvr_InputModule.RemovePoint(this);
        }

        protected virtual void ConfigureEventSystem()
        {
            if (!cachedEventSystem)
            {
                cachedEventSystem = FindObjectOfType<EventSystem>();
            }

            if (!cachedVRInputModule)
            {
                cachedVRInputModule = cachedEventSystem.GetComponent<Kvr_InputModule>();
            }

            if (cachedEventSystem && cachedVRInputModule)
            {
                if (pointerEventData == null)
                {
                    pointerEventData = new PointerEventData(cachedEventSystem);
                }
            }

            Kvr_InputModule.AddPoint(this);
        }
    }
}
#endif