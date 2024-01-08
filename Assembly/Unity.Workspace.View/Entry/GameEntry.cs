using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    public sealed class GameEntry : Entry<GameEntry>
    {
        public override void OnStart() => Init().Invoke();

        async ATask Init()
        {
            Game.AddGlobalComponent<VirtualFileSystem>().Init(new CosFileServer());

            Game.AddGlobalComponent<UIManager, int, int>(1920, 1080);

        }
    }
}
