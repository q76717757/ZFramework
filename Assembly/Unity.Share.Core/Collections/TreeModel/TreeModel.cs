using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZFramework
{
    public class TreeModel<T> where T : TreeModelItem
    {
        IList<T> items;
        T root;
        int maxID;

        public T Root => root;
        /// <summary>
        /// item的层级发生变化 不包括item的内容
        /// </summary>
        public event Action OnModelChanged;

        public TreeModel(IList<T> data)
        {
            SetData(data);
        }
        public void SetData(IList<T> data)
        {
            if (data == null)
                throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");

            items = data;
            if (items.Count > 0)
                root = ListToTree(data);

            maxID = items.Max(e => e.ID);
        }

        //模型内部使用的自增ID
        public int GenerateUniqueID()
        {
            return ++maxID;
        }

        //按ID获取一个元素
        public T GetElement(int id)
        {
            return items.FirstOrDefault(element => element.ID == id);
        }
        //按路径获取一个元素
        public T GetElement(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return default;
            }
            Queue<string> key = new Queue<string>();
            var keys = path.Split('/');
            foreach (var item in keys)
            {
                key.Enqueue(item);
            }
            return GetElementOfPath(root,ref key);
        }
        private T GetElementOfPath(T node, ref Queue<string> key)
        {
            if (key.Count == 0)
            {
                return node;
            }
            if (node.HasChildren)
            {
                var current = key.Dequeue();
                foreach (var child in node.Children)
                {
                    if (child.DisplayName == current)
                    {
                        return GetElementOfPath((T)child, ref key);
                    }
                }
            }
            return default;
        }
        //获取完整邻接表
        public IList<T> GetAllTreeElements()
        {
            return  new List<T>(items);
        }

        //递归获取祖先
        public IList<int> GetParents(int id)
        {
            var parents = new List<int>();
            TreeModelItem T = GetElement(id);
            if (T != null)
            {
                while (T.Parent != null)
                {
                    parents.Add(T.Parent.ID);
                    T = T.Parent;
                }
            }
            return parents;
        }
        //得到有孩子的后代 (有折叠箭头的)
        public IList<int> GetDescendantsThatHaveChildren(int id)
        {
            T searchFromThis = GetElement(id);
            if (searchFromThis != null)
            {
                return GetParentsBelowStackBased(searchFromThis);
            }
            return new List<int>();
        }
        IList<int> GetParentsBelowStackBased(TreeModelItem searchFromThis)
        {
            Stack<TreeModelItem> stack = new Stack<TreeModelItem>();
            stack.Push(searchFromThis);

            var parentsBelow = new List<int>();
            while (stack.Count > 0)
            {
                TreeModelItem current = stack.Pop();
                if (current.HasChildren)
                {
                    parentsBelow.Add(current.ID);
                    foreach (TreeModelItem T in current.Children)
                    {
                        stack.Push(T);
                    }
                }
            }
            return parentsBelow;
        }

        public void AddElement(T element, TreeModelItem parent, int insertPosition)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null");
            if (parent == null)
                throw new ArgumentNullException("parent", "parent is null");

            if (parent.Children == null)
                parent.Children = new List<TreeModelItem>();

            parent.Children.Insert(insertPosition, element);
            element.Parent = parent;

            UpdateDepthValues(parent);
            TreeToList(root, items);

            CallChangedEvent();
        }
        public void AddElements(IList<T> elements, TreeModelItem parent, int insertPosition)
        {
            if (elements == null)
                throw new ArgumentNullException("elements", "elements is null");
            if (elements.Count == 0)
                throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
            if (parent == null)
                throw new ArgumentNullException("parent", "parent is null");

            if (parent.Children == null)
                parent.Children = new List<TreeModelItem>();

            parent.Children.InsertRange(insertPosition, elements.Cast<TreeModelItem>());
            foreach (var element in elements)
            {
                element.Parent = parent;
                element.Depth = parent.Depth + 1;
                UpdateDepthValues(element);
            }

            TreeToList(root, this.items);

            CallChangedEvent();
        }
        public void MoveElements(TreeModelItem parentElement, int insertionIndex, List<TreeModelItem> elements)
        {
            if (insertionIndex < 0)
                throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

            if (parentElement == null)
                return;

            // We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
            if (insertionIndex > 0)
                insertionIndex -= parentElement.Children.GetRange(0, insertionIndex).Count(elements.Contains);

            // Remove draggedItems from their parents
            foreach (var draggedItem in elements)
            {
                draggedItem.Parent.Children.Remove(draggedItem);    // remove from old parent
                draggedItem.Parent = parentElement;                 // set new parent
            }

            if (parentElement.Children == null)
                parentElement.Children = new List<TreeModelItem>();

            // Insert dragged items under new parent
            parentElement.Children.InsertRange(insertionIndex, elements);

            UpdateDepthValues(root);
            TreeToList(root, this.items);

            CallChangedEvent();
        }
        public void RemoveElements(IList<int> elementIDs)
        {
            IList<T> elements = this.items.Where(element => elementIDs.Contains(element.ID)).ToArray();
            RemoveElements(elements);
        }
        public void RemoveElements(IList<T> elements)
        {
            foreach (var element in elements)
                if (element == root)
                    throw new ArgumentException("It is not allowed to remove the root element");

            var commonAncestors = FindCommonAncestorsWithinList(elements);

            foreach (var element in commonAncestors)
            {
                element.Parent.Children.Remove(element);
                element.Parent = null;
            }

            TreeToList(root, this.items);

            CallChangedEvent();
        }
        //模型的层次发生变化   不包括模型的数据单元发生变化
        void CallChangedEvent()
        {
            if (OnModelChanged != null)
                OnModelChanged();
        }


        // 输入树的根节点  将整棵树展开成列表
        void TreeToList(T root, IList<T> result)
        {
            if (result == null)
                throw new NullReferenceException("The input 'IList<T> result' list is null");
            result.Clear();

            Stack<T> stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                T current = stack.Pop();
                result.Add(current);

                if (current.Children != null && current.Children.Count > 0)
                {
                    for (int i = current.Children.Count - 1; i >= 0; i--)
                    {
                        stack.Push((T)current.Children[i]);
                    }
                }
            }
        }
        //输入邻接表  组装成一颗树,并返回树的根节点
        T ListToTree(IList<T> list)
        {
            // 检查输入
            ValidateDepthValues(list);

            // 清空父子级状态
            foreach (var element in list)
            {
                element.Parent = null;
                element.Children = null;
            }

            // 根据深度信息 设置父子引用
            for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
            {
                var parent = list[parentIndex];
                bool alreadyHasValidChildren = parent.Children != null;
                if (alreadyHasValidChildren)
                    continue;

                int parentDepth = parent.Depth;
                int childCount = 0;

                // 基于深度值 累计直系子节点的数量  直到深度浮出了起始的深度
                for (int i = parentIndex + 1; i < list.Count; i++)
                {
                    if (list[i].Depth == parentDepth + 1)
                        childCount++;
                    if (list[i].Depth <= parentDepth)
                        break;
                }

                // 填充子节点的数组
                if (childCount > 0)
                {
                    List<TreeModelItem>  childList = new List<TreeModelItem>(childCount);
                    for (int i = parentIndex + 1; i < list.Count; i++)
                    {
                        if (list[i].Depth == parentDepth + 1)
                        {
                            list[i].Parent = parent;
                            childList.Add(list[i]);
                        }
                        if (list[i].Depth <= parentDepth)
                            break;
                    }
                    parent.Children = childList;
                }
            }

            return list[0];
        }
        //检查整个邻接表的深度正确性
        void ValidateDepthValues(IList<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("list should have items, count is 0, check before calling ValidateDepthValues", "list");

            if (list[0].Depth != -1)
                throw new ArgumentException("list item at index 0 should have a depth of -1 (since this should be the hidden root of the tree). Depth is: " + list[0].Depth, "list");

            for (int i = 0; i < list.Count - 1; i++)
            {
                int depth = list[i].Depth;
                int nextDepth = list[i + 1].Depth;
                if (nextDepth > depth && nextDepth - depth > 1)
                    throw new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
            }

            for (int i = 1; i < list.Count; ++i)
                if (list[i].Depth < 0)
                    throw new ArgumentException("Invalid depth value for item at index " + i + ". Only the first item (the root) should have depth below 0.");

            if (list.Count > 1 && list[1].Depth != 0)
                throw new ArgumentException("Input list item at index 1 is assumed to have a depth of 0", "list");
        }
        //从某个节点开始往下更新深度
        void UpdateDepthValues(TreeModelItem node)
        {
            if (node == null)
                throw new ArgumentNullException("node", "The element is null");

            if (!node.HasChildren)
                return;

            Stack<TreeModelItem> stack = new Stack<TreeModelItem>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                TreeModelItem current = stack.Pop();
                if (current.Children != null)
                {
                    foreach (var child in current.Children)
                    {
                        child.Depth = current.Depth + 1;
                        stack.Push(child);
                    }
                }
            }
        }
        //在列表中找到共同的祖先
        IList<T> FindCommonAncestorsWithinList(IList<T> elements)
        {
            if (elements.Count == 1)
                return new List<T>(elements);

            List<T> result = new List<T>(elements);
            result.RemoveAll(g => IsChildOf(g, elements));
            return result;
        }
        // 如果元素列表中存在子元素的祖先，则返回true
        bool IsChildOf(T child, IList<T> elements)
        {
            while (child != null)
            {
                child = (T)child.Parent;
                if (elements.Contains(child))
                    return true;
            }
            return false;
        }

    }

}
