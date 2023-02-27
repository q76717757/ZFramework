#if !SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    internal class IEnumeratorSource
    {

    }

    public static partial class GetAwaiterExpansions
    {
        public static ATask GetAwaiter(this IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }
            return default;// new ATask<AssetBundleRequest>(new AssetBundleRequestSource(asyncOperation));
        }
    }
}
#endif