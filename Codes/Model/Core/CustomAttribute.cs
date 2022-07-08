using System;

namespace ZFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class BaseAttribute : Attribute
    {
        public Type AttributeType { get => GetType(); }
    }

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