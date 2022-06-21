/** Header
 *  ZEventDataBase.cs
 *  监听数据的基类 用来约束泛型 
 **/

namespace ZFramework
{
    public abstract class ZEventDataBase
    {
        internal abstract void Recycle();

#if UNITY_EDITOR
        internal virtual void DataVisual() { }//编辑器用的  数据可视化  
#endif
    }

}
