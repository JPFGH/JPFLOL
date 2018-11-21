using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkinScrollPanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler,IDragHandler
{
    public static SkinScrollPanel Instance;
    //单张皮肤图片的宽度和高度
    public const int Width = 588;
    public const int Height = 300;

    public Transform UIRootTran;
    public GameObject SkinItem_Prefab;//一张皮肤图片预制体
    
    public List<GameObject> nowItemsList;//当前面板的皮肤图片列表
    public List<GameObject> nowModelsList;//当前皮肤所对应的英雄模型列表
    public List<UnitType> nowUnitTypesList;


    


    public Dictionary<UnitType, Skin> dic;


    ScrollRect scrollRect;
    float[] scheduleArray = new float[] { 1, 0.75f, 0.5f, 0.25f, 0 };//元素个数就是皮肤数量（至少有两个皮肤）
    public float nowPosY;//当前进度值（从上到下从1开始）
    public float targetScrollNum = 1;//目标进度值
    public float scrollSmoothing = 6f;
    public bool isDragging = false;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        dic = ChooseHeroUIRoot.Instance.skinsDic;
        scrollRect = GetComponent<ScrollRect>();
        nowItemsList = new List<GameObject>();

        UIRootTran = GameObject.Find("UIRoot").GetComponent<Transform>();

        ChangeSkins(HeroType.HBSS);
        PlayerSettingData.Instance.PlayerChooseSkin = UnitType.HBSS_HAQS;
        ChangeModelAndPoster(0);

        


    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging == false)//不在拖动状态才能缓动到目标点
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition,
            targetScrollNum, Time.deltaTime * scrollSmoothing);
            nowPosY = scrollRect.verticalNormalizedPosition;
        }


        for(int i=0;i<nowItemsList.Count;i++)
        {
            nowItemsList[i].GetComponent<RectTransform>().localScale = 
                new Vector2(1 - Mathf.Abs(nowPosY - scheduleArray[i]), 1 - Mathf.Abs(nowPosY - scheduleArray[i]));
        }



    }


    public void ChangeSkins(HeroType hT)//根据英雄类型 切换皮肤组，模型组，技能图标，
    {
        targetScrollNum = 1;
        nowItemsList.Clear();//清空当前皮肤图片列表
        nowModelsList.Clear();//清空当前英雄模型列表
        nowUnitTypesList.Clear();
        scheduleArray = null;//清空滚动进度数组
        for (int i = 0; i < transform.Find("GridContent").transform.childCount; i++)
        {
            Destroy(transform.Find("GridContent").transform.GetChild(i).gameObject);//销毁所有皮肤预制体
        }

        List<Skin> allDataList = null;
        switch (hT)
        {
            case HeroType.HBSS:
                allDataList = ChooseHeroUIRoot.HBSS_List;
                break;
            case HeroType.PCNJ:
                allDataList = ChooseHeroUIRoot.PCNJ_List;
                break;
            case HeroType.SJLR:
                allDataList = ChooseHeroUIRoot.SJLR_List;
                break;
            default:
                break;
        }
        //修改高度
        transform.Find("GridContent").GetComponent<RectTransform>().sizeDelta = new Vector2(Width, (allDataList.Count + 1) * Height);
        for (int i = 0; i < allDataList.Count; i++)//在GridContent下创建若干个皮肤细胞预制体
        {
            GameObject obj= Instantiate(SkinItem_Prefab, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(transform.Find("GridContent").transform);
            nowItemsList.Add(obj);

            nowItemsList[i].transform.Find("Icon").GetComponent<Image>().sprite = allDataList[i].icon;//主图片
            nowItemsList[i].transform.Find("Icon/TextBG/HeroName").GetComponent<Text>().text = allDataList[i].heroName;
            nowItemsList[i].transform.Find("Icon/TextBG/SkinName").GetComponent<Text>().text = allDataList[i].skinName;
            nowModelsList.Add(allDataList[i].skinModel);//添加模型组

            nowUnitTypesList.Add(allDataList[i].type);

        }
        float num = (float) 1 / (allDataList.Count - 1);
        scheduleArray = new float[allDataList.Count];
        for(int i=0;i< scheduleArray.Length;i++)
        {
            scheduleArray[i] = 1 - num * i;
        }


    }


    public void ChangeModelAndPoster(int i)//改变英雄模型和海报
    {
        if(GameObject.Find("Stage/Hero").transform.childCount>0)
        {
            Destroy(GameObject.Find("Stage/Hero").transform.GetChild(0).gameObject);
        }
        //位置往上面去一点，遮住橘子和手机
        GameObject model = Instantiate(nowModelsList[i], new Vector3(0, 1, 0), Quaternion.identity);
        model.transform.SetParent(GameObject.Find("Stage/Hero").transform);

        PlayerSettingData.Instance.PlayerChooseSkin = nowUnitTypesList[i];

        UIRootTran.Find("Poster").GetComponent<Image>().sprite = dic[nowUnitTypesList[i]].poster;
    }










    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }




    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        nowPosY = scrollRect.verticalNormalizedPosition;//拖拽中的每一帧当前进度

    }




    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        //Vector2 temp = scrollRect.normalizedPosition;//表示当前滚动到哪里0到1的值
        nowPosY = scrollRect.verticalNormalizedPosition;//拖拽结束时，滚动进度
        //float offset=Mathf.Abs(pageArray[])
        float halfNum = Mathf.Abs((scheduleArray[1] - scheduleArray[0]) / 2);//每两页之间的进度值 的一半
        for (int i = 0; i < scheduleArray.Length; i++)
        {
            float offsetTemp = Mathf.Abs(scheduleArray[i] - nowPosY);//遍历每个页数所代表的进度值 与当前值的差
            if (offsetTemp < halfNum)
            {
                if(targetScrollNum!= scheduleArray[i])
                {
                    targetScrollNum = scheduleArray[i];//得到最近的那一个设置为目标皮肤
                    ChangeModelAndPoster(i);
                }
            }
        }



    }

}
