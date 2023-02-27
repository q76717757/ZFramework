using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class MapNames
    {
        public const string 大厅 = "MainMenu";
        public const string 主场景 = "MainSquare";
        public const string 场景A = "PersonalGallery";
        public const string 场景B = "SFEH";
    }

    public class Account//一个账号
    {
        public long id;
        public string username;
        public string password;
        public string phoneNum;
        public List<long> roles;

        //other.. 其他账号级别的个人信息
    }

    public class Role//账号下的一个角色
    {
        public long id;
        public string roleName;
        public long accountID;//从属账号

        public int modelIndex;
        public int gender;
        //other...  背包列表  好友列表等 以角色为单位

        public Role Clone()
        {
            return new Role()
            {
                 id = id,
                 roleName = roleName,
                 accountID = accountID,
                 modelIndex = modelIndex,
                 gender = gender,
            };
        }

    }

    public class RoleLocationInfo
    {
        public Role role;
        public string mapName;
        public float x, y, z, a, b, c, d;
    }

    //public class ModelConfig
    //{
    //    public int index;
    //    public int sex;
    //    public ModelConfig Clone()
    //    {
    //        return new ModelConfig()
    //        {
    //            index = index,
    //            sex = sex
    //        };
    //    }
    //}

}
