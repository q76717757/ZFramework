using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public enum UIGroupType : short
    {
        Down = 0,//背景
        Middle = 1000,//普通
        Top = 2000,//弹窗
        Mark = 3000, //全局遮罩mark
    }
}
