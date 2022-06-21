/** Header
 *  ZEventListenerBase.cs
 *  所有监听的基类 定义一些整个系统共有的功能
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    public abstract class ZEventListenerBase {
        internal abstract void Recycle();
    }

    public abstract class ZEventListenerBase<EventDataBase> : ZEventListenerBase where EventDataBase : ZEventDataBase
    {
        internal void SetMethodInfo(object callbackTarget, MethodInfo callBackMethodInfo) {
            CallbackTarget = callbackTarget;
            CallbackIntPrt = callBackMethodInfo.MethodHandle.Value;

            if (CallbackTarget is Component component)
            {
                IsComponentMethod = true;
                Component = component;
                DeclaringTypeName = string.Empty;
            }
            else
            {
                IsComponentMethod = false;
                Component = null;

                if (callBackMethodInfo.IsStatic)
                    DeclaringTypeName = $"{callBackMethodInfo.DeclaringType.Name}(static)";
                else
                    DeclaringTypeName = string.Empty;
            }
            ListenerName = callBackMethodInfo.Name;
        }

        internal override void Recycle()
        {
            CallbackTarget = null;
            CallbackIntPrt = IntPtr.Zero;
            DeclaringTypeName = string.Empty;
            ListenerName = string.Empty;
            Component = null;
        }

        public string DeclaringTypeName{ get; private set; }//静态声明类型
        public string ListenerName { get; private set; }//回调的方法名  在可视化中用到的

        //回调的信息  用来进行对比  action在子类 抽象父类拿不到 没法判等 把子类action的细节存到抽象类中进行对比可以解决 当target和intPrt都相等的时候 action就是相同的
        public object CallbackTarget { get; private set; }
        public IntPtr CallbackIntPrt { get; private set; }

        public Component Component { get; private set; }
        public bool IsComponentMethod { get; private set; }
        public bool IsInvalid => IsComponentMethod && Component == null;//is MonoBehaviour

        /// <summary> 驱动传入一部分数据(动态),监听自带另一部分数据(静态) 在这里合并后派发  操作的是抽象类(也就是传递的是容器) </summary>
        public abstract void Call(EventDataBase eventData);

        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) => ReferenceEquals(this, obj);
        public static bool operator == (ZEventListenerBase<EventDataBase> x, ZEventListenerBase<EventDataBase> y) {
            if (object.ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.CallbackTarget == y.CallbackTarget && x.CallbackIntPrt == y.CallbackIntPrt;
        }
        public static bool operator !=(ZEventListenerBase<EventDataBase> x, ZEventListenerBase<EventDataBase> y) => !(x == y);
    }

}
