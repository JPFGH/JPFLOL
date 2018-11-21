using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Head,
    Eye,
    Neck,
    Shoulder,//肩膀
    Bracer,//护腕
    Hand,
    Ring,
    Trinket,//饰品
    Belt,//腰带
    Leg,//腿
    Boot,//鞋子
}

public class Equipment : Item {

    public EquipmentType EquipType { get; set; }
    public int AttackNum { get; set; }//攻击力
    public float AttackSpeed { get; set; }//攻速
    public float DamageReduct { get; set; }//免伤率
    public float SuckPercent { get; set; }//吸血比率
    public int ViolentProbability { get; set; }//暴击率
    public float ViolentMul { get; set; }//暴击倍率
    public int HP { get; set; }//生命值上限
    public int MP { get; set; }//魔法值上限

    public Equipment(int id, string itemName, string description,
        ItemType itemType, ItemQuality itemQua, int capacity, int buyPrice, int sellPrice,
        Sprite icon, GameObject prefab,
        EquipmentType equipMent, int AttackNum, float AttackSpeed, float DamageReduct, float SuckPercent, int ViolentProbability, float ViolentMul,int HP,int MP)//
        : base(id, itemName, description, itemType, itemQua, capacity, buyPrice, sellPrice, icon, prefab)
    {
        this.EquipType = equipMent;

        this.AttackNum = AttackNum;
        this.AttackSpeed = AttackSpeed;
        this.DamageReduct = DamageReduct;
        this.SuckPercent = SuckPercent;
        this.ViolentProbability = ViolentProbability;
        this.ViolentMul = ViolentMul;
        this.HP = HP;
        this.MP = MP;
    }


    public Equipment(Equipment item) : base(item)
    {
        this.EquipType = item.EquipType;
        this.AttackNum = item.AttackNum;
        this.AttackSpeed = item.AttackSpeed;
        this.DamageReduct = item.DamageReduct;
        this.SuckPercent = item.SuckPercent;
        this.ViolentProbability = item.ViolentProbability;
        this.ViolentMul = item.ViolentMul;
        this.HP = item.HP;
        this.MP = item.MP;
    }
    //上面传子类构造，下面传父类构造
    public Equipment(Item item) : base(item)
    {
        Equipment tempE = item as Equipment;
        this.EquipType = tempE.EquipType;
        this.AttackNum = tempE.AttackNum;
        this.AttackSpeed = tempE.AttackSpeed;
        this.DamageReduct = tempE.DamageReduct;
        this.SuckPercent = tempE.SuckPercent;
        this.ViolentProbability = tempE.ViolentProbability;
        this.ViolentMul = tempE.ViolentMul;
        this.HP = tempE.HP;
        this.MP = tempE.MP;
    }






    public override string GetToolTipText()
    {
        string str = "";
        if(AttackNum!=0)
        {
            str += string.Format("攻击力+{0}\n", AttackNum);
        }
        if(AttackSpeed!=0)
        {
            str += string.Format("攻速+{0}%\n", AttackSpeed*100);
        }
        if(DamageReduct!=0)
        {
            str += string.Format("免伤率+{0}%\n", DamageReduct * 100);
        }
        if (SuckPercent != 0)
        {
            str += string.Format("吸血百分比+{0}%\n", SuckPercent * 100);
        }
        if (ViolentProbability != 0)
        {
            str += string.Format("暴击率+{0}%\n", ViolentProbability);
        }
        if (ViolentMul != 0)
        {
            str += string.Format("暴击伤害+{0}%", ViolentMul * 100);
        }
        if (HP != 0)
        {
            str += string.Format("生命值上限+{0}", HP);
        }
        if (MP != 0)
        {
            str += string.Format("魔法值上限+{0}", MP);
        }

        string text = string.Format("<size=48><color=white>{0}</color></size>\n" +//名字
            "<size=36><color=cyan>{1}</color></size>\n" +//描述
            "<size=36><color=orange>{2}</color></size>\n" +//类型
            "<size=36><color=magenta>{3}</color></size>\n" +//装备增加的效果-----------需要修改---
            "<size=36><color=yellow>分解所得：{4}</color></size>\n"//分解所得
            , ItemName, Description, Type, str, SellPrice);
        return text;
    }







}
