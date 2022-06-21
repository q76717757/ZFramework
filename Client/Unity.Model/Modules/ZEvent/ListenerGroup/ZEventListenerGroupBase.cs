/** Header
 *  ZEventListenerGroupBase.cs
 *  监听组的基类 定义了所有类型的监听组共同拥有的功能
 **/

using System;
using System.Collections.Generic;

namespace ZFramework
{
    public abstract class ZEventListenerGroupBase {
        internal abstract void Recycle();
        public abstract int RealCount { get; }
    }
    public abstract class ZEventListenerGroupBase<EventDataBase, EventListenerBase>: ZEventListenerGroupBase
        where EventDataBase : ZEventDataBase
        where EventListenerBase : ZEventListenerBase<EventDataBase>
    {
        private List<EventListenerBase> InnerList { get; } = new List<EventListenerBase>();//一个target一个List   多数情况都是只有1长度的
        public EventListenerBase this[int index] { get => InnerList[index]; set => InnerList[index] = value; }

        /// <summary> 包括了null </summary>
        public int Count => InnerList.Count;
        public void Clear() {
            for (int i = 0; i < Count; i++)
            {
                if (InnerList[i] != null)
                {
                    InnerList[i].Recycle();
                    InnerList[i] = null;
                }
            }
        }
        /// <summary> 不包括Null </summary>
        public override int RealCount {
            get {
                int count = 0;
                for (int i = 0; i < Count; i++)
                {
                    if (InnerList[i] != null)
                        count++;
                }
                return count;
            }
        }

        internal override void Recycle() => Clear();

        //整个事件系统设计最核心的方法就在下面
        internal void DispatchAll(EventDataBase eventData)  
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                if (InnerList[i] != null)
                {
                    if (InnerList[i].IsInvalid)
                    {
                        InnerList[i].Recycle();
                        InnerList[i] = null;
                        continue;
                    }
                    try
                    {
                        var cache = InnerList[i];
                        InnerList[i].Call(eventData);

                        //判断call有没有移除自己的操作
                        if (InnerList[i] == cache)//这一步可以解决上面这种情况的BUG(虽然基本不可能发生)
                        {
                            if (AutoRemoveJob(InnerList[i],eventData))
                            {
                                InnerList[i].Recycle();
                                InnerList[i] = null;
                            }
                        }
                        //有一种情况 在call的时候移除自己 同时添加新的监听  有可能导致i不为空 但已经变了对象
                        //当上家和自己同时都是autoremove的时候  就有可能因为上家的位置被下家占了 正好下家也是autoremove  导致下家没触发就没了
                        //call的前后记录一下this[i]  然后判断有没有变化 就知道有没有在call里面换监听了
                    }
                    catch (Exception e)
                    {
                        Log.Error($"注册的回调存在错误:{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}");
                    }
                    
                }
            }
        }
        internal abstract bool AutoRemoveJob(EventListenerBase listener, EventDataBase eventData);

        internal void AddListenerToGroup(EventListenerBase listener)//在这个目标组里面找相同的listener  找到则覆盖 没找到则添加
        {
            int nullIndex = -1;
            for (int i = 0; i < Count; i++)
            {
                if (InnerList[i] == null)
                {
                    if (nullIndex == -1)
                    {
                        nullIndex = i;//遍历的时候顺便取一个null位
                    }
                }
                else
                {
                    if (InnerList[i] == listener)//有一样的就覆盖
                    {
                        InnerList[i].Recycle();
                        InnerList[i] = listener;
                        return;
                    }
                }
                //因为传入的是抽象容器 不能直接拿到的数据逐一复制,只能整个覆盖 这样就可以把内部的值传进去了
            }

            if (nullIndex == -1)//没空位就增加
            {
                InnerList.Add(listener);
            }
            else//有空位就插入  一般来讲list不会太大  不移除列表空元素  让他自由膨胀行了
            {
                InnerList[nullIndex] = listener;
            }
        }
        internal void RemoveListenerFromGroup(EventListenerBase listener)
        {
            for (int i = 0; i < Count; i++)
            {
                if (InnerList[i] == listener)//listener不为空 可以不判null
                {
                    InnerList[i].Recycle();
                    InnerList[i] = null;
                    return;
                }
            }
        }

    }
}
