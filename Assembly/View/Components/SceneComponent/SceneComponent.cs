using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
//using UnityEngine.InputSystem;

namespace ZFramework
{
    public class SceneComponentAwake : OnAwakeImpl<SceneComponent>
    {
        public override void OnAwake(SceneComponent self)
        {
            //Log.Info("SceneComponent Awake");
            self.Init();
        }
    }
    //public class SceneComponentUpdate : OnUpdateImpl<SceneComponent>
    //{
    //    public override void OnUpdate(SceneComponent self)
    //    {
    //        Log.Info("SceneComponent Update");
    //    }
    //}
    //public class SceneComponentLateUpadte : OnLateUpdateImpl<SceneComponent>
    //{
    //    public override void OnLateUpdate(SceneComponent self)
    //    {
    //        Log.Info("SceneCompnent LateUpdate");
    //    }
    //}
    //public class SceneComponentDestory : OnDestoryImpl<SceneComponent>
    //{
    //    public override void OnDestory(SceneComponent self)
    //    {
    //        Log.Info("SceneComponent Destory");
    //    }
    //}

    public class SceneComponent : Component
    {
        Dictionary<string,SceneItem> sceneItems =  new Dictionary<string, SceneItem>();
        public void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            Type[] types = Game.GetTypesByAttribute<UnitySceneAttribute>();
            foreach (Type type in types)
            {
                var scene = Activator.CreateInstance(type) as SceneItem;
                sceneItems.Add(scene.SceneName, scene);
            }

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        public void Uninit()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mod)
        {
            if (sceneItems.TryGetValue(scene.name, out SceneItem item))
            {
                item.OnLoaded(mod);
            }
        }
        private void OnSceneUnloaded(Scene scene)
        {
            if (sceneItems.TryGetValue(scene.name, out SceneItem item))
            {
                item.OnUnload();
            }
        }

        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

    }

    public class UnitySceneAttribute : BaseAttribute
    {
    }

    [UnityScene]
    public abstract class SceneItem
    {
        public abstract string SceneName { get; }
        public abstract void OnLoaded(LoadSceneMode mode);
        public abstract void OnUnload();
    }

    public class BootScene : SceneItem
    {
        public override string SceneName => "Boot";
        public override void OnLoaded(LoadSceneMode mode)
        {
            Log.Info("Boot Loaded");
            //SceneManager.LoadScene("TianTao");
        }
        public override void OnUnload()
        {
            Log.Info("Boot Unload");
        }
    }

    public class TianTao : SceneItem
    {
        public override string SceneName => "TianTao";
        public override void OnLoaded(LoadSceneMode mode)
        {
            Log.Info("TianTao Loaded");
            //XRActiveController.SetActive(true);
        }
        public override void OnUnload()
        {
            Log.Info("TianTao Unload");
        }
    }

}
