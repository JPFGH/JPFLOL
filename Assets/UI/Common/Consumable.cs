using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item
{
    public int HP { get; set; }
    public int MP { get; set; }

    public Consumable(int id, string itemName, string description,
        ItemType itemType, ItemQuality itemQua, int capacity, int buyPrice, int sellPrice,
        Sprite icon, GameObject prefab,
        int hp, int mp)//
        : base(id, itemName, description, itemType, itemQua, capacity, buyPrice, sellPrice, icon, prefab)
    {
        this.HP = hp;
        this.MP = mp;
    }


    public Consumable(Consumable item) : base(item)
    {
        this.HP = item.HP;
        this.MP = item.MP;
    }
    //上面传子类构造，下面传父类构造
    public Consumable(Item item) : base(item)
    {
        Consumable tempC = item as Consumable;
        this.HP = tempC.HP;
        this.MP = tempC.MP;
    }




    public override string GetToolTipText()
    {
        string str = "";
        if(HP!=0)
        {
            str += string.Format("恢复生命值{0}\n", HP);
        }
        if (MP != 0)
        {
            str += string.Format("恢复魔法值{0}", MP);
        }

        string text = string.Format("<size=48><color=white>{0}</color></size>\n" +//名字
            "<size=36><color=cyan>{1}</color></size>\n" +//描述
            "<size=36><color=orange>{2}</color></size>\n" +//类型
            "<size=36><color=red>最大容量：{3}</color></size>\n" +//最大容量
            "<size=36><color=green>{4}</color></size>\n" +//增加的效果-----------需要修改
            "<size=36><color=yellow>分解所得：{5}</color></size>\n"//购买价格出售价格
            , ItemName, Description, Type,Capacity, str,SellPrice);
        return text;
    }

}
