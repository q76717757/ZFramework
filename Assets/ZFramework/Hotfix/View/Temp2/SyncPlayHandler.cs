using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class SyncPlayHandler
    {
        public long roleID;
        public bool IsMine;

        public SyncMovement syncMovement;

        public void Destory()
        {
            if (syncMovement != null)
            {
                UnityEngine.GameObject.Destroy(syncMovement.gameObject);
            }
        }
    }
}
