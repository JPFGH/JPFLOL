using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum PGButtonState//普攻按键当前状态
{
    PG,
    Build,
    Collect,
}




public class PGMainButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {


    public static Quaternion PGQua;//玩家普攻朝向
    public static PGButtonState buttonState = PGButtonState.PG;
    public static Image PGMainButtonIcon;

    public static Dictionary<PGButtonState, Sprite> dic;



    public void OnPointerDown(PointerEventData eventData)//玩家PG按钮按下事件
    {
        SkillAreaManager.Instance.AllToNormal();
        switch (buttonState)
        {
            case PGButtonState.PG:
                PGDownEvent();
                break;
            case PGButtonState.Build:
                for(int i=0;i< BackPack.Instance.StorageItemArray.Length;i++)
                {
                    if (BackPack.Instance.StorageItemArray[i]!=null)
                    {
                        if (BackPack.Instance.StorageItemArray[i].ItemName == "防御塔核心")
                        {
                            BackPack.Instance.StorageItemArray[i] = null;//消耗一个直接没有

                            Collider[] cols = Physics.OverlapSphere(GameObject.FindWithTag("Player").transform.position, 1, LayerMask.GetMask("Tower"));
                            Tower t = cols[0].GetComponent<Tower>();
                            if (t != null)
                            {
                                t.ToBuild();
                            }
                            break;
                        }
                    }
                }
                break;
            case PGButtonState.Collect:
                break;
            default:
                break;
        }
        

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SkillAreaManager.Instance.HideOuterCircleOnly();
        SkillAreaManager.Instance.HideTargetCircle();
    }

    void Start()
    {
        dic = new Dictionary<PGButtonState, Sprite>();
        dic.Add(PGButtonState.PG, Resources.Load<Sprite>("WZRYUI/PGButton"));
        dic.Add(PGButtonState.Build, Resources.Load<Sprite>("WZRYUI/BuildButton"));
        dic.Add(PGButtonState.Collect, Resources.Load<Sprite>("WZRYUI/CollectButton"));


        PGMainButtonIcon = GetComponent<Image>();
    }



    void Update()
    {
        if (GameObject.FindWithTag("Player").GetComponent<Unit>().isVertigo==true)//如果玩家眩晕状态
        {
            //出现红色禁止图标
            GetComponent<Image>().raycastTarget = false;
        }
        else
        {
            GetComponent<Image>().raycastTarget = true;
        }
    }





    void PGDownEvent()
    {
        SkillAreaManager.Instance.DrawOuterCircleOnly(GameObject.FindWithTag("Player").GetComponent<Unit>().PG.R, 0, 0);
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        float PG_R = GameObject.FindWithTag("Player").GetComponent<Unit>().PG.R;

        GameObject targetObj = ScopeManager.CheckTargetInCircle(GameObject.FindWithTag("Player"), PG_R);
        if (targetObj == null)
        {
            PGQua = GameObject.FindWithTag("Player").transform.rotation;
        }
        else
        {
            SkillAreaManager.Instance.DrawTargetCircle(targetObj.transform);
            PGQua = Quaternion.LookRotation(targetObj.transform.position - playerPos);
        }
        //最后去尝试执行Attack函数
        GameObject.FindWithTag("Player").GetComponent<Unit>().Attack();
    }


















}
