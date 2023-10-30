using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZFramework
{
    public class SceneManagementComponentAwake : OnAwakeImpl<SceneManagementComponent>
    {
        public override void OnAwake(SceneManagementComponent self)
        {
            self.Awake();
        }
    }
    public class SceneManagementComponentDestory : OnDestoryImpl<SceneManagementComponent>
    {
        public override void OnDestory(SceneManagementComponent self)
        {
            self.OnDestory();
        }
    }

    /// <summary>
    /// 场景管理组件
    /// </summary>
    public class SceneManagementComponent : Component
    {
        Dictionary<string, GameScene> gameScenes = new Dictionary<string, GameScene>();
        HashSet<GameScene> activeScene = new HashSet<GameScene>();

        public void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            Type[] types = Game.GetTypesByAttribute<SceneMarkAttribute>();
            foreach (Type type in types)
            {
                GameScene scene = Activator.CreateInstance(type) as GameScene;
                gameScenes.Add(scene.SceneName, scene);
            }
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (gameScenes.TryGetValue(scene.name, out GameScene gameScene) && activeScene.Add(gameScene))
            {
#if UNITY_EDITOR
                Log.Info($"LoadScene -> {gameScene.SceneName}");
#endif
                gameScene.OnLoaded(loadMode);
            }
        }
        private void OnSceneUnloaded(Scene scene)
        {
            if (gameScenes.TryGetValue(scene.name, out GameScene gameScene) && activeScene.Remove(gameScene))
            {
#if UNITY_EDITOR
                Log.Info($"UnloadScene -> {gameScene.SceneName}");
#endif
                gameScene.OnUnload();
            }
        }
        public void OnDestory()
        {
            foreach (GameScene gameScene in activeScene)
            {
#if UNITY_EDITOR
                Log.Info($"UnloadScene -> {gameScene.SceneName}");
#endif
                gameScene.OnUnload();
            }
            activeScene.Clear();
        }

        public void CreateScene(string sceneName, LoadSceneMode loadScene)
        {
            Scene scene = SceneManager.CreateScene(sceneName);//这个API不会触发SceneOnLoad事件  这里手动处理一下
            if (scene.IsValid())
            {
                OnSceneLoaded(scene, loadScene);
                if (loadScene == LoadSceneMode.Single)
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
                }
            }
        }
        public void LoadScene(string sceneName, LoadSceneMode loadScene)
        {
            //切换到Loading场景

            //检查场景的资源是否已经被加载??

            //加载场景资源

            SceneManager.LoadScene(sceneName, loadScene);
        }
        public void CallActiveSceneOnLoaded()
        {
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

    }
}
