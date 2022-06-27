using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public abstract class UICanvasComponent :Component
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
