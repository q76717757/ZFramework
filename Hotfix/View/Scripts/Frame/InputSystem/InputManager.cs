using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    /// <summary>
    /// ������ʹ������
    /// </summary>
    private bool InputDetection = false;

    /// <summary>
    /// �����б�
    /// </summary>
    public KeyCode KeyUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyUp", "W"));
    public KeyCode KeyDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyDown", "S"));
    public KeyCode KeyLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyLeft", "A"));
    public KeyCode KeyRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyRight", "D"));
    public KeyCode KeyRun = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyRun", "LeftShift"));
    public KeyCode KeyJump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyJump", "Space"));
    public KeyCode KeyInteract = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("KeyInteract", "E"));

    /// <summary>
    /// ���캯��
    /// </summary>
    public InputManager()
    {
        //�������֡�����¼�����
        RealTimeManager.Instance.AddUpdateListener(InputUpdate);
        RealTimeManager.Instance.AddUpdateListener(AxisUpdate);
    }

    /// <summary>
    /// �����⿪��
    /// </summary>
    /// <param name="state">����״̬</param>
    public void InputDetectionSwitch(bool state)
    {
        InputDetection = state;
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="key">����</param>
    private void KeyDetection(KeyCode key)
    {
        //�������������¼�
        if (Input.GetKeyDown(key))
            EventManager.Instance.EventTrigger(key + "Down", key);              
        //��������̧���¼�
        if (Input.GetKeyUp(key))
            EventManager.Instance.EventTrigger(key + "Up", key);                         
    }

    /// <summary>
    /// �������¼�
    /// </summary>
    private void InputUpdate()
    {
        if (!InputDetection) 
            return;
        //�����б�
        KeyDetection(KeyUp);
        KeyDetection(KeyDown);
        KeyDetection(KeyLeft);
        KeyDetection(KeyRight);
        KeyDetection(KeyRun);
        KeyDetection(KeyJump);
        KeyDetection(KeyInteract);
    }

    /// <summary>
    /// ������֡�����¼�
    /// </summary>
    private void AxisUpdate()
    {
        if (!InputDetection)
            return;
        //�ݴ���ֵ
        var AxisX = 0f;
        var AxisY = 0f;
        var TempX = 0f;
        var TempY = 0f;
        //����Y��������ʱ��Y�Ỻ�������򻺽�
        if (Input.GetKey(KeyUp) || Input.GetKey(KeyDown))
        {
            var TargetY = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
            AxisY = Mathf.SmoothDamp(AxisY, TargetY, ref TempY, 0.5f);
        }
        else
        {
            AxisY = Mathf.SmoothDamp(AxisY, 0, ref TempY, 0.1f);
        }
        //����X��������ʱ��X�Ỻ�������򻺽�
        if (Input.GetKey(KeyLeft) || Input.GetKey(KeyRight))
        {
            var TargetX = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);
            AxisX = Mathf.SmoothDamp(AxisX, TargetX, ref TempX, 0.5f);
        }
        else
        {
            AxisX = Mathf.SmoothDamp(AxisX, 0, ref TempX, 0.1f);
        }
        //�ϳ��ƶ�������
        var AxisMovement = new Vector2(AxisX, AxisY).normalized;
        //�����ƶ�������¼�
        EventManager.Instance.EventTrigger("X Axis Update", AxisX);
        EventManager.Instance.EventTrigger("Y Axis Update", AxisY);
        EventManager.Instance.EventTrigger("Movement Axis Update", AxisMovement);
        //�ϳ����������
        var AxisView = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //�������������¼�
        EventManager.Instance.EventTrigger("View Axis Update", AxisView);
    }
}
