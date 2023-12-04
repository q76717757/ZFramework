using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class UIPathAttribute :BaseAttribute
    {
        public string Path { get; }
        public UIPathAttribute(string path)
        {
            Path = path;
        }
    }
}
