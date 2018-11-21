using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 存储物品的数组类型
/// </summary>
public enum InventoryType
{
    Null,
    Backpack,
    Forge,
    Equip,
}


public class BackPackPanel:MonoBehaviour//控制玩家背包单例的所有数据渲染显示
{//挂载场景中的UI面板

    public SlotUI[] slotPrefabArray;//一页中所有格子预制体数组，每个格子挂载SlotUI脚本

    public Text pageNumText;//将页数Text组件拖过来

    private int pageIndex = 0;//当前页数索引
    private int maxPageIndex = 0;//最大索引


    //拖过来
    public static Text showContentT;


    public static InventoryType chooseInventoryType= InventoryType.Null;//所选的物品属于哪个类型的列表

    public static int chooseSlotIndex = 0;//鼠标点选的物品槽索引号

    public GameObject ChooseIconObj;//选择的方框样式



    public Text moneyT;


    public GameObject useButtonObj;
    public GameObject sellButtonObj;





    void Start()
    {
        slotPrefabArray = GetComponentsInChildren<SlotUI>();
        showContentT = transform.Find("BG/ExplainContent/Content").GetComponent<Text>();



    }
    void Update()
    {
        UpdateAllSlots();//更新所有格子显示

        
        CalculateMaxPage();
        UpdatePageNum();//更新页数显示
        moneyT.text = GameMode.PlayerMoney.ToString();



        ControlTwoButton();



    }









    void UpdateAllSlots()
    {
        //格子索引与背包索引保持一致
        //当前页的所有格子一定有编号，但是这些编号只是背包索引的其中一部分，或部分超出（有锁的位置也有编号）
        //格子当前存的item截取自背包，背包有的地方，相同索引格子也有，背包为空的地方，格子也为空
        for (int i = 0; i < slotPrefabArray.Length; i++)
        {
            slotPrefabArray[i].index = pageIndex * slotPrefabArray.Length + i;//给当前页的所有格子编号(索引)
            if (slotPrefabArray[i].index > BackPack.Instance.StorageItemArray.Length - 1)//索引过线
            {
                slotPrefabArray[i].Lock.gameObject.SetActive(true);//激活锁的图标
                slotPrefabArray[i].isLock = true;

                slotPrefabArray[i].nowItem = null;
                //过线的格子就没有东西显示
            }
            else//正常刷新
            {
                slotPrefabArray[i].Lock.gameObject.SetActive(false);
                slotPrefabArray[i].isLock = false;

                slotPrefabArray[i].SetItem(BackPack.Instance.StorageItemArray[slotPrefabArray[i].index]);
                //没过线格子才有东西去显示
            }
        }

    }
    void CalculateMaxPage()
    {
        int sum = BackPack.Instance.StorageItemArray.Length;//背包格子总数
        int shang = sum / slotPrefabArray.Length;//商
        maxPageIndex = shang;
    }
    void UpdatePageNum()
    {
        pageNumText.text = string.Format("{0} / {1}", pageIndex + 1, maxPageIndex + 1);
    }



    public void OnclickRightButton()//右翻页，加
    {
        if(pageIndex<maxPageIndex)
        {
            ChooseIconObj.SetActive(false);
            showContentT.text = null;
            pageIndex++;
        }
    }
    public void OnclickLeftButton()//左翻页，减
    {
        if (pageIndex>0)
        {
            ChooseIconObj.SetActive(false);
            showContentT.text = null;
            pageIndex--;
        }
    }
    
    /// <summary>
    /// 熔炉到背包
    /// </summary>
    public void FtoB()
    {
        if (chooseInventoryType == InventoryType.Forge && Forge.Instance.StorageItemArray[chooseSlotIndex] != null)
        {
            //遍历背包
            for (int i = 0; i < BackPack.Instance.StorageItemArray.Length; i++)
            {
                if (BackPack.Instance.StorageItemArray[i] == null)//找到了空格子
                {
                    showContentT.text = null;
                    chooseInventoryType = InventoryType.Null;
                    BackPack.Instance.StorageItemArray[i] = Forge.Instance.StorageItemArray[chooseSlotIndex];
                    Forge.Instance.StorageItemArray[chooseSlotIndex] = null;
                    return;
                }
            }
        }
    }
    /// <summary>
    /// 背包到熔炉
    /// </summary>
    public void BtoF()
    {
        if (chooseInventoryType == InventoryType.Backpack && BackPack.Instance.StorageItemArray[chooseSlotIndex] != null)
        {
            //遍历熔炉
            for (int i = 0; i < Forge.Instance.StorageItemArray.Length; i++)
            {
                if (Forge.Instance.StorageItemArray[i] == null)//找到了空格子
                {
                    showContentT.text = null;
                    chooseInventoryType = InventoryType.Null;
                    Forge.Instance.StorageItemArray[i] = BackPack.Instance.StorageItemArray[chooseSlotIndex];
                    BackPack.Instance.StorageItemArray[chooseSlotIndex] = null;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 背包到装备
    /// </summary>
    public void BtoE()
    {

        if (chooseInventoryType == InventoryType.Backpack && BackPack.Instance.StorageItemArray[chooseSlotIndex] != null)
        {
            if (BackPack.Instance.StorageItemArray[chooseSlotIndex].Type != ItemType.Equipment)
            {
                return;
            }

            //遍历装备
            for (int i = 0; i < Equips.Instance.StorageItemArray.Length; i++)
            {
                if (Equips.Instance.StorageItemArray[i] == null)//找到了空格子
                {
                    showContentT.text = null;
                    chooseInventoryType = InventoryType.Null;
                    Equips.Instance.StorageItemArray[i] = BackPack.Instance.StorageItemArray[chooseSlotIndex];
                    BackPack.Instance.StorageItemArray[chooseSlotIndex] = null;

                    //增加属性
                    Equipment equip = Equips.Instance.StorageItemArray[i] as Equipment;
                    Unit playerU = GameObject.FindWithTag("Player").GetComponent<Unit>();
                    playerU.attackNum += equip.AttackNum;
                    playerU.attackSpeed += equip.AttackSpeed;
                    playerU.damageReduct += equip.DamageReduct;
                    playerU.suckPercent += equip.SuckPercent;
                    playerU.violentProbability += equip.ViolentProbability;
                    playerU.violentMul += equip.ViolentMul;
                    playerU.HP_Max += equip.HP;
                    playerU.MP_Max += equip.MP;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 装备到背包
    /// </summary>
    public void EtoB()
    {

        if (chooseInventoryType == InventoryType.Equip && Equips.Instance.StorageItemArray[chooseSlotIndex] != null)
        {
            //遍历背包
            for (int i = 0; i < BackPack.Instance.StorageItemArray.Length; i++)
            {
                if (BackPack.Instance.StorageItemArray[i] == null)//找到了空格子
                {
                    showContentT.text = null;
                    chooseInventoryType = InventoryType.Null;
                    BackPack.Instance.StorageItemArray[i] = Equips.Instance.StorageItemArray[chooseSlotIndex];
                    Equips.Instance.StorageItemArray[chooseSlotIndex] = null;

                    //减少属性
                    Equipment equip = BackPack.Instance.StorageItemArray[i] as Equipment;
                    Unit playerU = GameObject.FindWithTag("Player").GetComponent<Unit>();
                    playerU.attackNum -= equip.AttackNum;
                    playerU.attackSpeed -= equip.AttackSpeed;
                    playerU.damageReduct -= equip.DamageReduct;
                    playerU.suckPercent -= equip.SuckPercent;
                    playerU.violentProbability -= equip.ViolentProbability;
                    playerU.violentMul -= equip.ViolentMul;
                    playerU.HP_Max -= equip.HP;
                    if(playerU.HP> playerU.HP_Max)
                    {
                        playerU.HP = playerU.HP_Max;
                    }

                    playerU.MP_Max -= equip.MP;
                    if (playerU.MP > playerU.MP_Max)
                    {
                        playerU.MP = playerU.MP_Max;
                    }
                    return;
                }
            }
        }
    }



    /// <summary>
    /// 控制两个按钮的显示与隐藏，只有消耗品才会出现按钮
    /// </summary>
    public void ControlTwoButton()
    {
        if (chooseInventoryType == InventoryType.Backpack)
        {
            if (BackPack.Instance.StorageItemArray[chooseSlotIndex] != null && BackPack.Instance.StorageItemArray[chooseSlotIndex].Type == ItemType.Consumable)
            {
                useButtonObj.SetActive(true);
            }
            sellButtonObj.SetActive(true);
        }
        else
        {
            useButtonObj.SetActive(false);
            sellButtonObj.SetActive(false);
        }
    }




    /// <summary>
    /// 使用背包里的物品
    /// </summary>
    public void UseItemButton()
    {
        //消耗品生效
        Consumable cons = BackPack.Instance.StorageItemArray[chooseSlotIndex] as Consumable;
        GameObject.FindWithTag("Player").GetComponent<Unit>().RecoverHP(cons.HP);
        GameObject.FindWithTag("Player").GetComponent<Unit>().RecoverMP(cons.MP);
        //能点到按钮说明是背包里的消耗品
        BackPack.Instance.StorageItemArray[chooseSlotIndex].count -= 1;
        if (BackPack.Instance.StorageItemArray[chooseSlotIndex].count == 0)
        {
            BackPack.Instance.StorageItemArray[chooseSlotIndex] = null;

            showContentT.text = null;
            chooseInventoryType = InventoryType.Null;
            chooseSlotIndex = -1;
        }
    }

    /// <summary>
    /// 分解背包里的物品
    /// </summary>
    public void SellItemButton()
    {
        GameMode.PlayerMoney += BackPack.Instance.StorageItemArray[chooseSlotIndex].SellPrice;

        BackPack.Instance.StorageItemArray[chooseSlotIndex].count -= 1;
        if (BackPack.Instance.StorageItemArray[chooseSlotIndex].count == 0)
        {
            BackPack.Instance.StorageItemArray[chooseSlotIndex] = null;

            showContentT.text = null;
            chooseInventoryType = InventoryType.Null;
            chooseSlotIndex = -1;
        }
    }












}
