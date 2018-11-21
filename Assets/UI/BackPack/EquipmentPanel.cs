using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour {

    public SlotEquipUI[] slotPrefabArray;//一页中所有格子预制体数组，每个格子挂载SlotUI脚本



    // Use this for initialization
    void Start()
    {
        slotPrefabArray = GetComponentsInChildren<SlotEquipUI>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllSlots();//更新所有格子显示








    }




    void UpdateAllSlots()
    {
        //格子索引与仓库索引保持一致
        for (int i = 0; i < slotPrefabArray.Length; i++)
        {
            slotPrefabArray[i].index = i;//给当前页的仓库所有格子编号(索引)
            slotPrefabArray[i].SetItem(Equips.Instance.StorageItemArray[slotPrefabArray[i].index]);
        }

    }
}
