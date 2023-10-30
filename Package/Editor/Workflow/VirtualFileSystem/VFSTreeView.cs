using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public VFSTreeView(TreeViewState state, IList<VFSTreeElement> datas) : base(state, datas)
        {
            showBorder = true;
            extraSpaceBeforeIconAndLabel = 20;//挤出空间 给图标
            //showAlternatingRowBackgrounds = true;//交替背景

            const string openFolderIconName = "d_FolderOpened Icon";
            openedFolderIcon = EditorGUIUtility.TrIconContent(openFolderIconName).image as Texture2D;
            emptyFolderIcon = EditorGUIUtility.IconContent(EditorResources.emptyFolderIconName).image as Texture2D;
            folderIcon = EditorGUIUtility.IconContent(EditorResources.folderIconName).image as Texture2D;

            Reload();
        }

        //重名
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
            var guid = GetTreeElement(id).data.guid;
            if (!string.IsNullOrEmpty(guid))//双击定位
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    EditorGUIUtility.PingObject(AssetImporter.GetAtPath(assetPath).GetInstanceID());
                }
            }
        }

        //拖拽
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
        protected override bool CanStartDragFromInside()
        {
            return true;
        }
        protected override VFSTreeElement CreateTreeElementFromOutside(UnityEngine.Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;//不是资产  (文件夹也是资产)
            }

            Log.Info("name->" + obj.name);

            Log.Info("assetPath->" + assetPath);
            var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
            Log.Info("metaPath->" + metaPath);
            var guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            Log.Info("assetGUID->" + guid);
            var isFolder = AssetDatabase.IsValidFolder(assetPath);
            Log.Info("isFolder->" + isFolder);

            var afi = new VFSMetaData()
            {
                name = obj.name,
                fileName = isFolder ? null : obj.name,
                guid = guid,
                filehash = isFolder ? null : MD5Helper.FileMD5(assetPath),
                metahash = isFolder ? null : MD5Helper.FileMD5(metaPath),//文件夹不需要任何hash 用不上
            };
            return new VFSTreeElement(GenerateUniqueID(), 0, afi);
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

            Texture2D icon;
            var oldColor = GUI.color;
            if (assetFileInfo.IsAsset)//资源显示默认图标
            {
                icon = AssetDatabase.GetCachedIcon(AssetDatabase.GUIDToAssetPath(assetFileInfo.guid)) as Texture2D;
            }
            else //文件夹分两种  引用文件夹和虚拟文件夹
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
            }
            Rect iconRect = new Rect(args.rowRect.position + new Vector2(offset, 0), new Vector2(extraSpaceBeforeIconAndLabel, args.rowRect.height));
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            GUI.color = oldColor;
        }

        //右键菜单
        void ShowGenericMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Folder"), false, CreateFolder, IsItemContext);//set IsItemContext fales比CreateFolder的回调执行更早
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
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Rename"));
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

                RenameOfNewItem = true;
                BeginRename(viewItem);
            }
        }
        VFSTreeElement GenerateFolderElement()
        {
            var afi = new VFSMetaData()//虚拟文件夹
            {
                name = "New Folder",
                fileName = null,
                guid = null,
                filehash = null,
                metahash = null,
            };
            return new VFSTreeElement(GenerateUniqueID(), 0, afi);
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

    }
}
