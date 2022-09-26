using System.Reflection;

namespace ZFramework
{
    public interface IEntry
    {
        void Load(Assembly code);
        void Load(Assembly model, Assembly logic);
        void Reload(Assembly logic);

        void Update();
        void LateUpdate();
        void Close();
    }

}
