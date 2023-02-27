using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : ScrollRect
{
    protected float Radius = 0f;

    private bool isMove = false;
    public JoyStickSettings settings;

    protected override void Start()
    {
        base.Start();
        settings = this.GetComponent<JoyStickSettings>();
        //根据大小进行缩放
        Radius = (transform as RectTransform).sizeDelta.x * 0.4f;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isMove = true;
        settings.Stick.GetComponent<Image>().sprite = settings.StickOn;
        settings.Range.SetActive(true);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        var stickPosition = this.content.anchoredPosition;
        if (stickPosition.magnitude > Radius)
        {
            stickPosition = stickPosition.normalized * Radius;
            SetContentAnchoredPosition(stickPosition);
        }
        (settings.Range.transform as RectTransform).localScale = new Vector3(stickPosition.magnitude / Radius, stickPosition.magnitude / Radius, 1.0f);
        var Angle = Vector2.Angle(stickPosition, new Vector2(0, 1));
        (settings.Range.transform as RectTransform).localEulerAngles = new Vector3(0, 0, (stickPosition.x >= 0 ? -Angle : Angle));
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        this.content.anchoredPosition = Vector2.zero;
        isMove = false;
        settings.Stick.GetComponent<Image>().sprite = settings.StickOff;
        settings.Range.SetActive(false);
    }

    public void Update()
    {
        AxisUpdate();
    }

    private void AxisUpdate()
    {
        var AxisX = settings.Stick.GetComponent<RectTransform>().anchoredPosition.x / settings.Stick.GetComponent<RectTransform>().sizeDelta.x;
        var AxisY = settings.Stick.GetComponent<RectTransform>().anchoredPosition.y / settings.Stick.GetComponent<RectTransform>().sizeDelta.y;
        //合成移动坐标轴
        var AxisMovement = new Vector2(AxisX, AxisY).normalized;
        //若当前未输入移动信号，则置零
        if (!isMove)
        {
            AxisX = AxisY = 0;
            AxisMovement = Vector2.zero;
        }
        bool isRun = false;
        var joystickrange = new Vector2(AxisX, AxisY);
        if (joystickrange.magnitude > 0.95f) 
        {
            isRun = true;
        }
        else isRun = false;
        //触发移动轴更新事件
        EventManager.Instance.EventTrigger("X Axis Update", AxisX);
        EventManager.Instance.EventTrigger("Y Axis Update", AxisY);
        EventManager.Instance.EventTrigger("Movement Axis Update", AxisMovement);
        EventManager.Instance.EventTrigger("RunState", isRun);
    }
}
