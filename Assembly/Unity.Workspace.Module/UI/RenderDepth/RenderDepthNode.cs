using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    internal sealed class RenderDepthNode//树和链表的混合结构
    {
        public UIWindow Window { get; }
        public int Depth { get; set; }
        public RenderDepthNode Left { get; set; }
        public RenderDepthNode Right { get; set; }
        public RenderDepthNode Parent { get; set; }
        public List<RenderDepthNode> Children { get; private set; }
        public bool HasChildren { get => Children != null && Children.Count > 0; }
        public int ChildCount { get => Children == null ? 0 : Children.Count; }

        public RenderDepthNode(UIWindow window)
        {
            Window = window;
        }

        public void AddChild(RenderDepthNode childNode)
        {
            if (Children == null)
            {
                Children = new List<RenderDepthNode>();
            }
            Children.Add(childNode);
        }
        public void RemoveChild(RenderDepthNode childNode)
        {
            if (Children != null)
            {
                Children.Remove(childNode);
            }
        }
        public void ApplyDepth()
        {
            if (Window != null)
            {
                Window.SetDepth(Depth);
            }
        }
    }

}
