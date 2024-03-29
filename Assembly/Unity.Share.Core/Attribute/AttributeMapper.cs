﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    /// <summary>
    /// 特性--类型 映射表
    /// </summary>
    internal class AttributeMapper
    {
        private readonly Dictionary<Type, List<Type>> mapper = new Dictionary<Type, List<Type>>();

        internal void Load(IList<Type> allTypes)
        {
            foreach (Type classType in allTypes)
            {
                if (classType.IsAbstract)
                {
                    continue;
                }
                foreach (BaseAttribute attribute in classType.GetCustomAttributes<BaseAttribute>(true))
                {
                    if (!mapper.TryGetValue(attribute.AttributeType, out List<Type> list))
                    {
                        list = new List<Type>();
                        mapper.Add(attribute.AttributeType, list);
                    }
                    list.Add(classType);
                }
            }
        }

        internal Type[] GetTypesByAttribute(Type type)
        {
            if (mapper.TryGetValue(type, out List<Type> list))
            {
                return list.ToArray();
            }
            else
            {
                return Array.Empty<Type>();
            }
        }
    }
}
