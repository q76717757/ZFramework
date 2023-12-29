using System;
using System.Collections.Generic;

namespace ZFramework
{
    /// <summary>
    /// 生命周期管理系统
    /// </summary>
    internal sealed class GameLoopSystem
    {
        //已经执行了awake 等待加入update/lateupdate队列
        private readonly List<BasedComponent> addedComponents = new List<BasedComponent>();
        //被调了destory  等到帧末再执行destory生命周期
        private readonly Queue<IDispose> removedObjects = new Queue<IDispose>();//包括entity和component

        private Queue<IUpdate> updates = new Queue<IUpdate>();
        private Queue<IUpdate> updates2 = new Queue<IUpdate>();
        private Queue<ILateUpdate> lateupdate = new Queue<ILateUpdate>();
        private Queue<ILateUpdate> lateupdate2 = new Queue<ILateUpdate>();

        internal void Update()
        {
            //Add
            if (addedComponents.Count > 0)
            {
                foreach (BasedComponent component in addedComponents)
                {
                    //CallEnable(component);  //TODO  未完整实现

                    if (component is IUpdate update)
                    {
                        updates.Enqueue(update);
                    }
                    if (component is ILateUpdate lateUpdate)
                    {
                        lateupdate.Enqueue(lateUpdate);
                    }
                }
                addedComponents.Clear();
            }

            //Update
            while (updates.Count > 0)
            {
                IUpdate component = updates.Dequeue();
                if (component.IsDisposed)
                {
                    continue;
                }
                try
                {
                    component.Update();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                finally
                {
                    updates2.Enqueue(component);
                }
            }
            (updates, updates2) = (updates2, updates);
        }
        internal void LateUpdate()
        {
            //LateUpdate
            while (lateupdate.Count > 0)
            {
                ILateUpdate component = lateupdate.Dequeue();
                if (component.IsDisposed)//已经销毁或者是本帧销毁
                {
                    continue;
                }
                try
                {
                    component.LateUpdate();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                finally
                {
                    lateupdate2.Enqueue(component);
                }
            }
            (lateupdate, lateupdate2) = (lateupdate2, lateupdate);

            //Destory
            while (removedObjects.Count > 0)
            {
                IDispose obj = removedObjects.Dequeue();
                if (obj is BasedComponent component)
                {
                    //CallDisable(component);  //TODO  未完整实现
                    CallDestory(component);
                }
                obj.EndDispose();
            }
        }
        internal void DestoryHandler(IDispose despose,bool isImmediate)
        {
            if (despose is BasedComponent component)
            {
                for (int i = 0; i < addedComponents.Count; i++)
                {
                    if (addedComponents[i] == component)
                    {
                        addedComponents.RemoveAt(i);
                        break;
                    }
                }
                if (isImmediate)
                {
                    CallDestory(component);
                }
            }
            if (isImmediate)
            {
                despose.EndDispose();
            }
            else
            {
                removedObjects.Enqueue(despose);
            }
        }

        //当组件使用带参AddComponent添加组件时, 会执行两次Awake  先执行有参 后执行无参
        //有时候需要在无参awake执行前就要做点事情,那么用带参的AddComponent正好可以插队在awake前面
        internal void CallAwake(BasedComponent component)
        {
            if (component is IAwake awake)
            {
                try
                {
                    awake.Awake();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            addedComponents.Add(component);
        }
        internal void CallAwake<A>(BasedComponent component,A a)
        {
            if (component is IAwake<A> awake)
            {
                try
                {
                    awake.Awake(a);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        internal void CallAwake<A, B>(BasedComponent component,A a,B b)
        {
            if (component is IAwake<A, B> awake)
            {
                try
                {
                    awake.Awake(a, b);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        internal void CallAwake<A, B, C>(BasedComponent component,A a,B b,C c)
        {
            if (component is IAwake<A, B, C> awake)
            {
                try
                {
                    awake.Awake(a, b, c);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        internal void CallEnable(BasedComponent component)
        {
            if (component is IEnable enable)
            {
                try
                {
                    enable.OnEnable();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        internal void CallDisable(BasedComponent component)
        {
            if (component is IDisable disable)
            {
                try
                {
                    disable.OnDisable();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        private void CallDestory(BasedComponent component)
        {
            if (component is IDestory destory)
            {
                try
                {
                    destory.OnDestory();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

    }
}
