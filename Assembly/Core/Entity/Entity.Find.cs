using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Entity
    {
        //查找相对实体
        public Entity Find(string path)
        {
            string[] names = path.Split('/');
            Queue<string> a = new Queue<string>();
            for (int i = 0; i < names.Length; i++)
            {
                a.Enqueue(names[i]);
            }
            return InnelFind(a);
        }
        private Entity InnelFind(Queue<string> names)
        {
            if (names.Count > 0)
            {
                string name = names.Dequeue();
                //遍历本身的组件 找名字
                if (FindSelf(name, out Entity entity))
                {
                    return entity.InnelFind(names);
                }
            }
            return null;
        }
        private bool FindSelf(string name, out Entity output)
        {
            foreach (Entity entity in childrens.Values)
            {
                if (entity.Name == name)
                {
                    output = entity;
                    return true;
                }
            }
            output = null;
            return false;
        }

    }
}
