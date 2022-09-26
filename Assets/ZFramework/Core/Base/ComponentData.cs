using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [Flags]
    public enum ComponentStatus : byte
    {
        None = 0,
        IsFromPool = 1,
        IsRegister = 1 << 1,
        IsComponent = 1 << 2,
        IsCreated = 1 << 3,
        IsNew = 1 << 4,
    }
      
    public abstract class ComponentData : Object
    {
        public Type Type => GetType();
        public Entity Entity { get; }
        public VirtualProcess Process { get => Entity.Process; }

        public bool Enable { get; set; }

        public ComponentData() : base(IdGenerater.Instance.GenerateInstanceId())
        {
            var a = new UnityEngine.Object();

        }
        public ComponentData(Entity entity) : base(IdGenerater.Instance.GenerateInstanceId())
        {
            Entity = entity;
        }

        public ComponentData GetComponent(Type type) => Entity.GetComponent(type);
        public T GetComponent<T>() where T : ComponentData => Entity.GetComponent<T>();
        public T GetComponentInParent<T>(bool includSelf = true) where T : ComponentData => Entity.GetComponentInParent<T>(includSelf);
        public T GetComponentInChildren<T>(bool includSelf = true) where T : ComponentData => Entity.GetComponentInChildren<T>(includSelf);
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : ComponentData => Entity.GetComponentsInChilren<T>(includSelf);

    }
}
