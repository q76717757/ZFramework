using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [SerializeField]
    public abstract class TreeElement
    {
        [SerializeField] public int id;
        [SerializeField] public int depth;//深度从0算起   隐藏根的深度固定-1

        [JsonIgnore][NonSerialized] public TreeElement parent;
        [JsonIgnore][NonSerialized] public List<TreeElement> children;

        public TreeElement(int id, int depth)
        {
            this.id = id;
            this.depth = depth;
        }
        [JsonIgnore] public bool HasChildren
        {
            get { return children != null && children.Count > 0; }
        }
        [JsonIgnore] public abstract string DisplayName { get; set; }
    }

}
