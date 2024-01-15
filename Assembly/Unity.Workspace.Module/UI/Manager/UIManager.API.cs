using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public partial class UIManager
    {
        //创建窗体   //TODO  连续创建一个需要下载的窗口  会重复下载资源, 待处理  拟通过VFS处理,让重复下载等待同一个异步任务?
        public static void CreateWindow<T>() where T: UIWindow
        {
            Instance.CreateWindowInner(typeof(T)).Invoke();
        }
        public static void CreateWindow(Type type)
        {
            Instance.CreateWindowInner(type).Invoke();
		}
        public static async ATask<T> CreateWindowAsync<T>() where T: UIWindow
        {
            return await Instance.CreateWindowInner(typeof(T)) as T;
        }
        public static async ATask<UIWindow> CreateWindowAsync(Type type)
        {
            return await Instance.CreateWindowInner(type);
		}

        //将指定窗体弹到栈顶
        public static void PopWindow(UIWindow window)
        {
            Instance.PopWindowInner(window);
        }
        //打开一个已经存在的UI
        public static void OpenWindow(UIWindow window)
        {
            Instance.OpenWindowInner(window);
        }
        //关闭一个UI 隐藏但不销毁  驻留在内存中  可以通过Open再次打开
        public static void CloseWindow(UIWindow window)
        {
            Instance.CloseWindowInner(window);
        }
        //销毁一个UI 同时释放资源 (这里需要做一下资源的引用计数?)
        public static void DestroyWindow(UIWindow window)
        {
            Instance.DestoryWindowInner(window);
        }
        //活跃窗口只有一个
        public static UIWindow GetActiveWindow()
        {
            return Instance.activeWindow;
        }


        //NotImplemented
        public static async ATask Preload(Type type)//预热  只下载资源  不是实例化
        {
            throw new NotImplementedException();
        }
        public static async ATask PreloadAllAsset()//预设所有的UI资源
        {
            throw new NotImplementedException();
        }
        public static bool AssetIsReady<T>() where T : UIWindow
        {
            return AssetIsReady(typeof(T));
        }
        public static bool AssetIsReady(Type type)
        {
            throw new NotImplementedException();
        }
        public static T GetWindow<T>() where T : UIWindow
        {
            throw new NotImplementedException();
        }
        public static T[] GetWindows<T>() where T : UIWindow
        {
            throw new NotImplementedException();
        }
        public static UIWindow GetWindowByType(Type type)
        {
            throw new NotImplementedException();
        }
        public static UIWindow[] GetWindowsByType(Type type)
        {
            throw new NotImplementedException();
        }
        public static UIWindow[] GetWindowsByLayer(UILayer layer)
        {
            throw new NotImplementedException();
        }
    }
}
