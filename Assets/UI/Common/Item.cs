using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ItemType//如果枚举类型定义在内部，则需要类名加枚举类型的名字
{
    Item,
    Consumable,
    Equipment,
    Chips,
    Material,
    Other,
}

public enum ItemQuality//整套代码没有考虑品质问题，默认为白色
{
    White,
    Green,
    Blue,
    Purple,//紫色
    Yellow,
    Red,
}


public class Item//数据层的封装
{
    public int ID { get; set; }
    public string ItemName { get; set; }
    public string Description { get; set; }
    public ItemType Type { get; set; }
    public ItemQuality Quality { get; set; }//品质的贴图是拖拽获得的

    public int Capacity { get; set; }

    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }

    public Sprite Icon { get; set; }
    public GameObject ItemPrefab { get; set; }

    public int count = 0;


    public Item(int id, string itemName, string description,
        ItemType itemType, ItemQuality itemQua, int capacity, int buyPrice, int sellPrice,
        Sprite icon, GameObject prefab)
    {
        this.ID = id;
        this.ItemName = itemName;
        this.Description = description;

        this.Type = itemType;
        this.Quality = itemQua;

        this.Capacity = capacity;
        this.BuyPrice = buyPrice;
        this.SellPrice = sellPrice;

        this.Icon = icon;
        this.ItemPrefab = prefab;
    }

    public Item(Item item)//根据已经实例化的母体完全复制一份
    {
        this.ID = item.ID;
        this.ItemName = item.ItemName;
        this.Description = item.Description;

        this.Type = item.Type;
        this.Quality = item.Quality;

        this.Capacity = item.Capacity;
        this.BuyPrice = item.BuyPrice;
        this.SellPrice = item.SellPrice;

        this.Icon = item.Icon;
        this.ItemPrefab = item.ItemPrefab;
    }



    public virtual string GetToolTipText()
    {
        string text = string.Format("<size=48><color=white>{0}</color></size>\n" +//名字
            "<size=36><color=cyan>{1}</color></size>\n" +//描述
            "<size=36><color=orange>{2}</color></size>\n" +//类型
            "<size=36><color=red>最大容量：{3}</color></size>\n" +//最大容量
            "<size=36><color=yellow>分解所得：{4}</color></size>\n"//分解所得
            , ItemName, Description, Type, Capacity,SellPrice);
        return text;
    }


}
