#if !SERVER
namespace System.Runtime.CompilerServices
{
    internal sealed class AsyncMethodBuilderAttribute : Attribute
    {
        public Type BuilderType { get; }

        public AsyncMethodBuilderAttribute(Type builderType)
        {
            BuilderType = builderType;
        }
    }
}
#endif


namespace ZFramework
{
    using System.Runtime.CompilerServices;

    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder))]
    public partial struct ATask
    {
    }

    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
    public partial struct ATask<TResult>
    {
    }
}

