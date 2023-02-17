using UnityEngine;

namespace ZFramework
{
    namespace EventType
    {
        public class OnSceneChangeFinish
        {
            public string mapName;
        }

        public class SceneChangeEvent : EventCallback<OnSceneChangeFinish>
        {
            public override void Callback(OnSceneChangeFinish arg)
            {
                switch (arg.mapName)
                {
                    case MapNames.主场景:
                        ZEvent.TriggerEvent.AddListener(Locations.Get("Port_PG"), TriggerCall, 0);
                        ZEvent.TriggerEvent.AddListener(Locations.Get("Port_SFEH"), TriggerCall, 1);
                        break;
                    case MapNames.场景A:
                        ZEvent.TriggerEvent.AddListener(Locations.Get("Teleport"), TriggerCall, 2);
                        break;
                    case MapNames.场景B:
                        ZEvent.TriggerEvent.AddListener(Locations.Get("Teleport"), TriggerCall, 3);
                        break;
                    case MapNames.大厅:
                        break;
                    default:
                        break;
                }
            }

            public void TriggerCall(TriggerEventData<int> eventData)
            {
                if (eventData.EventType == TriggerEventType.Enter)
                {
                    Log.Info(eventData.Other.name);
                    if (eventData.Other.name != GameManager.Instance.Location.role.id.ToString())
                    {
                        return;
                    }

                    switch (eventData.Data0)
                    {
                        case 0:// Main -> PG
                            TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色开始传送()
                            {
                                exitMap = MapNames.主场景,
                                enterMap = MapNames.场景A,
                                roleID = GameManager.Instance.Location.role.id
                            });
                            break;
                        case 1://  Main -> SF
                            TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色开始传送()
                            {
                                exitMap = MapNames.主场景,
                                enterMap = MapNames.场景B,
                                roleID = GameManager.Instance.Location.role.id
                            });
                            break;
                        case 2://PG -> Main
                            TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色开始传送()
                            {
                                exitMap = MapNames.场景A,
                                enterMap = MapNames.主场景,
                                roleID = GameManager.Instance.Location.role.id
                            });
                            break;
                        case 3:// SF  ->  Main
                            TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色开始传送()
                            {
                                exitMap = MapNames.场景B,
                                enterMap = MapNames.主场景,
                                roleID = GameManager.Instance.Location.role.id
                            });
                            break;
                    }
                }
            }

            void TriggerBoxCall(UnitEventData<GameObject> eventData)
            {
                if (eventData.EventType == UnitEventType.Click)
                {
                    //事件演示 点一下转一下
                    var t = eventData.Data0.transform;
                    t.rotation *= Quaternion.Euler(90,90,90);
                }
            }
        }
    }
}
