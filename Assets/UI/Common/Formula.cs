using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formula//配方
{
    public Item Item1 { get; set; }//数量直接封装到对象里的count
    public Item Item2 { get; set; }
    public int needMoney;

    public Item ResultItem { get; set; }

    public Formula(Item item1,Item item2,int needMoney, Item resultItem)
    {
        this.Item1 = item1;
        this.Item2 = item2;
        this.needMoney = needMoney;

        this.ResultItem = resultItem;
    }









}
