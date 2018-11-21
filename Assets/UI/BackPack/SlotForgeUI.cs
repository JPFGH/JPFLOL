using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotForgeUI : MonoBehaviour,  IPointerDownHandler
{

    public Item nowItem = null;
    public int index = 0;

    public Image QualityIcon;//得到拖过来的品质图标Image组件
    public Image icon;//得到拖过来的物体图标Image组件
    public Text uiCount;//得到拖过来的文本组件

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
        if (nowItem != null)
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
            if (nowItem.count == 1)//数量为一就不显示字
            {
                uiCount.gameObject.SetActive(false);
            }
            uiCount.text = nowItem.count.ToString();
        }

    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nowItem == null )
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














    //这是锻造格子的事件函数
    public void OnPointerDown(PointerEventData eventData)
    {

        ChooseIconObj.SetActive(true);
        ChooseIconObj.transform.SetParent(transform);
        ChooseIconObj.transform.localPosition = Vector3.zero;


        if (nowItem != null)
        {
            showContentT.text = nowItem.GetToolTipText();//显示说明
            BackPackPanel.chooseInventoryType = InventoryType.Forge;//表示选中的是背包的物品
            BackPackPanel.chooseSlotIndex = index;
        }
        else
        {
            showContentT.text = null ;
            BackPackPanel.chooseInventoryType = InventoryType.Forge;//表示选中的是背包的物品
            BackPackPanel.chooseSlotIndex = index;
        }

    }







}
