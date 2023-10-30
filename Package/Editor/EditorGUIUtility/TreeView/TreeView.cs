using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public abstract class TreeView<T> : TreeView where T : TreeElement
    {
        TreeModel<T> m_TreeModel;
        List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);

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
            m_TreeModel = new TreeModel<T>(datas);
            m_TreeModel.OnModelChanged += OnModelChange;//GUI操作修改Model的数据,model再回调gui重载
        }
        public void SetData(IList<T> datas)
        {
            m_TreeModel.SetData(datas);
            OnModelChange();
        }
        void OnModelChange()
        {
            Reload();
            OnTreeElementChange();
        }
        protected virtual void OnTreeElementChange() { }
        //构建树视图
        protected override sealed TreeViewItem BuildRoot()
        {
            const int depthForHiddenRoot = -1;
            return new TreeViewItem<T>(depthForHiddenRoot, m_TreeModel.Root);
        }
        protected override sealed IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (m_TreeModel.Root == null)
            {
                Debug.LogError("tree model root is null. did you call Reload()?");
            }

            m_Rows.Clear();
            if (hasSearch)
            {
                Search(m_TreeModel.Root, searchString, m_Rows);
            }
            else
            {
                if (m_TreeModel.Root.HasChildren)
                    AddChildrenRecursive(m_TreeModel.Root, 0, m_Rows);
            }

            SetupParentsAndChildrenFromDepths(root, m_Rows);//必须的步骤  根据深度信息 自动构建TreeViewItem的父子引用
            return m_Rows;
        }
        //递归添加子节点
        void AddChildrenRecursive(TreeElement parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (T child in parent.children.Cast<T>())
            {
                var item = new TreeViewItem<T>(depth, child);
                newRows.Add(item);

                if (child.HasChildren)
                {
                    if (IsExpanded(child.id))
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
            return m_Rows.Find(x => x.id == id) as TreeViewItem<T>;
        }
        public T GetTreeElement(int id)
        {
            return m_TreeModel.GetElement(id);
        }
        public T GetTreeElement(string path)
        {
            return m_TreeModel.GetElement(path);
        }
        public IList<T> GetAllTreeElements()
        {
            return m_TreeModel.GetAllTreeElements();
        }

        //添加    
        public TreeViewItem AddTreeViewItem(T element, TreeElement parent, int insertPosition) //parent传空 则默认添加到root上
        {
            if (element == null)
            {
                return null;
            }
            if (parent == null)
            {
                parent = m_TreeModel.Root;
            }
            m_TreeModel.AddElement(element, parent, insertPosition);
            SetSelection(new int[] { element.id }, TreeViewSelectionOptions.RevealAndFrame);
            return GetTreeViewItem(element.id);
        }
        public TreeViewItem AddTreeViewItemToTail(T element,TreeElement parent)
        {
            if (parent == null)
            {
                parent = m_TreeModel.Root;
            }
            int index = 0;
            if (parent.HasChildren)
            {
                index = parent.children.Count;
            }
            return AddTreeViewItem(element, parent, index);
        }
        public void AddTreeViewItems(IList<T> element, TreeElement parent, int insertPosition)
        {
            if (element != null && element.Count > 0)
            {
                m_TreeModel.AddElements(element, parent, insertPosition);
                int[] selectedIDs = element.Select(x => x.id).ToArray();
                SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
            }
        }

        //删除
        public void RemoveTreeViewItem(int id)
        {
            RemoveTreeViewItems(new List<int> { id });
        }
        public void RemoveTreeViewItems(IList<int> elementIDs)
        {
            m_TreeModel.RemoveElements(elementIDs);
        }


        //重命名
        protected bool RenameOfNewItem;
        protected override sealed void RenameEnded(RenameEndedArgs args)
        {
            if (args.acceptedRename && !string.IsNullOrEmpty(args.newName) && args.originalName != args.newName)
            {
                var element = GetTreeElement(args.itemID);
                element.DisplayName = args.newName;
                OnTreeElementChange();//只是更名不需要调reload 树结构不变  当层级发生变化时才需要调OnModelChange()
            }
            if (!args.acceptedRename && RenameOfNewItem)//对新建物体取消重名 == 撤销新建  把物体删掉
            {
                RemoveTreeViewItem(args.itemID);
            }
            RenameOfNewItem = false;
        }

        //搜索
        void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            const int kItemDepth = 0; // tree is flattened when searching

            Stack<T> stack = new Stack<T>();
            foreach (TreeElement element in searchFromThis.children)
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
                    foreach (TreeElement element in current.children)
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

        protected override sealed IList<int> GetAncestors(int id)
        {
            return m_TreeModel.GetParents(id);
        }
        protected override sealed IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return m_TreeModel.GetDescendantsThatHaveChildren(id);
        }

        //拖拽
        const string k_GenericDragID = "GenericDragColumnDragging";
        protected override sealed bool CanStartDrag(CanStartDragArgs args)
        {
            return CanStartDragFromInside();
        }
        protected virtual bool CanStartDragFromOutside(UnityEngine.Object[] objs)//可以通过外部拖拽元素进来
        {
            return false;
        }
        protected virtual bool CanStartDragFromInside()//可以拖拽自身元素
        {
            return false;
        }
        protected override sealed void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();//从展开的列表里面遍历 被折叠的就不会被拖拽出去
            DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);//把拖拽的元素放进hash表中
            DragAndDrop.objectReferences = new UnityEngine.Object[0];
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
            else
            {
                var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
                if (draggedRows != null && draggedRows.Count != 0 && CanStartDragFromInside())
                {
                    return HandleDragAndDropInside(draggedRows, args);
                }
            }
            return DragAndDropVisualMode.None;
        }
        //处理外部拖拽的元素
        DragAndDropVisualMode HandleDragAndDropOutside(UnityEngine.Object[] objs, DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    {
                        if (args.performDrop)
                        {
                            TreeElement parentElement = ((TreeViewItem<T>)args.parentItem).Element;
                            //-1是叠加  尾部插入
                            int insertAtIndex = args.insertAtIndex == -1 ? (parentElement.HasChildren ? parentElement.children.Count : 0) : args.insertAtIndex;
                            IList<T> addElements = CreateTreeElementsFromOutside(objs);
                            AddTreeViewItems(addElements, parentElement, insertAtIndex);
                        }
                        return DragAndDropVisualMode.Generic;
                    }
                case DragAndDropPosition.OutsideItems:
                    {
                        if (args.performDrop)
                        {
                            IList<T> addElements = CreateTreeElementsFromOutside(objs);
                            AddTreeViewItems(addElements, m_TreeModel.Root, m_TreeModel.Root.children.Count);
                        }
                        return DragAndDropVisualMode.Generic;
                    }
                default:
                    return DragAndDropVisualMode.None;
            }
        }
        IList<T> CreateTreeElementsFromOutside(UnityEngine.Object[] objs)
        {
            List<T> addElements = new List<T>();
            foreach (var item in objs)
            {
                var addElement = CreateTreeElementFromOutside(item);
                if (addElement != null)
                {
                    addElements.Add(addElement);
                }
            }
            return addElements;
        }
        protected virtual T CreateTreeElementFromOutside(UnityEngine.Object obj)//TreeElement是抽象的 从外部添加元素时 需要子类去执行实例化
        {
            throw new NotImplementedException("CanStartDragFromOutside() Is True,But Not Implemented CreateTreeElementFromOutside()");
        }
        //处理内部拖拽的元素
        DragAndDropVisualMode HandleDragAndDropInside(List<TreeViewItem> draggedRows, DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem://在之上
                case DragAndDropPosition.BetweenItems://在之间
                    {
                        bool validDrag = ValidDrag(args.parentItem, draggedRows);
                        if (args.performDrop && validDrag)
                        {
                            TreeElement parentElement = ((TreeViewItem<T>)args.parentItem).Element;
                            int insertAtIndex = args.insertAtIndex == -1 ? (parentElement.HasChildren ? parentElement.children.Count : 0) : args.insertAtIndex;
                            OnDropDraggedElementsAtIndex(draggedRows, parentElement, insertAtIndex);
                        }
                        return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
                    }
                case DragAndDropPosition.OutsideItems://在之外
                    {
                        if (args.performDrop)
                        {
                            OnDropDraggedElementsAtIndex(draggedRows, m_TreeModel.Root, m_TreeModel.Root.children.Count);
                        }
                        return DragAndDropVisualMode.Move;
                    }
                default:
                    return DragAndDropVisualMode.None;
            }
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
        void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, TreeElement parent, int insertIndex)
        {
            List<TreeElement> draggedElements = new List<TreeElement>();
            foreach (TreeViewItem item in draggedRows)
            {
                draggedElements.Add(((TreeViewItem<T>)item).Element);
            }

            int[] selectedIDs = draggedElements.Select(x => x.id).ToArray();
            m_TreeModel.MoveElements(parent, insertIndex, draggedElements);
            SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);//如果被拖进折叠的元素里面 折叠的元素就会自动展开
        }

        //子类实现从外部添加元素时,实例化泛型需要一个ID
        protected int GenerateUniqueID()
        {
            return m_TreeModel.GenerateUniqueID();
        }
    }
}
