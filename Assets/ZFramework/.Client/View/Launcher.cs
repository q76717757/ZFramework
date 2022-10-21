﻿using System;
using UnityEngine;
using System.Threading.Tasks;
using ZFramework.EventType;

namespace ZFramework
{
    public class Launcher : EventCallback<EventType.LoadAssemblyFinish>
    {
        public override void Callback(LoadAssemblyFinish arg)
        {
            Log.ILog = new UnityLogger();
            Debug.Log(AssemblyLoader.CurrentBoot.GetDllName());

            Game.Root.AddComponent<ZEventTemp>();//临时给Zevent接一下update生命周期



            Log.Info("<color=green>Game Start!</color>");
        }
    }

}

namespace Test
{
    using Cysharp.Threading.Tasks;
    using System.Collections;

    public class UniTaskDemo : MonoBehaviour
    {
        bool isActive;
        public async UniTask<string> DemoAsync()
        {
            // You can await Unity's AsyncObject
            var asset = await Resources.LoadAsync<TextAsset>("foo");
            var txt = (await UnityEngine.Networking.UnityWebRequest.Get("https://...").SendWebRequest()).downloadHandler.text;
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("scene2");

            // .WithCancellation enables Cancel, GetCancellationTokenOnDestroy synchornizes with lifetime of GameObject
            var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());

            // .ToUniTask accepts progress callback(and all options), Progress.Create is a lightweight alternative of IProgress<T>
            var asset3 = await Resources.LoadAsync<TextAsset>("baz").ToUniTask(Progress.Create<float>(x => Debug.Log(x)));

            // await frame-based operation like a coroutine
            await UniTask.DelayFrame(100);

            // replacement of yield return new WaitForSeconds/WaitForSecondsRealtime
            await UniTask.Delay(TimeSpan.FromSeconds(10), ignoreTimeScale: false);

            // yield any playerloop timing(PreUpdate, Update, LateUpdate, etc...)
            await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

            // replacement of yield return null
            await UniTask.Yield();
            await UniTask.NextFrame();

            // replacement of WaitForEndOfFrame(requires MonoBehaviour(CoroutineRunner))
            await UniTask.WaitForEndOfFrame(this); // this is MonoBehaviour

            // replacement of yield return new WaitForFixedUpdate(same as UniTask.Yield(PlayerLoopTiming.FixedUpdate))
            await UniTask.WaitForFixedUpdate();

            // replacement of yield return WaitUntil
            await UniTask.WaitUntil(() => isActive == false);

            // special helper of WaitUntil
            await UniTask.WaitUntilValueChanged(this, x => x.isActive);

            // You can await IEnumerator coroutines
            await FooCoroutineEnumerator();
            IEnumerator FooCoroutineEnumerator()
            {
                yield return null;
            }

            // You can await a standard task
            await Task.Run(() => 100);

            // Multithreading, run on ThreadPool under this code
            await UniTask.SwitchToThreadPool();

            /* work on ThreadPool */

            // return to MainThread(same as `ObserveOnMainThread` in UniRx)
            await UniTask.SwitchToMainThread();

            // get async webrequest
            async UniTask<string> GetTextAsync(UnityEngine.Networking.UnityWebRequest req)
            {
                var op = await req.SendWebRequest();
                return op.downloadHandler.text;
            }

            var task1 = GetTextAsync(UnityEngine.Networking.UnityWebRequest.Get("http://google.com"));
            var task2 = GetTextAsync(UnityEngine.Networking.UnityWebRequest.Get("http://bing.com"));
            var task3 = GetTextAsync(UnityEngine.Networking.UnityWebRequest.Get("http://yahoo.com"));

            // concurrent async-wait and get results easily by tuple syntax
            var (google, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);

            // shorthand of WhenAll, tuple can await directly
            var (google2, bing2, yahoo2) = await (task1, task2, task3);

            // return async-value.(or you can use `UniTask`(no result), `UniTaskVoid`(fire and forget)).
            return (asset as TextAsset)?.text ?? throw new InvalidOperationException("Asset not found");
        }

    }

}
