
using UnityEngine.Networking;

namespace Cysharp.Threading.Tasks.Internal
{

    internal static class UnityWebRequestResultExtensions
    {
        public static bool IsError(this UnityWebRequest unityWebRequest)
        {
            var result = unityWebRequest.result;
            return (result == UnityWebRequest.Result.ConnectionError)
                || (result == UnityWebRequest.Result.DataProcessingError)
                || (result == UnityWebRequest.Result.ProtocolError);
        }
    }

}