using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class TreeViewItem<T> : TreeViewItem where T : TreeModelItem
    {
        public T ModelItem { get; }

        public override sealed string displayName//treeView��������set ModelItem.name ����������Ҫ��дget ModelItem.name TreeView������ȷ��ʾ����
        {
            get => ModelItem.DisplayName;
        }
        public TreeViewItem(int depth, T element) : base(element.ID, depth, element.DisplayName)
        {
            ModelItem = element;
        }
    }
}
