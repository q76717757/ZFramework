using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class ZTUpdate : OnUpdateImpl<ZEventTemp>
    {
        public override void OnUpdate(ZEventTemp self)
        {
            self.callback?.Invoke();
        }
    }

    public class ZEventTemp:Component//临时接一下生命周期
    {
        public Action callback;
    }
}
