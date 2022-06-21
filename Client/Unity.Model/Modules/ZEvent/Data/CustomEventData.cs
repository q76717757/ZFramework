/** Header
 *  CustomEventData.cs
 *  自定义事件数据容器
 **/

namespace ZFramework {
    public class CustomEventDataBase : ZEventDataBase
    {
        public string Target { get; private set; }
        internal void SetStaticData(string target)
        {
            Target = target;
        }
        internal override void Recycle()
        {
            Target = string.Empty;
        }
    }

    public class CustomEventData : CustomEventDataBase
    {
        //internal CustomEventData() { }//构造必须public  不然泛型实例不了  对象池开发完了再改  用抽象工厂来实例  把构造封闭
        internal CustomEventData SetData(string target)
        {
            base.SetStaticData(target);
            return this;
        }
    }
    public class CustomEventData<D0> : CustomEventDataBase
    {
        internal CustomEventData<D0> SetData(string target, D0 data0)
        {
            base.SetStaticData(target);
            Data0 = data0;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Data0 = default;
        }
        public D0 Data0 { get; private set; }
    }
    public class CustomEventData<D0, D1> : CustomEventDataBase
    {
        internal CustomEventData<D0, D1> SetData(string target, D0 data0, D1 data1)
        {
            base.SetStaticData(target);
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Data0 = default;
            Data1 = default;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class CustomEventData<D0, D1, D2> : CustomEventDataBase
    {
        internal CustomEventData<D0, D1, D2> SetData(string target, D0 data0, D1 data1, D2 data2)
        {
            base.SetStaticData(target);
            Data0 = data0;
            Data1 = data1;
            Data2 = data2;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Data0 = default;
            Data1 = default;
            Data2 = default;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
        public D2 Data2 { get; private set; }
    }
}

