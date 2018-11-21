using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemInScene : MonoBehaviour {//挂载到场景中掉落的物品

    public Item dropItem;

    public void SetData(Item dItem)//掉落时开始记录
    {
        dropItem = dItem;//记录物品
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")//物体碰到玩家
        {
            if (dropItem == null)
            {
                return;
            }
            BackPack.Instance.AddItemToStorageArray(dropItem);
            Destroy(gameObject);
        }
    }








    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
