using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Configuration;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class VirtualFileSystemBrowser : EditorWindow
    {
        public static void OpenWindow()
        {
            var window = GetWindow<VirtualFileSystemBrowser>();
            window.minSize = Vector2.one * 200;
            window.Show();
        }

        [SerializeField] TreeViewState state;//在窗口布局文件中序列化，可以在Unity重载脚本的时候 还保持树结构的状态不变(折叠或展开)
        VFSTreeView view;
        SearchField searchField;

        private void OnGUI()
        {
            if (!VFSProfile.IsExists)
            {
                CreaetVFSConfig();
            }
            else
            {
                DrawElements();
                VFSProfile.GetInstance().SaveIfDirty();
            }
        }
        void CreaetVFSConfig()
        {
            if (GUILayout.Button("创建VFS配置文件", GUILayout.Height(50)))
            {
                var profile = new VFSProfile();
                profile.elements = new List<VFSTreeElement>
                {
                    new VFSTreeElement(0, -1, new VFSMetaData(){ name = "Root"}),
                    new VFSTreeElement(1, 0, new VFSMetaData() { name = "Prefabs" }),
                    new VFSTreeElement(2, 0, new VFSMetaData() { name = "Models" }),
                    new VFSTreeElement(3, 0, new VFSMetaData() { name = "Medias" }),
                    new VFSTreeElement(4, 0, new VFSMetaData() { name = "Others" }),
                };
                profile.Save();
                AssetDatabase.Refresh();

                if (view != null)
                {
                    view.SetData(profile.elements);
                }
            }
        }

        void DrawElements()
        {
            Init();

            SearchBar();

            TreeView();

            ToolButtons();
            //DrawHorizontalSplitter(new Rect(rect.width / 2, 21, 1, rect.height));
        }
        void Init()
        {
            if (state == null)
            {
                state = new TreeViewState();//将这个对象序列化 可以在Unity重载脚本的时候 还保持树结构的状态不变(折叠或展开)
            }
            if (view == null)
            {
                var profile = VFSProfile.GetInstance();
                view = new VFSTreeView(state, profile.elements);
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += view.SetFocusAndEnsureSelectedItem;
            }
        }

        void SearchBar()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 3);
            view.searchString = searchField.OnGUI(rect, view.searchString);
        }

        void TreeView()
        {
            Rect content = this.rootVisualElement.contentRect;
            Rect rect = new Rect(10, 25, content.width - 20, content.height - 55);
            view.OnGUI(rect);
        }

        void ToolButtons()
        {
            Rect content = this.rootVisualElement.contentRect;

            if (GUI.Button(GetBtnRect(0), "打AB包"))
            {
                BuildAssetBundle();
            }
            if (GUI.Button(GetBtnRect(1), "刷新*.vfs"))
            {
                //目前没有检测文件变化
                //当从外部修改文件内容时(例如替换同名PNG文件),filehash会变化
                //在unity中修改外部资产的导入设置 (例如修改png的采样模式)  metahash也会变化
                //或者文件名发生改变之后,重新打包后ab包的加载会失败
                //(后续补充版本控制,内容不迭代的(hash不变),不打补丁包,则还是以旧名字load)
                //有上述情况的时候 需要手动刷新,重建整个vfs清单.
            }

            //-------------------
            Rect GetBtnRect(int index)
            {
                float btnOffsetY = content.height - 35;
                float btnWidth = 100;
                float btnHeight = 30;
                return new Rect(10 + 100 * index, btnOffsetY, btnWidth, btnHeight);
            }
        }

        void BuildAssetBundle()
        {
            string outputPath = $"Assets/07.Bundles/{Defines.TargetRuntimePlatform}/AssetsPackingCache";
            Directory.CreateDirectory(outputPath);

            var profile = VFSProfile.GetInstance();
            var datas = profile.elements;

            List<string> assetNames = new List<string>();
            foreach (var item in datas)
            {
                var data = item.data;
                if (data.IsAsset)
                {
                    Log.Info(data.name + ":" + data.guid);
                    var assetPath =  AssetDatabase.GUIDToAssetPath(data.guid);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        assetNames.Add(assetPath);
                        
                    }
                }
            }

            if (assetNames.Count == 0)
            {
                return;
            }
            AssetBundleBuild abb = new AssetBundleBuild()
            {
                assetBundleName = "vfs.ab",
                assetNames = assetNames.ToArray(),
            };
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, new AssetBundleBuild[] { abb }, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);


            AssetDatabase.Refresh();
        }




        //分页
        float x = 0;
        void DrawHorizontalSplitter(Rect dragRect)
        {
            var a = new Rect(dragRect.x - 1, dragRect.y, 10, dragRect.height);
            EditorGUIUtility.AddCursorRect(a, MouseCursor.ResizeHorizontal);

            //x = a.x;
            if (Event.current.type == UnityEngine.EventType.MouseDrag)
            {
                x = Event.current.mousePosition.x;
                this.Repaint();
            }

            if (Event.current.type != UnityEngine.EventType.Repaint)
                return;

            Color orgColor = GUI.color;
            GUI.color = GUI.color * new Color(0.12f, 0.12f, 0.12f, 1.333f);//颜色加深
            Rect splitterRect = new Rect(x/*dragRect.x - 1*/, dragRect.y, 1, dragRect.height);

            GUI.DrawTexture(splitterRect, EditorGUIUtility.whiteTexture);
            GUI.color = orgColor;
        }
    }
}
