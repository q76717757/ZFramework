
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    //生命周期
    public class View_Canvas001Awake : AwakeSystem<View_Canvas001_Component>
    {
        public override void Awake(View_Canvas001_Component component)
        {

        }
    }
    public class View_Canvas001Update : UpdateSystem<View_Canvas001_Component>
    {
        public override void Update(View_Canvas001_Component component)
        {
        }
    }

    public class View_Canvas001_Show : UIShowSystem<View_Canvas001_Component>
    {
        public override void OnShow(View_Canvas001_Component canvas)
        {
        }
    }

    public class View_Canvas001_Hide : UIHideSystem<View_Canvas001_Component>
    {
        public override void OnHide(View_Canvas001_Component canvas)
        {
        }
    }

    //逻辑
    public static class View_Canvas001_System
    {

    }
}
