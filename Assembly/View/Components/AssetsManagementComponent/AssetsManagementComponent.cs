using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class AssetsManagementComponentAwake : OnAwakeImpl<AssetsManagementComponent>
    {
        public override void OnAwake(AssetsManagementComponent self)
        {
            self.Awake();
        }
    }
    public class AssetsManagementComponentDestory : OnDestoryImpl<AssetsManagementComponent>
    {
        public override void OnDestory(AssetsManagementComponent self)
        {
            self.OnDestroy();
        }
    }

    public class AssetsManagementComponent : Component
    {

        public void Awake() {
        }
        public void OnDestroy()
        {
        }


    }
}
