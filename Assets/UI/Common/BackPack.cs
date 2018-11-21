using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPack : Inventory {
    //玩家背包数据

    #region 单例模式
    private static BackPack instance;
    public static BackPack Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BackPack();
            }
            return instance;
        }
    }
    #endregion


    





}
