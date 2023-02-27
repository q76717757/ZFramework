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
        public static UserInterface userInterface;//临时用一下
        public override void Start()
        {
            Game.AddSingleComponent<TempA>();
            TempA.action += KeyACall;
            Exp().Invoke();
            return;

            Log.Info("ClientProcess");
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Application.targetFrameRate = 60;

            Game.AddSingleComponent<ZEventTemp>();//临时给Zevent接一下update生命周期
            Game.AddSingleComponent<TcpClientComponent>();
            Game.AddSingleComponent<WebRequestComponent>();
            Game.AddSingleComponent<ProtocolHandleComponent>();
            Game.AddSingleComponent<ScenePlayerHandleComponent>();
            Game.AddSingleComponent<JoyStickComponent>();

            var ui = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("User Interface"));
            userInterface = ui.AddComponent<UserInterface>();
            UIManager.Instance.Push(new StartPanel());
        }
        public override void Stop()
        {
        }
        public async ATask CallTest()//带async关键字的方法 会被编译器编译成状态机构造器 并直接返回AsyncVoid
        {
            Log.Error("Delay");
            await ATask.Delay(100);
            
            Log.Error("一般执行");
            //await Test1();

            //Log.Error("不执行");
            //var aa = Test2();//这个不执行  执行async方法会创建状态机实例  但是其他拓展的异步操作调用的时候就开始执行了  为了保持一直还是执行吧???还是学协程不执行?
            ////要执行还是不执行?

            //Log.Error("串行执行1");
            //int value = await aa;//等待已建任务
            //Log.Error("串行执行2");
            //int value2 = await aa;//执行完成的任务可以重新执行 可以串行  不能并行??或者全部并行
            //Log.Error(value + ":" + value2);


            Log.Info("完成");
            //Log.Error("延迟执行");
            //aa.Invoke();
            //Log.Error("并行执行");
            //aa.Invoke();//并行执行无效 //抛异常?还是跳过? 除非这个任务是同步的 直接完成的

            var atexta = Resources.LoadAsync<TextAsset>("");
            atexta.GetAwaiter().GetResult();

            Log.Info("完成---------->");
        }

        CancelToken token;
        void KeyACall()
        { 
            Log.Info("key A");
            token.Cancel();
        }
        void OnCalcel(TaskCancelType type)
        {
            Log.Info("CancelType --> " + type);
        }
        public async ATask Exp()
        {
            //空任务
            //await ATask.CompletedTask;
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

            ////取消操作-----------------------------
            token = new CancelToken();
            token.OnCancel(OnCalcel);
            await TestCancel().SetCancelToken(token);
            await TestCalcel2().SetCancelToken(token);

            //token.WithDestory(GameObject obj);
            //token.withDestort(Component com);
            //token.withTimeout(10);

            //token.OnCancel(() => { });
            //token.Cancel();

            //CancellationToken ct = new CancellationToken();
            //processToken.addCALLBACK((float t) => { Log.Info("进度 == " + t)});
            //Task A = new Task(() => { });
            //Task b = new Task(() => { });
            //Task c = new Task(() => { }, processToken);
            //ct.ThrowIfCancellationRequested();


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

            ////原生Task-----------------------------------
            //await Task.Run(() => 1234);
            //await Task.Delay(1000);
            //await Task.FromException(new System.Exception("异常"));
            //await Task.FromResult(1234);
            //await Task.FromCanceled(new CancellationToken());

            ////From
            //await Task.FromException(new System.Exception("异常"));
            //await Task.FromResult(1234);
            //await Task.FromCanceled(new CancellationToken());

            ////线程切换---------------------------------





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
            var index = await UniTask.WhenAny(task1, task2);

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