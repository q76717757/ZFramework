using System;

namespace ZFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class BaseAttribute : Attribute
    {
        public Type AttributeType { get => GetType(); }
    }

}