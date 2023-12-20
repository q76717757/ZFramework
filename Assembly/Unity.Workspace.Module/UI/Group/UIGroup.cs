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
    public class UIGroup//2D 3D
    {
        public UIGroupType GroupType { get; }
        public short MinOrder { get; }
        public short MaxOrder { get; }
        public RectTransform Root { get; }


        public List<UIWindowBase> instances = new List<UIWindowBase>();//属于当前组 所有打开的UI

        public UIGroup(UIGroupType groupType, short minOrder, short maxOrder, RectTransform root)
        {
            GroupType = groupType;
            MinOrder = minOrder;
            MaxOrder = maxOrder;
            Root= root;
        }

        //将组内的所有UI元素重新排Order
        public void Soft()
        {
            for (int i = MinOrder; i < instances.Count; i++)
            {
                if (i < MaxOrder)
                {
                    instances[i].Order = i;
                }
                else
                {
                    throw new OverflowException();
                }   
            }
        }
    }
}
