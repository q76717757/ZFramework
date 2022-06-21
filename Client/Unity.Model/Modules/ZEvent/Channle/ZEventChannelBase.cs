/** Header
 *  ZEventChannelBase.cs
 *  
 **/

namespace ZFramework
{
    public abstract class ZEventChannelBase<Handler> where Handler : ZEventHandlerBase
    {
        protected Handler _handler;
        internal ZEventChannelBase(Handler handler)
        {
            _handler = handler;
        }
    }
}