using System;

namespace ZFramework
{
    public abstract class ZObject
    {
        protected internal long instanceID;
        public long InstanceID { get => instanceID; }
        public bool IsDisposed { get => InstanceID == 0; }
        public string Name { get; set; }

        protected ZObject()
        {
            instanceID = Game.instance.IdGenerater.GenerateInstanceId();
        }

        public static bool operator ==(ZObject obj1, ZObject obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }
            if (obj1 is null)
            {
                return obj2.IsDisposed;
            }
            if (obj2 is null)
            {
                return obj1.IsDisposed;
            }
            return obj1.InstanceID == obj2.InstanceID;
        }
        public static bool operator !=(ZObject obj1, ZObject obj2) => !(obj1 == obj2);
        public override bool Equals(object obj)
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
        public override int GetHashCode() => (int)InstanceID;
        public override string ToString() => IsDisposed ? $"NULL({GetType()})" : $"{Name}({GetType()})";

        protected internal abstract void Dispose();


        public static void Destory(ZObject obj)
        {
            if (obj == null) return;
            obj.Dispose();
        }
    }
}