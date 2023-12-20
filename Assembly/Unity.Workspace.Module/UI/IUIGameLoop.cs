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
        void OnUpdate();//��������һ���������� һ����Active��Update һ����ȫ��Update(����UI��̨������Щ����)
        void SetInteractable(bool state);
    }

}
