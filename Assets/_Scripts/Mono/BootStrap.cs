using UnityEngine;
using System;

namespace ZFramework
{
    //备忘 当前架构是个ECS半成品 目前UIType的内容不用写
    //unity editor下用热重载模式 编译dll出来再运行
    //在场景中搭建好UI后右键点构建范例  会自动生成预制体设好bundle name
    //会自动生成Component/System脚本 以及对应的UIType 需要确保的是这个搭好的UI名字是可以正常用来当文件名的

    public enum RunMode
    {
        HotReload_EditorRuntime, //热重载模式
        Mono_RealMachine,//发布模式
    }

    [DisallowMultipleComponent]
    public class BootStrap : MonoBehaviour
    {
        public static Account Account;

        public IEntry entry;
        public RunMode mode;

        private void Awake()
        {
            Define.RunMode = mode;

            DontDestroyOnLoad(gameObject);
            Log.ILog = new UnityLogger();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };
            ET.ETTask.ExceptionHandler += Log.Error;

            //ZLogVisualization.Visua = true;

            Application.wantsToQuit += OnApplicationWantsToQuit;

            Screen.SetResolution(1920, 1080, true);
        }

        public void Start()
        {
            entry = AssemblyLoader.GetEntry(mode);
            if (entry != null)
            {
                entry.Start();
            }
            else
            {
                Log.Error("entry is null!!!");
                Destroy(this);
            }

        }

        private void Update() => entry.Update();
        private void LateUpdate() => entry.LateUpdate();
        private void OnApplicationFocus(bool focus) => entry?.Focus(focus);
        private bool OnApplicationWantsToQuit() => entry == null || entry.WantClose();
        private void OnApplicationQuit() => entry.Close();

    }

    public class Account
    {
        public int id;
        public string username;
        public string password;
        public int departmentid;
        public int postid;
        public int roleid;
        public string realname;
        public int sex;
        public string phone;
        public string email;


    }
}