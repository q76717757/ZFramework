using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public interface IUILiveSystem
    {
        Type ComponentType { get; }
        Type UILiveType { get; }
    }
    public interface IUILive
    {
    }
    public interface IUIShow: IUILive
    {
        void OnShow(UICanvasComponent canvas);
    }
    public interface IUIHide: IUILive
    {
        void OnHide(UICanvasComponent canvas);
    }

    [UILive]
    public abstract class UIShowSystem<T>: IUILiveSystem, IUIShow where T : UICanvasComponent
    {
        public Type UILiveType => typeof(IUIShow);
        public Type ComponentType => typeof(T);
        void IUIShow.OnShow(UICanvasComponent canvas) => OnShow((T)canvas);
        public abstract void OnShow(T canvas);
    }
    [UILive]
    public abstract class UIHideSystem<T> : IUILiveSystem, IUIHide where T : UICanvasComponent
    {
        public Type UILiveType => typeof(IUIHide);
        public Type ComponentType => typeof(T);
        void IUIHide.OnHide(UICanvasComponent canvas) => OnHide((T)canvas);
        public abstract void OnHide(T canvas);
    }
}
