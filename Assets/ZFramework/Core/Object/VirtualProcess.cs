using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class VirtualProcess : ComponentData
    {
        public ProcessType ProcessType { get; set; }

        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//当前entity下挂的子entity

    }
}
