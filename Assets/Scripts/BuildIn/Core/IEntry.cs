namespace ZFramework
{
    public interface IEntry
    {
        void Start();
        void Reload();

        void Update();
        void LateUpdate();
        void Close();
    }

}
