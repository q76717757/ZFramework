using System;

namespace ZFramework
{
    public class UITypeAttribute : BaseAttribute 
    {
        public string Name { get; }
        public UITypeAttribute(string uiName) {
            Name = uiName;
        }
    }
    public class EventAttribute : BaseAttribute
    {

    }

    public class LiveAttribute : BaseAttribute
    {

    }

    public class UILiveAttribute : BaseAttribute
    { 

    }
}