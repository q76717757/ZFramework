using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class RefHelper : MonoBehaviour
    {
        public static References References;

        private void Awake()
        {
            References = GetComponent<References>();
        }

    }
}
