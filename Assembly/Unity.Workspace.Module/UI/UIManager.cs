using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    public enum UIPopType
    {
        Pop,
        Modal
    }

    public class UIManager : GlobalComponent<UIManager> ,IAwake,IUpdate
    {
        private static int idIndex;//uiʵ��ID
        private Dictionary<Type, UISettingAttribute> uiSettingMap;
        //����ʵ����UI����
        private Dictionary<Type, Dictionary<int, UIWindowBase>> uiInstancesMap;
        private Dictionary<UIGroupType, List<UIWindowBase>> groupListMap;
        private Transform root;
        void IAwake.Awake()
        {
            Init().Invoke();
        }
        private async ATask Init()
        {
            GameObject GO = await VirtualFileSystem.LoadAsync<GameObject>("UIManager/UIManager");//-->root canvas
            GO = GameObject.Instantiate<GameObject>(GO);
            root = GO.transform;

            uiSettingMap = new Dictionary<Type, UISettingAttribute>();
            uiInstancesMap = new Dictionary<Type, Dictionary<int, UIWindowBase>>();
            //groupListMap = new Dictionary<UIGroupType, List<UIWindowBase>>();
            groupListMap = new Dictionary<UIGroupType, List<UIWindowBase>>();
            foreach (var item in Game.GetTypesByAttribute<UISettingAttribute>())
            {
                UISettingAttribute uiSetting = item.GetCustomAttribute<UISettingAttribute>();
                uiSettingMap.Add(item, uiSetting);
            }
        }


        void IUpdate.Update()
        {
            foreach (var uiWindows in uiInstancesMap.Values)
            {
                foreach (var uIWindow in uiWindows.Values)
                {
                    if (uIWindow != null && uIWindow.Active)
                    {
                        ToGameloop(uIWindow).OnUpdate();
                    }
                }
            }
        }
        public async ATask<GameObject> VFSDownLoad<T>() where T : UIWindowBase
        {
            Type uiType = typeof(T);

            //�õ�VFS��Ԥ�Ƽ�������·��
            if (!uiSettingMap.TryGetValue(uiType, out UISettingAttribute uiSetting))
                throw new Exception($"UISettingAttribute�����ڣ�����UI{uiType}�ϱ������");

            //��������
            GameObject prefabSource = await VirtualFileSystem.LoadAsync<GameObject>(uiSetting.Path);

            if (prefabSource == null)
                throw new Exception($"ui:{uiType} ����ʧ��,����·��:{uiSetting.Path}");

            return prefabSource;
        }

        //�������� --> ͨ��Type���� ������ӳ����õ�����   ��������������Դ�ͳ�ʼ��
        public async ATask<T> Creat<T>() where T : UIWindowBase
        {
            GameObject prefabSource = await VFSDownLoad<T>();

            Type uiType = typeof(T);
            //ʵ����
            GameObject uiInstance = GameObject.Instantiate(prefabSource);

            if (!uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> map))
            {
                map = new Dictionary<int, UIWindowBase>();
                uiInstancesMap.Add(uiType, map);
            }

            //����UIWindowBase
            T instance = (T)Activator.CreateInstance(uiType);

            //����UI��
            UISettingAttribute uiSetting = uiSettingMap[uiType];

            if (!groupListMap.TryGetValue(uiSetting.GroupType, out List<UIWindowBase> groupUIWindow))
            {
                groupUIWindow = new List<UIWindowBase>();
                groupListMap.Add(uiSetting.GroupType, groupUIWindow);

                GameObject instanceGO = new GameObject("UIGroup-" + uiSetting.GroupType.ToString());
                UIGroupData groupData = UIGroupExtends.GetData(uiSetting.GroupType);
                instanceGO.transform.SetParent(root);
                groupData.SetRoot(instanceGO.transform);
            }

            //���ø�����
            Transform groupRoot = UIGroupExtends.GetData(uiSetting.GroupType).Root;
            uiInstance.transform.SetParent(groupRoot);

            map.Add(++idIndex, instance);
            instance.Register(idIndex, uiSetting.GroupType, uiSetting.UIPopType, uiInstance.transform);
            uiInstance.name = $"(id:{idIndex})" + uiInstance.name.Replace("(Clone)", "");
            ToGameloop(instance).OnAwake();
            return instance;
        }
        private IUIGameLoop ToGameloop(UIWindowBase uiWindow) => uiWindow;
        private short ClampShort(short value, short min, short max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        /// <summary>
        /// ��uiWindow�ĸ���ʼ����ȡ���п��ŵ��Ӵ��ڲ���������ġ�
        /// </summary>
        /// <param name="uiWindow"></param>
        /// <param name="allWindowInUIWindow"></param>
        private void GetRelative(UIWindowBase uiWindow, out List<UIWindowBase> allWindowInUIWindow)
        {
            UIWindowBase rootUI = uiWindow.Root;
            //��ɸ�����м������UI
            allWindowInUIWindow = new List<UIWindowBase>();
            rootUI.GetAllChildrenWindow(ref allWindowInUIWindow, (childUI) =>
            {
                return childUI != null && childUI.Active;
            });
            //�Լ�������Root,��������ֻ��ӿ��ŵ�UI
            if (rootUI.Active && !allWindowInUIWindow.Contains(rootUI))
            {
                allWindowInUIWindow.Add(rootUI);
            }

            //ð����Order С->��
            for (int i = 0; i < allWindowInUIWindow.Count - 1; i++)
            {
                for (int j = 0; j < allWindowInUIWindow.Count - 1 - i; j++)
                {
                    if (allWindowInUIWindow[j].Order > allWindowInUIWindow[j + 1].Order)
                    {
                        UIWindowBase temp = allWindowInUIWindow[j];
                        allWindowInUIWindow[j] = allWindowInUIWindow[j + 1];
                        allWindowInUIWindow[j + 1] = temp;
                    }
                }
            }
        }

        static Queue<UIWindowBase> queue = new Queue<UIWindowBase>();
        static LinkedList<UIWindowBase> link = new LinkedList<UIWindowBase>();

        //��UI�Ĵ����߼�,����UIʵ������������
        private void OpenInternal(UIWindowBase uiWindow)
        {
            UIWindowBase parent = uiWindow.Parent;
            while (parent != null)
            {
                if (parent.Active == false)
                    throw new Exception($"�и�����:{parent.GetType()}û����Ҫ�ȿ�������");
                parent = parent.Parent;
            }

            PopTop(uiWindow);

            //CallSelf
            queue.Enqueue(uiWindow);
            if (queue.Count == 1)
            {
                var self = queue.Peek();
                Open��������(self);
                queue.Dequeue();
                TryCallNextWindow��������();//��ʼ�ݹ��ڲ���������������
            }
        }
        public void Pop(UIWindowBase uiWindow)
        {
            if (uiWindow.Active == false)
            {
                return;
            }
            PopTop(uiWindow);
            Open��������(uiWindow);
        }
        //�ݹ�
        private void TryCallNextWindow��������()
        {
            if (queue.Count > 0)//����Self��ʱ��Open������UI,��ӽ����д�call��������
            {
                var child = queue.Peek();
                Open��������(child);  //-->
                queue.Dequeue();
                TryCallNextWindow��������();
            }
        }
        private void Open��������(UIWindowBase uiWindow)
        {
            UIGroupType groupType = uiWindow.GroupType;
            List<UIWindowBase> groupLinkedList = groupListMap[groupType];
            UIGroupData groupData = UIGroupExtends.GetData(groupType);
            short minOrder = groupData.MinOrder;
            short maxOrder = groupData.MaxOrder;
            short uiCapcity = (short)(maxOrder - minOrder);

            UIWindowBase currentFouse = UIEventSystem.Instance.GetCurrentFoucs();
            //�����鵯����ui�Ƿ�����Ϊ����UI,�п��ܵ�����UI������ģ̬����
            //��ô���н�����Ӧ��Ӧ���������ߵ�ģ̬����Ŷ�
            UIWindowBase topFouse = uiWindow;
            List<UIWindowBase> modalInChildren = new List<UIWindowBase>();
            if (uiWindow.PopType == UIPopType.Modal)
            {
                modalInChildren.Add(uiWindow);
            }
            uiWindow.GetAllChildrenWindow(ref modalInChildren, (ui) =>
            {
                return ui.Active && ui.PopType == UIPopType.Modal;
            });
            if (modalInChildren.Count > 0)
            {
                //ð����Order С->��
                for (int i = 0; i < modalInChildren.Count - 1; i++)
                {
                    for (int j = 0; j < modalInChildren.Count - 1 - i; j++)
                    {
                        if (modalInChildren[j].Order > modalInChildren[j + 1].Order)
                        {
                            UIWindowBase temp = modalInChildren[j];
                            modalInChildren[j] = modalInChildren[j + 1];
                            modalInChildren[j + 1] = temp;
                        }
                    }
                }
                topFouse = modalInChildren[modalInChildren.Count - 1];
            }
            if (currentFouse != null && currentFouse != topFouse)
            {
                try
                {
                    ((IUIGameLoop)currentFouse).OnLoseFocus();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                UIEventSystem.Instance.SetFoucs(null);
            }

            if (uiWindow.PopType == UIPopType.Modal)
            {
                //ģ̬�Ķ��� �ǵ�ǰ���ŵ��Ӹ������У���ģ̬order�͵Ĵ��ڶ�ʧȥ�ɽ���
                GetRelative(uiWindow, out List<UIWindowBase> allWindow);
                for (int i = allWindow.Count - 1; i >= 0; i--)
                {
                    if ((allWindow[i].Order < uiWindow.Order))
                    {
                        try
                        {
                            ToGameloop(allWindow[i]).SetInteractable(false);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }

            //Open��������
            if (uiWindow.Active == false)
            {
                try
                {
                    ToGameloop(uiWindow).OnOpen();
                    ToGameloop(uiWindow).SetInteractable(true);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

            }
            currentFouse = UIEventSystem.Instance.GetCurrentFoucs();
            if (currentFouse != topFouse && UIEventSystem.Instance.SetFoucs(topFouse))
            {
                try
                {
                    ToGameloop(topFouse).OnFocus();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            //��ǰ�鼤���UI��,���Order== �����õ����Order �������ڿ�װ��UI������������ʾUI�ĸ���ʱ��
            //�ؽ�����Order����
            if (groupLinkedList.Count > 0 && groupLinkedList[groupLinkedList.Count - 1].Order >= maxOrder && uiCapcity >= groupLinkedList.Count)
            {
                ReBuildCanvasOrder(groupType);
            }

        }

        private bool AlreadyTop(UIWindowBase uiWindow)
        {
            List<UIWindowBase> groupList = groupListMap[uiWindow.GroupType];
            if (!groupList.Contains(uiWindow))
            {
                return false;
            }
            UIWindowBase topRoot = groupList[groupList.Count - 1].Root;
            return topRoot == uiWindow.Root;
        }
        private void PopTop(UIWindowBase uiWindow)
        {

            //��������ǲ�������һ��childParent
            //����ǣ�startOrder uiWindow�Ӹ����������еĺ���һ��
            //����,����ߵ�order��ʼ,�������еĺ���һ��
            //���Լ����Լ��ĺ����Ƴ�,����ǰparent�����һ����ʼ�����
            UIGroupData groupData = UIGroupExtends.GetData(uiWindow.GroupType);
            List<UIWindowBase> groupList = groupListMap[uiWindow.GroupType];

            if (uiWindow.Parent == null)
            {
                int startOrder = groupList.Count > 0
                    ? ClampShort((short)(groupList[groupList.Count - 1].Order + 1), groupData.MinOrder, groupData.MaxOrder)
                    : groupData.MinOrder;

                List<UIWindowBase> childActiveUI = new List<UIWindowBase>();
                uiWindow.GetAllChildrenWindow(ref childActiveUI, (child) =>
                {
                    return child.Active;
                });

                //ð����Order С->��
                for (int i = 0; i < childActiveUI.Count - 1; i++)
                {
                    for (int j = 0; j < childActiveUI.Count - 1 - i; j++)
                    {
                        if (childActiveUI[j].Order > childActiveUI[j + 1].Order)
                        {
                            UIWindowBase temp = childActiveUI[j];
                            childActiveUI[j] = childActiveUI[j + 1];
                            childActiveUI[j + 1] = temp;
                        }
                    }
                }
                childActiveUI.Insert(0, uiWindow);
                for (int i = 0; i < childActiveUI.Count; i++)
                {
                    if (groupList.Contains(childActiveUI[i]))
                        groupList.Remove(childActiveUI[i]);
                    groupList.Add(childActiveUI[i]);
                    childActiveUI[i].Order = ClampShort((short)startOrder++, groupData.MinOrder, groupData.MaxOrder);
                }
            }
            else
            {
                UIWindowBase rootUI = uiWindow.Root;

                GetRelative(rootUI, out List<UIWindowBase> allActiveWindow);

                //���е���
                List<UIWindowBase> childActiveUI = new List<UIWindowBase>();
                uiWindow.GetAllChildrenWindow(ref childActiveUI, (child) =>
                {
                    return child.Active;
                });

                //ð����Order С->��
                for (int i = 0; i < childActiveUI.Count - 1; i++)
                {
                    for (int j = 0; j < childActiveUI.Count - 1 - i; j++)
                    {
                        if (childActiveUI[j].Order > childActiveUI[j + 1].Order)
                        {
                            UIWindowBase temp = childActiveUI[j];
                            childActiveUI[j] = childActiveUI[j + 1];
                            childActiveUI[j + 1] = temp;
                        }
                    }
                }
                childActiveUI.Insert(0, uiWindow);

                //�Ƴ�
                for (int i = 0; i < childActiveUI.Count; i++)
                {
                    if (allActiveWindow.Contains(childActiveUI[i]))
                        allActiveWindow.Remove(childActiveUI[i]);
                }
                //��� Ŀ��������,��һ��UI���м��ƶ������
                for (int i = 0; i < childActiveUI.Count; i++)
                {
                    allActiveWindow.Add(childActiveUI[i]);
                }

                //StartOrder Ҫô��root.order,Ҫô��Group-1 order;
                int startOrder = 0;
                if (AlreadyTop(uiWindow))
                {
                    startOrder = rootUI.Order;


                }
                else
                {
                    startOrder = groupList.Count > 0
                      ? ClampShort((short)(groupList[groupList.Count - 1].Order + 1), groupData.MinOrder, groupData.MaxOrder)
                      : groupData.MinOrder;
                }
                for (int i = 0; i < allActiveWindow.Count; i++)
                {
                    if (groupList.Contains(allActiveWindow[i]))
                        groupList.Remove(allActiveWindow[i]);
                    groupList.Add(allActiveWindow[i]);

                    allActiveWindow[i].Order = ClampShort((short)startOrder++, groupData.MinOrder, groupData.MaxOrder);
                }
            }



            ////uiWindow���ŵ������ �������򸸵���
            ////uiWindow���ŵ������ ��鵱ǰ���ŵĵ����д��壬�ź���ģ����ŵ����

            //GetRelative(uiWindow, out List<UIWindowBase> childrenParentActiveSortUI);
            //int order = groupList.Count > 0 ? groupList[groupList.Count - 1].Order : groupData.MinOrder;
            ////�������Լ��ͼӽ�ȥ��׼����ӵ��б�ĩ�˽�������
            //if (childrenParentActiveSortUI.Contains(uiWindow))
            //{
            //    childrenParentActiveSortUI.Remove(uiWindow);
            //}
            //childrenParentActiveSortUI.Add(uiWindow);

            //for (int i = 0; i < childrenParentActiveSortUI.Count; i++)
            //{
            //    if (groupList.Contains(childrenParentActiveSortUI[i]))
            //        groupList.Remove(childrenParentActiveSortUI[i]);
            //    childrenParentActiveSortUI[i].Order = ClampShort((short)(++order), groupData.MinOrder, groupData.MaxOrder);
            //    groupList.Add(childrenParentActiveSortUI[i]);
            //}

        }
        /// <summary>
        /// �ؽ�UI���UI��Ⱦ����
        /// </summary>
        /// <param name="groupType"></param>
        private void ReBuildCanvasOrder(UIGroupType groupType)
        {
            List<UIWindowBase> groupList = groupListMap[groupType];
            UIGroupData groupData = UIGroupExtends.GetData(groupType);
            short minOrder = groupData.MinOrder;
            short maxOrder = groupData.MaxOrder;
            for (int i = 0; i < groupList.Count; i++)
            {
                groupList[i].Order = minOrder++;
            }
        }
        public async ATask<T> OpenAsync<T>() where T : UIWindowBase
        {
            Type uiType = typeof(T);
            T uiWindow = null;
            //û�еĻ���ȥ����
            if (!uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> uis))
            {
                uiWindow = await Creat<T>();
            }
            //����,ֱ�Ӵ򿪵�һ��
            else
            {
                uiWindow = (T)uis.ElementAt(0).Value;
            }
            OpenInternal(uiWindow);
            return uiWindow;
        }
        public async ATask<T> OpenNew<T>() where T : UIWindowBase
        {
            //�����µ�
            T uiInstance = await Creat<T>();
            Open<T>(uiInstance.ID);
            return uiInstance;
        }
        public void Open(int id)
        {
            foreach (var keyValueP in uiInstancesMap.Values)
            {
                if (keyValueP.TryGetValue(id, out UIWindowBase uiWindow))
                {
                    OpenInternal(uiWindow);
                    return;
                }
            }
        }
        public T Open<T>(int id) where T : UIWindowBase
        {
            T uiWindow = GetUIAndMap<T>(id, out Dictionary<int, UIWindowBase> allTypeOfUI);
            if (uiWindow != null)
                OpenInternal(uiWindow);
            return uiWindow;
        }
        public T Open<T>(T window) where T : UIWindowBase
        {
            if (window == null)
            {
                Log.Error($"����{window}�ǿյ�");
                return default;
            }
            return Open<T>(window.ID);
        }
        public void OpenByType<T>() where T : UIWindowBase
        {
            Type uiType = typeof(T);
            if (uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> windows))
            {
                foreach (var uiWindow in windows.Values)
                {
                    if (uiWindow != null)
                    {
                        OpenInternal(uiWindow);
                    }
                }
            }
        }
        public void OpenAll()
        {
            foreach (var uiWindowMap in uiInstancesMap.Values)
            {
                foreach (var uiWindow in uiWindowMap.Values)
                {
                    if (uiWindow != null)
                    {
                        OpenInternal(uiWindow);
                    }
                }
            }
        }
        public T Get<T>(int id) where T : UIWindowBase
        {
            T uiWindow = GetUIAndMap<T>(id, out Dictionary<int, UIWindowBase> allTypeOfUI);
            return uiWindow;
        }
        public T Get<T>() where T : UIWindowBase
        {
            Type type = typeof(T);
            if (uiInstancesMap.TryGetValue(type, out Dictionary<int, UIWindowBase> uis))
            {
                if (uis.Count > 0)
                {
                    KeyValuePair<int, UIWindowBase> uiWindow = uis.ElementAt(0);
                    if (uiWindow.Value != null)
                    {
                        T ui = (T)uiWindow.Value;
                        return ui;
                    }
                }
            }
            return default;
        }
        public List<T> GetAll<T>() where T : UIWindowBase
        {
            List<T> uiWindows = new List<T>();
            Type type = typeof(T);
            if (uiInstancesMap.TryGetValue(type, out Dictionary<int, UIWindowBase> uiMap))
            {
                foreach (var uiWindow in uiMap.Values)
                {
                    if (uiWindow != null)
                    {
                        uiWindows.Add((T)uiWindow);
                    }
                }
            }
            return uiWindows;
        }
        private void CloseInternal(UIWindowBase uiWindow)
        {
            if (uiWindow.Active == false)
            {
                Log.Info(uiWindow.ID + "�Ѿ��ǹر�״̬��");
                return;
            }

            UIGroupType groupType = uiWindow.GroupType;
            groupListMap.TryGetValue(groupType, out List<UIWindowBase> openedUIInGroup);

            //��ȡ��ǰUI�Ӹ��㼶�У�Order����uiWindow���Ҽ����UI(����uiWindow)
            List<UIWindowBase> selfAndChildrenActiveWindow = new List<UIWindowBase>() { uiWindow };
            uiWindow.GetAllChildrenWindow(ref selfAndChildrenActiveWindow, (UI) =>
            {
                return UI.Active;
            });

            //��¼����ģ̬UI,���ģ̬����ǰ���
            UIWindowBase modalUI = selfAndChildrenActiveWindow.Find((ui) => { return ui.PopType == UIPopType.Modal; });

            List<UIWindowBase> waitForSetInteractable = null;
            //��һ��������Ҫ�ⶳ��UI
            if (modalUI != null)
            {
                UIWindowBase root = uiWindow.Root;
                waitForSetInteractable = new List<UIWindowBase>();
                if (root.Active && root.Order < selfAndChildrenActiveWindow[0].Order)
                    waitForSetInteractable.Add(root);

                root.GetAllChildrenWindow(ref waitForSetInteractable, (ui) =>
                {
                    return ui.Active && ui.Order < selfAndChildrenActiveWindow[0].Order;
                });
            }

            //�����������ر��߼�������رյ�UI�ǽ��㣬�͵��ö�ʧ����ӿ�.
            UIWindowBase foucsUI = null;
            //ֱ��ѭ����ǰUI��������UI,���Ű�����
            for (int i = selfAndChildrenActiveWindow.Count - 1; i >= 0; i--)
            {

                foucsUI = UIEventSystem.Instance.GetCurrentFoucs();
                //���رյ�UI�Ƿ��ǽ��㣬�ǵĻ��Ͷ�����
                if (foucsUI != null && selfAndChildrenActiveWindow[i] == foucsUI)
                {
                    try
                    {
                        ToGameloop(foucsUI).OnLoseFocus();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                    UIEventSystem.Instance.SetFoucs(null);
                }

                try
                {
                    ToGameloop(selfAndChildrenActiveWindow[i]).SetInteractable(false);
                    //�ر���������
                    ToGameloop(selfAndChildrenActiveWindow[i]).OnClose();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                openedUIInGroup.Remove(selfAndChildrenActiveWindow[i]);

                //����ǰ��Ԫ��������Ϊ����
                if (openedUIInGroup.Count > 0)
                {
                    foucsUI = UIEventSystem.Instance.GetCurrentFoucs();
                    UIWindowBase top = openedUIInGroup[openedUIInGroup.Count - 1];
                    if (foucsUI != top && UIEventSystem.Instance.SetFoucs(top))
                    {
                        try
                        {
                            ToGameloop(top).OnFocus();
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }

            if (modalUI != null)
            {
                for (int i = 0; i < waitForSetInteractable.Count; i++)
                {
                    try
                    {
                        ToGameloop(waitForSetInteractable[i]).SetInteractable(true);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        public void Close(int id)
        {
            foreach (var keyValueP in uiInstancesMap.Values)
            {
                if (keyValueP.TryGetValue(id, out UIWindowBase uiWindow))
                {
                    CloseInternal(uiWindow);
                    return;
                }
            }
        }
        public T Close<T>() where T : UIWindowBase
        {
            Type type = typeof(T);
            if (uiInstancesMap.TryGetValue(type, out Dictionary<int, UIWindowBase> uis) && uis.Count > 0)
            {
                KeyValuePair<int, UIWindowBase> kv = uis.ElementAt(0);
                if (kv.Value != null)
                {
                    T uiWindow = (T)kv.Value;
                    CloseInternal(uiWindow);
                    return uiWindow;
                }
            }
            return default;
        }
        public T Close<T>(int id) where T : UIWindowBase
        {
            T uiWindow = GetUIAndMap<T>(id, out Dictionary<int, UIWindowBase> allTypeOfUI);
            if (uiWindow != null)
                CloseInternal(uiWindow);
            return uiWindow;
        }
        public T Close<T>(T window) where T : UIWindowBase
        {
            if (window == null)
            {
                Log.Error($"����{window}�ǿյ�");
                return default;
            }
            return Close<T>(window.ID);
        }
        public void CloseByType<T>() where T : UIWindowBase
        {
            Type uiType = typeof(T);
            if (uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> windows))
            {
                foreach (var uiWindow in windows.Values)
                {
                    if (uiWindow != null)
                    {
                        CloseInternal(uiWindow);
                    }
                }
            }
        }
        public void CloseAll()
        {
            foreach (var uiWindowMap in uiInstancesMap.Values)
            {
                foreach (var uiWindow in uiWindowMap.Values)
                {
                    if (uiWindow != null && uiWindow.Active)
                    {
                        CloseInternal(uiWindow);
                    }
                }
            }
        }
        private T GetUIAndMap<T>(int id, out Dictionary<int, UIWindowBase> allTypeOfUI) where T : UIWindowBase
        {
            Type type = typeof(T);
            if (uiInstancesMap.TryGetValue(type, out allTypeOfUI))
            {
                if (allTypeOfUI.TryGetValue(id, out UIWindowBase uiWindow))
                {
                    return (T)uiWindow;
                }
            }
            return default;
        }
        private void DestroyInternal(UIWindowBase uiWindow)
        {
            UIGroupType groupType = uiWindow.GroupType;
            //���п��ŵ�UI
            groupListMap.TryGetValue(groupType, out List<UIWindowBase> openedUIInGroup);

            //��ǰUI��������UI
            List<UIWindowBase> selfAndChildrenWindows = new List<UIWindowBase>() { uiWindow };
            //���ڵ�һ��λ�ã���������ġ�(�������û�б�Ҫ��������������)
            uiWindow.GetAllChildrenWindow(ref selfAndChildrenWindows, null);

            //������ٵ�UI����ģ̬UI���ͼ�¼����ģ̬UI,���ģ̬����ǰ���
            UIWindowBase modalUI = selfAndChildrenWindows.Find((ui) => { return ui.PopType == UIPopType.Modal; });

            List<UIWindowBase> waitForSetInteractable = null;
            //��һ��������Ҫ�ⶳ��UI
            if (modalUI != null)
            {
                UIWindowBase root = uiWindow.Root;
                waitForSetInteractable = new List<UIWindowBase>();
                if (root.Active && root.Order < selfAndChildrenWindows[0].Order)
                {
                    waitForSetInteractable.Add(root);
                }
                root.GetAllChildrenWindow(ref waitForSetInteractable, (ui) =>
                {
                    return ui.Active && ui.Order < uiWindow.Order;
                });
            }

            //����
            UIWindowBase foucsUI = null;

            //��������
            for (int i = selfAndChildrenWindows.Count - 1; i >= 0; i--)
            {
                UIWindowBase destroyUI = selfAndChildrenWindows[i];

                foucsUI = UIEventSystem.Instance.GetCurrentFoucs();
                //���رյ�UI�Ƿ��ǽ��㣬�ǵĻ��Ͷ�����
                if (foucsUI != null && destroyUI == foucsUI)
                {
                    try
                    {
                        ToGameloop(foucsUI).OnLoseFocus();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                    UIEventSystem.Instance.SetFoucs(null);
                }
                try
                {
                    ToGameloop(destroyUI).SetInteractable(false);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                //�ر���������
                try
                {
                    if (destroyUI.Active)
                        ToGameloop(destroyUI).OnClose();

                    ToGameloop(destroyUI).OnDestroy();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                //������Ϸ����
                if (destroyUI.Transform != null)
                {
                    GameObject.Destroy(destroyUI.Transform.gameObject);
                }
                if (uiInstancesMap.TryGetValue(destroyUI.GetType(), out Dictionary<int, UIWindowBase> instanceMap))
                {
                    if (instanceMap.ContainsKey(destroyUI.ID))
                    {
                        instanceMap.Remove(destroyUI.ID);
                    }
                }

                if (openedUIInGroup.Contains(destroyUI))
                {
                    openedUIInGroup.Remove(destroyUI);
                }

                //����ǰ��Ԫ��������Ϊ����
                if (openedUIInGroup.Count > 0)
                {
                    foucsUI = UIEventSystem.Instance.GetCurrentFoucs();
                    UIWindowBase top = openedUIInGroup[openedUIInGroup.Count - 1];
                    if (foucsUI != top && UIEventSystem.Instance.SetFoucs(top))
                    {
                        try
                        {
                            ToGameloop(top).OnFocus();
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                }
            }

            if (modalUI != null)
            {
                for (int i = 0; i < waitForSetInteractable.Count; i++)
                {
                    try
                    {
                        ToGameloop(waitForSetInteractable[i]).SetInteractable(true);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
        public void Destroy(int id)
        {
            foreach (var keyValueP in uiInstancesMap.Values)
            {
                if (keyValueP.TryGetValue(id, out UIWindowBase uiWindow))
                {
                    DestroyInternal(uiWindow);
                    if (uiInstancesMap.TryGetValue(uiWindow.GetType(), out Dictionary<int, UIWindowBase> uiMap))
                    {
                        if (uiMap.Count == 0)
                            uiInstancesMap.Remove(uiWindow.GetType());
                    }
                    return;
                }
            }
        }
        public void Destroy<T>(int uiWindowID) where T : UIWindowBase
        {
            T uiWindow = GetUIAndMap<T>(uiWindowID, out Dictionary<int, UIWindowBase> typeOfUI);
            if (uiWindow != null)
            {
                DestroyInternal(uiWindow);
            }
            if (typeOfUI != null && typeOfUI.Count == 0)
            {
                uiInstancesMap.Remove(typeof(T));
            }
        }
        public void Destroy<T>(T uiWindow) where T : UIWindowBase
        {
            if (uiWindow == null)
            {
                Log.Error($"Ҫ���ٵ�UI�ǿյ�{uiWindow}");
                return;
            }
            Destroy<T>(uiWindow.ID);
        }
        public void DestroyByType<T>() where T : UIWindowBase
        {
            Type uiType = typeof(T);
            if (uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> windows))
            {
                foreach (var window in windows.Values)
                {
                    if (window != null)
                    {
                        DestroyInternal(window);
                    }
                }
                uiInstancesMap.Remove(uiType);
            }
        }
        public void DestroyAll()
        {
            foreach (var item in uiInstancesMap.Values)
            {
                foreach (var window in item.Values)
                {
                    if (window != null)
                    {
                        DestroyInternal(window);
                    }
                }
            }
            uiInstancesMap.Clear();
        }

    }
}