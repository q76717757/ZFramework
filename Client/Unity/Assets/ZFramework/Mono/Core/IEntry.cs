namespace ZFramework
{
    public interface IEntry
    {
        void Start();
        void Reload();

        void Update();
        void LateUpdate();
        void Focus(bool focus);
        bool WantClose();
        void Close();
    }

}
