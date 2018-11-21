using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;//把LitJson.dll放到Plugins插件文件夹，Plugins文件夹下的代码会预先编译
using System.IO;
using System.Text;

public class InventoryManager//库存管理器：所有物品加载，控制物品的掉落
{

    #region 单例模式
    //预处理器指令
    private static InventoryManager instance;
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InventoryManager();
            }
            return instance;
        }
    }
    #endregion

    private Dictionary<int, Item> AllConfigItemByID;//ID为Key值的总字典
    private Dictionary<string, Item> AllConfigItemByName;//Name为Key值的总字典





    public void Start()//由于该脚本没有继承MonoBehaviour，该方法需要在继承了MonoBehaviour的地方一开始调用，必须是Awake里
    { 
        //加载物品有两种方法:任选一个


        //第一种方法：加载一个有所有物品的Json
        //DecodeJson();

        //第二种方法：加载每种类型物品的的CSV
        AllConfigItemByID = new Dictionary<int, Item>();
        AllConfigItemByName = new Dictionary<string, Item>();
        DecodeCSV("Config/Item",ItemType.Item);//确定有几种文件就写几个方法
        DecodeCSV("Config/Consumable", ItemType.Consumable);
        DecodeCSV("Config/Equipment", ItemType.Equipment);
        //DecodeCSV("Config/Chips", ItemType.Chips);
        DecodeCSV("Config/Material", ItemType.Material);
        //DecodeCSV("Config/Other", ItemType.Other);


        DecodeFormulaCSV("Config/Formula");//读取完物品后读取配方
    }


    #region 解析Json文件，存到字典
    public void DecodeJson()
    {
        //Json只能识别string和int，int只能识别0-9
        AllConfigItemByID = new Dictionary<int, Item>();
        AllConfigItemByName = new Dictionary<string, Item>();

        //方法一：text文件在Unity里是TextAsset类型，把配置表放到Resources文件夹下
        //TextAsset itemText = Resources.Load<TextAsset>("BackPack");
        //string itemJson = itemText.text;//物品信息的Json格式
        //还有后续代码没写
        //方法二，两种方法都要引入LitJson命名空间
        //JsonData jsondata = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Resources/Config/BackPack.json", Encoding.GetEncoding("GB2312")));
        JsonData jsondata = JsonMapper.ToObject(Resources.Load<TextAsset>("Config/BackPack").text);
        for (int i = 0; i < jsondata.Count; i++)
        {
            string typeStr = jsondata[i]["ItemType"].ToString();//部分插件会带引号要把ToString()换成str，这个不带
            ItemType itemType = (ItemType)System.Enum.Parse(typeof(ItemType), typeStr);//将object强转枚举类型

            int id = (int)jsondata[i]["ItemID"];
            string itemName = jsondata[i]["ItemName"].ToString();
            string desc = jsondata[i]["ItemDesc"].ToString();
            //类型提前转化
            ItemQuality qua = (ItemQuality)System.Enum.Parse(typeof(ItemQuality), jsondata[i]["ItemQuality"].ToString());
            int capacity = (int)jsondata[i]["ItemCapacity"];
            int buyPrice = (int)jsondata[i]["ItemBuyPrice"];
            int sellPrice = (int)jsondata[i]["ItemSellPrice"];
            string iconPath = jsondata[i]["ItemIconPath"].ToString();
            Sprite icon= Resources.Load<Sprite>(iconPath);
            string prefabPath = jsondata[i]["ItemPrefabPath"].ToString();
            GameObject prefab= Resources.Load<GameObject>(prefabPath);

            Item item = null;
            switch (itemType)//打完switch连按两次tab，将括号值修改，按回车
            {
                case ItemType.Item:
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                case ItemType.Consumable:
                    int hp = (int)jsondata[i]["HP"];
                    int mp = (int)jsondata[i]["MP"];
                    item = new Consumable(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab, hp, mp);
                    break;
                case ItemType.Equipment:
                    EquipmentType equipType = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), jsondata[i]["EquipmentType"].ToString());
                    int AttackNum = (int)jsondata[i]["AttackNum"];
                    float AttackSpeed = (float)jsondata[i]["AttackSpeed"];
                    float DamageReduct = (float)jsondata[i]["DamageReduct"];
                    float SuckPercent = (float)jsondata[i]["SuckPercent"];
                    int ViolentProbability = (int)jsondata[i]["ViolentProbability"];
                    float ViolentMul = (float)jsondata[i]["ViolentMul"];
                    int HP= (int)jsondata[i]["HP"];
                    int MP = (int)jsondata[i]["MP"];
                    item = new Equipment(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab, equipType, AttackNum, AttackSpeed, DamageReduct, SuckPercent, ViolentProbability, ViolentMul,HP,MP);
                    break;
                case ItemType.Chips:
                    //???
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                case ItemType.Material:
                    //???
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                case ItemType.Other:
                    //???
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                default:
                    return;
            }
            AllConfigItemByID.Add(item.ID, item);
            AllConfigItemByName.Add(item.ItemName, item);
        }
    }
    #endregion

    #region 根据路径和类型解析CSV(逗号分隔值)文件，存到字典
    public void DecodeCSV(string path, ItemType itemType)//路径传表格文件路径（与Json不一样，在Resources文件夹下），类型传枚举类型的值
    {
        //CSV用记事本打开编码格式设置为UTF8格式!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //带VS打开的CSV文件不能有空行，就是前面有行数，里面啥也没有!!!!!!!!!!!!!!!!!!!!!!!!
        TextAsset textAsset = Resources.Load<TextAsset>(path);//读取单个文件为很长很长的一串字符串
        string[] Lines = textAsset.text.Replace("\r\n", "\n").Split(new char[] { '\n' });//得到整个文件的每行字符串数组
        //Replace替换字段 把\r回车替换成\n换行符  再用split方法把换行符用作分割 得到分割后的数组
        //平台不一样是否有\r\n也不一样
        if (Lines.Length == 0)
        {
            Debug.LogError("Config error");//如果没读到东西就报错
        }
        string[] titles = Lines[0].Split(new char[] { ',' });//文件第一行的每个标题信息字符串数组
        for (int n = 1; n < Lines.Length; n++) //跳过第一行的标题遍历每一行（每个物品）的信息
        {
            //遍历到文件中的一行，下面是对这一行的操作
            string[] singles = Lines[n].Split(new char[] { ',' });//文件一行中的每个数据数组
            if (singles.Length != titles.Length)//每行分割的长度若和标题分割后的长度不一致就报错
            {
                Debug.LogError("cofig line error: " + n);
                continue;
            }
            int i = 0;
            //开始逐个解析-----------------------------------
            int id = int.Parse(singles[i++]);
            string itemName = singles[i++];
            string desc = singles[i++];
            //物品类型直接传进来，文件里没有
            ItemQuality qua = (ItemQuality)System.Enum.Parse(typeof(ItemQuality), singles[i++]);
            int capacity = int.Parse(singles[i++]);
            int buyPrice = int.Parse(singles[i++]);
            int sellPrice = int.Parse(singles[i++]);
            Sprite icon = Resources.Load<Sprite>(singles[i++]);
            GameObject prefab = Resources.Load<GameObject>(singles[i++]);
            Item item = null;
            switch (itemType)//打完switch连按两次tab，将括号值修改，按回车
            {
                case ItemType.Item:
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                case ItemType.Consumable:
                    int hp = int.Parse(singles[i++]);
                    int mp = int.Parse(singles[i++]);
                    item = new Consumable(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab, hp, mp);
                    break;
                case ItemType.Equipment:
                    EquipmentType equipType = (EquipmentType)System.Enum.Parse(typeof(EquipmentType), singles[i++]);
                    int AttackNum = int.Parse(singles[i++]);
                    float AttackSpeed = float.Parse(singles[i++]);
                    float DamageReduct = float.Parse(singles[i++]);
                    float SuckPercent = float.Parse(singles[i++]);
                    int ViolentProbability = int.Parse(singles[i++]);
                    float ViolentMul = float.Parse(singles[i++]);
                    int HP = int.Parse(singles[i++]);
                    int MP = int.Parse(singles[i++]);
                    item = new Equipment(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab, equipType, AttackNum, AttackSpeed, DamageReduct, SuckPercent, ViolentProbability, ViolentMul,HP,MP);
                    break;
                case ItemType.Chips:
                    //???
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                case ItemType.Material:
                    //???
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                case ItemType.Other:
                    //???
                    item = new Item(id, itemName, desc, itemType, qua, capacity, buyPrice, sellPrice, icon, prefab);
                    break;
                default:
                    return;
            }
            AllConfigItemByID.Add(item.ID, item);
            AllConfigItemByName.Add(item.ItemName, item);
            //一行解析完成--------------------------------------
        }

    }
    #endregion




    #region 根据路径解析CSV(逗号分隔值)合成配方文件，存到列表
    public void DecodeFormulaCSV(string path)
    {
        //CSV文件表格中一个格子可以为空！！！！但解析出来是""，这个和null是不一样的！！！！！！！！！！！
        //CSV用记事本打开编码格式设置为UTF8格式!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //带VS打开的CSV文件不能有空行，就是前面有行数，里面啥也没有!!!!!!!!!!!!!!!!!!!!!!!!
        TextAsset textAsset = Resources.Load<TextAsset>(path);//读取单个文件为很长很长的一串字符串
        string[] Lines = textAsset.text.Replace("\r\n", "\n").Split(new char[] { '\n' });//得到整个文件的每行字符串数组
        //Replace替换字段 把\r回车替换成\n换行符  再用split方法把换行符用作分割 得到分割后的数组
        //平台不一样是否有\r\n也不一样
        if (Lines.Length == 0)
        {
            Debug.LogError("Config error");//如果没读到东西就报错
        }
        string[] titles = Lines[0].Split(new char[] { ',' });//文件第一行的每个标题信息字符串数组
        for (int n = 1; n < Lines.Length; n++) //跳过第一行的标题遍历每一行（每个物品）的信息
        {
            //遍历到文件中的一行，下面是对这一行的操作
            string[] singles = Lines[n].Split(new char[] { ',' });//文件一行中的每个数据数组
            if (singles.Length != titles.Length)//每行分割的长度若和标题分割后的长度不一致就报错
            {
                Debug.LogError("cofig line error: " + n);
                continue;
            }
            int i = 0;
            //开始逐个解析-----------------------------------
            //约定好，没有的那个格子就是空的
            //一个配方至少要有一个原料
            Item item1;
            if(singles[i]!="")
            {
                item1 = InstantiateItemByName(singles[i++]);
                item1.count = int.Parse(singles[i++]);//数量直接封装到Item对象里
            }
            else
            {
                i += 2;
                item1 = null;
            }
            Item item2;
            if (singles[i] != "")
            {
                item2 = InstantiateItemByName(singles[i++]);
                item2.count = int.Parse(singles[i++]);//数量直接封装到Item对象里
            }
            else
            {
                i += 2;
                item2 = null;
            }
            int needMoney;
            needMoney = int.Parse(singles[i++]);
            Item result;
            if (singles[i] != "")
            {
                result = InstantiateItemByName(singles[i++]);
                result.count = int.Parse(singles[i++]);//数量直接封装到Item对象里
            }
            else
            {
                i += 2;
                result = null;
            }
            Formula formula = new Formula(item1, item2, needMoney, result);
            Forge.Instance.formulaList.Add(formula);//给配方表添加配方
            //Debug.Log("成功添加入一个配方");
            //一行解析完成--------------------------------------
        }
    }
    #endregion







    #region 根据物品ID，实例化复制出需要放在背包的item对象
    public Item InstantiateItemByID(int id)//与解码，数据层联动
    {
        Item mother = AllConfigItemByID[id];
        Item item = null;
        switch (mother.Type)//根据物品类型选择需要重载的构造函数
        {
            case ItemType.Item:
                return new Item(mother);
            case ItemType.Consumable:
                return new Consumable(mother);
            case ItemType.Equipment:
                return new Equipment(mother);
            case ItemType.Chips:
                //???
                return new Item(mother);
            case ItemType.Material:
                //???
                return new Item(mother);
            case ItemType.Other:
                //???
                return new Item(mother);
        }
        return item;
    }
    #endregion

    #region 根据物品Name，实例化复制出需要放在背包的item对象
    public Item InstantiateItemByName(string name)//与解码，数据层联动
    {
        Item mother = AllConfigItemByName[name];
        Item item = null;
        switch (mother.Type)//根据物品类型选择需要重载的构造函数
        {
            case ItemType.Item:
                return new Item(mother);
            case ItemType.Consumable:
                return new Consumable(mother);
            case ItemType.Equipment:
                return new Equipment(mother);
            case ItemType.Chips:
                //???
                return new Item(mother);
            case ItemType.Material:
                //???
                return new Item(mother);
            case ItemType.Other:
                //???
                return new Item(mother);
        }
        return item;
    }
    #endregion



    public Item CreatRandomItem()//随机爆出1个物品（不含0索引物品）
    {
        int num = Random.Range(1, 14);
        Item item = null;
        switch (num)
        {
            case 1:
                item = InstantiateItemByID(20005);
                break;
            case 2:
                item = InstantiateItemByID(20006);
                break;
            case 3:
                item = InstantiateItemByID(20001);
                break;
            case 4:
                item = InstantiateItemByID(20002);
                break;
            case 5:
                item = InstantiateItemByID(30001);
                break;
            case 6:
                item = InstantiateItemByID(30002);
                break;
            case 7:
                item = InstantiateItemByID(30003);
                break;
            case 8:
                item = InstantiateItemByID(30004);
                break;
            case 9:
                item = InstantiateItemByID(30005);
                break;
            case 10:
                item = InstantiateItemByID(30006);
                break;
            case 11:
                item = InstantiateItemByID(30007);
                break;
            case 12:
                item = InstantiateItemByID(50001);
                break;
            case 13:
                item = InstantiateItemByID(50002);
                break;
            case 14:
                item = InstantiateItemByID(50003);
                break;
            case 15:
                item = InstantiateItemByID(50004);
                break;

        }
        item.count = 1;
        return item;
    }



    /// <summary>
    /// 玩家得到某物品，并添加到背包
    /// </summary>
    /// <param name="level"></param>
    public void GetSomething(int level)
    {
        Item item = null;
        switch (level)
        {
            case 1://====================================================================
                int num = Random.Range(1, 3);
                switch (num)
                {
                    case 1:
                        item=InstantiateItemByName("苹果");
                        item.count = 2;
                        break;
                    case 2:
                        item = InstantiateItemByName("薄荷糖");
                        item.count = 2;
                        break;
                    case 3:
                        item = InstantiateItemByName("碎石头");
                        item.count = 2;
                        break;
                    default:
                        break;
                }
                break;
            case 15://====================================================================
                int num2 = Random.Range(1, 8);
                switch (num2)
                {
                    case 1:
                        item = InstantiateItemByName("红瓶药水");
                        item.count = 1;
                        break;
                    case 2:
                        item = InstantiateItemByName("蓝瓶药水");
                        item.count = 1;
                        break;
                    case 3:
                        item = InstantiateItemByName("铁");
                        item.count = 1;
                        break;
                    case 4:
                        item = InstantiateItemByName("铁");
                        item.count = 1;
                        break;
                    case 5:
                        item = InstantiateItemByName("碎石头");
                        item.count = 4;
                        break;
                    case 6:
                        item = InstantiateItemByName("苹果");
                        item.count = 4;
                        break;
                    case 7:
                        item = InstantiateItemByName("薄荷糖");
                        item.count = 4;
                        break;
                    default:
                        break;
                }
                break;
            case 30://====================================================================
                int num3 = Random.Range(1, 9);
                switch (num3)
                {
                    case 1:
                        item = InstantiateItemByName("匕首");
                        item.count = 1;
                        break;
                    case 2:
                        item = InstantiateItemByName("搏击拳套");
                        item.count = 1;
                        break;
                    case 3:
                        item = InstantiateItemByName("布甲");
                        item.count = 1;
                        break;
                    case 4:
                        item = InstantiateItemByName("红玛瑙");
                        item.count = 1;
                        break;
                    case 5:
                        item = InstantiateItemByName("蓝宝石");
                        item.count = 1;
                        break;
                    case 6:
                        item = InstantiateItemByName("铁剑");
                        item.count = 1;
                        break;
                    case 7:
                        item = InstantiateItemByName("吸血之镰");
                        item.count = 1;
                        break;
                    case 8:
                        item = InstantiateItemByName("钻石");
                        item.count = 1;
                        break;
                    default:
                        break;
                }
                break;
            case 99://====================================================================
                item = InstantiateItemByName("防御塔核心");
                item.count = 1;
                break;
            default:
                break;
        }

        BackPack.Instance.AddItemToStorageArray(item);
    }


    

    public void PlayerGetItem()//玩家背包存储随机物品（按键测试用）
    {
        Item item = CreatRandomItem();

        //Debug.Log(item.ItemName + "=============爆出的是这个");
        BackPack.Instance.AddItemToStorageArray(item);
    }








    //public GameObject ItemDrop(ref Item item)//掉落物品到场景
    //{
    //    //百分百掉落
    //    
    //    return item.ItemPrefab;

    //}
    


}
