using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace ZFramework.Editor
{
    /// <summary>
    /// 带过渡动画的折叠菜单
    /// </summary>
    public abstract class FadeFoldout
    {
        static GUIStyle _foldout;
        static GUIStyle Foldout
        {
            get
            {
                if (_foldout == null)
                {
                    _foldout = new GUIStyle(EditorStyles.foldout);
                    _foldout.fontStyle = FontStyle.Bold;
                }
                return _foldout;
            }
        }

        public abstract string Title { get; }
        public UnityEvent OnValueChange 
        {
            get
            {
                return animBool.valueChanged;
            }
        }

        AnimBool animBool;
        List<FadeFoldout> group;

        protected FadeFoldout()
        {
            animBool = new AnimBool();
        }

        public void OnGui()
        {
            if (BeginFadeGroup())
            {
                OnGUI();
            }
            EndFadeGroup();
        }
        public FadeFoldout JoinGroup(List<FadeFoldout> group)
        {
            this.group = group;
            this.group.Add(this);
            return this;
        }
        bool BeginFadeGroup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            bool exp = EditorGUI.Foldout(GUILayoutUtility.GetRect(20, 22), animBool.target, Title, true, Foldout);
            EditorGUI.indentLevel--;
            if (exp != animBool.target)
            {
                animBool.target = exp;

                if (exp && group != null)
                {
                    for (int i = 0; i < group.Count; i++)
                    {
                        if (group[i] != this)
                        {
                            group[i].animBool.target = false;
                        }
                    }
                }
            }
            return EditorGUILayout.BeginFadeGroup(animBool.faded);
        }
        void EndFadeGroup()
        {
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 在这里实现这些菜单的内容
        /// </summary>
        protected abstract void OnGUI();

    }
}
