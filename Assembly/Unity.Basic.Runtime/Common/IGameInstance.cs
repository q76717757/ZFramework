using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public interface IGameInstance
    {
        void Start(IList<Type> allTypes);

        void Update();
        void LateUpdate();
        void Close();
    }
}
