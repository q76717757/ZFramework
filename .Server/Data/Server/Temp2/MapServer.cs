using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Sockets;

namespace ZFramework
{
    public class MapAwake : OnAwakeImpl<MapServer>
    {
        public override void OnAwake(MapServer self)
        {
            self.mapping = new Dictionary<string, HashSet<long>>();
            self.mapping.Add(MapNames.大厅, new HashSet<long>());
            self.mapping.Add(MapNames.主场景, new HashSet<long>());
            self.mapping.Add(MapNames.场景A, new HashSet<long>());
            self.mapping.Add(MapNames.场景B, new HashSet<long>());
        }
    }

    public class MapServer:SingleComponent<MapServer>
    {
        //key = mapName  value = roleIDs
        public Dictionary<string, HashSet<long>> mapping = new Dictionary<string, HashSet<long>>(); //每个地图都有哪些角色
        //key = roleID
        public Dictionary<long, SocketClient> onlineRoles = new Dictionary<long, SocketClient>();//所有角色的连接

        public void RoleLogin(string map,long roleID, SocketClient socket)
        {
            if (mapping.TryGetValue(map, out HashSet<long> otherRoleInThisMap))
            {
                List<RoleLocationInfo> othersLocation = new List<RoleLocationInfo>();

                //广播出去 给该地图在线玩家生成这个角色
                var send = new S2C_角色完成登入返回()
                {
                    syncLocations = new List<RoleLocationInfo>()
                    {
                        TempDB.Instance.GetLocation(roleID)
                    }
                };
                foreach (var otherRole in otherRoleInThisMap)
                {
                    
                    if (onlineRoles.TryGetValue(otherRole, out SocketClient otherSocket))
                    {
                        TcpServerComponent.Instance.Send2ClientAsync(otherSocket.ID, send);
                    }
                    var otherLocation = TempDB.Instance.GetLocation(otherRole);
                    othersLocation.Add(otherLocation);
                }

                //返回该地图除己外的所有角色
                TcpServerComponent.Instance.Send2ClientAsync(socket.ID, new S2C_角色完成登入返回()
                {
                    syncLocations = othersLocation
                });

                otherRoleInThisMap.Add(roleID);
                onlineRoles.Add(roleID, socket);
            }
        }
        public void RoleLogout(string map , long roleID)
        {
            if (mapping.TryGetValue(map,out HashSet<long> rolesInThisMap))
            {
                var send = new S2C_角色登出返回()
                {
                    roleID = roleID
                };
                //广播出去 给该地图在线玩家移除这个角色 包括自己()
                foreach (var otherRole in rolesInThisMap)
                {
                    if (onlineRoles.TryGetValue(otherRole, out SocketClient otherSocket))
                    {
                        TcpServerComponent.Instance.Send2ClientAsync(otherSocket.ID, send);
                    }
                }
                rolesInThisMap.Remove(roleID);
            }
            onlineRoles.Remove(roleID);
        }

        public void RoleExit(long roleID, string exit,string enter)//从一个场景去都另一个场景
        {
            if (mapping.TryGetValue(exit,out HashSet<long> exitMap))
            {
                exitMap.Remove(roleID);
                var send = new S2C_角色登出返回()
                {
                    roleID = roleID
                };
                foreach (var otherRole in exitMap)
                {
                    if (onlineRoles.TryGetValue(otherRole,out SocketClient otherSocket))
                    {
                        TcpServerComponent.Instance.Send2ClientAsync(otherSocket.ID, send);//暂用登出协议  也可以删other
                    }
                }
            }
            if (onlineRoles.TryGetValue(roleID, out SocketClient socket))
            {
                TcpServerComponent.Instance.Send2ClientAsync(socket.ID, new S2C_角色开始传送返回()
                {
                    location = TempDB.Instance.GetEnterLocation(roleID, exit, enter)
                });
            }
        }
        public void RoleEnter(RoleLocationInfo location)
        {
            List<RoleLocationInfo> othersLocation = new List<RoleLocationInfo>();
            if (mapping.TryGetValue(location.mapName, out HashSet<long> otherRoleInThisMap))
            {
                //广播出去 给该地图在线玩家生成这个角色
                var send = new S2C_角色完成登入返回()
                {
                    syncLocations = new List<RoleLocationInfo>()
                    {
                        location
                    }
                };
                foreach (var otherRole in otherRoleInThisMap)
                {
                    if (onlineRoles.TryGetValue(otherRole, out SocketClient otherSocket))
                    {
                        TcpServerComponent.Instance.Send2ClientAsync(otherSocket.ID, send);
                    }
                    var otherLocation = TempDB.Instance.GetLocation(otherRole);
                    othersLocation.Add(otherLocation);
                }
                otherRoleInThisMap.Add(location.role.id);
            }

            if (onlineRoles.TryGetValue(location.role.id, out SocketClient socket))
            {
                //返回该地图除己外的所有角色
                TcpServerComponent.Instance.Send2ClientAsync(socket.ID, new S2C_角色完成传送返回()
                {
                    syncLocations = othersLocation
                });
            }
        }


        public void OnClientDisconnected(string clientID)
        {
            long offlineRoleID = 0L;
            foreach (var onlineSocket in onlineRoles)
            {
                if (onlineSocket.Value.ID == clientID)//找到掉线的角色
                {
                    offlineRoleID = onlineSocket.Key;
                    Log.Info($"角色{offlineRoleID}:{clientID} -->掉线");
                    onlineRoles.Remove(offlineRoleID);
                    break;
                }
            }
            if (offlineRoleID == 0L)
            {
                //仅账号离线了 但并没有登录角色
                return;
            }

            foreach (var map in mapping)
            {
                if (map.Value.Remove(offlineRoleID))//在地图中找到这个人  广播给地图中其他人  删除角色
                {
                    Log.Info($"在地图{map.Key}找到掉线角色");
                    var send = new S2C_角色登出返回()
                    {
                        roleID = offlineRoleID
                    };
                    foreach (var otherRole in map.Value)
                    {
                        if (onlineRoles.TryGetValue(otherRole,out SocketClient otherSocket))
                        {
                            Log.Info($"通知[{otherSocket.ID}]删除替身->" + offlineRoleID);
                            TcpServerComponent.Instance.Send2ClientAsync(otherSocket.ID, send);
                        }
                    }
                    return;
                }
            }
        }


        public List<long> GetThisMapOtherRolesFromRoleID(long roleID)//找同地图内的其他在线玩家
        {
            var output = new List<long>();
            foreach (var item in mapping)
            {
                if (item.Value.Contains(roleID))
                {
                    foreach (var id in item.Value)
                    {
                        if (id == roleID)
                        {
                            continue;
                        }
                        output.Add(id);
                    }
                    return output;
                }
            }
            return output;
        }

        public HashSet<long> GetRolesFromMap(string map)
        {
            if (mapping.TryGetValue(map,out HashSet<long> output))
            {
                return output;
            }
            return null;
        }
        public SocketClient GetSocketFromRoleID(long roleID)
        {
            if (onlineRoles.TryGetValue(roleID,out SocketClient socket))
            {
                return socket;
            }
            return null;
        }
    }
}
