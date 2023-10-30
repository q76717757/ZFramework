using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class TreeViewItem<T> : TreeViewItem where T : TreeElement
    {
        public T Element { get; }

        public override sealed string displayName
        {
            get
            {
                return Element.DisplayName;
            }
            set
            {
                Element.DisplayName = value;
            }
        }
        public override sealed int id
        {
            get
            {
                return Element.id;
            }
            set
            {
                Element.id = value;
            }
        }

        public TreeViewItem(int depth, T element) : base(element.id, depth, element.DisplayName)
        {
            this.Element = element;
        }
    }
}
