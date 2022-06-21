namespace ZFramework
{
    public static class Define
    {
        public const string UnityTempDllDirectory = "../Unity/Temp/Bin/Debug";

        public static RunMode RunMode;




#if UNITY_EDITOR
        public const bool IsEditor = true;
#else
        public const bool IsEditor = false;
#endif

    }
}
