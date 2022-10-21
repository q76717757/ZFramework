using System;

namespace ZFramework
{
    public interface IGameInstance
    {
        void Start(Type[] allTypes);
        void Reload(Type[] allTypes);

        void Update();
        void LateUpdate();
        void Close();
    }
}
