using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace ZFramework
{
    public interface IUIInput
    {
        void OnInput(UIFoucsEventData data);
    }

    public class UIFoucsEventData
    {
        public enum KeyType
        {
            hold,
            up,
            down
        }
        public KeyType keyType;
        public InputAction.CallbackContext callbackContext;
    }
}
