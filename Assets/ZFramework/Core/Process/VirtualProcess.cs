using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class VirtualProcess : Entity
    {
        public ProcessType ProcessType { get; }

        public new VirtualProcess Process
        {
            get => this;
        }
    }
}
