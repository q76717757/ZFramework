using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Rpc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;

namespace ZFramework
{
    public class FouseInputModel : GlobalComponent<FouseInputModel>,IAwake,IDestory ,IUpdate
    {
        UIFoucsEventData eventData = new UIFoucsEventData();
        Dictionary<string, bool> inputState = new Dictionary<string, bool>();
        Dictionary<string, InputAction.CallbackContext> inputMap = new Dictionary<string, InputAction.CallbackContext>();

        //自己配置的输入清单
        private InputActionAsset actionAssets;
        void IAwake.Awake()
        {
            var a = UnityEngine.EventSystems.EventSystem.current.GetComponent<InputSystemUIInputModule>();
            actionAssets = a.actionsAsset;

            ReadOnlyArray<InputActionMap> actionMaps = actionAssets.actionMaps;
            foreach (var actionMap in actionMaps)
            {
                foreach (InputAction inputAction in actionMap)
                {
                    inputAction.performed += OnButtonInput;
                }
            }
        }

        void IDestory.OnDestory()
        {
            ReadOnlyArray<InputActionMap> actionMaps = actionAssets.actionMaps;
            foreach (var actionMap in actionMaps)
            {
                foreach (InputAction inputAction in actionMap)
                {
                    inputAction.performed -= OnButtonInput;
                }
            }
        }

        private void OnButtonInput(InputAction.CallbackContext obj)
        {
            if (!inputMap.ContainsKey(obj.action.name))
            {
                inputMap.Add(obj.action.name, obj);
                inputState.Add(obj.action.name, false);
            }
        }
        private IUIInput uiFocusInterface;

        internal IUIInput UIFocusInterface => uiFocusInterface;

        private UIWindowBase focusUI;
        public UIWindowBase FocusUI => focusUI;

        internal void SetFocus(UIWindowBase uiInput)
        {
            focusUI = uiInput;
            if (focusUI != null)
            {
                uiFocusInterface = (IUIInput)focusUI;
            }
            else
            {
                uiFocusInterface = null;
            }
        }

        void IUpdate.Update()
        {
            if (focusUI == null) return;

            if (inputMap.Count > 0)
            {
                foreach (var key in inputMap)
                {
                    if (key.Value.phase == InputActionPhase.Performed)
                    {
                        if (inputState[key.Key] == false)
                        {
                            inputState[key.Key] = true;
                            eventData.keyType = UIFoucsEventData.KeyType.down;
                            eventData.callbackContext = key.Value;
                            if (uiFocusInterface == null)
                                return;
                            try
                            {
                                uiFocusInterface.OnInput(eventData);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                          
                        }
                        eventData.keyType = UIFoucsEventData.KeyType.hold;
                        eventData.callbackContext = key.Value;
                        if (uiFocusInterface == null)
                            return;
                        try
                        {
                            uiFocusInterface.OnInput(eventData);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }

                    if (key.Value.phase == InputActionPhase.Waiting)
                    {
                        if (inputState[key.Key])
                        {
                            inputState[key.Key] = false;
                            eventData.keyType = UIFoucsEventData.KeyType.up;
                            eventData.callbackContext = key.Value;
                            if (uiFocusInterface == null)
                                return;
                            try
                            {
                                uiFocusInterface.OnInput(eventData);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    }
                }
            }
        }
    }
}
