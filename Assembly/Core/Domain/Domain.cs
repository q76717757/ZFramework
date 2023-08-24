using System;
using System.Collections.Generic;

namespace ZFramework
{
    /// <summary>
    /// 业务域 是处理一个完整业务场景的最小颗粒 包含一颗完整的ECS节点树
    /// </summary>
    public abstract class Domain
    {
        private Entity root;//这个根节点是隐藏的 不对外提供任何访问 domain本质就是对root的一层保护
        internal Entity Root
        {
            get
            {
                if (IsLoaded)
                {
                    return root;
                }
                else
                {
                    throw new Exception("Domain is Unloaded");
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                return root != null;
            }
        }
        public int DomainID { get; set; }
        //public string DomainName { get; set; }
        //public int MachineID { get; set; }
        //public int InnerPort { get; set; }

        internal void OnLoad()
        {
            root = new Entity(this);
            OnStart();
        }
        internal void OnUnload()
        {
            Entity.Destory(root);
            //不需要set root null;
            //否则如果在domain unload 调了Root 会发生错误  因为destory方法帧末执行  但是这个当前帧set root null,帧末就调用不到Root了
            //root.deatory就是延迟的  真正root移除调之后虽然root对象还在  但是因为重载运算符的关系  ==null判断将会成立  也就是IsLoaded会正确返回false
            //root会和domain一起被gc
        }
        protected abstract void OnStart();




        //查找实体
        public Entity Find(string path)
        {
            return Root.Find(path);
        }

        //添加域级组件
        public Component AddComponentInDomain(Type type)
        { 
            return Root.AddComponent(type);
        }
        public T AddComponentInDomain<T>() where T : Component
        { 
            return Root.AddComponent<T>();
        }

        //获取域级组件
        public Component GetComponentInDomain(Type type)
        {
            return Root.GetComponent(type);
        }
        public T GetComponentInDomain<T>()
        { 
            return Root.GetComponent<T>();
        }
    }
}
