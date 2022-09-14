using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class UIComponent : ComponentData
    {
        public Transform root;//根节点[UI]

        public Dictionary<string, Type> types;//所有UI类型  通过字符串拿类型  然后通过这个类型去拿辅助对象 从而执行Show/Hide逻辑
        public Dictionary<Type, Dictionary<Type, IUILiveSystem>> maps;//UI生命周期辅助对象映射表//uiType iUILiveType iUILiveSystem

        public Dictionary<string, UICanvasComponent> UICanvas;//所有UI实例 目前单个type只有一个实例 暂用的UI架构
    }
}
