using UnityEngine;
using ZFramework;

public class ModelDisplay : MonoBehaviour
{
    public Transform _transform;
    private float rotateScale = 5f;

    float lastX = 0;
    Quaternion qua;

    public void Init(Transform targetModel)
    {
        _transform = targetModel;
        ZEvent.UIEvent.AddListener(gameObject, Drag);
    }

   
    void Drag(UIEventData eventData)
    {
        switch (eventData.EventType)
        {
            case UIEventType.Enter:
                break;
            case UIEventType.Exit:
                break;
            case UIEventType.Down:
                lastX = eventData.Position.x;
                qua = _transform.rotation;
                break;
            case UIEventType.Up:
                break;
            case UIEventType.Click:
                break;
            case UIEventType.Drag:
                var offset = eventData.Position.x - lastX;
                _transform.rotation = qua * Quaternion.Euler(0, -offset, 0);
                break;
            case UIEventType.Scroll:
                break;
            default:
                break;
        }
    }
}
