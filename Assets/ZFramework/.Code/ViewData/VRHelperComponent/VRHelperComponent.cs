#if VR
using KVR_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;

namespace ZFramework
{
    public class VRHelperComponent : ComponentData
    {
        public Player player;

        public SteamVR_Action_Boolean btnA;
        public SteamVR_Action_Boolean btnB;
        public SteamVR_Action_Boolean btnX;
        public SteamVR_Action_Boolean btnY;
        public SteamVR_Action_Boolean snapLeftAction;
        public SteamVR_Action_Boolean snapRightAction;

        public RemoteControl remoteControl;//遥控器

        public bool hasLineLeft;//从前瞻虚拟世界进来 也许本来就有lp了 离开的时候就不移除 
        public bool hasLineRight;
        public SteamVR_LaserPointer lineLeft;
        public SteamVR_LaserPointer lineRight;

        public Kvr_InputModule kvr_InputModule;
        public Kvr_UIPointer uiPointerLeft;
        public Kvr_UIPointer uiPointerRight;

    }
}
#endif