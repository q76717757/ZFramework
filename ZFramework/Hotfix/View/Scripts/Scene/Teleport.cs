using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public string MapName;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发事件:弹出加载界面");
        //var _LoadingPanel = new LoadingPanel();
        ////设置广告内容
        //_LoadingPanel.MapName = MapName;
        //_LoadingPanel.Advertisement = null;
        //_LoadingPanel.PlayerPosition = PlayerPosition;
        //_LoadingPanel.PlayerRotation = PlayerRotation;
        //UIManager.Instance.Pop();
        //UIManager.Instance.Push(_LoadingPanel);
    }
}
