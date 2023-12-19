using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZFramework
{
    internal class VFSTreeModelItem : TreeModelItem
    {
        public VFSMetaData Data { get; }
        public override string DisplayName
        {
            get => Data.name;
            set => Data.name = value;
        }
        public VFSTreeModelItem(int id, int depth, VFSMetaData data) : base(id, depth)
        {
            Data = data;
        }
    }

}
