using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forge : Inventory {//锻造临时仓库只有固定格子数

    #region 单例模式-修改了基类的仓库容量
    private static Forge instance;
    public static Forge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Forge();
                instance.StorageItemArray = new Item[2];
            }
            return instance;
        }
    }
    #endregion


    public List<Formula> formulaList = new List<Formula>();//所有配方的列表


    /// <summary>
    /// 每帧刷新合成提示，在熔炉面板调用
    /// </summary>
    public string UpdateTip()
    {
        string str = "";

        for (int j = 0; j < formulaList.Count; j++)//遍历配方表
        {
            //一
            Item need1 = formulaList[j].Item1;//第一个需求
            int index1 = -1;//满足第一个原料条件的槽中物品索引
            for (int m = 0; m < StorageItemArray.Length; m++)
            {
                if (StorageItemArray[m] != null)
                {
                    if (StorageItemArray[m].ID == need1.ID && StorageItemArray[m].count >= need1.count)//满足第一个原料
                    {
                        index1 = m;
                    }
                }
            }
            //二
            Item need2 = formulaList[j].Item2;//第一个需求
            int index2 = -1;//满足第一个原料条件的槽中物品索引
            for (int m = 0; m < StorageItemArray.Length; m++)
            {
                if (m != index1)//排除掉第一个相同的原料！！！！
                {
                    if (StorageItemArray[m] != null)
                    {
                        if (StorageItemArray[m].ID == need2.ID && StorageItemArray[m].count >= need2.count)//满足第二个原料
                        {
                            index2 = m;
                        }
                    }
                }

            }

            if (index1 >= 0 && index2 >= 0)//最终合成
            {
                str = string.Format("可合成{0}，预计花费{1}", formulaList[j].ResultItem.ItemName, formulaList[j].needMoney);
            }

        }

        return str;
    }





    /// <summary>
    /// 对比配方和合成槽，制作并添加到玩家背包
    /// </summary>
    public void ManufactureAndAdd()
    {
        Debug.Log("按下按钮");
        //if (StorageItemArray.Length == 0)//合成槽中没东西就不去进入算法
        //{
        //    Debug.Log("合成槽是空的");
        //    return;
        //}
        for(int i=0;i<BackPack.Instance.StorageItemArray.Length;i++)
        {
            if(BackPack.Instance.StorageItemArray[i]==null)//背包有空格子
            {
                //正式开始匹配配方======================================================================
                BackPackPanel.chooseInventoryType = InventoryType.Null;
                BackPackPanel.chooseSlotIndex = 0;
                BackPackPanel.showContentT.text = null;



                for (int j = 0; j < formulaList.Count; j++)//遍历配方表
                {
                    if (GameMode.PlayerMoney < formulaList[j].needMoney)//金钱不满足
                    {
                        continue;
                    }
                    //一
                    Item need1 = formulaList[j].Item1;//第一个需求
                    int index1 = -1;//满足第一个原料条件的槽中物品索引
                    for(int m=0;m<StorageItemArray.Length;m++)
                    {
                        if(StorageItemArray[m]!=null)
                        {
                            if(StorageItemArray[m].ID==need1.ID&& StorageItemArray[m].count>= need1.count)//满足第一个原料
                            {
                                index1 = m;
                            }
                        }
                    }
                    //二
                    Item need2 = formulaList[j].Item2;//第一个需求
                    int index2 = -1;//满足第一个原料条件的槽中物品索引
                    for (int m = 0; m < StorageItemArray.Length; m++)
                    {
                        if(m!=index1)//排除掉第一个相同的原料！！！！
                        {
                            if (StorageItemArray[m] != null)
                            {
                                if (StorageItemArray[m].ID == need2.ID && StorageItemArray[m].count >= need2.count)//满足第二个原料
                                {
                                    index2 = m;
                                }
                            }
                        }
                        
                    }


                    if(index1>=0&&index2>=0)//最终合成
                    {
                        StorageItemArray[index1].count -= need1.count;//消耗原料一
                        if(StorageItemArray[index1].count==0)
                        {
                            StorageItemArray[index1] = null;
                        }
                        StorageItemArray[index2].count -= need2.count;//消耗原料二
                        if (StorageItemArray[index2].count == 0)
                        {
                            StorageItemArray[index2] = null;
                        }
                        GameMode.PlayerMoney -= formulaList[j].needMoney;//消耗金钱

                        Item res = InventoryManager.Instance.InstantiateItemByID(formulaList[j].ResultItem.ID);
                        res.count = formulaList[j].ResultItem.count;
                        BackPack.Instance.StorageItemArray[i] = res;
                    }
                    









                }
                //======================================================================================
            }
        }

            
    }








}
