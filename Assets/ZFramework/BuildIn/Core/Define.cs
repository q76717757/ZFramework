namespace ZFramework
{
    public static class Define
    { 
        public static CompileMode RunMode;

#if UNITY_EDITOR
        public const bool IsEditor = true;
#else
        public const bool IsEditor = false;
#endif

    }
}
