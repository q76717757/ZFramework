using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZFramework
{
    public class TempXRActive : MonoBehaviour
    {
        void Update()
        {
            if (Keyboard.current[Key.Z].isPressed)
            {
                XRActiveController.SetActive(true);
            }
            if (Keyboard.current[Key.X].isPressed)
            {
                XRActiveController.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            XRActiveController.SetActive(false);
        }
    }
}
