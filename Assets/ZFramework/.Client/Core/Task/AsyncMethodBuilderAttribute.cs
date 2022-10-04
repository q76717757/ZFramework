
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Delegate |
        AttributeTargets.Enum |
        AttributeTargets.Interface |
        AttributeTargets.Method |
        AttributeTargets.Struct, 
        AllowMultiple = false, Inherited = false)]
    public sealed class AsyncMethodBuilderAttribute : Attribute
    {
        public Type BuilderType { get; }

        public AsyncMethodBuilderAttribute(Type builderType)
        {
            BuilderType = builderType;
        }
    }
}
