using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZFramework.Editor
{
    public class AssemblyFadeGroup : FadeFoldout
    {
        public override string Title =>"程序集设置";

        ReorderableList list;
        protected override void OnGUI()
        {
            BootProfile profile = BootProfile.GetInstance();

            if (list == null)
            {
                list = new ReorderableList(profile.AotMetaAssemblyNames, null, true, false, true, true);
                list.drawElementCallback += A;
            }
            list.DoLayoutList();

            profile.SaveIfDirty();
        }

        void A(Rect rect, int index, bool isActive, bool isFocused)
        {
            //Log.Info(index + ":" + isActive + ":" + isFocused);
            EditorGUI.TextField(rect, "123");
        }
    }
}
