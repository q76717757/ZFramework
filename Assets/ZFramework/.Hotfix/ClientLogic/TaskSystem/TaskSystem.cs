using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class TaskComponentAwake : AwakeSystem<TaskComponent>
    {
        public override void OnAwake(TaskComponent entity)
        {
        }
    }

    public static class TaskSystem
    {
        public static void Start(this TaskComponent component)
        {
        }

        public static void Stop(this TaskComponent component)
        {

        }
    }
}
