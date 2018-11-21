using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory  {//仓库基类：控制物品数据层面的存放处理
    //控制背包数据即可

    public Item[] StorageItemArray= new Item[8];//仓库已有物品数组，初始容量为



    /// <summary>
    /// 仓库加物品，用一次该方法时数量不能超过单格最大数量
    /// </summary>
    /// <param name="item"></param>
    public void AddItemToStorageArray(Item item)
    {
        if(item.count>item.Capacity)
        {
            Debug.LogError("进仓物品数量超过单格容量");
        }


        for(int i=0;i<StorageItemArray.Length;i++)
        {
            if(StorageItemArray[i]!=null)
            {
                if (StorageItemArray[i].ItemName == item.ItemName)
                {
                    if (StorageItemArray[i].count < item.Capacity)//到这一步，找到相同名称，还有剩余空间的物品格子
                    {
                        int canNum = item.Capacity - StorageItemArray[i].count;//该格子还能存的数量
                        if (item.count <= canNum)//刚好存下
                        {
                            StorageItemArray[i].count += item.count;
                            item.count = 0;
                            return;

                        }
                        else//只能存一部分
                        {
                            StorageItemArray[i].count += canNum;
                            item.count -= canNum;//???
                            Debug.Log("只能存一部分被调用");
                        }
                    }
                }
            }
            
        }

        for(int i = 0; i < StorageItemArray.Length; i++)
        {
            if(StorageItemArray[i]==null)//找到了空格子
            {
                StorageItemArray[i] = item;
                return;
            }
        }
        Debug.LogWarning("没有空的物品槽");
    }



    /// <summary>
    /// 清空存储空间
    /// </summary>
    public void ClearStorageArray()
    {
        for (int i = 0; i < StorageItemArray.Length; i++)
        {
            StorageItemArray[i] = null;
        }
    }



}
