using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class TestDomain : Domain
    {
        protected override void OnStart()
        {
            Log.Info($"Test Process Start");

            AddComponentInDomain<TestComponent>().TestMethod();
            TestComponent a = AddComponentInDomain<TestComponent>();
            Log.Info(a.ToString());

            //Entity.Destory(a);
        }
    }
}
