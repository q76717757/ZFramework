using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;


namespace ZFramework
{
    public class TempAUpdate : OnUpdateImpl<TempA>
    {
        public override void OnUpdate(TempA self)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                TempA.action?.Invoke();
            }
        }
    }
    public class TempA : SingleComponent<TempA>
    {
        public static Action action;
    }

    public class ClientProcess : VirtualProcess
    {
        public override void Start()
        {
            Game.AddSingleComponent<TempA>();
            TempA.action += KeyACall;
            
            Exp().Invoke();//开协程 不带取消

            //Exp().Invoke(new CancelToken());  //开协程 带取消token
            //TestCalcel2().Invoke(new CancelToken<int>()); //带取消 带返回
        }
        public override void Stop()
        {
        }

        TaskWatcher watcher;
        void KeyACall()
        { 
            Log.Info("key A");
            watcher.Cancel();
        }

        public async ATask TestCancel()
        {
            Log.Info("A");
            await Task.Delay(5000);
            Log.Info("B");
        }
        public async ATask<int> TestCalcel2()
        {
            Log.Info("C");
            await Task.Delay(5000);
            Log.Info("D");
            return 22;
        }
        public async ATask Exp() //异步方法的源是 AsyncMethod 其他源都是子源
        {
            #region 基本用法
            //await TestCancel();//简单用法 等任务执行 直到任务完成或者任务异常   完成往下走  异常往上返  异常了没必要往下走了
            //ATask dt = TestCancel();
            //await dt;
            //await dt;//重复等待怎么处理   onCompleted不使用=赋MoveNext  用+= 这样完成的时候可以同时让多个状态机走下一状态
            #endregion

            #region 特殊任务
            //空任务
            //await ATask.CompletedTask;
            //中断任务
            //await ATask.Break(); //类似协程中的yield break
            //结果任务
            //ATask.FromResult();
            #endregion

            #region Unity异步
            //GameObject gameobject = await Resources.LoadAsync<GameObject>("MapCamera") as GameObject;
            //Log.Info("Resources.LoadAsync-->" + gameobject);

            //AssetBundle asetbundle = await AssetBundle.LoadFromFileAsync(Application.dataPath + "/AssetBundle/test");
            //Log.Info("AssetBundle.LoadFromFileAsync-->" + asetbundle);

            //var gameobject2 = (await asetbundle.LoadAssetAsync<TextAsset>("Assembly-CSharp.dll.bytes")).asset;
            //Log.Info("AssetBundle.LoadAssetAsync-->" + gameobject2);

            //string text = (await UnityWebRequest.Get("Http://www.baidu.com").SendWebRequest()).text;

            //Log.Info("UnityWebRequest-->" + text);

            //await SceneManager.LoadSceneAsync(0);
            //Log.Info("SceneManager.LoasSceneAsync-->");
            #endregion


            //取消操作-----------------------------
            watcher = new TaskWatcher();
            TaskWatcher watcher2 = new TaskWatcher(); //多个wacther看同一个task呢?
            watcher.TimeOut = 0.5f;
            watcher.OnCountDown += (t, p) =>
            {
                Log.Info($"剩余时间{t} : 流逝进度{p}");
            };
            watcher.Cancel();//手动取消  //取消命令直接将任务强制完成  task完成的时候状态机会调GerResult 这个方法里面检查token如果是取消token 则抛出异常 这样外部就知道是被取消了

            //子任务异常/超时  逐层上报到始祖任务 抛异常  直到有token那一层的父为止
            //子任务取消 父任务不用取消
            //父任务取消  子任务肯定取消 如何实现token遗传?
            ATask target = TestCancel();
            target.Invoke();


            WatcherResult wactherResult = await target.SetWatcher(ref watcher);//也可以后绑定的方式  观察一个已启动的任务
            WatcherResult wactherResult2 = await target.SetWatcher(ref watcher2); //支持多个wacther看同一个task  但watcher是一次性的

            //await TestCancel().SetWatcher(ref watcher);//异步的可取消调用
            //TestCancel().SetWatcher(watcher).Invoke(); //同步的可取消调用
            //TestCancel().SetWatcher(watcher).SetWatcher(watcher).Invoke();//错误套娃用法 先不处理

            Log.Info("R1->" + wactherResult.CompletionType);
            Log.Info("R2->" + wactherResult2.CompletionType);

            switch (wactherResult.CompletionType)  //可以根据任务的完成情况  做逻辑分发  处理取消  超时  异常等
            {
                case TaskCompletionType.Success://任务完成  结果有效
                    //tokenResult.result
                    break;
                case TaskCompletionType.Cancel://手动取消
                    break;
                case TaskCompletionType.Timeout://任务超时
                    break;
                case TaskCompletionType.Exception://异常  异常对象
                    //tokenResult.exception
                    break;
                default:
                    break;
            }
            #region Other

            //进度-------------------------------
            //设置进度回调  每帧回调一次/仅当改变
            //await CallTest().Process();
            //await CallTest().SetProcess();
            ////设置当前任务的进度
            //for (int i = 0; i < 10; i++)
            //{
            //    await Task.SetProcess(0.1f * i);
            //    await YieldInstruction;
            //}X
            //await Task.SetProcess(0.2f);

            ////延时---------------------------------
            //await ATask.NextFrame();
            //await ATask.WaitForEndOfFrame();
            //await ATask.WaitForFixedUpdate();
            //await ATask.Delay(1, true);
            //await ATask.WaitUntil(default);
            //await ATask.WaitWhile();

            ////协程
            //IEnumerator IE()
            //{
            //    yield return null;
            //    yield return new WaitForEndOfFrame();
            //    yield return new WaitForFixedUpdate();
            //    yield return new WaitForSeconds(1);
            //    yield return new WaitForSecondsRealtime(1);
            //    yield return new WaitUntil(() => false);
            //    yield return new WaitWhile(() => false);
            //}
            //await IE();

            //异常处理
            //可以正常抛异常
            //也可以FromException(ex);//抛异常

            ////原生Task-----------------------------------
            //await Task.Run(() => 1234);
            //await Task.Delay(1000);
            //await Task.FromException(new System.Exception("异常"));
            //await Task.FromResult(1234);
            //await Task.FromCanceled(new CancellationToken());
            //CancellationToken ct = new CancellationToken();      //原生取消怎么处理 ???  如果等待一个带token原生task 原生taks被取消
            //Task A = new Task(() => { }, ct);
            //ct.ThrowIfCancellationRequested();

            ////From
            //await Task.FromException(new System.Exception("异常"));
            //await Task.FromResult(1234);
            //await Task.FromCanceled(new CancellationToken());

            ////线程切换---------------------------------
            // await SwitchToMain();      //切换到主线程
            // await SwitchToThreadPool();     //切换到子线程

            ////All Any------------------------------------
            //await Task.WhenAll();
            //await Task.WhenAny();
            //await ATask.WhenAll();
            //await ATask.WhenAny();

            ////DoTween
            ////Key
            ////Click
            ////Video
            ////Auidio 
            #endregion
        }
    }
}

namespace ZFramework.Temp
{
    using Cysharp.Threading.Tasks;
    public class Demo : MonoBehaviour
    {
        public async ATask DemoAsync()
        {
            // .WithCancellation 会启用取消功能，GetCancellationTokenOnDestroy 表示获取一个依赖对象生命周期的Cancel句柄，当对象被销毁时，将会调用这个Cancel句柄，从而实现取消的功能
            var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());

            // .ToUniTask 可接收一个 progress 回调以及一些配置参数，Progress.Create是IProgress<T>的轻量级替代方案
            var asset3 = await Resources.LoadAsync<TextAsset>("baz").ToUniTask(Progress.Create<float>(x => Debug.Log(x)));

            // 等待一个基于帧的延时操作（就像一个协程一样）
            await UniTask.DelayFrame(100);

            // yield return new WaitForSeconds/WaitForSecondsRealtime 的替代方案
            await UniTask.Delay(System.TimeSpan.FromSeconds(10), ignoreTimeScale: false);

            // 可以等待任何 playerloop 的生命周期(PreUpdate, Update, LateUpdate, 等...)
            await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

            // yield return null 替代方案
            await UniTask.Yield();
            await UniTask.NextFrame();

            // WaitForEndOfFrame 替代方案 (需要 MonoBehaviour(CoroutineRunner))
            await UniTask.WaitForEndOfFrame(this); // this 是一个 MonoBehaviour

            // yield return new WaitForFixedUpdate 替代方案，(和 UniTask.Yield(PlayerLoopTiming.FixedUpdate) 效果一样)
            await UniTask.WaitForFixedUpdate();

            // yield return WaitUntil 替代方案
            await UniTask.WaitUntil(() => 1 == 1);

            // WaitUntil拓展，指定某个值改变时触发
            await UniTask.WaitUntilValueChanged(this, x => 1);

            // 你可以直接 await 一个 IEnumerator 协程
            await FooCoroutineEnumerator();

            // 多线程示例，在此行代码后的内容都运行在一个线程池上
            await UniTask.SwitchToThreadPool();

            /* 工作在线程池上的代码 */

            // 转回主线程
            await UniTask.SwitchToMainThread();

            // 获取异步的 webrequest
            async UniTask<string> GetTextAsync(UnityWebRequest req)
            {
                //var op = await req.SendWebRequest().GetAwaiter();
                //return op.downloadHandler.text;
                return default;// (op as UnityWebRequestAsyncOperation).webRequest.downloadHandler.text;
            }

            var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
            var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
            var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));

            // 构造一个async-wait，并通过元组语义轻松获取所有结果
            var (google, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);
            var (winArgumentIndex, result1, result2) = await UniTask.WhenAny(task1, task2);

            // WhenAll简写形式
            var (google2, bing2, yahoo2) = await (task1, task2, task3);

            // 返回一个异步值，或者你也可以使用`UniTask`(无结果), `UniTaskVoid`(协程，不可等待)
            //return default;// (asset as TextAsset)?.text ?? throw new InvalidOperationException("Asset not found");
        }

        IEnumerator FooCoroutineEnumerator()
        {
            yield return null;
            //yield return new UnityEngine.wait
        }
    }
}