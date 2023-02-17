using System;
using System.Collections.Generic;
using UnityEngine;
using ZFramework;

public sealed class GameManager
{
    public static GameManager Instance { get; } = new GameManager();
    private GameManager() { }

    public Account Account { get; set; }//当前登录账号
    public Role[] roleCaches { get; set; }//角色的缓存 登录时一并给过来
    public RoleLocationInfo Location { get; set; }//角色登入时赋值 当前登录的角色  以及在什么位置


}

public static class EXP//临时写的拓展方法
{
    public static GameObject GetModelPrefab(this Role config)
    {
        string prefabName = config.gender == 0 ? "boy" : "girl";// $"m_{config.modelIndex}" : $"f_{config.modelIndex}";
        return Resources.Load<GameObject>($"Player/{prefabName}");
    }

}