using System;

namespace ZFramework
{
    public abstract class Object: IDisposable
    {
        protected Object()
        {
            InstanceID = IdGenerater.Instance.GenerateInstanceId();
        }

        [JsonIgnore]
        public long InstanceID { get; private set; }
        [JsonIgnore]
        public bool IsDisposed => InstanceID == 0;
        public void Dispose()
        {
            InstanceID = 0;
        }
        public override int GetHashCode()
        {
            return (int)InstanceID;
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
                return entity2.InstanceID == 0;
            }
            if (entity2 is null)
            {
                return entity1.InstanceID == 0;
            }
            return entity1.InstanceID == entity2.InstanceID;
        }
        public static bool operator !=(Object entity1, Object entity2)
        {
            return !(entity1 == entity2);
        }

    }
}