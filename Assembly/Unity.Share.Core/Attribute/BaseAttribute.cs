using System;

namespace ZFramework
{
    public abstract class BaseAttribute : Attribute
    {
        public Type AttributeType { get => GetType(); }
    }
}
