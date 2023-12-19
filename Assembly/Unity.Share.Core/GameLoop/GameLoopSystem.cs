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
        private readonly List<IComponent> addedComponents = new List<IComponent>();
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
                foreach (IComponent component in addedComponents)
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
                if (obj is IComponent component)
                {
                    //CallDisable(component);  //TODO  未完整实现
                    CallDestory(component);
                }
                obj.EndDispose();
            }
        }
        internal void DestoryHandler(IDispose despose,bool isImmediate)
        {
            if (despose is IComponent component)
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


        internal void CallAwake(IComponent component)
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
        internal void CallEnable(IComponent component)
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
        internal void CallDisable(IComponent component)
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
        private void CallDestory(IComponent component)
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
