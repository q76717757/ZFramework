/** Header
 *  CycleEventData.cs
 *  帧循环事件数据容器
 **/

namespace ZFramework
{
    public class CycleEventDataBase : ZEventDataBase
    {
        public CycleType Target { get; private set; }

        internal void SetStaticData(CycleType target)
        {
            Target = target;
        }
        internal override void Recycle()
        {

        }
    }

    public class CycleEventData : CycleEventDataBase
    {
        internal CycleEventData SetData() {
            return this;
        }
    }
    public class CycleEventData<D0> : CycleEventDataBase
    {
        internal CycleEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class CycleEventData<D0, D1> : CycleEventDataBase
    {
        internal CycleEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class CycleEventData<D0, D1, D2> : CycleEventDataBase
    {
        internal CycleEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
        {
            Data0 = data0;
            Data1 = data1;
            Data2 = data2;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
        public D2 Data2 { get; private set; }
    }
}
