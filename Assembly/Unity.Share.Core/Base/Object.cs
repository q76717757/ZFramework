using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    public abstract class ZObject : IDispose
    {
        //什么时候需要这个?  在使用了序列化之后,存在反序列化出不同的对象,但指的是同一个对象,如果这种情况,对内容进行修改,需要格外注意,会存在不同步的问题
        //反序列化的时候使用缓存,反序列化时先从缓存查,没有再实例新的,通过这种方式确保同一ID的对象在内存中的引用唯一,这样可以避免不同引用引起的修改不同步
        //internal static Dictionary<int, ZObject> objPool = new Dictionary<int, ZObject>();
        private static int idPosition = 0;

        private int instanceID;
        public int InstanceID { get => instanceID; }
        public string Name { get; set; }
        private bool IsDisposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return instanceID == 0;
            }
        }

        protected ZObject()
        {
            instanceID = ++idPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new NullReferenceException();
            }
        }

        //运算符重载
        public static bool operator ==(ZObject obj1, ZObject obj2)
        {
            bool anull = obj1 is null;
            bool bnull = obj2 is null;
            if (anull && bnull)
            {
                return true;
            }
            if (anull)
            {
                return obj2.IsDisposed;
            }
            if (bnull)
            {
                return obj1.IsDisposed;
            }
            return obj1.InstanceID == obj2.InstanceID;
        }
        public static bool operator !=(ZObject obj1, ZObject obj2) => !(obj1 == obj2);
        public sealed override bool Equals(object obj)
        {
            if (obj is ZObject o)
            {
                return this == o;
            }
            else
            {
                return obj is null && IsDisposed;
            }
        }
        public sealed override int GetHashCode() => InstanceID;
        public sealed override string ToString() => IsDisposed ? $"NULL({GetType().Name})" : $"{Name}({GetType().Name}:{InstanceID})";


        //静态方法
        public static void Destory(ZObject obj) => DestoryInner(obj, false);
        public static void DestroyImmediate(ZObject obj) => DestoryInner(obj, true);
        internal static void DestoryInner(IDispose obj, bool isImmediate)
        {
            if (obj == null || obj.IsDisposed)
            {
                return;
            }
            obj.BeginDispose(isImmediate);
        }

        //销毁
        bool IDispose.IsDisposed => IsDisposed;
        void IDispose.BeginDispose(bool isImmediate) => BeginDispose(isImmediate);
        void IDispose.EndDispose() => EndDispose();
        protected internal abstract void BeginDispose(bool isImmediate);
        protected internal abstract void EndDispose();
        protected internal void ClearInstanceID()
        {
            instanceID = 0;
        }

    }
}