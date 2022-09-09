#if VR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class VRMapComponent : ComponentData
    {
        public bool isOn;

        public Transform transform;
        public Transform[] part;
        public Vector3[] pos;

        public int holding;

    }
}
#endif