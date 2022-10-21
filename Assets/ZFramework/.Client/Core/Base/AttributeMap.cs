using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    internal class AttributeMap
    {
        private readonly Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();//特性-类型映射表

        internal void Load(Type[] allTypes)
        {
            attributeMap.Clear();
            foreach (Type classType in allTypes)
            {
                if (classType.IsAbstract)
                {
                    continue;
                }
                foreach (BaseAttribute attribute in classType.GetCustomAttributes<BaseAttribute>(true))
                {
                    if (!attributeMap.TryGetValue(attribute.AttributeType, out List<Type> list))
                    {
                        list = new List<Type>();
                        attributeMap.Add(attribute.AttributeType, list);
                    }
                    list.Add(classType);
                }
            }
        }

        internal Type[] GetTypesByAttribute(Type AttributeType)
        {
            if (!attributeMap.TryGetValue(AttributeType, out List<Type> list))
            {
                list = new List<Type>();
                attributeMap.Add(AttributeType, list);
            }
            return list.ToArray();
        }
    }
}
