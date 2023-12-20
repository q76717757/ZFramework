using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
   public static class ShortUtility
    {
        public static short Clamp(short value, short min, short max)
        {
            return (short)Mathf.Clamp(value, min, max);
        }
    }
}
