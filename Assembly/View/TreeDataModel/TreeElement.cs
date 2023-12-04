using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public abstract class TreeElement
    {
        public int id;
        public int depth;//深度从0算起   隐藏根的深度固定-1

        public TreeElement parent;
        public List<TreeElement> children;

        public TreeElement(int id, int depth)
        {
            this.id = id;
            this.depth = depth;
        }
        public bool HasChildren
        {
            get { return children != null && children.Count > 0; }
        }
        public abstract string DisplayName { get; set; }
    }

}
