using System;

namespace ZFramework
{
    public abstract class Object : IDisposable
    {
        public long InstanceID { get; internal set; }
        public bool IsDisposed => InstanceID == 0;

        protected Object()
        {
        }

        public virtual void Dispose()
        {
        }







        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return this == obj as Object;
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
    }
}