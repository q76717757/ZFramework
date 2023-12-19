using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Entity
    {
        //查找相对实体
        public Entity Find(string path)
        {
            ThrowIfDisposed();
            return InnerFind(path.Split('/'), 0);
        }
        private Entity InnerFind(string[] names,int index)
        {
            if (index < names.Length)
            {
                foreach (Entity entity in childrens.Values)
                {
                    if (entity.Name == names[index])
                    {
                        return InnerFind(names, index + 1);
                    }
                }
            }
            return null;
        }
    }
}
