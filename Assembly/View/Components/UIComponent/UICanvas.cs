using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public abstract class UICanvas
    {
        public GameObject gameObject;


        public References Refs;

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            Refs = gameObject.GetComponent<References>();
            OnShow();
        }
        public abstract void OnShow();
        public abstract void OnHide();
    }
}
