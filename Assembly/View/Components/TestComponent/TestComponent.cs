using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class TestAwake : OnAwakeImpl<TestComponent>
    {
        public override void OnAwake(TestComponent self)
        {
            Log.Error("Awake");
        }
    }
    public class TestUpdate : OnUpdateImpl<TestComponent>
    {
        public override void OnUpdate(TestComponent self)
        {
            Log.Error("Update" + (self == null).ToString());
            if (Input.GetKeyDown(KeyCode.A))
            {
                Log.Error("----A----");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Entity.Destory(self);
                self.TestMethod();
            }
        }
    }
    public class TestLateUpdate : OnLateUpdateImpl<TestComponent>
    {
        public override void OnLateUpdate(TestComponent self)
        {
            Log.Error("LateUpdate");
            if (Input.GetKeyDown(KeyCode.A))
            {
                Log.Error("----B----");
            }
        }
    }
    public class TestDestory : OnDestoryImpl<TestComponent>
    {
        public override void OnDestory(TestComponent self)
        {
            Log.Error("Destory");
        }
    }
    public class TestReload : OnReloadImpl<TestComponent>
    {
        public override void OnReload(TestComponent self)
        {
            Log.Error("Reload");
        }
    }

    public class TestComponent : Component
    {
        public void TestMethod()
        {
            Log.Error("TestMethod");
        }
    }
}