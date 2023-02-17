using UnityEngine;
using ZFramework;

/// <summary>
/// 摄像机控制器
/// </summary>
public class CameraController : MonoBehaviour
{
    public const string 镜头 = "CametaCtrl";

    /// <summary>
    /// 组件
    /// </summary>
    //public GameObject Controller;

    ///// <summary>
    ///// 摄像机参数
    ///// </summary>
    //private float HorizontalSpeed = 150.0f;
    //private float VerticalSpeed = 80.0f;
    //private float CameraDampSpeed = 0.25f;

    ///// <summary>
    ///// 摄像机变量
    ///// </summary>
    //private float VerticalRotation = 0.0f;  //摄像机当前旋转
    //private Vector3 CameraTempPosition;     //摄像机过渡位置

    public Transform target;

    private void Start()
    {
        //若当前平台为PC,请启用以下代码
        //InputManager.Instance.InputDetectionSwitch(true);
        //监听视角移动轴
        //EventManager.Instance.AddEventListener<Vector2>("View Axis Update", View);

        ZEvent.CustomEvent.AddListener<Vector2>(镜头, CamCall);
    }

    void CamCall(CustomEventData<Vector2> eventData)
    {
        targetRotaY = Mathf.Clamp(targetRotaY + -eventData.Data0.y * 0.1f, -30, 30);
        targetRotaX += eventData.Data0.x * 0.1f;
    }

    float targetRotaX;
    float targetRotaY;
    float lerpRotaX;
    float lerpRotaY;

    private void Update()
    {
        if (target == null) return;

        lerpRotaX = Mathf.Lerp(lerpRotaX, targetRotaX, 0.05f);
        lerpRotaY = Mathf.Lerp(lerpRotaY, targetRotaY, 0.05f);

        transform.rotation = Quaternion.Euler(0, lerpRotaX, 0) * Quaternion.Euler(lerpRotaY, 0, 0);

        var offsetPos = transform.rotation * Vector3.back;
        transform.position = target.position + Vector3.up * 2f + offsetPos * 3;
    }
}
