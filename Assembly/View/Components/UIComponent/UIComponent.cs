using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class UIComponent :Component
    {
        public static UIComponent Instance;

        public Dictionary<Type,string> uiPaths = new Dictionary<Type,string>();
        public Dictionary<Type, UICanvas> uis = new Dictionary<Type, UICanvas>();

        private Canvas rootCanvas;

        public void Init(Canvas rootCanvas)
        {
            Instance = this;
            this.rootCanvas = rootCanvas;

            foreach (var item in Game.GetTypesByAttribute<UIPathAttribute>())
            {
                uiPaths.Add(item, item.GetCustomAttribute<UIPathAttribute>().Path);
            }
        }

        public void ShowUI<T>()
        {
            Type uiType = typeof(T);
            if (uis.TryGetValue(uiType,out UICanvas ui))
            {
                ui.OnShow();
            }
            else
            {
                OpenUI(uiType).Invoke();
            }
        }

        async ATask OpenUI(Type type)
        {
            if (uiPaths.TryGetValue(type, out string path))
            {
                var uiPrefab = await VirtualFileSystem.LoadAsync<GameObject>(path);
                var uiInstance = GameObject.Instantiate(uiPrefab, rootCanvas.transform);
                var uiCanvas = Activator.CreateInstance(type) as UICanvas;
                uiCanvas.Init(uiInstance);
                uis.Add(type, uiCanvas);
            }
        }

        public void CloseUI<T>()
        {
            if (uis.TryGetValue(typeof(T),out UICanvas canvas))
            {
                canvas.OnHide();
            }
        }
    }
}
