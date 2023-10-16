using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZFramework
{
    [SceneMark]
    public abstract class GameScene
    {
        public abstract string SceneName { get; }
        public abstract void OnLoaded(LoadSceneMode mode);
        public abstract void OnUnload();

    }
}
