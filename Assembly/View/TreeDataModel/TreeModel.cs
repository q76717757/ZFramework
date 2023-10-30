using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// 基于邻接表模式的树模型   用来做层级树  列表第一个元素为隐藏根节点
    /// </summary>
    public class TreeModel<T> where T : TreeElement
    {
        IList<T> elements;//数据元素
        T m_Root;
        int m_MaxID;

        public T Root { get { return m_Root; } }
        public event Action OnModelChanged;
         
        public TreeModel(IList<T> data)
        {
            SetData(data);
        }
        public void SetData(IList<T> data)
        {
            if (data == null)
                throw new ArgumentNullException("data", "Input data is null. Ensure input is a non-null list.");

            elements = data;
            if (elements.Count > 0)
                m_Root = ListToTree(data);

            m_MaxID = elements.Max(e => e.id);
        }

        //模型内部使用的自增ID
        public int GenerateUniqueID()
        {
            return ++m_MaxID;
        }
        //重建模型中所有元素的ID
        public void RebuildElementsID()
        {
            m_MaxID = 0;
            foreach (var element in elements)
            {
                element.id = GenerateUniqueID();
            }
        }

        //按ID获取一个元素
        public T GetElement(int id)
        {
            return elements.FirstOrDefault(element => element.id == id);
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
            return GetElementOfPath(m_Root,ref key);
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
                foreach (var child in node.children)
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
            return new List<T>(elements);
        }

        //递归获取祖先
        public IList<int> GetParents(int id)
        {
            var parents = new List<int>();
            TreeElement T = GetElement(id);
            if (T != null)
            {
                while (T.parent != null)
                {
                    parents.Add(T.parent.id);
                    T = T.parent;
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
        IList<int> GetParentsBelowStackBased(TreeElement searchFromThis)
        {
            Stack<TreeElement> stack = new Stack<TreeElement>();
            stack.Push(searchFromThis);

            var parentsBelow = new List<int>();
            while (stack.Count > 0)
            {
                TreeElement current = stack.Pop();
                if (current.HasChildren)
                {
                    parentsBelow.Add(current.id);
                    foreach (var T in current.children)
                    {
                        stack.Push(T);
                    }
                }
            }
            return parentsBelow;
        }

        //空模型才可以加根
        public void AddRoot(T root)
        {
            if (root == null)
                throw new ArgumentNullException("root", "root is null");

            if (elements == null)
                throw new InvalidOperationException("Internal Error: data list is null");

            if (elements.Count != 0)
                throw new InvalidOperationException("AddRoot is only allowed on empty data list");

            root.id = GenerateUniqueID();
            root.depth = -1;
            elements.Add(root);
        }
        public void AddElement(T element, TreeElement parent, int insertPosition)
        {
            if (element == null)
                throw new ArgumentNullException("element", "element is null");
            if (parent == null)
                throw new ArgumentNullException("parent", "parent is null");

            if (parent.children == null)
                parent.children = new List<TreeElement>();

            parent.children.Insert(insertPosition, element);
            element.parent = parent;

            UpdateDepthValues(parent);
            TreeToList(m_Root, elements);

            CallChangedEvent();
        }
        public void AddElements(IList<T> elements, TreeElement parent, int insertPosition)
        {
            if (elements == null)
                throw new ArgumentNullException("elements", "elements is null");
            if (elements.Count == 0)
                throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
            if (parent == null)
                throw new ArgumentNullException("parent", "parent is null");

            if (parent.children == null)
                parent.children = new List<TreeElement>();

            parent.children.InsertRange(insertPosition, elements.Cast<TreeElement>());
            foreach (var element in elements)
            {
                element.parent = parent;
                element.depth = parent.depth + 1;
                UpdateDepthValues(element);
            }

            TreeToList(m_Root, this.elements);

            CallChangedEvent();
        }
        public void MoveElements(TreeElement parentElement, int insertionIndex, List<TreeElement> elements)
        {
            if (insertionIndex < 0)
                throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

            // Invalid reparenting input
            if (parentElement == null)
                return;

            // We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
            if (insertionIndex > 0)
                insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);

            // Remove draggedItems from their parents
            foreach (var draggedItem in elements)
            {
                draggedItem.parent.children.Remove(draggedItem);    // remove from old parent
                draggedItem.parent = parentElement;                 // set new parent
            }

            if (parentElement.children == null)
                parentElement.children = new List<TreeElement>();

            // Insert dragged items under new parent
            parentElement.children.InsertRange(insertionIndex, elements);

            UpdateDepthValues(m_Root);
            TreeToList(m_Root, this.elements);

            CallChangedEvent();
        }
        public void RemoveElements(IList<int> elementIDs)
        {
            IList<T> elements = this.elements.Where(element => elementIDs.Contains(element.id)).ToArray();
            RemoveElements(elements);
        }
        public void RemoveElements(IList<T> elements)
        {
            foreach (var element in elements)
                if (element == m_Root)
                    throw new ArgumentException("It is not allowed to remove the root element");

            var commonAncestors = FindCommonAncestorsWithinList(elements);

            foreach (var element in commonAncestors)
            {
                element.parent.children.Remove(element);
                element.parent = null;
            }

            TreeToList(m_Root, this.elements);

            CallChangedEvent();
        }

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

                if (current.children != null && current.children.Count > 0)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push((T)current.children[i]);
                    }
                }
            }
        }
        //输入邻接表  组装成一颗树,并返回树的根节点
        T ListToTree(IList<T> list)
        {
            // Validate input
            ValidateDepthValues(list);

            // Clear old states
            foreach (var element in list)
            {
                element.parent = null;
                element.children = null;
            }

            // Set child and parent references using depth info
            for (int parentIndex = 0; parentIndex < list.Count; parentIndex++)
            {
                var parent = list[parentIndex];
                bool alreadyHasValidChildren = parent.children != null;
                if (alreadyHasValidChildren)
                    continue;

                int parentDepth = parent.depth;
                int childCount = 0;

                // Count children based depth value, we are looking at children until it's the same depth as this object
                for (int i = parentIndex + 1; i < list.Count; i++)
                {
                    if (list[i].depth == parentDepth + 1)
                        childCount++;
                    if (list[i].depth <= parentDepth)
                        break;
                }

                // Fill child array
                List<TreeElement> childList = null;
                if (childCount != 0)
                {
                    childList = new List<TreeElement>(childCount); // Allocate once
                    childCount = 0;
                    for (int i = parentIndex + 1; i < list.Count; i++)
                    {
                        if (list[i].depth == parentDepth + 1)
                        {
                            list[i].parent = parent;
                            childList.Add(list[i]);
                            childCount++;
                        }

                        if (list[i].depth <= parentDepth)
                            break;
                    }
                }

                parent.children = childList;
            }

            return list[0];
        }
        //检查整个邻接表的深度正确性
        void ValidateDepthValues(IList<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("list should have items, count is 0, check before calling ValidateDepthValues", "list");

            if (list[0].depth != -1)
                throw new ArgumentException("list item at index 0 should have a depth of -1 (since this should be the hidden root of the tree). Depth is: " + list[0].depth, "list");

            for (int i = 0; i < list.Count - 1; i++)
            {
                int depth = list[i].depth;
                int nextDepth = list[i + 1].depth;
                if (nextDepth > depth && nextDepth - depth > 1)
                    throw new ArgumentException(string.Format("Invalid depth info in input list. Depth cannot increase more than 1 per row. Index {0} has depth {1} while index {2} has depth {3}", i, depth, i + 1, nextDepth));
            }

            for (int i = 1; i < list.Count; ++i)
                if (list[i].depth < 0)
                    throw new ArgumentException("Invalid depth value for item at index " + i + ". Only the first item (the root) should have depth below 0.");

            if (list.Count > 1 && list[1].depth != 0)
                throw new ArgumentException("Input list item at index 1 is assumed to have a depth of 0", "list");
        }
        //从某个节点开始往下更新深度
        void UpdateDepthValues(TreeElement node)
        {
            if (node == null)
                throw new ArgumentNullException("node", "The element is null");

            if (!node.HasChildren)
                return;

            Stack<TreeElement> stack = new Stack<TreeElement>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                TreeElement current = stack.Pop();
                if (current.children != null)
                {
                    foreach (var child in current.children)
                    {
                        child.depth = current.depth + 1;
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
                child = (T)child.parent;
                if (elements.Contains(child))
                    return true;
            }
            return false;
        }

    }

}
