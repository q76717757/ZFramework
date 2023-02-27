using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZFramework
{
    public partial struct ATask
    {
        public static ATask CompletedTask { get; } = new ATask();
        public static ATask WhenAll() => default;
        public static ATask WhenAny() => default;
        public static ATask SwitchToThreadPool() => default;
        public static ATask SwitchToMainThread() => default;
        public static Task ToTask(ATask task) => default;
    }
}
