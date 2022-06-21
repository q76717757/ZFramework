/** Header
 *  ZEventDriverBase.cs
 *  Unity事件驱动单元的抽象类   用来接收Unity的事件  然后转发给ZEvent的Handler   相当于是对Unity事件系统接口的实现
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework {
    public abstract class ZEventDriverBase<H> : MonoBehaviour where H : ZEventHandlerBase
    {
        private int _instanceID;
        protected int InstanceID => _instanceID;

        private H _handler;
        protected H Handler {
            get {
                if (_handler == null)
                    Log.Error($"{gameObject.name}:Driver Not Init!");
                return _handler;
            }
        }

        internal void Init(H handler) {
            _handler = handler;
            _instanceID = gameObject.GetInstanceID();
        }

        internal void DestoryDriver() => Destroy(this);
    }
}