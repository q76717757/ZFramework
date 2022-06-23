using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public abstract class Component : Object
    {
        protected Component() { }

        [JsonIgnore]
        public Entity Entity { get; set; }

        public override string ToString()
        {
            return $"{GetType()}(Component)[{InstanceID}]";
        }

        public static T CreateComponent<T>()  where T : Component
        {
            Component com = (Component)Activator.CreateInstance(typeof(T));
            //PlayLoop.Instance.Awake(com);
            PlayLoop.Instance.AddComponentToPlayloop(com);
            return (T)com;
        }
        public static Component CreateComponent(Type type)
        {
            if (type.IsAbstract || !type.IsSubclassOf(typeof(Component)))
            {
                return null;
            }
            Component com = (Component)Activator.CreateInstance(type);
           
            PlayLoop.Instance.AddComponentToPlayloop(com);
            return com;
        }
    }
}
