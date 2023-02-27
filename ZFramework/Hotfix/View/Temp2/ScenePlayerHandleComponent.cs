using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class ScenePlayerHandleAwake : OnAwakeImpl<ScenePlayerHandleComponent>
    {
        public override void OnAwake(ScenePlayerHandleComponent self)
        {
            ZEvent.CustomEvent.AddListener<S2C_聊天室>("聊天室", self.OnChatMsg);
        }
    }

    public class ScenePlayerHandleComponent : SingleComponent<ScenePlayerHandleComponent>
    {
        long selfRoleID;
        public Movement self;
        public Dictionary<long, SyncPlayHandler> localMapSyncRoles = new Dictionary<long, SyncPlayHandler>();//当前地图内其他玩家的替身

        public Dictionary<long, TempMonoTitleCanvas> chatBoxs = new Dictionary<long, TempMonoTitleCanvas>();//场景中所有人的聊天盒子 临时写在这里 这应该UI管理的

        GameObject GetTitlePre()
        {
            return Resources.Load<GameObject>("UI/GamePanel/TitleCanvas");
        }

        public void OnChatMsg(CustomEventData<S2C_聊天室> eventData)
        {
            var roleID = eventData.Data0.roleID;
            var msg = eventData.Data0.msg;

            if (self != null)
            {
                if (selfRoleID == roleID)
                {
                    SetChatBox(self.gameObject, roleID, msg);
                }
            }
            foreach (var item in localMapSyncRoles)
            {
                if (item.Key == roleID)
                {
                    SetChatBox(item.Value.syncMovement.gameObject ,roleID, msg);
                }
            }
        }
        void SetChatBox(GameObject modelGO , long roleID,string msg)
        {
            if (chatBoxs.TryGetValue(roleID, out TempMonoTitleCanvas obj))
            {
                obj.Init(modelGO, msg);
            }
            else
            {
                var pre = GetTitlePre();
                var go = GameObject.Instantiate<GameObject>(pre);
                obj = go.AddComponent<TempMonoTitleCanvas>();
                obj.Init(modelGO, msg);
                chatBoxs.Add(roleID, obj);
            }
        }

        public void CreateSelf(RoleLocationInfo location)
        {
            var pos = new Vector3(location.x, location.y, location.z);
            var qua = new Quaternion(location.a, location.b, location.c, location.d);

            //CreatePlayer   self
            var playerGO = GameObject.Instantiate<GameObject>(location.role.GetModelPrefab());
            playerGO.transform.SetPositionAndRotation(pos, qua);
            playerGO.name = location.role.id.ToString();

            //AddSyncController
            selfRoleID = location.role.id;
            self = playerGO.AddComponent<Movement>();

            //AddCameraComtroller
            Camera.main.gameObject.AddComponent<CameraController>().target = playerGO.transform;

            
        }

        public void CreateOther(RoleLocationInfo location)
        {
            if (location.role != null)
            {
                var pos = new Vector3(location.x, location.y, location.z);
                var qua = new Quaternion(location.a, location.b, location.c, location.d);

                //CreatePlayer other
                var playerGO = GameObject.Instantiate<GameObject>(location.role.GetModelPrefab());
                playerGO.transform.SetPositionAndRotation(pos, qua);
                playerGO.name = location.role.id.ToString();

                //AddAnimController
                var Controller = playerGO.AddComponent<CharacterController>();
                Controller.center = Vector3.up;

                
                var syncMovement = playerGO.AddComponent<SyncMovement>();
                syncMovement.Init(pos, qua);

                var otherHandler = new SyncPlayHandler()
                {
                    roleID = location.role.id,
                    IsMine = false,
                    syncMovement = syncMovement,
                };
                localMapSyncRoles.Add(otherHandler.roleID, otherHandler);
            }
        }


        public SyncPlayHandler GetSyncPlayFromRoleID(long roleID)
        {
            if (localMapSyncRoles.TryGetValue(roleID,out SyncPlayHandler other))
            {
                return other;
            }
            return null;
        }

        public void RemoveOther(long roleID)
        {
            if (localMapSyncRoles.TryGetValue(roleID,out SyncPlayHandler other))
            {
                Log.Info("移除角色" + roleID);
                other.Destory();
                localMapSyncRoles.Remove(roleID);
            }
        }

        public void ClearAll()
        {
            GameObject.Destroy(Camera.main.gameObject.GetComponent<CameraController>());
            GameObject.Destroy(self.gameObject);
            foreach (var item in localMapSyncRoles)
            {
                UnityEngine.GameObject.Destroy(item.Value.syncMovement.gameObject);
            }
            localMapSyncRoles.Clear();
        }

        
    }
}
