using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    [Entry]
    public abstract class Entry<T> : IEntry where T : Entry<T>
    {
        private static Entity _root;
        public static Entity Root { get => _root; }
        void IEntry.OnStart(Entity gameRoot)
        {
            _root = gameRoot.AddChild(typeof(T).Name);
            OnStart();
        }
        public abstract void OnStart();
    }
}
