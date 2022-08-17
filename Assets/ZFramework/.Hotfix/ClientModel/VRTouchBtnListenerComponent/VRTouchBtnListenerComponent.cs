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
    public class VRTouchBtnListenerComponent:Entity
    {
        public SteamVR_Action_Boolean btnA;
        public SteamVR_Action_Boolean btnB;
        public SteamVR_Action_Boolean btnX;
        public SteamVR_Action_Boolean btnY;
        public SteamVR_Action_Boolean snapLeftAction;
        public SteamVR_Action_Boolean snapRightAction;

        public SnapTurn snapTurn;

        public Kvr_InputModule kvr_InputModule;
        public SteamVR_LaserPointer lineLeft;
        public SteamVR_LaserPointer lineRight;
        public Kvr_UIPointer uiPointerLeft;
        public Kvr_UIPointer uiPointerRight;
    }
}
