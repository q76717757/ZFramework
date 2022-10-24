using UnityEngine;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        static BootStrap instance;
        [SerializeField]
        private BootFile boot;
        private DriveStrap drive;

        void Awake()
        {
            Log.ILog = new UnityLogger();
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
        void Start() => StartGame(boot);
        void OnApplicationQuit() => CloseGame();

        public static async void StartGame(BootFile boot)
        {
            if (instance == null)
            {
                var go = new GameObject("[BootStrap]");
                go.AddComponent<BootStrap>().boot = boot;
            }
            else
            {
                var assembly = await AssemblyLoader.LoadCode(boot);
                instance.drive = instance.gameObject.AddComponent<DriveStrap>();
                instance.drive.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
                instance.drive.StartGame(assembly);
            }
        }
        public static void CloseGame()
        {
            if (instance != null)
            {
                instance.drive.CloseGame();
                Destroy(instance.gameObject);
                instance = null;
            }
        }

    }
}