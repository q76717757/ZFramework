using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine; 

namespace ZFramework
{
    public class UIManager : GlobalComponent<UIManager>, IUpdate
    {
        private static int idIndex;//uiʵ��ID
        //UI����ӳ���
        private Dictionary<Type, UISettingAttribute> uiSettingMap;
        //����ʵ����UI����  type  ID           instance
        private Dictionary<Type, Dictionary<int, UIWindowBase>> uiInstancesMap;
        //����UI��
        private Dictionary<UIGroupType, UIGroup> uiGruops;
        //������� ��������������е��ӿ��ط��� ��ϸ���������˳�������
        Queue<UIWindowBase> queue = new Queue<UIWindowBase>();

        public void Init(Canvas rootCanvas)
        {
            InitSettings();

            InitUIGroup(rootCanvas.GetComponent<RectTransform>());

            uiInstancesMap = new Dictionary<Type, Dictionary<int, UIWindowBase>>();

            Game.AddGlobalComponent<UIEventSystem>();
            Game.AddGlobalComponent<FouseInputModel>();
        }

        void InitSettings()
        {
            uiSettingMap = new Dictionary<Type, UISettingAttribute>();
            foreach (var item in Game.GetTypesByAttribute<UISettingAttribute>())
            {
                UISettingAttribute uiSetting = item.GetCustomAttribute<UISettingAttribute>();
                uiSettingMap.Add(item, uiSetting);
            }
        }
        void InitUIGroup(RectTransform rootCanvas)
        {
            uiGruops = new Dictionary<UIGroupType, UIGroup>();
            UIGroupType[] groupTypes = Enum.GetValues(typeof(UIGroupType)).Cast<UIGroupType>().ToArray();
            for (int i = 0,size = groupTypes.Length-1; i <= size; i++)
            {
                UIGroupType type = groupTypes[i];
                short min = i == 0 ? (short)type : (short)(type + 1);
                short max = i == size ? short.MaxValue : (short)groupTypes[i + 1];
                RectTransform instanceGO = new GameObject($"{type}[{min}-{max}]").AddComponent<RectTransform>();
                instanceGO.SetParent(rootCanvas);
                instanceGO.anchorMin = Vector2.zero;
                instanceGO.anchorMax = Vector2.one;
                instanceGO.offsetMin = Vector2.zero;
                instanceGO.offsetMax = Vector2.zero;
                Canvas canvas = instanceGO.gameObject.AddComponent<Canvas>();

                uiGruops.Add(type, new UIGroup(type, min, max, instanceGO));
            }
        }

        //uiʵ����Update��������
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
        private IUIGameLoop ToGameloop(UIWindowBase uiWindow) => uiWindow;

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

        private bool AlreadyTop(UIWindowBase uiWindow)//����ҵ�root�ǲ��ǵ�ǰgroup��top root  ����POP+�����Լ�һ��
        {
            List<UIWindowBase> groupList = uiGruops[uiWindow.GroupType].instances;
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
            UIGroup groupData = uiGruops[uiWindow.GroupType];
            List<UIWindowBase> groupList = groupData.instances;

            if (uiWindow.Parent == null)
            {
                short startOrder = groupList.Count > 0
                    ? ShortUtility.Clamp((short)(groupList[groupList.Count - 1].Order + 1), groupData.MinOrder, groupData.MaxOrder)
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
                    childActiveUI[i].Order = ShortUtility.Clamp(startOrder++, groupData.MinOrder, groupData.MaxOrder);
                }
            }
            else
            {
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

                UIWindowBase rootUI = uiWindow.Root;
                GetRelative(rootUI, out List<UIWindowBase> allActiveWindow);
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
                      ? ShortUtility.Clamp((short)(groupList[groupList.Count - 1].Order + 1), groupData.MinOrder, groupData.MaxOrder)
                      : groupData.MinOrder;
                }
                for (int i = 0; i < allActiveWindow.Count; i++)
                {
                    if (groupList.Contains(allActiveWindow[i]))
                        groupList.Remove(allActiveWindow[i]);
                    groupList.Add(allActiveWindow[i]);

                    allActiveWindow[i].Order = ShortUtility.Clamp((short)startOrder++, groupData.MinOrder, groupData.MaxOrder);
                }
            }

        }
        private void Open��������(UIWindowBase uiWindow)
        {
            UIGroupType groupType = uiWindow.GroupType;
            List<UIWindowBase> groupLinkedList = uiGruops[groupType].instances;
            UIGroup groupData = uiGruops[uiWindow.GroupType];
            short minOrder = groupData.MinOrder;
            short maxOrder = groupData.MaxOrder;

            UIWindowBase currentFouse = UIEventSystem.Instance.GetCurrentFoucs();
            //�����鵯����ui�Ƿ�����Ϊ����UI,�п��ܵ�����UI������ģ̬����
            //��ô���н�����Ӧ��Ӧ���������ߵ�ģ̬����Ŷ�
            UIWindowBase topFouse = uiWindow;
            List<UIWindowBase> modalInChildren = new List<UIWindowBase>();
            if (uiWindow.IsModal)
            {
                modalInChildren.Add(uiWindow);
            }
            uiWindow.GetAllChildrenWindow(ref modalInChildren, (ui) =>
            {
                return ui.Active && ui.IsModal;
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

            if (uiWindow.IsModal)
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
            if (groupLinkedList.Count > 0 && groupLinkedList[groupLinkedList.Count - 1].Order >= maxOrder)
            {
                uiGruops[groupType].Soft();//UI����
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

        //��UI����ջ��
        public void Pop(UIWindowBase uiWindow)
        {
            if (uiWindow.Active == false)
            {
                return;
            }
            PopTop(uiWindow);
            Open��������(uiWindow);
        }


        //�������� --> ͨ��Type���� ������ӳ����õ�����   ��������������Դ�ͳ�ʼ��
        public async ATask<T> Creat<T>(bool isModal) where T : UIWindowBase
        {
            Type uiType = typeof(T);

            //ʵ����
            GameObject prefabSource = await VFSDownLoad<T>();
            GameObject uiInstance = GameObject.Instantiate(prefabSource);

            //���ø�����
            UISettingAttribute uiSetting = uiSettingMap[uiType];
            RectTransform groupRoot = uiGruops[uiSetting.GroupType].Root;
            uiInstance.name = $"[ID:{idIndex}] {prefabSource.name}";
            uiInstance.transform.SetParent(groupRoot);

            //ע��  ����UIWindowBase
            if (!uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> map))
            {
                map = new Dictionary<int, UIWindowBase>();
                uiInstancesMap.Add(uiType, map);
            }
            T instance = (T)Activator.CreateInstance(uiType);
            map.Add(++idIndex, instance);
            instance.Register(idIndex, uiSetting.GroupType, isModal, uiInstance);

            //awake
            ToGameloop(instance).OnAwake();
            return instance;
        }
        public async ATask<T> OpenNew<T>(bool isModal) where T : UIWindowBase
        {
            //�����µ�
            T uiInstance = await Creat<T>(isModal);
            Open<T>(uiInstance.ID);
            return uiInstance;
        }
        public async ATask<T> OpenAsync<T>(bool isModal) where T : UIWindowBase
        {
            Type uiType = typeof(T);
            T uiWindow = null;
            //û�еĻ���ȥ����
            if (!uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> uis))
            {
                uiWindow = await Creat<T>(isModal);
            }
            //����,ֱ�Ӵ򿪵�һ��
            else
            {
                uiWindow = (T)uis.ElementAt(0).Value;
            }
            OpenInternal(uiWindow);
            return uiWindow;
        }
        public async ATask<GameObject> VFSDownLoad<T>() where T : UIWindowBase
        {
            Type uiType = typeof(T);

            //�õ�VFS��Ԥ�Ƽ�������·��
            if (!uiSettingMap.TryGetValue(uiType, out UISettingAttribute uiSetting))
                throw new Exception($"UISettingAttribute�����ڣ�����UI{uiType}�ϱ������");

            //��������
            GameObject prefabSource = await VirtualFileSystem.LoadAsync<GameObject>(uiSetting.AssetPath);

            if (prefabSource == null)
                throw new Exception($"ui:{uiType} ����ʧ��,����·��:{uiSetting.AssetPath}");

            return prefabSource;
        }


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
        //�ݹ�
        }
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

        
            //��ȡ��ǰUI�Ӹ��㼶�У�Order����uiWindow���Ҽ����UI(����uiWindow)
            List<UIWindowBase> selfAndChildrenActiveWindow = new List<UIWindowBase>() { uiWindow };
            uiWindow.GetAllChildrenWindow(ref selfAndChildrenActiveWindow, (UI) =>
            {
                return UI.Active;
            });

            //��¼����ģ̬UI,���ģ̬����ǰ���
            UIWindowBase modalUI = selfAndChildrenActiveWindow.Find((ui) => ui.IsModal);

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

                UIGroupType groupType = uiWindow.GroupType;
                List<UIWindowBase> openedUIInGroup = uiGruops[groupType].instances;
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


        private void DestroyInternal(UIWindowBase uiWindow)
        {
            UIGroupType groupType = uiWindow.GroupType;
            //���п��ŵ�UI
            List<UIWindowBase> openedUIInGroup = uiGruops[groupType].instances;

            //��ǰUI��������UI
            List<UIWindowBase> selfAndChildrenWindows = new List<UIWindowBase>() { uiWindow };
            //���ڵ�һ��λ�ã���������ġ�(�������û�б�Ҫ��������������)
            uiWindow.GetAllChildrenWindow(ref selfAndChildrenWindows, null);

            //������ٵ�UI����ģ̬UI���ͼ�¼����ģ̬UI,���ģ̬����ǰ���
            UIWindowBase modalUI = selfAndChildrenWindows.Find(ui => ui.IsModal);

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
                if (destroyUI.GameObject != null)
                {
                    GameObject.Destroy(destroyUI.GameObject);
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