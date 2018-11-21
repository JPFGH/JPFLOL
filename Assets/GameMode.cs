using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour {


    public static GameMode Instance;
    public GameObject uiRoot;
    public static int PlayerMoney;

    public static float AliveTime;

    /// <summary>
    /// 概率触发,百分之n的概率
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static bool Probability(int n)
    {
        bool happen;
        int num = Random.Range(1, 101);
        if (num <= n)
            happen = true;
        else
            happen = false;
        return
            happen;
    }





    // Use this for initialization
    void Awake() {
        Instance = this;
        PlayerMoney = 800;
        AliveTime = 0;
        InventoryManager.Instance.Start();//加载物品资源

        uiRoot = GameObject.Find("UIRoot");
        
	}

    private void Start()
    {
        uiRoot.transform.Find("SettingPanel").GetComponent<SettingPanel>().InitSettingData();



        Item item1 = InventoryManager.Instance.InstantiateItemByName("狂暴双刃");
        item1.count = 1;
        Item item2 = InventoryManager.Instance.InstantiateItemByName("狂暴双刃");
        item2.count = 1;
        Item item3 = InventoryManager.Instance.InstantiateItemByName("泣血之刃");
        item3.count = 1;
        Item item4 = InventoryManager.Instance.InstantiateItemByName("防御塔核心");
        item4.count = 1;
        Item item5 = InventoryManager.Instance.InstantiateItemByName("闪电匕首");
        item5.count = 1;
        Item item6 = InventoryManager.Instance.InstantiateItemByName("闪电匕首");
        item6.count = 1;
        BackPack.Instance.AddItemToStorageArray(item1);
        BackPack.Instance.AddItemToStorageArray(item2);
        BackPack.Instance.AddItemToStorageArray(item3);
        BackPack.Instance.AddItemToStorageArray(item4);
        BackPack.Instance.AddItemToStorageArray(item5);
        BackPack.Instance.AddItemToStorageArray(item6);
    }





    // Update is called once per frame
    void Update () {
        AliveTime += Time.deltaTime;

    }
}
