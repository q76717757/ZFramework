using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TouchSocket.Sockets;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace ZFramework
{
    public interface INetSerialize
    {
        public PType Code { get; }
        public string GetMessage();
        public void SetMesssage(string msg);
    }
    public abstract class C2S_Message : INetSerialize
    {
        public SocketClient Client { get; set; }
        public abstract PType Code { get; }
        public abstract string GetMessage();
        public abstract void SetMesssage(string msg);
    }
    public abstract class S2C_Message : INetSerialize
    {
        public TcpClient Client { get; set; }
        public abstract PType Code { get; }
        public abstract string GetMessage();
        public abstract void SetMesssage(string msg);
    }



    public enum PType
    {
        C2S_心跳,
        S2C_心跳,

        C2S_账号登入,
        C2S_账号登出,
        C2S_保存模型,

        C2S_角色开始登入,
        C2S_角色完成登入,

        C2S_角色登出,

        C2S_角色开始传送,
        C2S_角色完成传送,

        C2S_位置同步,

        C2S_聊天室,

        S2C_账号登入返回,
        S2C_账号登出返回,
        S2C_顶号,
        S2C_保存模型返回,

        S2C_角色开始登入返回,
        S2C_角色完成登入返回,

        S2C_角色登出返回,

        S2C_角色开始传送返回,
        S2C_角色完成传送返回,

        S2C_位置同步返回,
        S2C_角色离线,

        S2C_聊天室
    }
    public class C2S_心跳 : C2S_Message
    {
        public override PType Code => PType.C2S_心跳;

        public long time;

        public override string GetMessage()
        {
            return time.ToString();
        }
        public override void SetMesssage(string msg)
        {
            time = long.Parse(msg);
        }
    }
    public class S2C_心跳 : S2C_Message
    {
        public override PType Code => PType.S2C_心跳;
        public long time;
        public long serverTime;

        public override string GetMessage()
        {
            return $"{serverTime}|{time}";
        }

        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            serverTime = long.Parse(s[0]);
            time = long.Parse(s[1]);
        }
    }

    public class C2S_账号登入 : C2S_Message
    {
        public override PType Code => PType.C2S_账号登入;

        public string PhoneNum;
        public override string GetMessage()
        {
            return PhoneNum;
        }
        public override void SetMesssage(string msg)
        {
            PhoneNum = msg;
        }
    }
    public class C2S_账号登出 : C2S_Message
    {
        public override PType Code => PType.C2S_账号登出;

        public long accountID;
        public override string GetMessage()
        {
            return accountID.ToString();
        }

        public override void SetMesssage(string msg)
        {
            accountID = long.Parse(msg);
        }
    }
    public class C2S_保存模型 : C2S_Message
    {
        public override PType Code => PType.C2S_保存模型;

        public Role roleConfig;
        public override string GetMessage()
        {
            return Json.ToJson(roleConfig);
        }
        public override void SetMesssage(string msg)
        {
            roleConfig = Json.ToObject<Role>(msg);
        }
    }
    public class C2S_角色开始登入 : C2S_Message
    {
        public override PType Code => PType.C2S_角色开始登入;

        public long roleID;
        public override string GetMessage()
        {
            return roleID.ToString();
        }
        public override void SetMesssage(string msg)
        {
            roleID = long.Parse(msg);
        }
    }
    public class C2S_角色完成登入 : C2S_Message
    {
        public override PType Code => PType.C2S_角色完成登入;

        public long roleID;
        public string sceneName;
        public override string GetMessage()
        {
            return $"{roleID}|{sceneName}";
        }
        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            roleID = long.Parse(s[0]);
            sceneName = s[1];
        }
    }

    public class C2S_角色登出 : C2S_Message
    {
        public override PType Code => PType.C2S_角色登出;

        public long roleID;
        public string levelMap;
        public override string GetMessage()
        {
            return $"{roleID}|{levelMap}";
        }

        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            roleID = long.Parse(s[0]);
            levelMap = s[1];
        }
    }

    public class C2S_角色开始传送 : C2S_Message
    {
        public override PType Code => PType.C2S_角色开始传送;

        public long roleID;
        public string exitMap;
        public string enterMap;
        public override string GetMessage()
        {
            return $"{roleID}|{exitMap}|{enterMap}";
        }

        public override void SetMesssage(string msg)
        {
            var s= msg.Split('|');
            roleID= long.Parse(s[0]);
            exitMap= s[1];
            enterMap = s[2];
        }
    }

    public class C2S_角色完成传送 : C2S_Message
    {
        public override PType Code => PType.C2S_角色完成传送;

        public RoleLocationInfo location;

        public override string GetMessage()
        {
            return location.ToJson();
        }
        public override void SetMesssage(string msg)
        {
            location = msg.ToObject<RoleLocationInfo>();
        }
    }




    public class C2S_位置同步 : C2S_Message
    {
        public override PType Code => PType.C2S_位置同步;

        public long roleID;
        public string sceneName;
        public float x, y, z, a, b, c, d;
        public float animKey;
        public long time;

        public override string GetMessage()
        {
            return $"{roleID}|{sceneName}|{x}|{y}|{z}|{a}|{b}|{c}|{d}|{animKey}|{time}";
        }
        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            roleID = long.Parse(s[0]);
            sceneName = s[1];
            x = float.Parse(s[2]);
            y = float.Parse(s[3]);
            z = float.Parse(s[4]);
            a = float.Parse(s[5]);
            b = float.Parse(s[6]);
            c = float.Parse(s[7]);
            d = float.Parse(s[8]);
            animKey = float.Parse(s[9]);
            time = long.Parse(s[10]);
        }
    }

    public class C2S_聊天室 : C2S_Message
    {
        public override PType Code => PType.C2S_聊天室;

        public long roleID;
        public string msg;
        public override string GetMessage()
        {
            return $"{roleID}|{msg}";
        }

        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            roleID = long.Parse(s[0]);
            this.msg = s[1];
        }
    }

    //--------------------------------------------------------------------------------

    public class S2C_账号登入返回 : S2C_Message
    {
        public override PType Code => PType.S2C_账号登入返回;

        public Account account;
        public Role[] roles;
        public override string GetMessage()
        {
            return $"{Json.ToJson(account)}|{Json.ToJson(roles)}";
        }
        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            account = Json.ToObject<Account>(s[0]);
            roles = Json.ToObject<Role[]>(s[1]);
        }
    }
    public class S2C_账号登出返回 : S2C_Message
    {
        public override PType Code => PType.S2C_账号登出返回;
        public override string GetMessage()
        {
            return "1";
        }
        public override void SetMesssage(string msg)
        {
        }
    }
    public class S2C_顶号 : S2C_Message
    {
        public override PType Code => PType.S2C_顶号;

        public long accountID;
        public override string GetMessage()
        {
            return accountID.ToString();
        }
        public override void SetMesssage(string msg)
        {
            accountID = long.Parse(msg);
        }
    }
    public class S2C_保存模型返回 : S2C_Message
    {
        public override PType Code => PType.S2C_保存模型返回;

        public Role config;
        public override string GetMessage()
        {
            return Json.ToJson(config);
        }
        public override void SetMesssage(string msg)
        {
            config = Json.ToObject<Role>(msg);
        }
    }
    public class S2C_角色开始登入返回 : S2C_Message
    {
        public override PType Code => PType.S2C_角色开始登入返回;

        public RoleLocationInfo location;

        public override string GetMessage()
        {
            return location.ToJson();
        }
        public override void SetMesssage(string msg)
        {
            location = Json.ToObject<RoleLocationInfo>(msg);
        }
    }
    public class S2C_角色完成登入返回 : S2C_Message
    {
        public override PType Code => PType.S2C_角色完成登入返回;

        public List<RoleLocationInfo> syncLocations;

        public override string GetMessage()
        {
            return Json.ToJson(syncLocations);
        }
        public override void SetMesssage(string msg)
        {
            syncLocations = Json.ToObject<List<RoleLocationInfo>>(msg);
        }
    }
    public class S2C_角色登出返回 : S2C_Message
    {
        public override PType Code => PType.S2C_角色登出返回;

        public long roleID;
        public override string GetMessage()
        {
            return roleID.ToString();
        }
        public override void SetMesssage(string msg)
        {
            roleID = long.Parse(msg);
        }
    }
    public class S2C_角色开始传送返回 : S2C_Message
    {
        public override PType Code => PType.S2C_角色开始传送返回;

        public RoleLocationInfo location;
        public override string GetMessage()
        {
            return location.ToJson();
        }
        public override void SetMesssage(string msg)
        {
            location = Json.ToObject<RoleLocationInfo>(msg);

        }
    }
    public class S2C_角色完成传送返回 : S2C_Message
    {
        public override PType Code => PType.S2C_角色完成传送返回;

        public List<RoleLocationInfo> syncLocations;
        public override string GetMessage()
        {
            return Json.ToJson(syncLocations);
        }
        public override void SetMesssage(string msg)
        {
            syncLocations = Json.ToObject<List<RoleLocationInfo>>(msg);
        }
    }



    public class S2C_位置同步返回 : S2C_Message
    {
        public override PType Code => PType.S2C_位置同步返回;

        public long roleID;
        //public string sceneName;
        public float x, y, z, a, b, c, d;
        public float animKey;
        public long serverTime;

        public override string GetMessage()
        {
            return $"{roleID}|{x}|{y}|{z}|{a}|{b}|{c}|{d}|{animKey}|{serverTime}";
        }
        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            if (s.Length != 10)
            {
                Log.Error("同步解析长度不对" + s.Length);
            }
            else
            {
                roleID = long.Parse(s[0]);
                x = float.Parse(s[1]);
                y = float.Parse(s[2]);
                z = float.Parse(s[3]);
                a = float.Parse(s[4]);
                b = float.Parse(s[5]);
                c = float.Parse(s[6]);
                d = float.Parse(s[7]);
                animKey = float.Parse(s[8]);
                serverTime = long.Parse(s[9]);

            }
        }
    }
    public class S2C_角色离线 : S2C_Message
    {
        public override PType Code => PType.S2C_角色离线;

        public long roleID;

        public override string GetMessage()
        {
            return roleID.ToString();
        }

        public override void SetMesssage(string msg)
        {
            roleID = long.Parse(msg);
        }
    }
    public class S2C_聊天室 : S2C_Message
    {
        public override PType Code => PType.S2C_聊天室;

        public long roleID;
        public string roleName;
        public string msg;

        public override string GetMessage()
        {
            return $"{roleID}|{roleName.Replace("|", "")}|{msg.Replace("|", "")}";
        }

        public override void SetMesssage(string msg)
        {
            var s = msg.Split('|');
            this.roleID = long.Parse(s[0]);
            this.roleName = s[1];
            this.msg = s[2];
        }
    }




    public static class ByteHelp
    {
        public static string RemoveCode(this byte[] bytes)
        {
            var s = Encoding.UTF8.GetString(bytes, 1, bytes.Length - 1);
            return s;
        }
        public static byte[] AddCode(this string message, PType code)
        {
            var output = new List<byte>();
            output.Add((byte)code);
            output.AddRange(Encoding.UTF8.GetBytes(message));
            return output.ToArray();
        }
    }

}
