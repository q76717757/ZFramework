using UnityEngine;

namespace ZFramework
{
    public abstract class UICanvasComponent :Entity
    {
        public GameObject gameObject;
        public RectTransform rect;

        private References _ref;
        public References Refs
        {
            get {
                if (_ref == null && gameObject != null)
                {
                    _ref = gameObject.GetComponent<References>();
                }
                return _ref;
            }
        }
    }
}
