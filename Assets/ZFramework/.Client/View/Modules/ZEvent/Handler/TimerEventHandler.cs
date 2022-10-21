/** Header
* TimerEventHandler.cs
* 计时器事件处理程序
**/

using System.Collections.Generic;

namespace ZFramework
{
    public sealed class TimerEventHandler : ZEventHandlerBase
    {
        internal List<TimerEventListenerBase> AllListeners { get; } = new List<TimerEventListenerBase>();//有效的listener 
        private List<TimerEventListenerBase> WaitingForAdd { get; } = new List<TimerEventListenerBase>();//等待添加的listener

        private TimerEventDataBase TransferContainer = new TimerEventDataBase();//传递容器

        public TimerEventHandler()
        {
            Game.Root.GetComponent<ZEventTemp>().callback += Update;//临时用一下
        }


        internal void AddListener(TimerEventListenerBase newlistener)//计时器比较特殊 每次add都是new timer 所以wait不存在重复    计时器中途替换listener作为待开发项 有需要再改
        {
            for (int i = 0; i < WaitingForAdd.Count; i++)
            {
                if (WaitingForAdd[i] != null && WaitingForAdd[i] == newlistener)
                {
                    return;
                }
            }
            newlistener.EventDataCaches.Add(TimerEventType.Start);
            WaitingForAdd.Add(newlistener);
        }
        internal void RemoveListener(Timer target)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] != null && WaitingForAdd[i].Target == target)
                {
                    WaitingForAdd[i] = null;
                    break;
                }
            }
            count = AllListeners.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListeners[i] != null && AllListeners[i].Target == target)
                {
                    AllListeners[i] = null;
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

            count = AllListeners.Count;
            for (int i = 0; i < count; i++)
            {
                AllListeners[i] = null;
            }
        }

        internal void CallEvent(TimerEventListenerBase listener, TimerEventType eventType) {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] != null && WaitingForAdd[i] == listener)
                {
                    WaitingForAdd[i].EventDataCaches.Add(eventType);
                    return;
                }
            }
            count = AllListeners.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListeners[i] != null && AllListeners[i] == listener)
                {
                    AllListeners[i].EventDataCaches.Add(eventType);
                    return;
                }
            }
            if (eventType == TimerEventType.Restart)
            {
                listener.EventDataCaches.Clear();
                listener.EventDataCaches.Add(TimerEventType.Restart);
                listener.State = TimerState.Running;
                WaitingForAdd.Add(listener);
            }
        }

        int c = 0;
        void Update()
        {
            //添加
            int count = WaitingForAdd.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (WaitingForAdd[i] != null)
                    {
                        for (int j = 0; j < AllListeners.Count; j++)//重复添加会有BUG  如  当a==b  add.a  add.b  kill.a  kill.b  kill.b会被第一个add.a触发  导致第二个add.b并没有触发kill.b 产生冗余b  
                        {
                            //前奏项目鼠标繁忙的转圈动画序列 发现的
                            if (WaitingForAdd[i] == AllListeners[j])
                            {
                                break;
                            }
                        }
                        AllListeners.Add(WaitingForAdd[i]);
                    }
                }
                WaitingForAdd.RemoveRange(0, count);
            }

            //清除
            if (c++ % 100 == 0)
            {
                count = AllListeners.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (AllListeners[i] == null || AllListeners[i].IsInvalid)
                    {
                        AllListeners.RemoveAt(i);
                    }
                }
            }

            //派发
            count = AllListeners.Count;
            if (count > 0)
            {
                long timenow = TimeHelper.Now();
                for (int i = 0; i < count; i++)
                {
                    if (AllListeners[i] == null) continue;
                    if (AllListeners[i].State == TimerState.Death || AllListeners[i].IsInvalid)//已经死亡 或者无效的
                    {
                        AllListeners[i] = null;
                        continue;
                    }
                    AllListeners[i].Current = timenow;

                    //特殊事件
                    int eventCacheCount = AllListeners[i].EventDataCaches.Count;
                    for (int j = 0; j < eventCacheCount; j++)
                    {
                        if (AllListeners[i] == null)//重复kill事件 set null
                            break;

                        var eventType = AllListeners[i].EventDataCaches[j];
                        switch (eventType)
                        {
                            case TimerEventType.Start:
                            case TimerEventType.Restart:
                                AllListeners[i].State = TimerState.Running;
                                AllListeners[i].Remaining = (long)(AllListeners[i].Length * 1000);
                                AllListeners[i].Expiration = timenow + AllListeners[i].Remaining;
                                AllListeners[i].LoopIndex = 0;
                                TransferContainer.SetStaticData(AllListeners[i].Target, eventType);
                                AllListeners[i].Call(TransferContainer);
                                break;
                            case TimerEventType.Pause:
                                if (AllListeners[i].State == TimerState.Running)
                                {
                                    AllListeners[i].State = TimerState.Paused;
                                    AllListeners[i].Remaining = AllListeners[i].Expiration - timenow;
                                    TransferContainer.SetStaticData(AllListeners[i].Target, eventType);
                                    AllListeners[i].Call(TransferContainer);
                                }
                                break;
                            case TimerEventType.Continue:
                                if (AllListeners[i].State == TimerState.Paused)
                                {
                                    AllListeners[i].State = TimerState.Running;
                                    AllListeners[i].Expiration = timenow + AllListeners[i].Remaining;
                                    TransferContainer.SetStaticData(AllListeners[i].Target, eventType);
                                    AllListeners[i].Call(TransferContainer);
                                }
                                break;
                            case TimerEventType.Kill:
                                if (AllListeners[i].State != TimerState.Death)
                                {
                                    AllListeners[i].State = TimerState.Death;
                                    TransferContainer.SetStaticData(AllListeners[i].Target, eventType);
                                    AllListeners[i].Call(TransferContainer);
                                    AllListeners[i] = null;
                                }
                                break;
                            default: continue;
                        }
                    }

                    if (AllListeners[i] == null) continue;//杀死或者注销后将为null 没有后续的updata了
                    //Log.Info(i + ":" + AllListeners[i].State.ToString());

                    AllListeners[i].EventDataCaches.RemoveRange(0, eventCacheCount);

                    switch (AllListeners[i].State)
                    {
                        case TimerState.Running:
                            if (timenow >= AllListeners[i].Expiration)//到期
                            {
                                if (AllListeners[i].LoopIndex < AllListeners[i].LoopCount || AllListeners[i].LoopCount == -1)//在有效的循环次数内 或者是-1无限循环
                                {
                                    TransferContainer.SetStaticData(AllListeners[i].Target, TimerEventType.Expire);
                                    AllListeners[i].Call(TransferContainer);

                                    if (AllListeners[i] == null) continue;//可能在回调中注销自己

                                    if (++AllListeners[i].LoopIndex == AllListeners[i].LoopCount)//最后一次
                                    {
                                        AllListeners[i].State = TimerState.Death;//自然死亡 不发kill事件
                                        AllListeners[i] = null;
                                        continue;
                                    }
                                    else//不是最后一次  设置新的到期时间
                                    {
                                        AllListeners[i].Expiration = timenow + (long)(AllListeners[i].Length * 1000);
                                    }
                                }
                                else //循环次数是无效的  循环次数0  好像没有应用场景??
                                {

                                }
                            }
                            else//没到期
                            {
                                TransferContainer.SetStaticData(AllListeners[i].Target, TimerEventType.Update);
                                AllListeners[i].Call(TransferContainer);
                            }
                            break;
                        case TimerState.Paused:
                        case TimerState.Death:continue;
                    }
                }
            }
        }
    }

}
