using System.Reflection;

namespace ZFramework
{
    public interface IGameInstance
    {
        void Load(Assembly code);
        void Load(Assembly data, Assembly func);
        void Reload(Assembly func);

        void Update();
        void LateUpdate();
        void Close();
    }

}
