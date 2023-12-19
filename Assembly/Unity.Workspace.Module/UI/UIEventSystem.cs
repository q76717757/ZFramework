using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZFramework
{
    public class FoucsCallAttribute : BaseAttribute
    {

    }

    public class UIEventSystem : GlobalComponent<UIEventSystem> ,IAwake
    {
        //UIType ----> 继承了所有的接口Type
        private Dictionary<Type, HashSet<Type>> foucsMap = new Dictionary<Type, HashSet<Type>>();
        private bool GetUIWindow接口<T>(Type uiType) where T : IUIInput
        {
            if (foucsMap.TryGetValue(uiType, out HashSet<Type> hashSet))
            {
                Type interfaceType = typeof(T);
                if (hashSet.Contains(interfaceType))
                {
                    return true;
                }
            }
            return false;
        }

        void IAwake.Awake()
        {
            Type[] uiWindosType = Game.GetTypesByAttribute<FoucsCallAttribute>();

            foreach (var uiwindowType in uiWindosType)
            {
                if (!foucsMap.TryGetValue(uiwindowType, out HashSet<Type> uiFoucsTypeList))
                {
                    uiFoucsTypeList = new HashSet<Type>();
                    foucsMap.Add(uiwindowType, uiFoucsTypeList);
                }

                Type[] allInterfacesType = uiwindowType.GetInterfaces();

                allInterfacesType = allInterfacesType.Where((iuiFoucs) => iuiFoucs == typeof(IUIInput)).ToArray();

                foreach (var item in allInterfacesType)
                {
                    uiFoucsTypeList.Add(item);
                }
            }
        }

        internal bool SetFoucs(UIWindowBase uiWindow)
        {
            if (uiWindow == null)
            {
                FouseInputModel.Instance.SetFocus(null);
                return false;
            }

            bool canFoucs = GetUIWindow接口<IUIInput>(uiWindow.GetType());
            if (canFoucs)
                FouseInputModel.Instance.SetFocus(uiWindow);
            return canFoucs;
        }

        internal bool IsFoucs(UIWindowBase uiWindow)
        {
            if (uiWindow == null)
            {
                return false;
            }
            return GetUIWindow接口<IUIInput>(uiWindow.GetType());
        }

        internal UIWindowBase GetCurrentFoucs()
        {
            return FouseInputModel.Instance.FocusUI;
        }

    }
}
