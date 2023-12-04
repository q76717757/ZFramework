using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZFramework
{
    public class VFSTreeElement : TreeElement
    {
        public VFSMetaData data;
        public override string DisplayName
        {
            get => data.name;
            set => data.name = value;
        }

        public VFSTreeElement(int id, int depth, VFSMetaData data) : base(id, depth)
        {
            this.data = data;
        }


    }

}
