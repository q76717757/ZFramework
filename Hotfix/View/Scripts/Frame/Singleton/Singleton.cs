/// <summary>
/// 单例模板
/// </summary>
/// <typeparam name="T">需要单例化的类名</typeparam>
public class Singleton<T> where T : new()
{
    
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                (_instance as IInitable)?.Init();
            }

            return _instance;
        }
    }
}


