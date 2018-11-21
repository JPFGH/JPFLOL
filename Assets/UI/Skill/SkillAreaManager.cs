using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAreaManager:MonoBehaviour
{
    //挂载到所有技能指示器的父节点

    //约定好人物的范围是半径为0.5的圆
    //旗下所有面片由于绕X轴旋转过，只需要控制scale的x，y即可
    //先隐藏所有旗下指示器
    //一张初始面片的长宽是1米，相当于一半是0.5米

    private static SkillAreaManager instance;
    public static SkillAreaManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkillAreaManager>();
            }
            return instance;
        }
    }

    public Transform OuterCircleTran;
    public Transform InnerCircleTran;
    public Transform Sector120Tran;
    public Transform Sector60Tran;
    public Transform ShortArrowTran;
    public Transform LongArrowTran;

    public Transform targetCircleTran;//这个只能拖过来

    void Start()
    {
        OuterCircleTran = transform.Find("OuterCircle").GetComponent<Transform>();//找旗下指定路径
        InnerCircleTran = transform.Find("InnerCircle").GetComponent<Transform>();
        Sector120Tran = transform.Find("Sector120").GetComponent<Transform>();
        Sector60Tran = transform.Find("Sector60").GetComponent<Transform>();
        ShortArrowTran = transform.Find("ShortArrow").GetComponent<Transform>();
        LongArrowTran = transform.Find("LongArrow").GetComponent<Transform>();

    }

    void Update()
    {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        transform.position += new Vector3(0, 0.3f, 0);//将面片上移
    }

    //画指示器和隐藏指示器是成对的
    //约定好填参数时，按照指定的顺序填写

    /// <summary>
    /// 只画外圆
    /// </summary>
    /// <param name="外圆半径"></param>
    public void DrawOuterCircleOnly(float R,float r,float wide)
    {
        OuterCircleTran.gameObject.SetActive(true);
        OuterCircleTran.localScale = new Vector3(2 * R, 2 * R, 1);
    }
    public void HideOuterCircleOnly()
    {
        OuterCircleTran.gameObject.SetActive(false);
    }

    /// <summary>
    /// 内圆，外圆一起画
    /// </summary>
    /// <param name="内圆半径"></param>
    /// <param name="外圆半径"></param>
    public void DrawInnerCircle(float R, float r, float wide)
    {
        OuterCircleTran.gameObject.SetActive(true);
        InnerCircleTran.gameObject.SetActive(true);
        InnerCircleTran.localScale = new Vector3(2 * r, 2 * r, 1);
        OuterCircleTran.localScale = new Vector3(2 * R, 2 * R, 1);
    }
    public void HideInnerCircle()
    {
        OuterCircleTran.gameObject.SetActive(false);
        InnerCircleTran.gameObject.SetActive(false);
    }

    /// <summary>
    /// 画120度扇形
    /// </summary>
    /// <param name="R"></param>
    public void DrawSector120(float R, float r, float wide)
    {
        OuterCircleTran.gameObject.SetActive(true);
        OuterCircleTran.localScale = new Vector3(2 * R, 2 * R, 1);
        Sector120Tran.gameObject.SetActive(true);
        //???
        Sector120Tran.localPosition = new Vector3(0, 0, 2.6f / 10 * 2 * R);
        Sector120Tran.localScale = new Vector3(0.9f * R * 2, 0.9f * R * 2, 1);

    }
    public void HideSector120()
    {
        OuterCircleTran.gameObject.SetActive(false);
        Sector120Tran.gameObject.SetActive(false);
    }

    /// <summary>
    /// 画60度扇形
    /// </summary>
    /// <param name="R"></param>
    public void DrawSector60(float R, float r, float wide)
    {
        OuterCircleTran.gameObject.SetActive(true);
        OuterCircleTran.localScale = new Vector3(2 * R, 2 * R, 1);
        Sector60Tran.gameObject.SetActive(true);
        //???
        Sector60Tran.localPosition = new Vector3(0, 0, 3f / 10 * 2 * R);
        Sector60Tran.localScale = new Vector3(0.585f * R*2, 0.6f * R*2, 1);

    }
    public void HideSector60()
    {
        OuterCircleTran.gameObject.SetActive(false);
        Sector60Tran.gameObject.SetActive(false);
    }

    /// <summary>
    /// 画短箭头
    /// </summary>
    /// <param name="R"></param>
    /// <param name="wide"></param>
    public void DrawShortArrow(float R, float r, float wide)
    {
        OuterCircleTran.gameObject.SetActive(true);
        OuterCircleTran.localScale = new Vector3(2 * R, 2 * R, 1);
        ShortArrowTran.gameObject.SetActive(true);
        //????
        ShortArrowTran.localPosition = new Vector3(0, 0, 0.3f*R);//控制短箭头在半径上的位置
        ShortArrowTran.localScale = new Vector3(wide * 2.3f, 0.6f*R, 0);//控制短箭头的宽度
    }
    public void HideShortArrow()
    {
        OuterCircleTran.gameObject.SetActive(false);
        ShortArrowTran.gameObject.SetActive(false);
    }

    /// <summary>
    /// 画超长箭头
    /// </summary>
    /// <param name="wide"></param>
    public void DrawLongArrow(float R, float r, float wide)//默认宽度为0.5
    {
        LongArrowTran.gameObject.SetActive(true);
        LongArrowTran.localPosition = new Vector3(0, 0, 2);//要写这一句，不然有偏移
        LongArrowTran.localScale = new Vector3(wide * 2.3f, 4, 0);
    }
    public void HideLongArrow()
    {
        LongArrowTran.gameObject.SetActive(false);
    }





    /// <summary>
    /// 画锁定的目标圈
    /// </summary>
    /// <param name="targetPos"></param>
    public void DrawTargetCircle(Transform targetTran)
    {
        if(targetTran!=null)
        {
            targetCircleTran.gameObject.SetActive(true);
            targetCircleTran.SetParent(targetTran);
            targetCircleTran.localPosition = Vector3.zero;
        }
        
    }
    public void HideTargetCircle()
    {
        targetCircleTran.gameObject.SetActive(false);
    }



    /// <summary>
    /// 修改所有技能指示器为红色
    /// </summary>
    public void AllToRed()
    {
        OuterCircleTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor",new Color32(255, 20, 0, 128));
        InnerCircleTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(255, 20, 0, 128));
        Sector120Tran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(255, 20, 0, 128));
        Sector60Tran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(255, 20, 0, 128));
        ShortArrowTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(255, 20, 0, 128));
        LongArrowTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(255, 20, 0, 128));
    }

    /// <summary>
    /// 修改所有技能指示器为正常颜色
    /// </summary>
    public void AllToNormal()
    {
        OuterCircleTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(0, 220, 255, 128));
        InnerCircleTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(0, 220, 255, 128));
        Sector120Tran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(0, 220, 255, 128));
        Sector60Tran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(0, 220, 255, 128));
        ShortArrowTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(0, 220, 255, 128));
        LongArrowTran.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color32(0, 220, 255, 128));
    }




}
