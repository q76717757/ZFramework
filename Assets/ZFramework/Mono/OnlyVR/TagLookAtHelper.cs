using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace ZFramework
{
    public class TagLookAtHelper : MonoBehaviour
    {
        public Transform[] childs;

        Transform target;

        public Transform rotaTrans; 

        void Start()
        {
            var player = Player.instance;
            if (player != null)
            { 
                target = player.hmdTransform;
            }
        }

        void Update()
        {
            if (target != null && childs.Length > 0)
            {
                foreach (var item in childs)
                {
                    item.LookAt(target, Vector3.up);
                }
            }

            rotaTrans.rotation *= Quaternion.Euler(0, -Time.deltaTime, 0);
        }

    }
}
