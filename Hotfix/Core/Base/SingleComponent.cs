using System;

namespace ZFramework
{
    public abstract class SingleComponent<T> : Component where T:SingleComponent<T>
    {
        private static T _instance;
        public static T Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = Game.AddSingleComponent<T>() as T;
                }
                return _instance;
            }
        }
    }
}
