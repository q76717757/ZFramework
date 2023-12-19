using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZFramework.Editor
{
    public abstract class TreeView<T> : TreeView where T : TreeModelItem
    {
        TreeModel<T> treeModel;
        List<TreeViewItem> viewItems = new List<TreeViewItem>(100);

        //初始化
        public TreeView(TreeViewState state, IList<T> datas) : base(state)
        {
            Init(datas);
        }
        public TreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, IList<T> datas) : base(state, multiColumnHeader)
        {
            Init(datas);
        }
        void Init(IList<T> datas)
        {
            treeModel = new TreeModel<T>(datas);
            treeModel.OnModelChanged += OnModelLevelChange;//模型层次发生变化的回调   //GUI操作修改Model的数据,model再回调gui重载
        }
        public void SetData(IList<T> datas)
        {
            treeModel.SetData(datas);
            OnModelLevelChange();
        }
        //树层次结构发生变化
        void OnModelLevelChange()
        {
            Reload();
            OnTreeModelItemValueChange();
        }
        //树叶的内容发生变化
        protected virtual void OnTreeModelItemValueChange() { }
        //构建树视图
        protected override sealed TreeViewItem BuildRoot()
        {
            const int depthForHiddenRoot = -1;
            return new TreeViewItem<T>(depthForHiddenRoot, treeModel.Root);
        }
        protected override sealed IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (treeModel.Root == null)
            {
                Debug.LogError("tree model root is null. did you call Reload()?");
            }

            viewItems.Clear();
            if (hasSearch)
            {
                Search(treeModel.Root, searchString, viewItems);
            }
            else
            {
                if (treeModel.Root.HasChildren)
                    AddChildrenRecursive(treeModel.Root, 0, viewItems);
            }

            SetupParentsAndChildrenFromDepths(root, viewItems);//必须的步骤  根据深度信息 自动构建TreeViewItem的父子引用
            return viewItems;
        }
        //递归添加子节点
        void AddChildrenRecursive(TreeModelItem parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (T child in parent.Children.Cast<T>())
            {
                var item = new TreeViewItem<T>(depth, child);
                newRows.Add(item);

                if (child.HasChildren)
                {
                    if (IsExpanded(child.ID))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        //查询
        public TreeViewItem<T> GetTreeViewItem(int id)
        {
            return viewItems.Find(x => x.id == id) as TreeViewItem<T>;
        }
        public T GetTreeModelItem(int id)
        {
            return treeModel.GetElement(id);
        }
        public T GetTreeModelItem(string path)
        {
            return treeModel.GetElement(path);
        }
        public IList<T> GetTreeModelItems()
        {
            return treeModel.GetAllTreeElements();
        }

        //添加
        public TreeViewItem AddTreeViewItem(T element, TreeModelItem parent, int insertPosition,bool select = true) //parent传空 则默认添加到root上
        {
            if (element == null)
            {
                return null;
            }
            if (parent == null)
            {
                parent = treeModel.Root;
            }
            treeModel.AddElement(element, parent, insertPosition);
            if (select)//创建后选中
            {
                SetSelection(new int[] { element.ID }, TreeViewSelectionOptions.RevealAndFrame);
            }
            return GetTreeViewItem(element.ID);
        }
        public TreeViewItem AddTreeViewItemToTail(T element,TreeModelItem parent,bool select = true)
        {
            if (parent == null)
            {
                parent = treeModel.Root;
            }
            int index = 0;
            if (parent.HasChildren)
            {
                index = parent.Children.Count;
            }
            return AddTreeViewItem(element, parent, index, select);
        }
        public void AddTreeViewItems(IList<T> element, TreeModelItem parent, int insertPosition,bool select = true)
        {
            if (element != null && element.Count > 0)
            {
                treeModel.AddElements(element, parent, insertPosition);
                int[] selectedIDs = element.Select(x => x.ID).ToArray();
                if (select)
                {
                    SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
                }
            }
        }
        
        //删除
        public void RemoveTreeViewItem(int id)
        {
            RemoveTreeViewItems(new List<int> { id });
        }
        public void RemoveTreeViewItems(IList<int> elementIDs)
        {
            treeModel.RemoveElements(elementIDs);
        }

        //重命名
        private bool RenameOfNewItem;
        protected new void BeginRename(TreeViewItem item)
        {
            RenameOfNewItem = true;
            base.BeginRename(item);
        }
        protected override sealed void RenameEnded(RenameEndedArgs args)
        {
            var newName = args.newName.Replace("/", "");//斜杠有特殊用途 要用来分割字符,禁用斜杠作为明明
            if (args.acceptedRename && !string.IsNullOrEmpty(newName) && args.originalName != newName)
            {
                var element = GetTreeModelItem(args.itemID);
                element.DisplayName = GenerateUniqueName(args.itemID, newName);
                OnTreeModelItemValueChange();//只是更名不需要调reload 树结构不变  当层级发生变化时才需要调OnModelChange()
            }
            if (!args.acceptedRename && RenameOfNewItem)//对新建物体取消重名 == 撤销新建  把物体删掉
            {
                RemoveTreeViewItem(args.itemID);
            }
            RenameOfNewItem = false;
        }
        string GenerateUniqueName(int itemID, string newName)
        {
            var parent = GetTreeModelItem(itemID).Parent;
            if (parent == null || !parent.HasChildren)
            {
                return newName;
            }
            else
            {
                List<TreeModelItem> brother = parent.Children;
                foreach (var item in brother)
                {
                    if (item.DisplayName == newName)
                    {
                        return GenerateUniqueName(itemID, $"{newName}+");
                    }
                }
            }
            return newName;
        }

        //搜索
        void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            const int kItemDepth = 0; // tree is flattened when searching

            Stack<T> stack = new Stack<T>();
            foreach (TreeModelItem element in searchFromThis.Children)
            {
                stack.Push((T)element);
            }
            while (stack.Count > 0)
            {
                T current = stack.Pop();
                // Matches search?
                if (current.DisplayName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(new TreeViewItem<T>(kItemDepth, current));
                }

                if (current.HasChildren)
                {
                    foreach (TreeModelItem element in current.Children)
                    {
                        stack.Push((T)element);
                    }
                }
            }
            SortSearchResult(result);
        }
        protected virtual void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
        }

        //血缘
        protected override sealed IList<int> GetAncestors(int id)
        {
            return treeModel.GetParents(id);
        }
        protected override sealed IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return treeModel.GetDescendantsThatHaveChildren(id);
        }

        //拖拽
        const string k_GenericDragID = "GenericDragColumnDragging";
        protected override sealed bool CanStartDrag(CanStartDragArgs args)
        {
            return CanStartDragFromInside(args.draggedItemIDs);
        }
        protected virtual bool CanStartDragFromOutside(Object[] objs)//可以通过外部拖拽元素进来
        {
            return false;
        }
        protected virtual bool CanStartDragFromInside(IList<int> viewItemIDs)//可以拖拽自身元素
        {
            return false;
        }
        protected override sealed void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            List<TreeViewItem> draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();//从展开的列表里面遍历 被折叠的就不会被拖拽出去
            DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);//把拖拽的元素放进hash表中
            DragAndDrop.objectReferences = new Object[0];
            string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
            DragAndDrop.StartDrag(title);
        }
        protected override sealed DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var objs = DragAndDrop.objectReferences;
            if (objs != null && objs.Length != 0 && CanStartDragFromOutside(objs))//从其他地方拖东西过来
            {
                return HandleDragAndDropOutside(objs, args);
            }
            else//内部拖拽
            {
                List<TreeViewItem> draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
                if (draggedRows != null && draggedRows.Count != 0 && CanStartDragFromInside(draggedRows.Select((x) => x.id).ToList()))
                {
                    return HandleDragAndDropInside(draggedRows, args);
                }
            }
            return DragAndDropVisualMode.None;
        }

        //处理外部拖拽的元素
        protected virtual bool CanPerformDropOutside(Object[] objs, DragAndDropArgs args)
        {
            return true;
        }
        DragAndDropVisualMode HandleDragAndDropOutside(Object[] objs, DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    {
                        if (CanPerformDropOutside(objs, args))
                        {
                            if (args.performDrop)
                            {
                                TreeModelItem parentElement = ((TreeViewItem<T>)args.parentItem).ModelItem;
                                //-1是叠加  尾部插入
                                int insertAtIndex = args.insertAtIndex == -1 ? (parentElement.HasChildren ? parentElement.Children.Count : 0) : args.insertAtIndex;
                                OnDropDraggedElementsAtIndex(objs, parentElement, insertAtIndex);
                            }
                            return DragAndDropVisualMode.Generic;
                        }
                        else
                        {
                            return DragAndDropVisualMode.None;
                        }
                    }
                case DragAndDropPosition.OutsideItems:
                    {
                        if (CanPerformDropOutside(objs, args))
                        {
                            if (args.performDrop)
                            {
                                OnDropDraggedElementsAtIndex(objs, treeModel.Root, treeModel.Root.Children.Count);
                            }
                            return DragAndDropVisualMode.Generic;
                        }
                        else
                        {
                            return DragAndDropVisualMode.None;
                        }
                    }
                default:
                    return DragAndDropVisualMode.None;
            }
        }
        void OnDropDraggedElementsAtIndex(Object[] objs, TreeModelItem parent, int insertIndex)
        {
            IList<T> addElements = CreateTreeModelItemsFromOutside(objs);
            AddTreeViewItems(addElements, parent, insertIndex);
            OnAddTreeModelItemsFromOutside(addElements);
        }
        IList<T> CreateTreeModelItemsFromOutside(Object[] objs)
        {
            List<T> addElements = new List<T>();
            foreach (var item in objs)
            {
                var addElement = CreateTreeModelItemFromOutside(item);
                if (addElement != null)
                {
                    addElements.Add(addElement);
                }
            }
            return addElements;
        }
        protected virtual T CreateTreeModelItemFromOutside(Object obj)//TreeModelItem是抽象的 从外部添加元素时 需要子类去执行实例化
        {
            throw new NotImplementedException("允许从外部拖拽元素,但子类没有实现元素的构造方法 CreateTreeModelItemFromOutside");
        }
        public virtual void OnAddTreeModelItemsFromOutside(IList<T> addElements)//从外部添加元素进去的回调
        {
        }

        //处理内部拖拽的元素
        protected virtual bool CanPerformDropInside(List<TreeViewItem> draggedRows, DragAndDropArgs args)
        {
            return true;
        }
        bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
        {
            TreeViewItem currentParent = parent;
            while (currentParent != null)
            {
                if (draggedItems.Contains(currentParent))
                    return false;
                currentParent = currentParent.parent;
            }
            return true;
        }
        DragAndDropVisualMode HandleDragAndDropInside(List<TreeViewItem> draggedRows, DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem://在之上
                case DragAndDropPosition.BetweenItems://在之间
                    {
                        if (ValidDrag(args.parentItem, draggedRows) && CanPerformDropInside(draggedRows, args))
                        {
                            if (args.performDrop)
                            {
                                TreeModelItem parentElement = ((TreeViewItem<T>)args.parentItem).ModelItem;
                                int insertAtIndex = args.insertAtIndex == -1 ? (parentElement.HasChildren ? parentElement.Children.Count : 0) : args.insertAtIndex;
                                OnDropDraggedElementsAtIndex(draggedRows, parentElement, insertAtIndex);
                            }
                            return DragAndDropVisualMode.Move;
                        }
                        else
                        {
                            return DragAndDropVisualMode.None;
                        }
                    }
                case DragAndDropPosition.OutsideItems://在之外
                    {
                        if (CanPerformDropInside(draggedRows, args))
                        {
                            if (args.performDrop)
                            {
                                OnDropDraggedElementsAtIndex(draggedRows, treeModel.Root, treeModel.Root.Children.Count);
                            }
                            return DragAndDropVisualMode.Move;
                        }
                        else
                        {
                            return DragAndDropVisualMode.None;
                        }
                        
                    }
                default:
                    return DragAndDropVisualMode.None;
            }
        }
        void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, TreeModelItem parent, int insertIndex)
        {
            List<TreeModelItem> draggedElements = new List<TreeModelItem>();
            foreach (TreeViewItem item in draggedRows)
            {
                draggedElements.Add(((TreeViewItem<T>)item).ModelItem);
            }
            int[] selectedIDs = draggedElements.Select(x => x.ID).ToArray();
            treeModel.MoveElements(parent, insertIndex, draggedElements);
            SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);//如果被拖进折叠的元素里面 折叠的元素就会自动展开
        }

        //子类实现从外部添加元素时,实例化泛型需要一个ID
        protected int GenerateUniqueID()
        {
            return treeModel.GenerateUniqueID();
        }
    }
}
