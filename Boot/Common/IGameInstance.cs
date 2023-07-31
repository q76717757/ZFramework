using System;

namespace ZFramework
{
    public interface IGameInstance
    {
        void Init(Type[] allTypes);
        void Start();
        void Reload(Type[] allTypes);

        void Update();
        void LateUpdate();
        void Close();
    }
}
