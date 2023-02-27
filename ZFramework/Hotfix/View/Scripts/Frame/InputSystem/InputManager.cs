using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    /// <summary>
    /// 输入检测使能旗帜
    /// </summary>
    private bool InputDetection = false;

    /// <summary>
    /// 按键列表
    /// </summary>
    public KeyCode KeyUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyUp", "W"));
    public KeyCode KeyDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyDown", "S"));
    public KeyCode KeyLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyLeft", "A"));
    public KeyCode KeyRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyRight", "D"));
    public KeyCode KeyRun = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyRun", "LeftShift"));
    public KeyCode KeyJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyJump", "Space"));
    public KeyCode KeyInteract = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyInteract", "E"));

    /// <summary>
    /// 构造函数
    /// </summary>
    public InputManager()
    {
        //添加输入帧更新事件监听
        RealTimeManager.Instance.AddUpdateListener(InputUpdate);
        RealTimeManager.Instance.AddUpdateListener(AxisUpdate);
    }

    /// <summary>
    /// 输入检测开关
    /// </summary>
    /// <param name="state">开关状态</param>
    public void InputDetectionSwitch(bool state)
    {
        InputDetection = state;
    }

    /// <summary>
    /// 按键检测
    /// </summary>
    /// <param name="key">按键</param>
    private void KeyDetection(KeyCode key)
    {
        //触发按键按下事件
        if (Input.GetKeyDown(key))
            EventManager.Instance.EventTrigger(key + "Down", key);              
        //触发按键抬起事件
        if (Input.GetKeyUp(key))
            EventManager.Instance.EventTrigger(key + "Up", key);                         
    }

    /// <summary>
    /// 输入检测事件
    /// </summary>
    private void InputUpdate()
    {
        if (!InputDetection) 
            return;
        //按键列表
        KeyDetection(KeyUp);
        KeyDetection(KeyDown);
        KeyDetection(KeyLeft);
        KeyDetection(KeyRight);
        KeyDetection(KeyRun);
        KeyDetection(KeyJump);
        KeyDetection(KeyInteract);
    }

    /// <summary>
    /// 输入轴帧更新事件
    /// </summary>
    private void AxisUpdate()
    {
        if (!InputDetection)
            return;
        //暂存轴值
        var AxisX = 0f;
        var AxisY = 0f;
        var TempX = 0f;
        var TempY = 0f;
        //当有Y轴有输入时，Y轴缓增，否则缓降
        if (Input.GetKey(KeyUp) || Input.GetKey(KeyDown))
        {
            var TargetY = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
            AxisY = Mathf.SmoothDamp(AxisY, TargetY, ref TempY, 0.5f);
        }
        else
        {
            AxisY = Mathf.SmoothDamp(AxisY, 0, ref TempY, 0.1f);
        }
        //当有X轴有输入时，X轴缓增，否则缓降
        if (Input.GetKey(KeyLeft) || Input.GetKey(KeyRight))
        {
            var TargetX = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);
            AxisX = Mathf.SmoothDamp(AxisX, TargetX, ref TempX, 0.5f);
        }
        else
        {
            AxisX = Mathf.SmoothDamp(AxisX, 0, ref TempX, 0.1f);
        }
        //合成移动坐标轴
        var AxisMovement = new Vector2(AxisX, AxisY).normalized;
        //触发移动轴更新事件
        EventManager.Instance.EventTrigger("X Axis Update", AxisX);
        EventManager.Instance.EventTrigger("Y Axis Update", AxisY);
        EventManager.Instance.EventTrigger("Movement Axis Update", AxisMovement);
        //合成鼠标坐标轴
        var AxisView = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //触发鼠标轴更新事件
        EventManager.Instance.EventTrigger("View Axis Update", AxisView);
    }
}
