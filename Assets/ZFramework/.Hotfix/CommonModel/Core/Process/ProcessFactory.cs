using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class ProcessFactory
    {
        public static Process CreateProcess(ProcessType processType)
        { 
            Process process = new Process(processType);


            return process;
        }

    }
}
