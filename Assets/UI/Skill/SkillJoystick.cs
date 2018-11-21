using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SkillJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    Skill nowSkill;//现在的技能摇杆在控制哪个技能的 范围显示
    float remainCDTime = 0;//当前技能CD还剩多少秒，大于0则表示正在CD状态
    Image thisImage;
    Image cdImage;//子物体用来控制CD状态显示的
    Text cdText;//子物体用来控制秒数显示
    GameObject lockObj;//被眩晕的锁

    Color startColor;
    
    float UIOuterCircleRadius ;//UI技能圈半径
    Transform CaculatePosTran;


    Vector2 touchOffset;//UI上触摸点相对于外圆中心位置的偏移向量
    float lastFrameRad;//上一帧的摇杆转的弧度0到2PI


    public Quaternion skillAreaQua;//当前指示器技能朝向（平行于地面）


    public RectTransform UIOuterCircleTran;      //得到外圆Transform组件================要拖
    Transform UIInnerCircleTrans;//得到外圆子物体，技能内圆Transform组件

    public RectTransform CancelAreaTran;            //取消技能图标================要拖


    //Action委托类型，位于System下，Action委托只能指向没有返回类型的方法
    public Action<float,float,float> onJoystickDownEvent;       // 按下事件
    public Action onJoystickUpEvent;                            // 抬起事件
    public Action onJoystickMoveEvent;                          // 滑动事件



    bool confirmCanUse = true;

    void Start()
    {

        CaculatePosTran = UIOuterCircleTran.Find("CaculatePos");
        UIOuterCircleRadius = Vector3.Distance(UIOuterCircleTran.position,CaculatePosTran.position);


        startColor = GetComponent<Image>().color;
        UIInnerCircleTrans = UIOuterCircleTran.GetChild(0);//得到排第一个的子物体
        thisImage = GetComponent<Image>();
        cdImage = transform.Find("CDImage").GetComponent<Image>();
        cdText = GetComponentInChildren<Text>();//得到子物体组件也会得到自身！！！要小心

        lockObj = transform.Find("Lock").gameObject;

        //初始技能图片和CD图片要同时给
        InitIcon();




        
    }

    void Update()
    {
        AddDelegateEventBySkill();
        JudgeSkillMPAndCD();

    }

    /// <summary>
    /// 初始化技能图片
    /// </summary>
    public void InitIcon()
    {
        if(PlayerSettingData.Instance.PlayerChooseHero!=HeroType.NULL)
        {
            if (gameObject.name == "S1")
            {
                thisImage.sprite = PlayerSettingData.Instance.heroUIDic[PlayerSettingData.Instance.PlayerChooseHero].s1Icon;
                cdImage.sprite = PlayerSettingData.Instance.heroUIDic[PlayerSettingData.Instance.PlayerChooseHero].s1Icon;
            }
            if (gameObject.name == "S2")
            {
                thisImage.sprite = PlayerSettingData.Instance.heroUIDic[PlayerSettingData.Instance.PlayerChooseHero].s2Icon;
                cdImage.sprite = PlayerSettingData.Instance.heroUIDic[PlayerSettingData.Instance.PlayerChooseHero].s2Icon;
            }
            if (gameObject.name == "S3")
            {
                thisImage.sprite = PlayerSettingData.Instance.heroUIDic[PlayerSettingData.Instance.PlayerChooseHero].s3Icon;
                cdImage.sprite = PlayerSettingData.Instance.heroUIDic[PlayerSettingData.Instance.PlayerChooseHero].s3Icon;
            }
        }
        
    }
















    #region 根据当前挂载脚本的那个技能摇杆添加委托事件
    void AddDelegateEventBySkill()
    {

        onJoystickDownEvent = null;
        onJoystickUpEvent = null;
        onJoystickMoveEvent = null;


        GameObject playerObj = GameObject.FindWithTag("Player");

        if(playerObj!=null)
        {
            string str = gameObject.name;
            switch (str)//这段代码是不变的
            {
                case "S1":
                    nowSkill = playerObj.GetComponent<Unit>().S1;
                    remainCDTime = playerObj.GetComponent<Unit>().remainS1CDTime;
                    break;
                case "S2":
                    nowSkill = playerObj.GetComponent<Unit>().S2;
                    remainCDTime = playerObj.GetComponent<Unit>().remainS2CDTime;
                    break;
                case "S3":
                    nowSkill = playerObj.GetComponent<Unit>().S3;
                    remainCDTime = playerObj.GetComponent<Unit>().remainS3CDTime;
                    break;
                case "SX"://闪现
                    nowSkill = playerObj.GetComponent<Unit>().SX;
                    remainCDTime = playerObj.GetComponent<Unit>().remainSXCDTime;
                    break;
                case "GZ"://钩子
                    nowSkill = playerObj.GetComponent<Unit>().GZ;
                    remainCDTime = playerObj.GetComponent<Unit>().remainGZCDTime;
                    break;
            }



            switch (nowSkill.viewArea)
            {
                case SkillArea.Null:
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.identity;
                    };
                    break;
                case SkillArea.OuterCircle://只有外圆
                    onJoystickDownEvent += SkillAreaManager.Instance.DrawOuterCircleOnly;
                    onJoystickUpEvent += SkillAreaManager.Instance.HideOuterCircleOnly;
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.identity;
                    };
                    break;
                case SkillArea.InnerCircle://内外圆
                    onJoystickDownEvent += SkillAreaManager.Instance.DrawInnerCircle;
                    onJoystickUpEvent += SkillAreaManager.Instance.HideInnerCircle;
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.LookRotation(SkillAreaManager.Instance.InnerCircleTran.position - SkillAreaManager.Instance.OuterCircleTran.position);
                    };
                    onJoystickMoveEvent = () =>
                    {
                        float R = SkillAreaManager.Instance.OuterCircleTran.localScale.x / 4;
                        //1980是60
                        SkillAreaManager.Instance.InnerCircleTran.localPosition = new Vector3(touchOffset.x / (60 * ((float)Screen.height / 1080)) * R, 0, touchOffset.y / (60 * ((float)Screen.height / 1080)) * R);
                        Debug.Log(touchOffset);
                    };
                    break;
                case SkillArea.Sector120://扇形120度===================================================
                    onJoystickDownEvent += SkillAreaManager.Instance.DrawSector120;
                    onJoystickDownEvent += (float a, float b, float c) =>
                    {
                        float angle = Offset0To2Pi();
                        SkillAreaManager.Instance.Sector120Tran.RotateAround(playerObj.GetComponent<Transform>().position, Vector3.up, angle * Mathf.Rad2Deg);
                    };
                    onJoystickUpEvent += SkillAreaManager.Instance.HideSector120;
                    onJoystickUpEvent += () =>//追加重置Sector120Tran的旋转
                    {
                        SkillAreaManager.Instance.Sector120Tran.eulerAngles = new Vector3(90, 0, 0);
                    };
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.LookRotation(SkillAreaManager.Instance.Sector120Tran.position - SkillAreaManager.Instance.OuterCircleTran.position);
                    };
                    onJoystickMoveEvent = () =>
                    {
                        TestFunc(SkillAreaManager.Instance.Sector120Tran);
                    };
                    break;
                case SkillArea.Sector60://扇形60度======================================================
                    onJoystickDownEvent += SkillAreaManager.Instance.DrawSector60;
                    onJoystickDownEvent += (float a, float b, float c) =>
                    {
                        float angle = Offset0To2Pi();
                        SkillAreaManager.Instance.Sector60Tran.RotateAround(playerObj.GetComponent<Transform>().position, Vector3.up, angle * Mathf.Rad2Deg);
                    };
                    onJoystickUpEvent += SkillAreaManager.Instance.HideSector60;
                    onJoystickUpEvent += () =>
                    {
                        SkillAreaManager.Instance.Sector60Tran.eulerAngles = new Vector3(90, 0, 0);
                    };
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.LookRotation(SkillAreaManager.Instance.Sector60Tran.position - SkillAreaManager.Instance.OuterCircleTran.position);
                    };
                    onJoystickMoveEvent = () =>
                    {
                        TestFunc(SkillAreaManager.Instance.Sector60Tran);
                    };
                    break;
                case SkillArea.ShortArrow://有限短箭头======================================================
                    onJoystickDownEvent += SkillAreaManager.Instance.DrawShortArrow;
                    onJoystickDownEvent += (float a, float b, float c) =>
                    {
                        float angle = Offset0To2Pi();
                        SkillAreaManager.Instance.ShortArrowTran.RotateAround(playerObj.GetComponent<Transform>().position, Vector3.up, angle * Mathf.Rad2Deg);
                    };
                    onJoystickUpEvent += SkillAreaManager.Instance.HideShortArrow;
                    onJoystickUpEvent += () =>
                    {
                        SkillAreaManager.Instance.ShortArrowTran.eulerAngles = new Vector3(90, 0, 0);
                    };
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.LookRotation(SkillAreaManager.Instance.ShortArrowTran.position - SkillAreaManager.Instance.OuterCircleTran.position);
                    };
                    onJoystickMoveEvent = () =>
                    {
                        TestFunc(SkillAreaManager.Instance.ShortArrowTran);
                    };
                    break;
                case SkillArea.LongArrow://无限长箭头======================================================
                    onJoystickDownEvent += SkillAreaManager.Instance.DrawLongArrow;
                    onJoystickDownEvent += (float a, float b, float c) =>
                    {
                        float angle = Offset0To2Pi();
                        SkillAreaManager.Instance.LongArrowTran.RotateAround(playerObj.GetComponent<Transform>().position, Vector3.up, angle * Mathf.Rad2Deg);
                    };
                    onJoystickUpEvent += SkillAreaManager.Instance.HideLongArrow;
                    onJoystickUpEvent += () =>
                    {
                        SkillAreaManager.Instance.LongArrowTran.eulerAngles = new Vector3(90, 0, 0);
                    };
                    onJoystickUpEvent += () =>
                    {
                        skillAreaQua = Quaternion.LookRotation(SkillAreaManager.Instance.LongArrowTran.position - SkillAreaManager.Instance.OuterCircleTran.position);
                    };
                    onJoystickMoveEvent = () =>
                    {
                        TestFunc(SkillAreaManager.Instance.LongArrowTran);
                    };
                    break;
                default:
                    break;
            }

            if (confirmCanUse)//处于可使用技能状态
            {
                switch (str)//转向的委托事件必须在人物想使用技能函数之前
                {
                    case "S1":
                        onJoystickUpEvent += playerObj.GetComponent<Unit>().Skill1;
                        break;
                    case "S2":
                        onJoystickUpEvent += playerObj.GetComponent<Unit>().Skill2;
                        break;
                    case "S3":
                        onJoystickUpEvent += playerObj.GetComponent<Unit>().Skill3;
                        break;
                    case "SX":
                        onJoystickUpEvent += playerObj.GetComponent<Unit>().SkillSX;
                        break;
                    case "GZ":
                        onJoystickUpEvent += playerObj.GetComponent<Unit>().SkillGZ;
                        break;
                }
            }
        }

    }
    #endregion


    #region 根据玩家MP状况和CD状态控制技能图标的显示
    void JudgeSkillMPAndCD()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if(playerObj!=null)
        {
            int playerMp = playerObj.GetComponent<Unit>().MP;
            if (playerMp >= nowSkill.needMP && remainCDTime <= 0&& !playerObj.GetComponent<Unit>().isVertigo)//蓝够且不在CD且不在眩晕
            {
                GetComponent<Image>().raycastTarget = true;
                GetComponent<Image>().color = startColor;
                
                
            }
            else
            {
                Debug.Log(GetComponent<Image>().gameObject.name+"=====");
                GetComponent<Image>().raycastTarget = false;
            }

            if (playerMp < nowSkill.needMP)//蓝不够
            {
                GetComponent<Image>().color = new Color(0.2f, 0.7f, 1);
            }
            if (remainCDTime > 0)//当前技能正在CD中
            {
                cdImage.fillAmount = remainCDTime / nowSkill.CD;//从1变到0

                cdText.gameObject.SetActive(true);
                if (remainCDTime > 1)
                {
                    int integer = (int)remainCDTime;
                    cdText.text = integer.ToString();
                }
                else
                {
                    cdText.text = remainCDTime.ToString("0.#");
                }
            }
            else
            {
                cdText.gameObject.SetActive(false);
            }

            if (playerObj.GetComponent<Unit>().isVertigo)
            {
                lockObj.SetActive(true);
            }
            else
            {
                lockObj.SetActive(false);
            }
        }

        



    }
    #endregion







    #region 辅助方法
    public void TestFunc(Transform tran)
    {
        Vector3 panelPoint = tran.position;//扇形世界坐标
        Vector3 playerPoinr = GameObject.FindWithTag("Player").GetComponent<Transform>().position;//玩家的世界坐标

        panelPoint.y = playerPoinr.y;//保险让y轴影响为0
        Vector3 PlayerTOPanel = (panelPoint - playerPoinr).normalized;//保险归一化

        float DotVaule = Vector3.Dot(PlayerTOPanel, Vector3.right);//与X轴点乘
        float angle2 = Offset0To2Pi()*Mathf.Rad2Deg;
        float angle = Vector3.Angle(PlayerTOPanel,Vector3.forward);
        if (DotVaule > 0)
        {
            tran.RotateAround(playerPoinr, Vector3.up, (angle2-angle));
        }
        else
        {
            tran.RotateAround(playerPoinr, Vector3.up, (angle2 + angle));
        }
    }
    #endregion


    /// <summary>
    /// 按下
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        UIOuterCircleTran.gameObject.SetActive(true);//激活内外圈
        UIOuterCircleTran.position = transform.position;//外圈位置变为技能图标位置
        UIInnerCircleTrans.position = eventData.position;//内圆位置变为鼠标点击位置
        touchOffset = eventData.position - new Vector2(UIOuterCircleTran.position.x, UIOuterCircleTran.position.y);

        CancelAreaTran.gameObject.SetActive(true);//显示取消技能区域

        SkillAreaManager.Instance.AllToNormal();
        confirmCanUse = true;

        if (onJoystickDownEvent != null)
            onJoystickDownEvent(nowSkill.R, nowSkill.r, nowSkill.wide);//实现 按下 委托变量指向的函数
        else
        {
            Debug.Log("函数为空");
        }
    }

    /// <summary>
    /// 抬起
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        UIOuterCircleTran.gameObject.SetActive(false);//关闭内外圈
        UIInnerCircleTrans.localPosition = Vector3.zero;//内圆位置归零

        CancelAreaTran.gameObject.SetActive(false);//隐藏取消技能区域

        if (onJoystickUpEvent != null)
            onJoystickUpEvent();//实现 抬起 委托变量指向的函数
    }

    /// <summary>
    /// 滑动
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        touchOffset = eventData.position - new Vector2(UIOuterCircleTran.position.x, UIOuterCircleTran.position.y) ;//触摸点相对于外圆中心位置的偏移向量
        if (Vector3.Magnitude(touchOffset) < UIOuterCircleRadius)//内圆位置没有越外圈
        {
            UIInnerCircleTrans.position = eventData.position;
        }
        else
        {
            UIInnerCircleTrans.position = new Vector2(UIOuterCircleTran.position.x, UIOuterCircleTran.position.y) + touchOffset.normalized * UIOuterCircleRadius;//越了外圈
            touchOffset = touchOffset.normalized * UIOuterCircleRadius;//将要用到的UI绝对偏移限制住
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(CancelAreaTran, eventData.position))//如果在滑动时在取消技能图标里面
        {
            SkillAreaManager.Instance.AllToRed();
            confirmCanUse = false;
        }
        else
        {
            SkillAreaManager.Instance.AllToNormal();
            confirmCanUse = true;
        }

        if (onJoystickMoveEvent != null)
            onJoystickMoveEvent();//实现 滑动 委托变量指向的函数
    }


    #region 辅助方法：得到两向量的0到2PI的角度
    public float Offset0To2Pi()
    {
        //想得到摇杆偏移向量与UI的Y轴正向的0到360度夹角
        float dotNum = Vector2.Dot(touchOffset, Vector2.up);//点乘
        float cosA = dotNum / touchOffset.magnitude;//返回cos值，这个值在-1到1之间，
        float rad = Mathf.Acos(cosA);       //问题在于这里返回的肯定是0到PI的弧度，即0到180

        float dotNumX = Vector2.Dot(touchOffset, Vector2.right);
        if (dotNumX < 0)//与X轴正向点乘，小于零，表示摇杆转角在180到360之间
        {
            rad = Mathf.PI * 2 - rad;//将弧度变为180到360之间
        }
        //Debug.Log(rad + "++++++++++++++");//到这一步得到了0到360的转角
        return rad;
    }
    #endregion


}
