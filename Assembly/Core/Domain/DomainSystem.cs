using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DomainLoaderAttribute : BaseAttribute
    {
        /// <summary>
        /// 优先级,默认是0,数字越大优先级越高,仅优先级最高的Loader会被执行.
        /// </summary>
        public int Priority { get; }
        public DomainLoaderAttribute(int priority = 0)
        { 
            Priority = priority;
        }
    }

    /// <summary>
    /// 业务域管理系统
    /// </summary>
    internal class DomainSystem
    {
        private readonly Dictionary<int, Domain> domains = new Dictionary<int, Domain>();

        //实例优先级最高的domainloader并执行
        internal void Start()
        {
            Type activeLoader = null;
            int priority = 0;

            Type[] loaderTypes = Game.GetTypesByAttribute<DomainLoaderAttribute>();
            foreach (Type loaderType in loaderTypes)
            {
                DomainLoaderAttribute attribute = loaderType.GetCustomAttribute<DomainLoaderAttribute>();
                if (activeLoader == null)
                {
                    activeLoader = loaderType;
                    priority = attribute.Priority;
                }
                else
                {
                    if (attribute.Priority > priority)
                    {
                        priority = attribute.Priority;
                        activeLoader = loaderType;
                    }
                }
            }
            if (activeLoader != null && Activator.CreateInstance(activeLoader) is IDomainLoader loader)
            {
                try
                {
                    loader.Load();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        internal void Close()
        {
            foreach (int domainID in domains.Keys.ToArray())
            {
                UnloadDomain(domainID);
            }
        }

        internal Domain GetDomain(int domainID)
        {
            domains.TryGetValue(domainID, out Domain output);
            return output;
        }
        internal void LoadDomain(Domain domain)
        {
            if (domain == null)
            {
                return;
            }
            if (domain.IsLoaded || domains.ContainsKey(domain.DomainID))
            {
                Log.Error($"domainID:{domain.DomainID}已加载,应该为每个要加载的domain都分配一个独立ID");
                return;
            }
            try
            {
                domain.OnLoad();
            }
            catch (Exception e)
            {
                Log.Error($"加载domain:{domain.DomainID},但期间发生了错误" + e);
            }
            finally
            {
                domains.Add(domain.DomainID, domain);
            }
        }
        internal void UnloadDomain(int domainID)
        {
            if (domains.TryGetValue(domainID, out Domain domain))
            {
                try
                {
                    domain.OnUnload();
                }
                catch (Exception e)
                {
                    Log.Error($"卸载domain:{domain.DomainID},但期间发生了错误" + e);
                }
                finally
                {
                    domains.Remove(domainID);
                }
            }
            else
            {
                Log.Error($"尝试卸载并未加载的域{domainID},已跳过");
            }
        }

    }
}
