using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Configuration;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class VFSBrowser : EditorWindow
    {
        public static void OpenWindow()
        {
            GetWindow<VFSBrowser>("VFS虚拟文件系统",true);
        }

        [SerializeField] TreeViewState state;//在窗口布局文件中序列化，可以在Unity重载脚本的时候 还保持树结构的状态不变(折叠或展开)
        VFSTreeView view;
        SearchField searchField;
        IFileServer fileServer;

        private void OnEnable()
        {
            if (!VFSProfile.IsExists)
            {
                new VFSProfile().Save();
                AssetDatabase.Refresh();
            }
        }

        private void OnGUI()
        {
            CreateTreeView();

            DrawTreeView();

            BundleView();

            ToolButtons();

            VFSProfile.GetInstance().SaveIfDirty();
        }


        void CreateTreeView()
        {
            if (state == null)
            {
                state = new TreeViewState();//将这个对象序列化 可以在Unity重载脚本的时候 还保持树结构的状态不变(折叠或展开)
            }
            if (fileServer == null)
            {
                fileServer = new CosFileServer();
            }
            if (view == null)
            {
                var profile = VFSProfile.GetInstance();
                view = new VFSTreeView(state, profile.elements);
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += view.SetFocusAndEnsureSelectedItem;
            }
        }

        //层级树
        void DrawTreeView() //TODO  层级树的内容唯一 不给重复添加  ,  bundle分包还没实现  , 资源自定义命名取消,跨平台重写将多个平台的配置写在一个文件上
        {
            //搜索框
            Rect searchRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight + 3);
            view.searchString = searchField.OnGUI(searchRect, view.searchString);
            //层级树
            Rect viewRect = EditorGUILayout.GetControlRect(false, GUILayout.ExpandHeight(true));
            view.OnGUI(viewRect);
        }

        //ab包的分析结果
        void BundleView()
        {

        }

        //底部按钮
        void ToolButtons()
        {
            if (GUILayout.Button("刷新"))
            {
                view.RefreshItemsNameAndPath();
            }
            //if (GUILayout.Button("自动分包"))
            //{

            //}
            //if (GUILayout.Button("清空缓存"))
            //{

            //}
            if (GUILayout.Button("一键打包"))
            {
                BuildAssetBundle();
            }

            if (GUILayout.Button("上传VFS+Bundles(暂时是全量上传)"))
            {
                UploadAssetBundles().Invoke();
            }
        }

        //打包AB包
        void BuildAssetBundle()
        {
            //刷新
            view.RefreshItemsNameAndPath();

            //收集资源
            AssetCollection collection = new AssetCollection(view.GetTreeModelItems());

            //建立资源依赖关系
            AssetDependencies dependencies = new AssetDependencies(collection);

            //分包策略
            //TODO 场景可以和场景合包 但场景不能和其他VFS合包 (元数据需要增加Scene类型 特殊处理)
            GroupingRule rule = new GroupingRule(dependencies.GetAllAssets()); 

            //分包结果清单
            BundleManifest manifest = rule.GetManifest();

            //打包管线       **传入清单 打包完成后 清单的hash会被赋值   //TODO 这个hash赋值考虑换个方式
            VFSBuildPipeline.BuildAssetBundles(manifest);

            //将信息保存到VFS配置文件
            VFSProfile.GetInstance().manifest = manifest;
            VFSProfile.GetInstance().SetDirty();

            AssetDatabase.Refresh();
            Log.Info("打包完成");
        }

        async ATask UploadAssetBundles()
        {
            //上传AB包
            var manifest = VFSProfile.GetInstance().manifest;
            string outputPath = $"Assets/07.Bundles/{Defines.TargetRuntimePlatform}/AssetsPackingCache";
            foreach (var item in manifest.GetBundles())
            {
                string filePath = $"{outputPath}/{item.bundleName}";
                using (FileStream fs = new FileStream(filePath,FileMode.Open,FileAccess.Read))
                {
                    var savePath = $"{Defines.PROJECT_CODE}/{Defines.TargetRuntimePlatform}/VFS/{item.FileName}";
                    await fileServer.UploadFile(savePath, fs);
                }
            }

            //上传VFS配置
            var profileSavePath = $"{Defines.PROJECT_CODE}/{Defines.TargetRuntimePlatform}/VFS/VFSProfile.xml";
            var bytes = File.ReadAllBytes(VFSProfile.BuildInPath);
            await fileServer.UploadFile(profileSavePath, bytes);

            Log.Info("全部上传完成");
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
