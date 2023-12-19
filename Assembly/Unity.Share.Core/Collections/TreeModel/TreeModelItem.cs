using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public abstract class TreeModelItem
    {
        public int ID { get; }
        public int Depth { get; set; }//深度从0算起   隐藏根的深度固定-1
        public TreeModelItem Parent { get; set; }
        public List<TreeModelItem> Children { get; set; }
        public bool HasChildren
        {
            get { return Children != null && Children.Count > 0; }
        }
        public abstract string DisplayName { get; set; }

        public TreeModelItem(int id, int depth)
        {
            ID = id;
            Depth = depth;
        }

    }

}
