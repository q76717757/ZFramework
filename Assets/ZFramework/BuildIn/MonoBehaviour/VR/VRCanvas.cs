using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class VRCanvas : MonoBehaviour
    {

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            var btns = GetComponentsInChildren<TouchUGUI>();
            foreach (var item in btns)
            {
                item.BulidBoxCollider();
            }
        }

    }
}
