using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 处理用户的输入数据
/// </summary>
public class PlayerInput : MonoBehaviour {

    GameObject playerObj;

    Vector3 playerPos;

    // Use this for initialization
    void Start () {
        playerObj = GameObject.FindWithTag("Player");
        

    }
	
	// Update is called once per frame
	void Update () {
        //控制输入
        playerPos = playerObj.GetComponent<Transform>().position;
        //下面是二维坐标转化为三维坐标的代码
        if (MoveJoystick.moveInput!=Vector2.zero)
        {
            playerObj.GetComponent<Unit>().dest = new Vector3(playerPos.x + MoveJoystick.moveInput.x, 0, playerPos.z + MoveJoystick.moveInput.y);
        }
        else
        {
            playerObj.GetComponent<Unit>().dest = Vector3.zero;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if(h!=0||v!=0)//有键盘输入
        {
            playerObj.GetComponent<Unit>().dest = new Vector3(playerPos.x + h, 0, playerPos.z + v);
        }


        ChangeMainButton();
    }


    public void GetTest()
    {
        InventoryManager.Instance.PlayerGetItem();
    }



    public void ChangeMainButton()
    {
        Collider[] cols = Physics.OverlapSphere(playerPos, 1, LayerMask.GetMask("Tower"));
        bool contactTower = false;//接触塔
        bool contactCollect = false;
        for (int i=0;i< cols.Length;i++)
        {
            Tower t = cols[i].GetComponent<Tower>();
            if(t!=null)
            {
                if (t.isBuild==false)//如果碰到了防御塔且没有被建造
                {
                    for (int j = 0; j < BackPack.Instance.StorageItemArray.Length; j++)
                    {
                        if (BackPack.Instance.StorageItemArray[j] != null)
                        {
                            if (BackPack.Instance.StorageItemArray[j].ItemName == "防御塔核心")
                            {
                                //背包有防御塔核心
                                contactTower = true;
                                PGMainButton.buttonState = PGButtonState.Build;
                                PGMainButton.PGMainButtonIcon.sprite = PGMainButton.dic[PGButtonState.Build];
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (contactTower==false&& contactCollect==false)
        {
            PGMainButton.buttonState = PGButtonState.PG;
            PGMainButton.PGMainButtonIcon.sprite = PGMainButton.dic[PGButtonState.PG];
        }

    }





}
