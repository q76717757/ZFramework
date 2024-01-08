using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    //UI渲染层级  可以按需增删   到下一项枚举的差值就是该层的容量
    public enum UISortLayer : short
    {
        Background = 0,//背景
        Normal = 1000,//中景
        Foreground = 2000,//前景
        Top = 3000, //遮罩
    }

}
