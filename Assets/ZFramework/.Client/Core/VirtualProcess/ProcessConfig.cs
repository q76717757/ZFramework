using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    internal class ProcessConfig
    {
        /// <summary>Id</summary>
        public int Id { get; set; }
        /// <summary>所属机器</summary>
        public int MachineId { get; set; }
        /// <summary>内网端口</summary>
        public int InnerPort { get; set; }
        /// <summary>程序名</summary>
        public string AppName { get; set; }

    }
}
