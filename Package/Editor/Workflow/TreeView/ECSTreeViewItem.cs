using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ECSTreeViewItem : TreeViewItem
    {
        Entity entity;
        public ECSTreeViewItem() { }
        public ECSTreeViewItem(Entity entity)
        { 
            this.entity = entity;
        }

        public override int id { get => base.id; set => base.id = value; }
        public override TreeViewItem parent { get => base.parent; set => base.parent = value; }
        public override List<TreeViewItem> children { get => base.children; set => base.children = value; }
        public override bool hasChildren => base.hasChildren;
        public override int depth { get => base.depth; set => base.depth = value; }
        public override string displayName { get => base.displayName; set => base.displayName = value; }

    }
}
