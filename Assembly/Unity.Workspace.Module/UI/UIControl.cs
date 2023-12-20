using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    /// <summary>
    /// UI控件    控件必须挂在窗体上  一个窗体有多个控件
    /// </summary>
    public abstract class UIControl
    {
        public UIWindowBase uiWindow;
        public UIControl(UIWindowBase ui)
        {
            this.uiWindow = ui;
        }
    }
}
