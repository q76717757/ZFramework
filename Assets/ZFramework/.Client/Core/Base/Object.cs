using System;

namespace ZFramework
{
    public abstract class Object : IDisposable
    {
        public long InstanceID { get; private set; }
        public bool IsDisposed => InstanceID == 0;
        public string Name { get; set; }

        protected Object(long instanceID)
        {
            InstanceID = instanceID;
        }

        public virtual void Dispose()
        {
            InstanceID = 0;
        }







        public override int GetHashCode()
        {
            return (int)InstanceID;
        }
        public override bool Equals(object obj)
        {
            if (obj is Object o)
            {
                return this == o;
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(Object entity1, Object entity2)
        {
            if (ReferenceEquals(entity1, entity2))
            {
                return true;
            }
            if (entity1 is null)
            {
                return entity2.IsDisposed;
            }
            if (entity2 is null)
            {
                return entity1.IsDisposed;
            }
            return entity1.InstanceID == entity2.InstanceID;
        }
        public static bool operator !=(Object entity1, Object entity2)
        {
            return !(entity1 == entity2);
        }
        public override string ToString()
        {
            return IsDisposed ? $"NULL({GetType()})" : $"{Name}({GetType()})";
        }
    }
}