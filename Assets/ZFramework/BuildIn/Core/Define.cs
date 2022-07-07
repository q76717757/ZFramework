namespace ZFramework
{
    public static class Define  //热更脚本是后编译,排除在unity主工程外  编译宏不同步  用一些静态字段作为桥梁  进行逻辑分发
    {
       
        public static CompileMode RunMode;

#if UNITY_EDITOR
        //热更程序集的名称
        public static string[] csprojNames = new string[]
        {
            "Model","Logic","ViewModel","ViewLogic",
        };

        public const bool IsEditor = true;
#else
        public const bool IsEditor = false;
#endif

    }
}
