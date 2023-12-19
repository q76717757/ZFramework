using Codice.Client.BaseCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    public enum UIGroupType
    {
        Down,
        Middle,
        Top,
    }
    public class UIGroupData
    {
        private Transform root;
        private UIGroupType groupType;
        private short minOrder;
        private short maxOrder;

        public UIGroupType GroupType => groupType;
        public short MinOrder => minOrder;
        public short MaxOrder => maxOrder;
        public Transform Root => root;

        public UIGroupData SetRoot(Transform root)
        {
            this.root = root;
            return this;
        }

        public UIGroupData(UIGroupType groupType, short minOrder, short maxOrder)
        {
            this.groupType = groupType;
            this.minOrder = minOrder;
            this.maxOrder = maxOrder;
        }
        public UIGroupData Clone()
        {
            return new UIGroupData(groupType, minOrder, maxOrder).SetRoot(this.root);
        }
    }

    internal static class UIGroupExtends
    {
        private readonly static Dictionary<UIGroupType, UIGroupData> map = new Dictionary<UIGroupType, UIGroupData>();

        static UIGroupExtends()
        {
            ConfigGroup();
        }

        private static void ConfigGroup()
        {
            AddGroup(UIGroupType.Down, new UIGroupData(UIGroupType.Down, 0, 1000));
            AddGroup(UIGroupType.Middle, new UIGroupData(UIGroupType.Middle, 1001, 2000));
            AddGroup(UIGroupType.Top, new UIGroupData(UIGroupType.Top, 2001, 3000));
        }

        private static void AddGroup(UIGroupType key, UIGroupData value) => map.Add(key, value);
        public static UIGroupData GetData(UIGroupType groupType) => map[groupType]/*.Clone()*/;


    }
}
