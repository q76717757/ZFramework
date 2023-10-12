using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public sealed class UnityEntry : Entry
    {
        public override void OnStart()
        {
            Log.Info("Game Start!");
        }
    }
}
