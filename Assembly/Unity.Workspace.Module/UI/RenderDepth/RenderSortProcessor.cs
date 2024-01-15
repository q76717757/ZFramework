using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

 
namespace ZFramework
{
    internal sealed class RenderSortProcessor : Component
    {
        private short minOrder;
        private short maxOrder;
        private RenderDepthNode root;//链表起点 隐藏根
        private readonly Dictionary<int, RenderDepthNode> nodes = new Dictionary<int, RenderDepthNode>();//全部节点 除了隐藏根
        private readonly Stack<RenderDepthNode> forkStartNodes = new Stack<RenderDepthNode>();//分支节点缓存栈

        internal RectTransform rectTransform;

        internal void Init(short minOrder, short maxOrder, RectTransform rectTransform)
        {
            this.rectTransform = rectTransform;
            this.minOrder = minOrder;
            this.maxOrder = maxOrder;
            root = new RenderDepthNode(null) { Depth = this.minOrder - 1 };
        }

        internal void Registry(UIWindow newWindow)
        {
            //新建映射节点
            RenderDepthNode newNode = new RenderDepthNode(newWindow);
            nodes.Add(newWindow.InstanceID, newNode);

            //绑定节点父子关系
            RenderDepthNode parentNode = newWindow.Parent == null ? root : GetNode(newWindow.Parent.InstanceID);
            LinkPaternity(parentNode, newNode);

            //插入父节点的链尾
            RenderDepthNode position = GetDeepestChildNode(parentNode);
            InsertAfter(position, newNode, newNode);

            //更新depth
            UpdateDepth(parentNode);
            //应用depth
            ApplyDepth(parentNode);
        }
        internal RenderDepthNode Unregistry(UIWindow window)
        {
            RenderDepthNode node = GetNode(window.InstanceID);
            RenderDepthNode end = GetDeepestChildNode(node);
            RenderDepthNode left = node.Left;

            //剪掉分支
            ClipRange(node, end);

            //解绑节点父子关系
            RenderDepthNode parentNode = window.Parent == null ? root : GetNode(window.Parent.InstanceID);
            UnLinkPaternity(parentNode, node);

            //移除节点
            RenderDepthNode right = node;
            while (right != null)
            {
                nodes.Remove(node.Window.InstanceID);
                Log.Info("Remove->" + node.Window.InstanceID);
                right = right.Right;
            }

            //更新depth
            UpdateDepth(parentNode);
            //应用depth
            ApplyDepth(parentNode);

            return left;
        }


        internal RenderDepthNode GetNode(int id)
        {
            return nodes[id];
        }
        internal RenderDepthNode GetDeepestChildNode(int id)
        {
            return GetDeepestChildNode(GetNode(id));
        }


        internal void PopNode(int id)
        {
            //从下往上找出所有分叉链的起点
            RenderDepthNode node = GetNode(id);
            GetForkNodes(node);
            if (forkStartNodes.Count > 0)
            {
                //从上往下修剪分支
                RenderDepthNode forkRoot = forkStartNodes.Peek();
                while (forkStartNodes.Count > 0)
                {
                    RenderDepthNode forkStart = forkStartNodes.Pop();

                    RenderDepthNode forkEnd = GetDeepestChildNode(forkStart);//要剪断的片段
                    ClipRange(forkStart, forkEnd);

                    RenderDepthNode position = GetDeepestChildNode(forkStart.Parent);//片段插入的位置
                    InsertAfter(position, forkStart, forkEnd);

                    UpdateDepth(position);//逐分支更新深度
                }
                ApplyDepth(forkRoot);//最后应用深度值
            }
        }



        //从指定节点开始 向下更新深度
        void UpdateDepth(RenderDepthNode start)
        {
            RenderDepthNode next = start;
            int nextDepth = next.Depth;
            while (next != null)
            {
                if (nextDepth > maxOrder)//越界了 尝试从root开始重排
                {
                    if (start == root)
                    {
                        throw new OverflowException($"链的长度超出范围了");
                    }
                    else
                    {
                        UpdateDepth(root);
                        ApplyDepth(root);
                        return;
                    }
                }
                next.Depth = nextDepth++;
                next = next.Right;
                if (next == start)
                    throw new StackOverflowException("这是环形链");
            }
        }
        //从指定节点开始 向下应用深度
        void ApplyDepth(RenderDepthNode start)
        {
            RenderDepthNode next = start;
            while (next != null)
            {
                next.ApplyDepth();
                next = next.Right;
                if (next == start)
                    throw new StackOverflowException("这是环形链");
            }
        }

        //建立父子关系
        void LinkPaternity(RenderDepthNode parent, RenderDepthNode child)
        {
            child.Parent = parent;
            parent.AddChild(child);
        }
        //解除父子关系
        void UnLinkPaternity(RenderDepthNode parent, RenderDepthNode child)
        {
            parent.RemoveChild(child);
            child.Parent = null;
        }

        //获取指定一脉的所有分叉节点 (即有兄弟的祖先 有兄弟才要处理兄弟间的遮挡关系  父子必然子挡父,无须处理)
        //  如       1
        //         |   |
        //         2   3
        //         |  | |
        //         4  5 6   GetForkNodes(6) return [6,3]   GetForkNodes(4) return [2]
        void GetForkNodes(RenderDepthNode node)
        {
            forkStartNodes.Clear();
            RenderDepthNode self = node;
            while (self != null)
            {
                RenderDepthNode parent = self.Parent;
                if (parent == null)
                {
                    return;
                }
                if (parent.ChildCount > 1)//self has brother
                {
                    forkStartNodes.Push(self);
                }
                self = parent;
                if (self == node)
                    throw new StackOverflowException("父子关系存在环形依赖");
            }
        }
        //向下找出深度最深的子
        RenderDepthNode GetDeepestChildNode(RenderDepthNode node)
        {
            if (node.HasChildren)
            {
                RenderDepthNode maxDepthNode = node;
                foreach (RenderDepthNode child in node.Children)
                {
                    if (child.Left == null)//被剪掉的分支
                    {
                        continue;
                    }
                    if (child.Depth > maxDepthNode.Depth)
                    {
                        maxDepthNode = child;
                    }
                }
                if (maxDepthNode != node)
                {
                    return GetDeepestChildNode(maxDepthNode);
                }
            }
            return node;
        }


        //剪掉一个片段
        void ClipRange(RenderDepthNode start,RenderDepthNode end)
        {
            //校验
            ThrowIfNotLink(start, end);
            //断开连接处
            RenderDepthNode left = DisconnectLeft(start);
            RenderDepthNode right = DisconnectRight(end);
            //原链焊接
            ConnectNodes(left, right);
        }
        //在某节后 插入片段
        void InsertAfter(RenderDepthNode insert, RenderDepthNode start,RenderDepthNode end)
        {
            ThrowIfNotLink(start, end);
            if (insert == start)
            {
                //不能插入自身
                return;
            }
            RenderDepthNode targetRight = insert.Right;
            ConnectNodes(insert, start);
            ConnectNodes(end, targetRight);
        }


        //把两个节点焊接
        void ConnectNodes(RenderDepthNode left, RenderDepthNode right)
        {
            if (left == right)
            {
                throw new ArgumentException("left 不能和 right相同");
            }
            if (left != null)
            {
                if (left.Right != null)
                {
                    throw new ArgumentException("左节点未断开 不能焊接");
                }
                left.Right = right;
            }
            if (right != null)
            {
                if (right.Left != null)
                {
                    throw new ArgumentException("右节点未断开 不能焊接");
                }
                right.Left = left;
            }
        }
        //断开节点的一边
        RenderDepthNode DisconnectLeft(RenderDepthNode node)
        {
            RenderDepthNode left = node.Left;
            node.Left = null;
            if (left != null)
                left.Right = null;
            return left;
        }
        RenderDepthNode DisconnectRight(RenderDepthNode node)
        {
            RenderDepthNode right = node.Right;
            node.Right = null;
            if (right != null)
                right.Left = null;
            return right;
        }


        //检查是不是有效的双向直链片段
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ThrowIfNotLink(RenderDepthNode start, RenderDepthNode end)
        {
            ThrowIfNull(start);
            ThrowIfNull(end);

            RenderDepthNode next = start;
            while (next != null)
            {
                if (next == end)
                {
                    return;//合法  单个节点也算是一条链
                }
                else
                {
                    next = next.Right;
                    if (next == start)
                    {
                        throw new StackOverflowException("这是环形链");
                    }
                }
            }
            throw new ArgumentException("链是断开的,start无法抵达end");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ThrowIfNull(RenderDepthNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException();
            }
        }
    }

}
