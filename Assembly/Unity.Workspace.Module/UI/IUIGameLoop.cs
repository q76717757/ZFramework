using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZFramework
{
    public interface IUIGameLoop
    {
        void OnAwake();
        void OnDestroy();
        void OnOpen();
        void OnClose();
        void OnFocus();
        void OnLoseFocus();
        void OnUpdate();//考虑增加一个生命周期 一个是Active才Update 一个是全程Update(允许UI后台持续做些事情)
        void SetInteractable(bool state);
    }

}
