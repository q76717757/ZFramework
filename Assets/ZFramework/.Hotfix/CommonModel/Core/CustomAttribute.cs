using System;

namespace ZFramework
{
    public abstract class BaseAttribute : Attribute
    {
        public Type AttributeType { get => GetType(); }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GameLoopAttribute : BaseAttribute
    {
        public EntityGameLoopUsing LoopType;
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EventAttribute : BaseAttribute
    {
    }

}