﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class Process : Entity
    {
        public ProcessType ProcessType { get; }

        public Process(ProcessType processType)
        { 
            ProcessType = processType;
        }




    }
}
