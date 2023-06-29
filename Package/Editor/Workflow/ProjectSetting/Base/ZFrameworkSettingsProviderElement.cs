using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZFramework.Editor
{
    public abstract class ZFrameworkSettingsProviderElement<T> : SettingsProvider where T : ZFrameworkSettingsProviderElement<T>,new()
    {
        public ZFrameworkSettingsProviderElement() : base($"{ZFrameworkSettingProvider.Path}/{typeof(T).Name.Replace("Settings", "")}", SettingsScope.Project) { }
        protected static T GetInstance() => new T();

        private bool enableIsCalled;
        private SerializedObject _runtimeSettings;
        private SerializedObject _editorSettings;
        protected SerializedObject RuntimeSettings
        {
            get
            {
                if (_runtimeSettings == null && ZFrameworkRuntimeSettings.Get() != null)
                {
                    _runtimeSettings = new SerializedObject(ZFrameworkRuntimeSettings.Get());
                }
                return _runtimeSettings;
            }
        }
        protected SerializedObject EdtiorSettings
        {
            get
            {
                if (_editorSettings == null || _editorSettings.targetObject == null)//在打包的时候targetObject报空引用
                {
                    _editorSettings?.Dispose();
                    _editorSettings = new SerializedObject(ZFrameworkEditorSettings.instance);
                }
                return _editorSettings;
            }
        }

        public sealed override void OnGUI(string searchContext)
        {
            if (ZFrameworkRuntimeSettingsEditor.CheckExistsAndDrawCreateBtnIfNull())
            {
                if (!enableIsCalled)
                {
                    enableIsCalled = true;
                    OnEnable();
                }
                OnGUI();
            }
        }
        public sealed override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            enableIsCalled = false;
        }
        public sealed override void OnDeactivate()
        {
            base.OnDeactivate();
            if (enableIsCalled)
            {
                OnDisable();
            }
            _runtimeSettings?.Dispose();
            _runtimeSettings = null;
            _editorSettings?.Dispose();
            _editorSettings = null;
        }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnGUI() { }
    }

}
