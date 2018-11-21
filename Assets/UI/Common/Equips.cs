using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equips : Inventory {

    #region 单例模式-修改了基类的仓库容量
    private static Equips instance;
    public static Equips Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Equips();
                instance.StorageItemArray = new Item[6];
            }
            return instance;
        }
    }
    #endregion

    
}
