/** Header
 *  KeyEventHandler.cs
 *  按键事件处理程序
 **/

using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public sealed class KeyEventHandler : ZEventHandlerBase
    {
        private KeyEventDataBase TransferContainer = new KeyEventDataBase();
        public List<KeyEventListenerGroup> AllListennerGroup { get; } = new List<KeyEventListenerGroup>(32);
        private List<KeyEventListenerBase> WaitingForAdd { get; } = new List<KeyEventListenerBase>();

        public KeyEventHandler()
        {
            Game.Root.GetComponent<ZEventTemp>().callback += Update;//临时用一下
        }


        internal void AddListener(KeyEventListenerBase newlistener)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] != null &&
                    WaitingForAdd[i].Target == newlistener.Target &&
                    WaitingForAdd[i] == newlistener)
                {
                    WaitingForAdd[i] = newlistener;
                    return;
                }
            }
            WaitingForAdd.Add(newlistener);

            int nullIndex = -1;
            count = AllListennerGroup.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListennerGroup[i] == null)
                {
                    if (nullIndex == -1)
                    {
                        nullIndex = i;
                    }
                }
                else
                {
                    if (AllListennerGroup[i].Target == newlistener.Target)
                    {
                        return;
                    }
                }
            }

            if (nullIndex == -1)
            {
                AllListennerGroup.Add(ZEvent.GetNewGroup<KeyEventListenerGroup>().SetTarget(newlistener.Target));
            }
            else
            {
                AllListennerGroup[nullIndex] = ZEvent.GetNewGroup<KeyEventListenerGroup>().SetTarget(newlistener.Target);
            }
        }
        internal void RemoveListener(KeyEventListenerBase targetlistener)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] != null &&
                    WaitingForAdd[i].Target == targetlistener.Target &&
                    WaitingForAdd[i] == targetlistener)
                {
                    WaitingForAdd[i] = null;
                    break;
                }
            }

            count = AllListennerGroup.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListennerGroup[i] != null && AllListennerGroup[i].Target == targetlistener.Target)
                {
                    AllListennerGroup[i].RemoveListenerFromGroup(targetlistener);
                    break;
                }
            }
        }

        internal void ClearListener(KeyCode target)
        {
            int count = WaitingForAdd.Count;
            for (int i = 0; i < count; i++)
            {
                if (WaitingForAdd[i] != null && WaitingForAdd[i].Target == target)
                    WaitingForAdd[i] = null;
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

        void Update()
        {
            int count = WaitingForAdd.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
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
                WaitingForAdd.RemoveRange(0, count);
            }

            count = AllListennerGroup.Count;
            for (int i = 0; i < count; i++)
            {
                if (AllListennerGroup[i] == null) continue;
                if (Input.GetKey(AllListennerGroup[i].Target) != AllListennerGroup[i].LastKeyIsDown)//获取当前帧和上一帧比  //有改变  是弹起或者按下
                {
                    AllListennerGroup[i].LastKeyIsDown = !AllListennerGroup[i].LastKeyIsDown;//反过来之后等于当前的状态

                    TransferContainer.SetStaticData(AllListennerGroup[i].Target, AllListennerGroup[i].LastKeyIsDown ? KeyEventType.Down : KeyEventType.Up);
                }
                else //没有改变  是没按或者按住
                {
                    if (AllListennerGroup[i].LastKeyIsDown)//按住
                    {
                        TransferContainer.SetStaticData(AllListennerGroup[i].Target, KeyEventType.Press);
                    }
                    else
                    {
                        TransferContainer.SetStaticData(AllListennerGroup[i].Target, KeyEventType.NotPress);
                    }
                }
                AllListennerGroup[i].DispatchAll(TransferContainer);
            }
        }
    }

}
