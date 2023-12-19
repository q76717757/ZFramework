using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class TreeViewItem<T> : TreeViewItem where T : TreeModelItem
    {
        public T ModelItem { get; }

        public override sealed string displayName//treeView重命名是set ModelItem.name 所以这里需要重写get ModelItem.name TreeView才能正确显示名字
        {
            get => ModelItem.DisplayName;
        }
        public TreeViewItem(int depth, T element) : base(element.ID, depth, element.DisplayName)
        {
            ModelItem = element;
        }
    }
}
