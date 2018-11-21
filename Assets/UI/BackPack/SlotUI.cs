using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SlotUI : MonoBehaviour,IPointerDownHandler  
{//挂载背包格子槽上
 //根据背包的一个空间，打印一个格子
 

    public Item nowItem = null;

    //下面两项值由各自背包面板控制
    public int index = 0;
    public bool isLock = false;//该格是否上锁

    public Image QualityIcon;//得到拖过来的品质图标Image组件
    public Image icon;//得到拖过来的物体图标Image组件
    public Text uiCount;//得到拖过来的文本组件

    public Image Lock;//得到拖过来的锁图标Image组件

    public GameObject ItemUI;//得到拖过来的旗下拖拽要用的子物体

    //拖过来
    public Text showContentT;

    public GameObject ChooseIconObj;//选择的方框样式

    public void SetItem(Item item)
    {
        nowItem = item;
    }

    private void SetData()
    {
        if(nowItem!=null)
        {
            switch (nowItem.Quality)
            {
                case ItemQuality.White:
                    QualityIcon.color = Color.white;
                    break;
                case ItemQuality.Green:
                    QualityIcon.color = Color.green;
                    break;
                case ItemQuality.Blue:
                    QualityIcon.color = Color.blue;
                    break;
                case ItemQuality.Purple:
                    QualityIcon.color = Color.cyan;//暂时有问题
                    break;
                case ItemQuality.Yellow:
                    QualityIcon.color = Color.yellow;
                    break;
                case ItemQuality.Red:
                    QualityIcon.color = Color.red;
                    break;
                default:
                    break;
            }

            icon.sprite = nowItem.Icon;
            if(nowItem.count==1)//数量为一就不显示字
            {
                uiCount.gameObject.SetActive(false);
            }
            uiCount.text = nowItem.count.ToString();
        }
        
    }


    void Start()
    {
        
    }
    void Update()
    {

        if (nowItem==null)
        {
            QualityIcon.gameObject.SetActive(false); 
            icon.gameObject.SetActive(false);
            uiCount.gameObject.SetActive(false);
        }
        else
        {
            QualityIcon.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            uiCount.gameObject.SetActive(true);
        }


        SetData();



    }


    //IPointerDownHandler---------------------------------
    //鼠标点击A对象，按下鼠标时A对象响应此事件
    //IPointerUpHandler-----------------------------------
    //鼠标点击A对象，抬起鼠标时响应
    //无论鼠标在何处抬起（即不在A对象中）
    //都会在A对象中响应此事件
    //注：响应此事件的前提是A对象必须响应过OnPointerDown事件
    //IPointerClickHandler--------------------------------
    //鼠标点击A对象，抬起鼠标时A对象响应此事件
    //注：按下和抬起时鼠标要处于同一对象上



    //一切都没变，只是物品图标及其子物体在屏幕上位置改变
    //把鼠标状态权限交给BackPackPanel管理
    public void OnPointerDown(PointerEventData eventData)
    {
        

        if (isLock)//点击了锁
        {
            if(GameMode.PlayerMoney>=100)
            {
                GameMode.PlayerMoney -= 100;
                //转移到新数组
                Item[] newArray = new Item[BackPack.Instance.StorageItemArray.Length + 1];
                //newArray.CopyTo(BackPack.Instance.StorageItemArray, 0);
                for (int i = 0; i < BackPack.Instance.StorageItemArray.Length; i++)
                {
                    newArray[i] = BackPack.Instance.StorageItemArray[i];
                }
                BackPack.Instance.StorageItemArray = newArray;
            }
            
        }
        else
        {
            ChooseIconObj.SetActive(true);
            ChooseIconObj.transform.SetParent(transform);
            ChooseIconObj.transform.localPosition = Vector3.zero;
        }

        if (nowItem != null)
        {
            showContentT.text = nowItem.GetToolTipText();//显示说明
            BackPackPanel.chooseInventoryType = InventoryType.Backpack;//表示选中的是背包的物品
            BackPackPanel.chooseSlotIndex = index;
        }
        else
        {

            showContentT.text = null;
            BackPackPanel.chooseInventoryType = InventoryType.Null;//表示选中的是背包的物品
            BackPackPanel.chooseSlotIndex = index;
        }
        




    }












}
