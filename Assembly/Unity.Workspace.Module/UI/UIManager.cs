using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine; 

namespace ZFramework
{
    public class UIManager : GlobalComponent<UIManager>, IUpdate
    {
        private static int idIndex;//ui实例ID
        //UI设置映射表
        private Dictionary<Type, UISettingAttribute> uiSettingMap;
        //所有实例的UI对象  type  ID           instance
        private Dictionary<Type, Dictionary<int, UIWindowBase>> uiInstancesMap;
        //所有UI组
        private Dictionary<UIGroupType, UIGroup> uiGruops;
        //缓冲队列 解决父生命周期中调子开关方法 打断父生命周期顺序的问题
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

        //ui实例的Update生命周期
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
        /// 从uiWindow的根开始，获取所有开着的子窗口并且是有序的。
        /// </summary>
        /// <param name="uiWindow"></param>
        /// <param name="allWindowInUIWindow"></param>
        private void GetRelative(UIWindowBase uiWindow, out List<UIWindowBase> allWindowInUIWindow)
        {
            UIWindowBase rootUI = uiWindow.Root;
            //先筛出所有激活的子UI
            allWindowInUIWindow = new List<UIWindowBase>();
            rootUI.GetAllChildrenWindow(ref allWindowInUIWindow, (childUI) =>
            {
                return childUI != null && childUI.Active;
            });
            //自己可能是Root,但是这里只添加开着的UI
            if (rootUI.Active && !allWindowInUIWindow.Contains(rootUI))
            {
                allWindowInUIWindow.Add(rootUI);
            }

            //冒泡排Order 小->大
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

        private bool AlreadyTop(UIWindowBase uiWindow)//检查我的root是不是当前group的top root  是则POP+排序自己一脉
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
            //检查现在是不是最顶层的一组childParent
            //如果是，startOrder uiWindow从根，带着所有的孩子一起弹
            //否则,从最高的order开始,带着所有的孩子一起弹
            //把自己和自己的孩子移除,到当前parent的最后一个开始诸葛店家
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

                //冒泡排Order 小->大
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
                //所有的子
                List<UIWindowBase> childActiveUI = new List<UIWindowBase>();
                uiWindow.GetAllChildrenWindow(ref childActiveUI, (child) =>
                {
                    return child.Active;
                });

                //冒泡排Order 小->大
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
                //移除
                for (int i = 0; i < childActiveUI.Count; i++)
                {
                    if (allActiveWindow.Contains(childActiveUI[i]))
                        allActiveWindow.Remove(childActiveUI[i]);
                }
                //添加 目的是排序,把一组UI从中间移动到最后
                for (int i = 0; i < childActiveUI.Count; i++)
                {
                    allActiveWindow.Add(childActiveUI[i]);
                }

                //StartOrder 要么是root.order,要么是Group-1 order;
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
        private void Open生命周期(UIWindowBase uiWindow)
        {
            UIGroupType groupType = uiWindow.GroupType;
            List<UIWindowBase> groupLinkedList = uiGruops[groupType].instances;
            UIGroup groupData = uiGruops[uiWindow.GroupType];
            short minOrder = groupData.MinOrder;
            short maxOrder = groupData.MaxOrder;

            UIWindowBase currentFouse = UIEventSystem.Instance.GetCurrentFoucs();
            //这里检查弹出的ui是否能作为焦点UI,有可能弹出的UI的子是模态窗体
            //那么进行焦点响应的应该是这个最高的模态窗体才对
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
                //冒泡排Order 小->大
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
                //模态的定义 是当前开着的子父窗体中，比模态order低的窗口都失去可交互
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

            //Open生命周期
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

            //当前组激活的UI中,最高Order== 组设置的最大Order 并且组内可装载UI的容量大于显示UI的个数时。
            //重建所有Order排序
            if (groupLinkedList.Count > 0 && groupLinkedList[groupLinkedList.Count - 1].Order >= maxOrder)
            {
                uiGruops[groupType].Soft();//UI重排
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

        //将UI弹到栈顶
        public void Pop(UIWindowBase uiWindow)
        {
            if (uiWindow.Active == false)
            {
                return;
            }
            PopTop(uiWindow);
            Open生命周期(uiWindow);
        }


        //创建过程 --> 通过Type类型 在设置映射表拿到配置   根据配置下载资源和初始化
        public async ATask<T> Creat<T>(bool isModal) where T : UIWindowBase
        {
            Type uiType = typeof(T);

            //实例化
            GameObject prefabSource = await VFSDownLoad<T>();
            GameObject uiInstance = GameObject.Instantiate(prefabSource);

            //设置父物体
            UISettingAttribute uiSetting = uiSettingMap[uiType];
            RectTransform groupRoot = uiGruops[uiSetting.GroupType].Root;
            uiInstance.name = $"[ID:{idIndex}] {prefabSource.name}";
            uiInstance.transform.SetParent(groupRoot);

            //注册  创建UIWindowBase
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
            //创建新的
            T uiInstance = await Creat<T>(isModal);
            Open<T>(uiInstance.ID);
            return uiInstance;
        }
        public async ATask<T> OpenAsync<T>(bool isModal) where T : UIWindowBase
        {
            Type uiType = typeof(T);
            T uiWindow = null;
            //没有的话就去下载
            if (!uiInstancesMap.TryGetValue(uiType, out Dictionary<int, UIWindowBase> uis))
            {
                uiWindow = await Creat<T>(isModal);
            }
            //否则,直接打开第一个
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

            //拿到VFS中预制件的下载路径
            if (!uiSettingMap.TryGetValue(uiType, out UISettingAttribute uiSetting))
                throw new Exception($"UISettingAttribute不存在，请在UI{uiType}上标记特性");

            //启动下载
            GameObject prefabSource = await VirtualFileSystem.LoadAsync<GameObject>(uiSetting.AssetPath);

            if (prefabSource == null)
                throw new Exception($"ui:{uiType} 下载失败,下载路径:{uiSetting.AssetPath}");

            return prefabSource;
        }


        //打开UI的处理逻辑,调用UI实例的生命周期
        private void OpenInternal(UIWindowBase uiWindow)
        {
            UIWindowBase parent = uiWindow.Parent;
            while (parent != null)
            {
                if (parent.Active == false)
                    throw new Exception($"有父窗体:{parent.GetType()}没开，要先开父窗体");
                parent = parent.Parent;
            }

            PopTop(uiWindow);

            //CallSelf
            queue.Enqueue(uiWindow);
            if (queue.Count == 1)
            {
                var self = queue.Peek();
                Open生命周期(self);
                queue.Dequeue();
                TryCallNextWindow生命周期();//开始递归内部开启的生命周期
            }
        //递归
        }
        private void TryCallNextWindow生命周期()
        {
            if (queue.Count > 0)//有在Self的时候Open其他的UI,会加进队列待call生命周期
            {
                var child = queue.Peek();
                Open生命周期(child);  //-->
                queue.Dequeue();
                TryCallNextWindow生命周期();
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
                Log.Error($"类型{window}是空的");
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
                Log.Info(uiWindow.ID + "已经是关闭状态了");
                return;
            }

        
            //获取当前UI子父层级中，Order高于uiWindow并且激活的UI(包括uiWindow)
            List<UIWindowBase> selfAndChildrenActiveWindow = new List<UIWindowBase>() { uiWindow };
            uiWindow.GetAllChildrenWindow(ref selfAndChildrenActiveWindow, (UI) =>
            {
                return UI.Active;
            });

            //记录最后的模态UI,这个模态窗体前面的
            UIWindowBase modalUI = selfAndChildrenActiveWindow.Find((ui) => ui.IsModal);

            List<UIWindowBase> waitForSetInteractable = null;
            //存一下所有需要解冻的UI
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

            //下面是正常关闭逻辑，如果关闭的UI是焦点，就调用丢失焦点接口.
            UIWindowBase foucsUI = null;
            //直接循环当前UI的所有子UI,倒着挨个关
            for (int i = selfAndChildrenActiveWindow.Count - 1; i >= 0; i--)
            {

                foucsUI = UIEventSystem.Instance.GetCurrentFoucs();
                //检查关闭的UI是否是焦点，是的话就丢焦点
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
                    //关闭生命周期
                    ToGameloop(selfAndChildrenActiveWindow[i]).OnClose();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                UIGroupType groupType = uiWindow.GroupType;
                List<UIWindowBase> openedUIInGroup = uiGruops[groupType].instances;
                openedUIInGroup.Remove(selfAndChildrenActiveWindow[i]);

                //拿最前的元素试着作为焦点
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
                Log.Error($"对象{window}是空的");
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
            //所有开着的UI
            List<UIWindowBase> openedUIInGroup = uiGruops[groupType].instances;

            //当前UI的所有子UI
            List<UIWindowBase> selfAndChildrenWindows = new List<UIWindowBase>() { uiWindow };
            //根在第一个位置，子是无序的。(不清楚有没有必要把子排序，先这样)
            uiWindow.GetAllChildrenWindow(ref selfAndChildrenWindows, null);

            //如果销毁的UI中有模态UI，就记录最后的模态UI,这个模态窗体前面的
            UIWindowBase modalUI = selfAndChildrenWindows.Find(ui => ui.IsModal);

            List<UIWindowBase> waitForSetInteractable = null;
            //存一下所有需要解冻的UI
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

            //焦点
            UIWindowBase foucsUI = null;

            //生命周期
            for (int i = selfAndChildrenWindows.Count - 1; i >= 0; i--)
            {
                UIWindowBase destroyUI = selfAndChildrenWindows[i];

                foucsUI = UIEventSystem.Instance.GetCurrentFoucs();
                //检查关闭的UI是否是焦点，是的话就丢焦点
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

                //关闭生命周期
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

                //销毁游戏对象
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

                //拿最前的元素试着作为焦点
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
                Log.Error($"要销毁的UI是空的{uiWindow}");
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