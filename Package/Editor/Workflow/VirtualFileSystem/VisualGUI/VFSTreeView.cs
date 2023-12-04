using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class VFSTreeView : TreeView<VFSTreeElement>
    {
        Texture2D emptyFolderIcon;
        Texture2D folderIcon;
        Texture2D openedFolderIcon;
        Texture2D missingIcon;
        Texture2D newIcon;
        Texture2D bundleIcon;

        public VFSTreeView(TreeViewState state, IList<VFSTreeElement> datas) : base(state, datas)
        {
            showBorder = true;
            extraSpaceBeforeIconAndLabel = 20;//挤出空间 给图标
            //showAlternatingRowBackgrounds = true;//交替背景

            const string openFolderIconName = "d_FolderOpened Icon";
            const string missingIconName = "d__Help@2x";
            const string newIconName = "PackageBadgeNew";
            const string bundleIconName = "package_installed";
            openedFolderIcon = EditorGUIUtility.FindTexture(openFolderIconName);
            emptyFolderIcon = EditorGUIUtility.FindTexture(EditorResources.emptyFolderIconName);
            folderIcon = EditorGUIUtility.FindTexture(EditorResources.folderIconName);
            missingIcon = EditorGUIUtility.FindTexture(missingIconName);
            newIcon = EditorGUIUtility.FindTexture(newIconName);
            bundleIcon = EditorGUIUtility.IconContent(bundleIconName).image as Texture2D;

            Reload();
        }


        //重名  //TODO 重名好像没有必要 因为VFS和object对不上 runtime可能出现误解  只给文件夹提供重名
        protected override bool CanRename(TreeViewItem item)
        {
            return true;
        }

        //点击
        bool IsItemContext;
        protected override void ContextClickedItem(int id)//右键上下文点击 点中元素这个会先执行
        {
            IsItemContext = true;
        }
        protected override void ContextClicked()//不管点不点中元素 只要在区域内就会执行
        {
            ShowGenericMenu();
            IsItemContext = false;
        }
        protected override void DoubleClickedItem(int id)
        {
            var data = GetTreeElement(id).data;
            var guid = data.guid;
            if (!string.IsNullOrEmpty(guid))//双击定位
            {
                var currentPath = AssetDatabase.GUIDToAssetPath(guid);
                AssetImporter ai = AssetImporter.GetAtPath(currentPath);
                if (ai != null)
                {
                    EditorGUIUtility.PingObject(ai);
                }
                else
                {
                    var path = data.path;
                    Log.Info($"Asset is Missing: [GUID:{guid}--PATH:{path}]");
                }
            }
        }

        //拖拽  //TODO AB也给拖拽排序 但是不给嵌套
        protected override bool CanStartDragFromInside(IList<int> itemIDs)
        {
            foreach (var item in itemIDs)
            {
                var type = GetTreeElement(item).data.type;
                if (type == VFSMetaData.MetaType.Bundle || 
                    type == VFSMetaData.MetaType.ReferenceFolderSubAsset ||
                    type == VFSMetaData.MetaType.ReferenceFolderSunbFolder)
                {
                    return false;//ab包固定第一层  不给拖动
                }
            }
            return true;
        }
        protected override bool CanStartDragFromOutside(UnityEngine.Object[] objs)
        {
            foreach (var item in objs)
            {
                if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(item)))//只有assets的资产或者文件夹才给拖进来
                {
                    return false;
                }
            }
            return true;
        }
        //TODO 排除掉一些runtime不支持的类型 如SceneAsest DefaultAsset LightingDataAsset等  VFS只能添加runtime资源
        protected override VFSTreeElement CreateTreeElementFromOutside(UnityEngine.Object obj)
        {
            
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;//不是资产  (文件夹也是资产)
            }
            var guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            var data = new VFSMetaData(VFSMetaData.MetaType.None, guid).Refresh();
            return new VFSTreeElement(GenerateUniqueID(), 0, data);
        }
        public override void OnAddTreeElementsFromOutside(IList<VFSTreeElement> addElements)
        {
            foreach (VFSTreeElement item in addElements)
            {
                if (item.data.IsReferenceFolder)//添加的是引用文件夹  递归把里面的东西都添加进去
                {
                    OnAddReferenceFolder(item);
                }
            }
        }
        void OnAddReferenceFolder(VFSTreeElement folder)
        {
            AddReferenceSubFolder(folder);
        }
        void AddReferenceSubFolder(VFSTreeElement folder)
        {
            var path = AssetDatabase.GUIDToAssetPath(folder.data.guid);
            var files = Directory.GetFiles(path); //当前文件夹中的文件
            foreach (var item in files)
            {
                if (item.StartsWith(".") || !item.EndsWith(".meta"))//忽略隐藏文件和meta文件
                {
                    var guid = AssetDatabase.AssetPathToGUID(item);
                    var add = new VFSTreeElement(GenerateUniqueID(), 0, new VFSMetaData(VFSMetaData.MetaType.ReferenceFolderSubAsset, guid).Refresh());
                    AddTreeViewItemToTail(add, folder, false);
                }
            }
            var dirs = Directory.GetDirectories(path);//子文件夹 不递归
            foreach (var item in dirs)
            {
                if (AssetDatabase.IsValidFolder(item))//忽略隐藏文件夹
                {
                    var guid = AssetDatabase.AssetPathToGUID(item);
                    var add = new VFSTreeElement(GenerateUniqueID(), 0, new VFSMetaData(VFSMetaData.MetaType.ReferenceFolderSunbFolder, guid).Refresh());
                    AddTreeViewItemToTail(add, folder, false);

                    AddReferenceSubFolder(add);//递归添加子目录
                }
            }
        }

        //序列化
        protected override void OnTreeElementChange()
        {
            var datas = GetAllTreeElements().ToList();
            VFSProfile.GetInstance().elements = datas;
            VFSProfile.GetInstance().SetDirty();
        }

        //GUI
        protected override void RowGUI(RowGUIArgs args)
        {
            IconGUI(in args);
            base.RowGUI(args);
        }
        void IconGUI(in RowGUIArgs args)
        {
            var offset = GetContentIndent(args.item);
            VFSMetaData assetFileInfo = (args.item as TreeViewItem<VFSTreeElement>).Element.data;
            Texture2D icon = null;
            var oldColor = GUI.color;
            if (assetFileInfo.IsAsset || assetFileInfo.type == VFSMetaData.MetaType.ReferenceFolderSubAsset)//资源显示默认图标
            {
                if (assetFileInfo.type == VFSMetaData.MetaType.ReferenceFolderSubAsset)
                {
                    GUI.color = Color.gray;
                }

                if (!string.IsNullOrEmpty(assetFileInfo.path))
                {
                    icon = AssetDatabase.GetCachedIcon(assetFileInfo.path) as Texture2D;
                }
                if (icon == null)
                {
                    icon = missingIcon;
                }
            }
            else if(assetFileInfo.IsBundle)
            {
                GUI.color = Color.yellow;
                icon = bundleIcon;
            }
            else //文件夹分两种  引用文件夹和虚拟文件夹      TODO 还有一个情况目前没有处理  就是引用文件夹missing的情况
            {
                if (args.item.hasChildren)
                {
                    if (IsExpanded(args.item.id))
                    {
                        icon = openedFolderIcon;
                    }
                    else
                    {
                        icon = folderIcon;
                    }
                }
                else
                {
                    icon = emptyFolderIcon;
                }
                if (assetFileInfo.IsVirtualFolder)//虚拟文件夹是绿色的
                {
                    GUI.color = Color.green;
                }
                if (assetFileInfo.type == VFSMetaData.MetaType.ReferenceFolderSunbFolder)
                {
                    GUI.color = Color.gray;
                }
            }
            Rect iconRect = new Rect(args.rowRect.position + new Vector2(offset, 0), new Vector2(extraSpaceBeforeIconAndLabel, args.rowRect.height));
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            GUI.color = oldColor;

            //if (assetFileInfo.IsAsset)
            //{
            //    Rect iconRect2 = new Rect(args.rowRect.position + new Vector2(args.rowRect.width-35, 0), new Vector2(extraSpaceBeforeIconAndLabel, args.rowRect.height));
            //    GUI.DrawTexture(iconRect2, newIcon, ScaleMode.ScaleToFit);
            //}
        }

        //右键菜单
        void ShowGenericMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Folder"), false, CreateFolder, IsItemContext);//set IsItemContext fales比CreateFolder的回调执行更早
            menu.AddItem(new GUIContent("Create Bundle"), false, CreateBundle);
            menu.AddSeparator(""); //分隔符
            var selects = GetSelection();
            if (IsItemContext)
            {
                menu.AddItem(new GUIContent("Delect"), false, DeleteElement);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Delect"));
            }
            if (IsItemContext && selects.Count == 1)
            {
                menu.AddItem(new GUIContent("Rename"), false, Rename);
                menu.AddItem(new GUIContent("Save Path"), false, SaveElementFullPath);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Rename"));
                menu.AddDisabledItem(new GUIContent("Save Path"));
            }
            //menu.AddItem(new GUIContent("11/22"), false, A, "123");

            menu.ShowAsContext(); //显示菜单
        }
        void CreateFolder(object IsItemContext)
        {
            if ((bool)IsItemContext)//添加子文件夹
            {
                IList<int> selectionIDs = GetSelection().ToArray();
                foreach (var item in selectionIDs)
                {
                    var parentElement = GetTreeElement(item);
                    AddTreeViewItemToTail(GenerateFolderElement(), parentElement);
                }
            }
            else//添加一级文件夹
            {
                var viewItem = AddTreeViewItemToTail(GenerateFolderElement(), null);//当element为空 则添加的是根文件夹
                BeginRename(viewItem);
            }
        }
        void CreateBundle()
        {
            var viewItem = AddTreeViewItemToTail(GenerateBundleElement(), null);//当element为空 则添加的是根文件夹
            BeginRename(viewItem);
        }
        VFSTreeElement GenerateFolderElement()
        {
            return new VFSTreeElement(GenerateUniqueID(), 0, new VFSMetaData(VFSMetaData.MetaType.VirtualFolder) { name = "New Folder"});//虚拟文件夹
        }
        VFSTreeElement GenerateBundleElement()
        {
            return new VFSTreeElement(GenerateUniqueID(), 0, new VFSMetaData(VFSMetaData.MetaType.Bundle) { name = "New Bundle"});//虚拟文件夹
        }
        void SaveElementFullPath()
        {
            TreeViewItem element = GetTreeViewItem(GetSelection().First());

            Stack<string> pathStack = new Stack<string>();

            while (element != null)
            {
                pathStack.Push(element.displayName);
                element = element.parent;
            }

            StringBuilder sb = new StringBuilder();
            pathStack.Pop();//排除root
            while (pathStack.Count > 1)
            {
                var name = pathStack.Pop();
                sb.Append(name);
                sb.Append('/');
            }
            sb.Append(pathStack.Pop());
            GUIUtility.systemCopyBuffer = sb.ToString();
        }

        void DeleteElement()
        {
            IList<int> selectionIDs = GetSelection();
            if (selectionIDs.Count > 0)
            {
                if (EditorUtility.DisplayDialog("确认删除?", "删除" + selectionIDs.Count + "项?\r\n没有做撤销,此操作不能撤销", "YES", "NO"))
                {
                    RemoveTreeViewItems(selectionIDs);
                }
            }
        }
        KeyCode lastKey;
        protected override void KeyEvent()
        {
            if (Event.current.keyCode == KeyCode.Delete && lastKey != KeyCode.Delete)
            {
                DeleteElement();
            }
            lastKey = Event.current.keyCode;
        }
        void Rename()
        {
            BeginRename(GetTreeViewItem(GetSelection().First()));
        }

        //刷新所有VFSTreeElement   
        public void Refresh()
        {
            var elements = GetAllTreeElements();
            foreach (var item in elements)
            {
                item.data.Refresh();
            }
            OnTreeElementChange();
        }
    }
}
