/** Header
 *  ScreenEventHandler.cs
 *  触发器事件处理程序
 **/

using System.Collections.Generic;

namespace ZFramework
{
    public sealed class ScreenEventHandler : ZEventHandlerBase
    {
        internal List<ScreenEventListenerGroup> AllListennerGroup { get; } = new List<ScreenEventListenerGroup>(32);
        private List<ScreenEventListenerBase> WaitingForAdd { get; } = new List<ScreenEventListenerBase>();
        public List<ScreenEventDataBase> EventDataCaches { get; } = new List<ScreenEventDataBase>(16);
        internal void AddListener(ScreenEventListenerBase newlistener)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                //找到一样的就覆盖 因为重写了等号表面上相等 实际内部带的data并不一定相等
                //(重写等号的主要作用就是把data藏在listener里面) 
                if (WaitingForAdd[i] == newlistener)
                {
                    WaitingForAdd[i] = newlistener;
                    return;//如果已经有了 那肯定是已经有driver的
                }
            }
            WaitingForAdd.Add(newlistener);//没找到一样的就入列

            //如果入列了 监听的载体可能没有驱动
            count = AllListennerGroup.Count;//找一下有没有这个监听组
            for (int i = 0; i < count; i++)
            {
                if (AllListennerGroup[i] != null && AllListennerGroup[i].Target == newlistener.Target)
                {
                    return;//找到目标载体的监听表 有就是有驱动
                }
            }
            //没有这个载体的监听组 就新建一个
            AllListennerGroup.Add(ZEvent.GetNewGroup<ScreenEventListenerGroup>().SetTarget(newlistener.Target));
        }

        internal void RemoveListener(ScreenEventListenerBase targetlistener)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] == targetlistener)
                {
                    WaitingForAdd[i] = null;
                    break;
                }
            }

            count = AllListennerGroup.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListennerGroup[i] != null && AllListennerGroup[i].Target == targetlistener.Target)//找到对应的监听组
                {
                    AllListennerGroup[i].RemoveListenerFromGroup(targetlistener);//监听被移除了 但是allg[i].target 还在
                    break;
                }
            }
        }

        internal void ClearListener(PointerType target)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] != null && WaitingForAdd[i].Target == target)
                {
                    WaitingForAdd[i] = null;
                    return;
                }
            }

            count = AllListennerGroup.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListennerGroup[i] != null && AllListennerGroup[i].Target == target)
                {
                    AllListennerGroup[i] = null;
                    break;
                }
            }
        }
        internal void ClearAllListener()
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                WaitingForAdd[i] = null;
            }
            count = AllListennerGroup.Count;
            for (int i = 0; i < count; i++)
            {
                AllListennerGroup[i] = null;
            }
        }

        //清理监听的计数 每帧+1  100帧清一次 暂用  想到更好的方式再替换
        int clearCount = 0;

        void Update()
        {
            //添加
            int count = WaitingForAdd.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (WaitingForAdd[i] != null && !WaitingForAdd[i].IsInvalid)
                    {
                        int groupCount = AllListennerGroup.Count;
                        for (int j = 0; j < groupCount; j++)
                        {
                            if (AllListennerGroup[j] != null && AllListennerGroup[j].Target == WaitingForAdd[i].Target)
                            {
                                AllListennerGroup[j].AddListenerToGroup(WaitingForAdd[i]);
                                break;
                            }
                        }
                    }
                }
                WaitingForAdd.RemoveRange(0, count);
            }

            count = AllListennerGroup.Count;
            ////清除
            //if (++clearCount > 100)
            //{
            //    clearCount = 0;
            //    for (int i = count - 1; i >= 0; i--)
            //    {
            //        if (AllListennerGroup[i] == null)
            //        {
            //            AllListennerGroup.RemoveAt(i);
            //            continue;
            //        }
            //        if (AllListennerGroup[i].RealCount == 0)//载体还在 但是已经没有监听了
            //        {
            //            AllListennerGroup.RemoveAt(i);
            //        }
            //    }
            //}

            //派发
            int dataCount = EventDataCaches.Count;
            if (dataCount > 0)
            {
                for (int i = 0; i < dataCount; i++)
                {
                    if (EventDataCaches[i] == null) continue;

                    int groupCount = AllListennerGroup.Count;
                    for (int j = 0; j < groupCount; j++)
                    {
                        if (AllListennerGroup[j] != null && AllListennerGroup[j].Target == EventDataCaches[i].Target)//找到对应的监听组 考虑做成字典?
                        {
                            AllListennerGroup[j].DispatchAll(EventDataCaches[i]);
                            EventDataCaches[i] = null;
                            break;
                        }
                    }
                }
                EventDataCaches.RemoveRange(0, dataCount);
            }
        }
    }

}
