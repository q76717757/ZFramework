#if ENABLE_HYBRIDCLR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace ZFramework
{
    [Preserve]
    public class ReservedAssembly : MonoBehaviour
    {
        private void Awake()
        {
            StringBuilder sb = new StringBuilder();
#if UNITY_STANDALONE_WIN
#if NET_UNITY_4_8 || NET_4_6
            sb.AppendLine(typeof(Microsoft.Win32.SafeHandles.SafeRegistryHandle).ToString());//mscorlib.dll
            sb.AppendLine(typeof(Microsoft.Win32.IntranetZoneCredentialPolicy).ToString());//System.dll
            sb.AppendLine(typeof(Microsoft.Win32.SafeHandles.SafePipeHandle).ToString());//System.Core.dll
#endif
#endif
            sb.AppendLine(typeof(AOT.MonoPInvokeCallbackAttribute).ToString());//UnityEngine.CoreModule.dll
            sb.AppendLine(typeof(UnityEngine.Event).ToString());//UnityEngine.IMGUIModule.dll
            sb.AppendLine(typeof(UnityEngine.AndroidJavaRunnable).ToString());//UnityEngine.AndroidJNIModule.dll
            sb.AppendLine(typeof(UnityEngine.AudioSettings).ToString());//UnityEngine.AudioModule.dll
            sb.AppendLine(typeof(UnityEngine.UI.AnimationTriggers).ToString()); //UnityEngine.UI.dll
            sb.AppendLine(typeof(UnityEngine.WWWForm).ToString());//UnityEngine.UnityWebRequestModule.dll
            sb.AppendLine(typeof(UnityEngine.AssetBundleCreateRequest).ToString());//UnityEngine.AssetBundleModule.dll
            sb.AppendLine(typeof(UnityEngine.CanvasGroup).ToString());//UnityEngine.UIModule.dll
            sb.AppendLine(typeof(UnityEngine.TextGenerator).ToString());//UnityEngine.TextRenderingModule.dll
            sb.AppendLine(typeof(UnityEngine.SharedBetweenAnimatorsAttribute).ToString());//UnityEngine.AnimationModule.dll
            sb.AppendLine(typeof(UnityEngine.ParticleSystem).ToString());//UnityEngine.ParticleSystemModule.dll
            sb.AppendLine(typeof(UnityEngine.Playables.PlayableDirector).ToString());//UnityEngine.DirectorModule.dll
            sb.AppendLine(typeof(UnityEngine.Video.VideoPlayer).ToString());//UnityEngine.VideoModule.dll
            sb.AppendLine(typeof(UnityEngine.LocationService).ToString());//UnityEngine.InputLegacyModule.dll
            sb.AppendLine(typeof(UnityEngine.Networking.DownloadHandlerTexture).ToString());//UnityEngine.UnityWebRequestTextureModule.dll
            sb.AppendLine(typeof(UnityEngine.JsonUtility).ToString());//UnityEngine.JSONSerializeModule
            Debug.Log(sb.ToString());
        }
    }
}
#endif