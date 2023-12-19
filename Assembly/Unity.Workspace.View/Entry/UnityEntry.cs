using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    public sealed class UnityEntry : Entry<UnityEntry>
    {
        public override void OnStart()
        {
            VirtualFileSystem.Init(new CosFileServer());

            Game.AddGlobalComponent<UIEventSystem>();
            Game.AddGlobalComponent<UIManager>();
            Game.AddGlobalComponent<FouseInputModel>();


            UIManager.Instance.OpenAsync<TopUIWindow>().Invoke();
            UIManager.Instance.OpenNew<BackgroundUIWindow>().Invoke();
        }
    }
}
