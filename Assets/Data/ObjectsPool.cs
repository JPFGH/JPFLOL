using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//解决内存碎片问题
public enum ObjectsPoolType//对象池类型
{
    //数字UI2D对象
    NumShow_Prefab,
    //场景3D对象
    Hook_Prefab,
    TowerMissile_Prefab,
    HBSS_Arrow_Prefab,
    HBSS_BigArrow_Prefab,
    PCNJ_Bullet_Prefab,
    SJLR_Bullet_Prefab,


    PoolType_End,
}



public class ObjectsPool : MonoBehaviour
{
    public static ObjectsPool Instance;

    public Vector3 initPosition3 ;//所有3D对象池对象初始位置
    public Vector2 initPosition2;//所有2D对象池对象初始位置

    Dictionary<ObjectsPoolType, Queue<GameObject>> AllPools;//所有对象池字典
    Dictionary<ObjectsPoolType, GameObject> AllPrefabs;//所有预制体字典

    public Transform AllNumShowTran;//所有数值显示的父节点（画布）


    //2D预制体
    public GameObject NumShow_Prefab;//数值显示
    //3D预制体
    public GameObject Hook_Prefab;//钩子
    public GameObject TowerMissile_Prefab;//防御塔子弹
    public GameObject HBSS_Arrow_Prefab;//寒冰射手普通箭矢
    public GameObject HBSS_BigArrow_Prefab;//寒冰射手大招箭矢
    public GameObject PCNJ_Bullet_Prefab;//皮城女警子弹
    public GameObject SJLR_Bullet_Prefab;//赏金猎人子弹



    void AddPrefabs()//将所有预制体加入字典
    {
        AllPrefabs.Add(ObjectsPoolType.NumShow_Prefab, NumShow_Prefab);
        AllPrefabs.Add(ObjectsPoolType.Hook_Prefab, Hook_Prefab);
        AllPrefabs.Add(ObjectsPoolType.TowerMissile_Prefab, TowerMissile_Prefab);
        AllPrefabs.Add(ObjectsPoolType.HBSS_Arrow_Prefab, HBSS_Arrow_Prefab);
        AllPrefabs.Add(ObjectsPoolType.HBSS_BigArrow_Prefab, HBSS_BigArrow_Prefab);
        AllPrefabs.Add(ObjectsPoolType.PCNJ_Bullet_Prefab, PCNJ_Bullet_Prefab);
        AllPrefabs.Add(ObjectsPoolType.SJLR_Bullet_Prefab, SJLR_Bullet_Prefab);
    }


    void Awake()
    {
        Instance = this;
        initPosition3 = new Vector3(50, -50, 50);
        initPosition2 = GameObject.Find("UIRoot").transform.position + new Vector3(3000, 3000, 0);

        AllPools = new Dictionary<ObjectsPoolType, Queue<GameObject>>();
        AllPrefabs = new Dictionary<ObjectsPoolType, GameObject>();
        AddPools();
        AddPrefabs();


        InitObjectsPool();
    }



    void AddPools()//将所有对象池加到字典
    {
        for (int i=0; i<(int)ObjectsPoolType.PoolType_End; i++)
        {
            AllPools.Add((ObjectsPoolType)i, new Queue<GameObject>());
        }

    }

    /// <summary>
    /// 初始化所有类型对象池，放到指定位置
    /// </summary>
    void InitObjectsPool()
    {
        foreach (var pair in AllPools)
        {
            //2D对象池初始化
            if(pair.Key== ObjectsPoolType.NumShow_Prefab)
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject obj = Instantiate(NumShow_Prefab, AllNumShowTran);//适配要求
                    obj.transform.localPosition = initPosition2;
                    obj.SetActive(false);
                    pair.Value.Enqueue(obj);
                }
            }
            else//3D对象池初始化
            {
                for (int i = 0; i < 20; i++)
                {
                    GameObject obj = Instantiate(AllPrefabs[pair.Key], initPosition3,Quaternion.identity);
                    obj.transform.SetParent(transform);
                    obj.transform.localPosition = new Vector3(0, 0, 0);
                    obj.SetActive(false);
                    pair.Value.Enqueue(obj);
                }
            }
        }
    }

    
    /// <summary>
    /// 出池，检测数量，激活，需外部初始化
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject DequeueInstance(ObjectsPoolType type)
    {
        //Queue<GameObject> operateQueue = AllPools[type];
        if(AllPools[type].Count>0)//池中有东西
        {
            GameObject obj = AllPools[type].Dequeue();//出队
            obj.SetActive(true);
            if(type!=ObjectsPoolType.NumShow_Prefab)
            {
                obj.transform.SetParent(null);
            }
            return obj;
        }
        //空池
        if(type== ObjectsPoolType.NumShow_Prefab)
        {
            GameObject obj = Instantiate(NumShow_Prefab, AllNumShowTran);
            obj.transform.localPosition = initPosition2;
            return obj;
        }
        else
        {
            return Instantiate(AllPrefabs[type], initPosition3, Quaternion.identity);
        }

    }


    /// <summary>
    /// 进池，关闭活性，设置位置
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    public void EnqueueInstance(ObjectsPoolType type,GameObject obj)
    {
        Queue<GameObject> operateQueue = AllPools[type];
        operateQueue.Enqueue(obj);
        obj.SetActive(false);
        if(type!=ObjectsPoolType.NumShow_Prefab)
        {
            obj.transform.SetParent(gameObject.transform);
        }
        else
        {
            obj.transform.SetParent(AllNumShowTran);
            obj.transform.localPosition = new Vector3(0, 0, 0);
        }
    }




}
