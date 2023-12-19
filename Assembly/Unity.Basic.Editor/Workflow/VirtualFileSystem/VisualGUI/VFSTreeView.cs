using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZFramework.Editor
{
    internal class VFSTreeView : TreeView<VFSTreeModelItem>
    {
        Texture2D emptyFolderIcon;
        Texture2D folderIcon;
        Texture2D openedFolderIcon;
        Texture2D missingIcon;
        Texture2D bundleIcon;

        public VFSTreeView(TreeViewState state, IList<VFSTreeModelItem> datas) : base(state, datas)
        {
            showBorder = true;//显示边框
            extraSpaceBeforeIconAndLabel = 34;//挤出空间 给图标
            //showAlternatingRowBackgrounds = true;//交替背景

            const string openFolderIconName = "d_FolderOpened Icon";
            const string missingIconName = "d__Help@2x";
            const string bundleIconName = "package_installed";
            openedFolderIcon = EditorGUIUtility.FindTexture(openFolderIconName);
            emptyFolderIcon = EditorGUIUtility.FindTexture(EditorResources.emptyFolderIconName);
            folderIcon = EditorGUIUtility.FindTexture(EditorResources.folderIconName);
            missingIcon = EditorGUIUtility.FindTexture(missingIconName);
            bundleIcon = EditorGUIUtility.IconContent(bundleIconName).image as Texture2D;

            Reload();
        }


        //重名
        protected override bool CanRename(TreeViewItem item)
        {
            return CanRename2((item as TreeViewItem<VFSTreeModelItem>).ModelItem);
        }
        bool CanRename2(VFSTreeModelItem modelItem)
        {
            //不能和上面CanRename同名重载,Unity会反射失败
            switch (modelItem.Data.type)
            {
                case VFSMetaData.MetaType.Asset:
                case VFSMetaData.MetaType.Scene:
                case VFSMetaData.MetaType.ReferenceFolder:
                case VFSMetaData.MetaType.ReferenceAsset:
                case VFSMetaData.MetaType.ReferenceScene:
                case VFSMetaData.MetaType.ReferenceSubFolder:
                    return false;
                case VFSMetaData.MetaType.Bundle:
                case VFSMetaData.MetaType.VirtualFolder:
                    return true;
                default:
                    return false;
            }
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
            var data = GetTreeModelItem(id).Data;
            var guid = data.Guid;
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

        //拖拽 
        protected override bool CanStartDragFromOutside(Object[] objs)
        {
            foreach (var item in objs)
            {
                var metaType = GetAssetMetaType(item);
                switch (metaType)
                {
                    case VFSMetaData.MetaType.Asset:
                    case VFSMetaData.MetaType.Scene:
                    case VFSMetaData.MetaType.ReferenceFolder:
                        continue;
                    default:
                        return false;
                }
            }
            return true;
        }
        VFSMetaData.MetaType GetAssetMetaType(Object assetObject)
        {
            if (assetObject == null)
            {
                return VFSMetaData.MetaType.Unknown;
            }
            var assetPath = AssetDatabase.GetAssetPath(assetObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                return VFSMetaData.MetaType.Unknown;
            }
            var assetType = assetObject.GetType();
            var ignoreType = new List<Type>()
                {
                    typeof(LightingDataAsset),
                    typeof(MonoScript),
                };
            if (ignoreType.Contains(assetType))//排除的类型
            {
                return VFSMetaData.MetaType.Unknown;
            }
            if (assetType == typeof(DefaultAsset))
            {
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    return VFSMetaData.MetaType.ReferenceFolder;//允许文件夹
                }
                else
                {
                    return VFSMetaData.MetaType.Unknown;
                }
            }
            if (assetType == typeof(SceneAsset))
            {
                return VFSMetaData.MetaType.Scene;
            }
            else
            {
                return VFSMetaData.MetaType.Asset;
            }
        }
        protected override bool CanPerformDropOutside(Object[] objs, DragAndDropArgs args)
        {
            TreeViewItem<VFSTreeModelItem> parentViewItem = null;
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    if (args.parentItem.depth != -1)//根
                    {
                        parentViewItem = args.parentItem as TreeViewItem<VFSTreeModelItem>;
                    }
                    break;
                case DragAndDropPosition.OutsideItems:
                    break;
            }
            foreach (var item in objs)
            {
                var type = GetAssetMetaType(item);
                if (!CanPerformDropOutside(type, parentViewItem))
                {
                    return false;
                }
            }
            return true;
        }
        bool CanPerformDropOutside(VFSMetaData.MetaType metaType, TreeViewItem<VFSTreeModelItem> parent)
        {
            if (metaType == VFSMetaData.MetaType.Unknown)
            {
                return false;
            }
            if (parent == null)
            {
                return true;
            }
            else//type在必是 资源/场景/文件夹之一
            {
                var parentData = parent.ModelItem.Data;
                return parentData.IsFolder || parentData.IsBundle;
            }
        }

        protected override bool CanStartDragFromInside(IList<int> viewItemIDs)
        {
            foreach (var viewItemID in viewItemIDs)
            {
                //引用文件夹下的内容不给拖,这些内容保持和工程中的目录一致
                if (GetTreeModelItem(viewItemID).Data.IsReferences)
                {
                    return false;
                }
            }
            return true;
        }
        protected override bool CanPerformDropInside(List<TreeViewItem> draggedRows, DragAndDropArgs args)
        {
            TreeViewItem<VFSTreeModelItem> parentViewItem = null;
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    if (args.parentItem.depth != -1)//根
                    {
                        parentViewItem = args.parentItem as TreeViewItem<VFSTreeModelItem>;
                    }
                    break;
                case DragAndDropPosition.OutsideItems:
                    break;
            }
            foreach (var item in draggedRows)
            {
                var selfViewItem = item as TreeViewItem<VFSTreeModelItem>;
                if (!CanPerformDropInside(selfViewItem, parentViewItem))
                {
                    return false;
                }
            }
            return true;
        }
        bool CanPerformDropInside(TreeViewItem<VFSTreeModelItem> self, TreeViewItem<VFSTreeModelItem> parent)
        {
            if (self == null)
            {
                return false;
            }
            VFSMetaData metaData = self.ModelItem.Data;
            if (parent == null)
            {
                return !metaData.IsReferences;//引用文件夹里面的东西就不给拖 其他的都可以 资源/文件夹/包
            }
            else
            {
                VFSMetaData parentData = parent.ModelItem.Data;
                switch (metaData.type)
                {
                    case VFSMetaData.MetaType.Asset:
                    case VFSMetaData.MetaType.Scene:
                    case VFSMetaData.MetaType.VirtualFolder:
                    case VFSMetaData.MetaType.ReferenceFolder:
                        return parentData.IsFolder || parentData.IsBundle;  //只能放在文件夹里面

                    case VFSMetaData.MetaType.Bundle://包只能作为根
                        return false;
                    case VFSMetaData.MetaType.ReferenceAsset://子引用不给拖
                    case VFSMetaData.MetaType.ReferenceScene:
                    case VFSMetaData.MetaType.ReferenceSubFolder:
                        return false;
                    default:
                        return false;
                }
            }
        }

        protected override VFSTreeModelItem CreateTreeModelItemFromOutside(Object obj)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;//不是资产  (文件夹也是资产)
            }
            var guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            var data = new VFSMetaData(guid).Refresh();
            return new VFSTreeModelItem(GenerateUniqueID(), 0, data);
        }
        public override void OnAddTreeModelItemsFromOutside(IList<VFSTreeModelItem> addElements)
        {
            foreach (VFSTreeModelItem item in addElements)
            {
                if (item.Data.type == VFSMetaData.MetaType.ReferenceFolder)//添加的是引用文件夹  递归把里面的东西都添加进去
                {
                    AddReferenceFolder(item);
                }
            }
        }
        void AddReferenceFolder(VFSTreeModelItem folder)
        {
            var path = AssetDatabase.GUIDToAssetPath(folder.Data.Guid);
            var files = Directory.GetFiles(path); //当前文件夹中的文件
            foreach (var item in files)
            {
                if (item.StartsWith(".") || !item.EndsWith(".meta"))//忽略隐藏文件和meta文件
                {
                    var guid = AssetDatabase.AssetPathToGUID(item);
                    var add = new VFSTreeModelItem(GenerateUniqueID(), 0, new VFSMetaData(guid, VFSMetaData.MetaType.ReferenceAsset).Refresh());
                    AddTreeViewItemToTail(add, folder, false);
                }
            }
            var dirs = Directory.GetDirectories(path);//子文件夹 不递归
            foreach (var item in dirs)
            {
                if (AssetDatabase.IsValidFolder(item))//忽略隐藏文件夹
                {
                    var guid = AssetDatabase.AssetPathToGUID(item);
                    var add = new VFSTreeModelItem(GenerateUniqueID(), 0, new VFSMetaData(guid, VFSMetaData.MetaType.ReferenceSubFolder).Refresh());
                    AddTreeViewItemToTail(add, folder, false);

                    AddReferenceFolder(add);//递归添加子目录
                }
            }
        }

        //序列化
        protected override void OnTreeModelItemValueChange()
        {
            var datas = GetTreeModelItems().ToList();
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
            TreeViewItem<VFSTreeModelItem> viteItem = (args.item as TreeViewItem<VFSTreeModelItem>);
            VFSMetaData assetMetaData = viteItem.ModelItem.Data;
            Texture2D icon = null;
            Color oldColor = GUI.color;

            if (assetMetaData.IsAsset || assetMetaData.IsScene)//资产
            {
                if (!string.IsNullOrEmpty(assetMetaData.path))
                {
                    icon = AssetDatabase.GetCachedIcon(assetMetaData.path) as Texture2D;
                }
                if (icon == null)
                {
                    icon = missingIcon;
                }
            }
            else if (assetMetaData.IsBundle) //包
            {
                GUI.color = Color.yellow;
                icon = bundleIcon;
            }
            else if (assetMetaData.IsFolder) //文件夹分两种  引用文件夹和虚拟文件夹      TODO 还有一个情况目前没有处理  就是引用文件夹missing的情况
            {
                if (args.item.hasChildren)//文件夹折叠图标
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

                if (assetMetaData.type == VFSMetaData.MetaType.VirtualFolder)//虚拟文件夹是绿色的
                {
                    GUI.color = Color.green;
                }
            }

            if (assetMetaData.IsReferences)
            {
                GUI.color = Color.gray;
            }


            float offset = GetContentIndent(args.item);
            float rectHeight = args.rowRect.height;
            Rect iconRect = new Rect(args.rowRect.position + new Vector2(offset + rectHeight, 0), new Vector2(rectHeight, rectHeight));
            GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            GUI.color = oldColor;

            Rect toggleRect = new Rect(args.rowRect.position + new Vector2(offset, 0), new Vector2(rectHeight, rectHeight));
            if (GUI.Toggle(toggleRect, assetMetaData.toggle, string.Empty) != assetMetaData.toggle)
            {
                ToggleCall(viteItem, !assetMetaData.toggle);
                OnTreeModelItemValueChange();
            }
        }

        //toggle
        void ToggleCall(TreeViewItem<VFSTreeModelItem> item,bool value)
        {
            item.ModelItem.Data.toggle = value;
            //向上递归
            ToggleParent(item.ModelItem, value);
            //向下递归
            ToggleChild(item.ModelItem, value);
        }
        void ToggleParent(TreeModelItem item, bool value)
        {
            var parent = item.Parent;
            if (parent == null || parent.Depth == -1)
            {
                return;
            }
            if (!value)
            {
                foreach (var brother in parent.Children)
                {
                    var brotherModel = brother as VFSTreeModelItem;
                    if (brotherModel.Data.toggle) //所有兄弟都是false 才把上级set false
                    {
                        return;
                    }
                }
            }
            (parent as VFSTreeModelItem).Data.toggle = value;
            ToggleParent(parent, value);
        }
        void ToggleChild(TreeModelItem item, bool value)
        {
            if (item.HasChildren)
            {
                foreach (TreeModelItem child in item.Children)
                {
                    (child as VFSTreeModelItem).Data.toggle = value;
                    ToggleChild(child, value);
                }
            }
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
                TreeViewItem<VFSTreeModelItem> item = GetTreeViewItem(selects[0]);
                if (CanRename2(item.ModelItem))
                {
                    menu.AddItem(new GUIContent("Rename"), false, Rename);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Rename"));
                }
                menu.AddItem(new GUIContent("Save Path"), false, SaveElementFullPath);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Save Path"));
            }

            menu.ShowAsContext(); //显示菜单
        }
        void CreateFolder(object IsItemContext)
        {
            if ((bool)IsItemContext)//添加子文件夹
            {
                IList<int> selectionIDs = GetSelection().ToArray();
                foreach (var item in selectionIDs)
                {
                    var parentElement = GetTreeModelItem(item);
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
        VFSTreeModelItem GenerateFolderElement()
        {
            return new VFSTreeModelItem(GenerateUniqueID(), 0, new VFSMetaData(VFSMetaData.MetaType.VirtualFolder) { name = "New Folder"});//虚拟文件夹
        }
        VFSTreeModelItem GenerateBundleElement()
        {
            return new VFSTreeModelItem(GenerateUniqueID(), 0, new VFSMetaData(VFSMetaData.MetaType.Bundle) { name = "New Bundle"});//虚拟文件夹
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
        public void RefreshItemsNameAndPath()//为什么刷新?  guid不变 在资源发生更名|移动后,根据guid修正名称和路径(目前没有做资源监听,所以手动刷新一下)
        {
            var elements = GetTreeModelItems();
            foreach (var item in elements)
            {
                item.Data.Refresh();
            }
            OnTreeModelItemValueChange();
        }
    }
}
