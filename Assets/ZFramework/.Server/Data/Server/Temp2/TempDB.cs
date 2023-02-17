using System;
using System.Collections.Generic;

namespace ZFramework
{
    public class TempDB :SingleComponent<TempDB>
    {
        //数据库建表对应这些字典
        //key = accountID
        Dictionary<long, Account> accounts = new Dictionary<long, Account>();//所有注册账号
        //key = roleID
        Dictionary<long, Role> roles = new Dictionary<long, Role>();//所有注册角色
        //key = roleID
        Dictionary<long, RoleLocationInfo> locations = new Dictionary<long, RoleLocationInfo>();//所有登录过角色的定位信息

        public Account RegisterAccount(string phoneNum)
        {
            foreach (var item in accounts)
            {
                if (item.Value.phoneNum == phoneNum)
                {
                    return item.Value;//已注册
                }
            }

            //测试数据 自动注册
            var newAccount = new Account()
            {
                id = Game.GetID(),
                phoneNum = phoneNum,
                username = "新用户" + (DateTime.UtcNow.Ticks / 10000).ToString("X2"),
                roles = new List<long>()
            };
            //前端目前默认有3个角色
            for (int i = 0; i < 3; i++)
            {
                var newRole = new Role()
                {
                    id = Game.GetID(),
                    accountID = newAccount.id,
                    gender = i%2,
                    modelIndex = i,
                    roleName = "默认角色" + (i + 1)
                };
                roles.Add(newRole.id, newRole);
                newAccount.roles.Add(newRole.id);
            }
            accounts.Add(newAccount.id, newAccount);
            return newAccount;
        }
        public Account GetAccount(long accountID)
        {
            if (accounts.TryGetValue(accountID,out Account account))
            {
                return account;
            }
            Log.Error($"account {accountID} is Null");
            return null;
        }

        public Role GetRole(long roleID)
        {
            if (roles.TryGetValue(roleID, out Role role))
            {
                return role;
            }
            Log.Error($"role {roleID} is Null");
            return null;
        }
        public bool SetRole(Role role)
        {
            if (roles.ContainsKey(role.id))
            {
                Log.Info("保存模型->" + role.ToJson());
                roles[role.id] = role;
                return true;
            }
            Log.Error($"role {role.id} is Null");
            return false;
        }


        public RoleLocationInfo GetLocation(long roleID)//拿上次登录的位置
        {
            if (!locations.TryGetValue(roleID, out RoleLocationInfo location))//还没有定位信息 就是没登录过的新角色
            {
                location = GetEnterLocation(roleID, MapNames.大厅, MapNames.主场景);
                locations.Add(roleID, location); 
            }
            return location;//最后一次下线的位置
        }
        public void SetLocation(long roleID, RoleLocationInfo location)
        {
            locations[roleID] = location;
        }

        public RoleLocationInfo GetEnterLocation(long roleID,string exitMap , string enterMap)//拿换场景的起始点
        {
            var role = GetRole(roleID);
            switch (exitMap)
            {
                case MapNames.大厅:
                    {
                        var pos = new UnityEngine.Vector3(0, 0, 0);
                        var qua = UnityEngine.Quaternion.Euler(0, 0, 0);
                        var location = new RoleLocationInfo()
                        {
                            role = role,
                            mapName = MapNames.主场景,
                            x = pos.x,
                            y = pos.y,
                            z = pos.z,
                            a = qua.x,
                            b = qua.y,
                            c = qua.z,
                            d = qua.w,
                        };
                        return location;
                    }
                case MapNames.主场景:
                    if (enterMap == MapNames.场景A)
                    {
                        var pos = new UnityEngine.Vector3(0, 0, 0);
                        var qua = UnityEngine.Quaternion.Euler(0, 0, 0);
                        var location = new RoleLocationInfo()
                        {
                            role = role,
                            mapName = MapNames.场景A,
                            x = pos.x,
                            y = pos.y,
                            z = pos.z,
                            a = qua.x,
                            b = qua.y,
                            c = qua.z,
                            d = qua.w,
                        };
                        return location;
                    }
                    if (enterMap == MapNames.场景B)
                    {
                        var pos = new UnityEngine.Vector3(0, 0, 0);
                        var qua = UnityEngine.Quaternion.Euler(0, 0, 0);
                        var location = new RoleLocationInfo()
                        {
                            role = role,
                            mapName = MapNames.场景B,
                            x = pos.x,
                            y = pos.y,
                            z = pos.z,
                            a = qua.x,
                            b = qua.y,
                            c = qua.z,
                            d = qua.w,
                        };
                        return location;
                    }
                    break;
                case MapNames.场景A:
                    {
                        var pos = new UnityEngine.Vector3(0, 0, 0);
                        var qua = UnityEngine.Quaternion.Euler(0, 0, 0);
                        var location = new RoleLocationInfo()
                        {
                            role = role,
                            mapName = MapNames.主场景,
                            x = pos.x,
                            y = pos.y,
                            z = pos.z,
                            a = qua.x,
                            b = qua.y,
                            c = qua.z,
                            d = qua.w,
                        };
                        return location;
                    }
                case MapNames.场景B:
                    {
                        var pos = new UnityEngine.Vector3(0, 0, 0);
                        var qua = UnityEngine.Quaternion.Euler(0, 0, 0);
                        var location = new RoleLocationInfo()
                        {
                            role = role,
                            mapName = MapNames.主场景,
                            x = pos.x,
                            y = pos.y,
                            z = pos.z,
                            a = qua.x,
                            b = qua.y,
                            c = qua.z,
                            d = qua.w,
                        };
                        return location;
                    }
                default:
                    break;
            }
            return null;
        }
    }
}
