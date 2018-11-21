using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner;


public enum HeroType
{
    NULL,
    HBSS,
    PCNJ,
    SJLR,
}

public enum UnitType
{
    HBSS_YGDX,
    HBSS_ZJSS,
    HBSS_HAQS,

    PCNJ_HYZAG,
    PCNJ_YJYC,
    PCNJ_BYJJ,
    PCNJ_TXJJS,
    PCNJ_NJJJ,

    SJLR_FGHS,
    SJLR_YCPD,
    SJLR_ND,
    SJLR_TGKH,
    SJLR_SDTGB,

}

/// <summary>
/// 英雄属性参数类别
/// </summary>
public enum AttributeType
{
    //正常数据
    HBSS_Normal,
    PCNJ_Normal,
    SJLR_Normal,
    //可能是怪物的数据
    Small,
    HBSS_Big,
    PCNJ_Big,
    SJLR_Big,
    Boss,

}


public class Attribute
{
    public AttributeType attributeType;
    public int level;
    public int HP_Max;
    public int MP_Max;
    public int attackNum;         //攻击力
    public float damageReduct;    //伤害减免比率
    public float suckPercent;     //吸血比率，上限为100
    public int violentProbability;   //暴击几率指数，上限为100
    public float violentMul;       //暴击倍率
    public float defaultSpeed;    //初始移动速度-动画默认播放速度-----修改速度时不要修改这个

    public Attribute(AttributeType attributeType,int level,
        int HP_Max, int MP_Max, int attackNum, float damageReduct, float suckPercent,
        int violentProbability, float violentMul, float defaultSpeed)
    {
        this.attributeType = attributeType;
        this.level = level;
        this.HP_Max = HP_Max;
        this.MP_Max = MP_Max;
        this.attackNum = attackNum;
        this.damageReduct = damageReduct;
        this.suckPercent = suckPercent;
        this.violentProbability = violentProbability;
        this.violentMul = violentMul;
        this.defaultSpeed = defaultSpeed;
    }

}



/// <summary>
/// 单位管理器，控制玩家敌人标签，给新生成的单位数据
/// </summary>
public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance;

    public int enemyIDNow = 100;//管理器怪物ID当前计数

    public Dictionary<AttributeType, Attribute> attributeDic;//所有单位数据模型字典



    //寒冰射手
    public GameObject HBSS_YGDX_Prefab;
    public GameObject HBSS_ZJSS_Prefab;
    public GameObject HBSS_HAQS_Prefab;
    //皮城女警
    public GameObject PCNJ_HYZAG_Prefab;
    public GameObject PCNJ_YJYC_Prefab;
    public GameObject PCNJ_BYJJ_Prefab;
    public GameObject PCNJ_TXJJS_Prefab;
    public GameObject PCNJ_NJJJ_Prefab;
    //赏金猎人
    public GameObject SJLR_FGHS_Prefab;
    public GameObject SJLR_YCPD_Prefab;
    public GameObject SJLR_ND_Prefab;
    public GameObject SJLR_TGKH_Prefab;
    public GameObject SJLR_SDTGB_Prefab;


    Vector3 playerStartPos = new Vector3(10, 0, 10);//玩家初始位置

    Vector3 enemyStartPos1 = new Vector3(34, 0, 71);
    Vector3 enemyStartPos2 = new Vector3(56, 0, 85);
    Vector3 enemyStartPos3 = new Vector3(54, 0, 70);
    Vector3 enemyStartPos4 = new Vector3(77, 0, 65);
    Vector3 enemyStartPos5 = new Vector3(82, 0, 46);
    Vector3 enemyStartPos6 = new Vector3(67, 0, 33);

    void Awake()
    {
        Instance = this;

    }


    void Start()
    {
        DecodeAttribute();
        //创建玩家
        InitPlayerHero(PlayerSettingData.Instance.PlayerChooseSkin);




        //创建初始敌人

        GameObject obj2 = InitEnemyUnit(UnitType.SJLR_TGKH, enemyStartPos6, AttributeType.Small);
        obj2.AddComponent<EnemyAI>();
        obj2.GetComponent<EnemyAI>().nowAIType = AIType.SJLR;




        nowTime = 0;
        smallTimer = 0;
        normalTimer = 0;
        bigTimer = 0;
        bossTimer = 0;
    }

    float nowTime;//当前时间


    float smallTimer;//小怪刷新计时器
    float normalTimer;//中怪刷新
    float bigTimer;//大怪刷新
    float bossTimer;//Boss刷新


    void Update()
    {
        nowTime += Time.deltaTime;
        smallTimer += Time.deltaTime;
        normalTimer += Time.deltaTime;
        bigTimer += Time.deltaTime;
        bossTimer += Time.deltaTime;

        if(GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>().alive)
        {
            if (nowTime < 90)
            {
                if (smallTimer > 10)
                {
                    CreatSmallEnemy();
                    smallTimer = 0;
                }
            }
            else if (90 <= nowTime && nowTime < 180)
            {
                if (smallTimer > 10)
                {
                    CreatSmallEnemy();
                    smallTimer = 0;
                }
                if (normalTimer > 24)
                {
                    CreatNormalEnemy();
                    normalTimer = 0;
                }
            }
            else if (180 <= nowTime && nowTime < 480)
            {
                if (normalTimer > 10)
                {
                    CreatNormalEnemy();
                    normalTimer = 0;
                }
                if (bigTimer > 24)
                {
                    CreatBigEnemy();
                    bigTimer = 0;
                }
            }
            else if (480 <= nowTime)
            {
                if (normalTimer > 10)
                {
                    CreatNormalEnemy();
                    normalTimer = 0;
                }
                if (bigTimer > 20)
                {
                    CreatBigEnemy();
                    bigTimer = 0;
                }
                if (bossTimer > 30)
                {
                    CreatBoss();
                    bossTimer = 0;
                }
            }

















        }
        


    }


    void CreatSmallEnemy()
    {
        int num = Random.Range(1, 4);
        switch (num)
        {
            case 1:
                GameObject obj = InitEnemyUnit((UnitType)Random.Range((int)UnitType.HBSS_YGDX, (int)UnitType.HBSS_HAQS + 1), RandomEnemyPos(), AttributeType.Small);
                obj.AddComponent<EnemyAI>();
                obj.GetComponent<EnemyAI>().nowAIType = AIType.HBSS;
                break;
            case 2:
                GameObject obj1 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.PCNJ_HYZAG, (int)UnitType.PCNJ_NJJJ + 1), RandomEnemyPos(), AttributeType.Small);
                obj1.AddComponent<EnemyAI>();
                obj1.GetComponent<EnemyAI>().nowAIType = AIType.PCNJ;
                break;
            default:
                GameObject obj2 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.SJLR_FGHS, (int)UnitType.SJLR_SDTGB + 1), RandomEnemyPos(), AttributeType.Small);
                obj2.AddComponent<EnemyAI>();
                obj2.GetComponent<EnemyAI>().nowAIType = AIType.SJLR;
                break;
        }
    }

    void CreatNormalEnemy()
    {
        int num = Random.Range(1, 4);
        switch (num)
        {
            case 1:
                GameObject obj = InitEnemyUnit((UnitType)Random.Range((int)UnitType.HBSS_YGDX, (int)UnitType.HBSS_HAQS+1), RandomEnemyPos(), AttributeType.HBSS_Normal);
                obj.AddComponent<EnemyAI>();
                obj.GetComponent<EnemyAI>().nowAIType = AIType.HBSS;
                break;
            case 2:
                GameObject obj1 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.PCNJ_HYZAG, (int)UnitType.PCNJ_NJJJ + 1), RandomEnemyPos(), AttributeType.PCNJ_Normal);
                obj1.AddComponent<EnemyAI>();
                obj1.GetComponent<EnemyAI>().nowAIType = AIType.PCNJ;
                break;
            default:
                GameObject obj2 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.SJLR_FGHS, (int)UnitType.SJLR_SDTGB + 1), RandomEnemyPos(), AttributeType.SJLR_Normal);
                obj2.AddComponent<EnemyAI>();
                obj2.GetComponent<EnemyAI>().nowAIType = AIType.SJLR;
                break;
        }
    }

    void CreatBigEnemy()
    {
        int num = Random.Range(1, 4);
        switch (num)
        {
            case 1:
                GameObject obj = InitEnemyUnit((UnitType)Random.Range((int)UnitType.HBSS_YGDX, (int)UnitType.HBSS_HAQS + 1), RandomEnemyPos(), AttributeType.HBSS_Big);
                obj.AddComponent<EnemyAI>();
                obj.GetComponent<EnemyAI>().nowAIType = AIType.HBSS;
                break;
            case 2:
                GameObject obj1 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.PCNJ_HYZAG, (int)UnitType.PCNJ_NJJJ + 1), RandomEnemyPos(), AttributeType.PCNJ_Big);
                obj1.AddComponent<EnemyAI>();
                obj1.GetComponent<EnemyAI>().nowAIType = AIType.PCNJ;
                break;
            default:
                GameObject obj2 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.SJLR_FGHS, (int)UnitType.SJLR_SDTGB + 1), RandomEnemyPos(), AttributeType.SJLR_Big);
                obj2.AddComponent<EnemyAI>();
                obj2.GetComponent<EnemyAI>().nowAIType = AIType.SJLR;
                break;
        }
    }

    void CreatBoss()
    {
        int num = Random.Range(1, 4);
        switch (num)
        {
            case 1:
                GameObject obj = InitEnemyUnit((UnitType)Random.Range((int)UnitType.HBSS_YGDX, (int)UnitType.HBSS_HAQS + 1), RandomEnemyPos(), AttributeType.Boss);
                obj.AddComponent<EnemyAI>();
                obj.GetComponent<EnemyAI>().nowAIType = AIType.HBSS;
                break;
            case 2:
                GameObject obj1 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.PCNJ_HYZAG, (int)UnitType.PCNJ_NJJJ + 1), RandomEnemyPos(), AttributeType.Boss);
                obj1.AddComponent<EnemyAI>();
                obj1.GetComponent<EnemyAI>().nowAIType = AIType.PCNJ;
                break;
            default:
                GameObject obj2 = InitEnemyUnit((UnitType)Random.Range((int)UnitType.SJLR_FGHS, (int)UnitType.SJLR_SDTGB + 1), RandomEnemyPos(), AttributeType.Boss);
                obj2.AddComponent<EnemyAI>();
                obj2.GetComponent<EnemyAI>().nowAIType = AIType.SJLR;
                break;
        }
    }




    Vector3 RandomEnemyPos()//随机出生点
    {
        int num = Random.Range(1, 7);
        switch (num)
        {
            case 1:
                return new Vector3(34, 0, 71);
            case 2:
                return new Vector3(56, 0, 85);
            case 3:
                return new Vector3(54, 0, 70);
            case 4:
                return new Vector3(77, 0, 65);
            case 5:
                return new Vector3(82, 0, 46);
            default:
                return new Vector3(67, 0, 33);
        }
    }














    public void DecodeAttribute()
    {
        attributeDic = new Dictionary<AttributeType, Attribute>();

        TextAsset textAsset = Resources.Load<TextAsset>("Config/Attribute");//读取单个文件为很长很长的一串字符串
        string[] Lines = textAsset.text.Replace("\r\n", "\n").Split(new char[] { '\n' });//得到整个文件的每行字符串数组
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
            Attribute attri = null;
            AttributeType attributeType = (AttributeType)System.Enum.Parse(typeof(AttributeType), singles[i++]);
            int level = int.Parse(singles[i++]);
            int HP_Max = int.Parse(singles[i++]);
            int MP_Max = int.Parse(singles[i++]);
            int attackNum = int.Parse(singles[i++]);
            float damageReduct = float.Parse(singles[i++]);
            float suckPercent = float.Parse(singles[i++]);
            int violentProbability = int.Parse(singles[i++]);
            float violentMul = float.Parse(singles[i++]);
            float defaultSpeed = float.Parse(singles[i++]);
            attri = new Attribute(attributeType, level, HP_Max, MP_Max, attackNum, damageReduct, suckPercent, violentProbability, violentMul, defaultSpeed);


            attributeDic.Add(attributeType, attri);
        }

    }
    

    public void InitPlayerHero(UnitType uT)//初始化玩家英雄
    {
        GameObject playerObj = null;
        switch (uT)
        {
            case UnitType.HBSS_YGDX:
                playerObj = Instantiate(HBSS_YGDX_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 1;
                InitAttribute(playerObj, AttributeType.HBSS_Normal);
                break;
            case UnitType.HBSS_ZJSS:
                playerObj = Instantiate(HBSS_ZJSS_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 2;
                InitAttribute(playerObj, AttributeType.HBSS_Normal);
                break;
            case UnitType.HBSS_HAQS:
                playerObj = Instantiate(HBSS_HAQS_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 3;
                InitAttribute(playerObj, AttributeType.HBSS_Normal);
                break;
            case UnitType.PCNJ_HYZAG:
                playerObj = Instantiate(PCNJ_HYZAG_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 11;
                InitAttribute(playerObj, AttributeType.PCNJ_Normal);
                break;
            case UnitType.PCNJ_YJYC:
                playerObj = Instantiate(PCNJ_YJYC_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 12;
                InitAttribute(playerObj, AttributeType.PCNJ_Normal);
                break;
            case UnitType.PCNJ_BYJJ:
                playerObj = Instantiate(PCNJ_BYJJ_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 13;
                InitAttribute(playerObj, AttributeType.PCNJ_Normal);
                break;
            case UnitType.PCNJ_TXJJS:
                playerObj = Instantiate(PCNJ_TXJJS_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 14;
                InitAttribute(playerObj, AttributeType.PCNJ_Normal);
                break;
            case UnitType.PCNJ_NJJJ:
                playerObj = Instantiate(PCNJ_NJJJ_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 15;
                InitAttribute(playerObj, AttributeType.PCNJ_Normal);
                break;
            case UnitType.SJLR_FGHS:
                playerObj = Instantiate(SJLR_FGHS_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 21;
                InitAttribute(playerObj, AttributeType.SJLR_Normal);
                break;
            case UnitType.SJLR_YCPD:
                playerObj = Instantiate(SJLR_YCPD_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 22;
                InitAttribute(playerObj, AttributeType.SJLR_Normal);
                break;
            case UnitType.SJLR_ND:
                playerObj = Instantiate(SJLR_ND_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 23;
                InitAttribute(playerObj, AttributeType.SJLR_Normal);
                break;
            case UnitType.SJLR_TGKH:
                playerObj = Instantiate(SJLR_TGKH_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 24;
                InitAttribute(playerObj, AttributeType.SJLR_Normal);
                break;
            case UnitType.SJLR_SDTGB:
                playerObj = Instantiate(SJLR_SDTGB_Prefab, playerStartPos, Quaternion.identity);
                playerObj.GetComponent<Unit>().ID = 25;
                InitAttribute(playerObj, AttributeType.SJLR_Normal);
                break;
            default:
                break;
        }
        playerObj.tag = "Player";
        playerObj.GetComponent<Unit>().Name = PlayerSettingData.Instance.PlayerName;
        UIManager.Instance.SetUnitStateShow(playerObj);
    }






    public GameObject InitEnemyUnit(UnitType uT, Vector3 enemyPos, AttributeType attri)//实例化敌人单位
    {
        GameObject obj = null;
        switch (uT)
        {
            case UnitType.HBSS_YGDX:
                obj = Instantiate(HBSS_YGDX_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "HBSS_勇敢的心";
                break;
            case UnitType.HBSS_ZJSS:
                obj = Instantiate(HBSS_ZJSS_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "HBSS_紫晶射手";
                break;
            case UnitType.HBSS_HAQS:
                obj = Instantiate(HBSS_HAQS_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "HBSS_黑暗骑士";
                break;
            case UnitType.PCNJ_HYZAG:
                obj = Instantiate(PCNJ_HYZAG_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "PCNJ_荒野治安官";
                break;
            case UnitType.PCNJ_YJYC:
                obj = Instantiate(PCNJ_YJYC_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "PCNJ_玉净夜叉";
                break;
            case UnitType.PCNJ_BYJJ:
                obj = Instantiate(PCNJ_BYJJ_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "PCNJ_冰原狙击";
                break;
            case UnitType.PCNJ_TXJJS:
                obj = Instantiate(PCNJ_TXJJS_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "PCNJ_铁血狙击手";
                break;
            case UnitType.PCNJ_NJJJ:
                obj = Instantiate(PCNJ_NJJJ_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "PCNJ_女警狙击";
                break;
            case UnitType.SJLR_FGHS:
                obj = Instantiate(SJLR_FGHS_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "SJLR_法国皇室";
                break;
            case UnitType.SJLR_YCPD:
                obj = Instantiate(SJLR_YCPD_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "SJLR_泳池派对";
                break;
            case UnitType.SJLR_ND:
                obj = Instantiate(SJLR_ND_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "SJLR_女帝";
                break;
            case UnitType.SJLR_TGKH:
                obj = Instantiate(SJLR_TGKH_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "SJLR_特工狂花";
                break;
            case UnitType.SJLR_SDTGB:
                obj = Instantiate(SJLR_SDTGB_Prefab, enemyPos, Quaternion.identity);
                obj.GetComponent<Unit>().Name = "SJLR_圣诞糖果棒";
                break;
            default:
                break;
        }

        InitAttribute(obj, attri);

        obj.tag = "Enemy";
        obj.GetComponent<Unit>().ID = enemyIDNow;
        enemyIDNow += 1;
        UIManager.Instance.SetUnitStateShow(obj);

        return obj;
    }



    void InitAttribute(GameObject unitObj, AttributeType attri)//初始化单位数据
    {
        Unit unit = unitObj.GetComponent<Unit>();

        unit.attributeType = attributeDic[attri].attributeType;
        unit.level = attributeDic[attri].level;

        unit.HP_Max = attributeDic[attri].HP_Max;
        unit.HP = unit.HP_Max;
        unit.MP_Max = attributeDic[attri].MP_Max;
        unit.MP = unit.MP_Max;

        unit.attackNum = attributeDic[attri].attackNum;
        unit.damageReduct = attributeDic[attri].damageReduct;
        unit.suckPercent = attributeDic[attri].suckPercent;
        unit.violentProbability = attributeDic[attri].violentProbability;
        unit.violentMul = attributeDic[attri].violentMul;
        unit.defaultSpeed = attributeDic[attri].defaultSpeed;



    }










}
