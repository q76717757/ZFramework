using System.Collections;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    private bool isDrag = false;
    private Vector2 Axis = Vector2.zero;

    private void Update()
    {
        if (isDrag)
            Axis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        EventManager.Instance.EventTrigger("View Axis Update", Axis);
    }

    public void Begin()
    {
        isDrag = true;
    }

    public void End()
    {
        isDrag = false;
    }
}
