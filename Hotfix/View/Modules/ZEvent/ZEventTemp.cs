using System;

namespace ZFramework
{
    public class ZTUpdate : OnUpdateImpl<ZEventTemp>
    {
        public override void OnUpdate(ZEventTemp self)
        {
            self.callback?.Invoke();
        }
    }

    public class ZEventTemp:SingleComponent<ZEventTemp>//临时接一下生命周期
    {
        public Action callback;
    }
}
