/** Header
 *  AudioEventData.cs
 *  
 **/

namespace ZFramework {
    public class AudioEventDataBase : ZEventDataBase
    {
        public ZAudioPlayer Target { get; private set; }
        public AudioEventType EventType { get; private set; }

        internal void SetStaticData(ZAudioPlayer target, AudioEventType eventType)
        {
            Target = target;
            EventType = eventType;
        }
        internal override void Recycle()
        {
            Target = null;
        }
    }

    public class AudioEventData : AudioEventDataBase
    {
        internal AudioEventData SetData() {
            return this;
        }
    }

    public class AudioEventData<D0> : AudioEventDataBase
    {
        internal AudioEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }

    public class AudioEventData<D0, D1> : AudioEventDataBase
    {
        internal AudioEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }

    public class AudioEventData<D0, D1, D2> : AudioEventDataBase
    {
        internal AudioEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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