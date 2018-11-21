using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 钩子类:挂载到实例化的钩子上，钩子上有画线组件
/// </summary>
public class Hook : MonoBehaviour {

    public ObjectsPoolType nowPool;//当前使用了池的类型

    bool isGoFlyNow = true;//当前正在出钩过程，收钩为false
    bool canHook = true;//当前能否钩人

    float alreadyGoFly;//出钩过程已经飞行的距离
    float maxDistance;//钩子最大飞行距离

    float attackNum;               //攻击力

    float goFlySpeed = 14f;//出钩速度
    float backFlySpeed = 20f;//收钩速度
    float leaveTime = 0.5f;//眩晕遗留时间

    string targetTag;//将要碰撞目标的标签
    Unit beHookedUnit = null;//被勾中的倒霉蛋
    Unit fireUnit;//发射人引用


    State vertigoState;//眩晕状态

    LineRenderer attackLine;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (fireUnit == null || fireUnit.alive == false)//发射人不见或发射人死亡
        {
            OverHook();//----------
        }
        attackLine.SetPosition(0, transform.position);//设置世界坐标的第一个点位置为自己
        attackLine.SetPosition(1, fireUnit.transform.position + fireUnit.transform.up * 1.4f);//设置世界坐标的第二个点位置为发射人
        //每帧飞行
        if (isGoFlyNow)//出钩飞行状态
        {
            //向固定方向飞行
            GetComponent<Rigidbody>().velocity = transform.forward * goFlySpeed;
            alreadyGoFly += goFlySpeed * Time.deltaTime;
            if(alreadyGoFly>=maxDistance)//飞到最大距离都没勾到人
            {
                isGoFlyNow = false;//转为收钩飞行
                canHook = false;//也不能钩人了
            }

        }
        else//收钩飞行状态
        {
            //向发射人飞行
            transform.rotation = Quaternion.LookRotation(fireUnit.transform.position + fireUnit.transform.up * 1.4f - transform.position);
            GetComponent<Rigidbody>().velocity = transform.forward * backFlySpeed;
            if (beHookedUnit != null)//钩中人了
            {
                if (beHookedUnit.alive)//倒霉蛋存活
                {
                    float dis = Vector3.Distance(transform.position, fireUnit.transform.position);
                    if (dis < 2f)
                    {
                        //终于勾到发射人身边
                        beHookedUnit.transform.SetParent(null);//松开位置绑定
                        OverHook();
                    }
                }
                else//倒霉蛋死了
                {
                    OverHook();
                }
            }
            else//空钩回来
            {
                float dis = Vector3.Distance(transform.position, fireUnit.transform.position);
                if (dis < 2f)
                {
                    OverHook();
                }
            }
        }




    }

    public void InitializeHook(float hurtNum,Unit fireUnitInfo,float distance,Vector3 startPos,Quaternion startQua)
    {
        fireUnit = fireUnitInfo;//发射人信息
        //目标标签
        if (fireUnitInfo == null || fireUnitInfo.gameObject.tag == "Player")
        {
            targetTag = "Enemy";
        }
        else if (fireUnitInfo.gameObject.tag == "Enemy")
        {
            targetTag = "Player";
        }
        else
        {
            Debug.LogError("发射人信息错误");
        }
        isGoFlyNow = true;
        canHook = true;
        alreadyGoFly = 0;
        maxDistance = distance;
        attackNum = hurtNum;
        beHookedUnit = null;

        transform.position = startPos;
        transform.rotation = startQua;

        attackLine = GetComponent<LineRenderer>();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.gameObject.GetComponent<Unit>();

        if (canHook&& other.tag== targetTag)//还能钩人 且 目标符合===终于勾到人啦！！！！！！！！
        {
            canHook = false;//不再有钩人能力
            isGoFlyNow = false;//进入收钩过程-----------------------
            HurtAndState(other);
            beHookedUnit = otherUnit;//锁定倒霉蛋
            beHookedUnit.transform.SetParent(transform);//位置绑定

            otherUnit.GetComponent<NavMeshAgent>().enabled = false;
            otherUnit.beHooked = true;//让单位处于已被勾中状态
        }
    }

    /// <summary>
    /// 造成钩子伤害和初次眩晕
    /// </summary>
    /// <param name="col"></param>
    void HurtAndState(Collider col)
    {
        Unit colInfo = col.gameObject.GetComponent<Unit>();
        NumShowDir defaultDir = NumShowDir.Right;
        if (fireUnit != null)
        {
            float dif = colInfo.transform.position.x - fireUnit.transform.position.x;//x坐标的差
            if (dif >= 0)//受害者在加害者右边
            {
                defaultDir = NumShowDir.Right;//数值在右侧显示
            }
            if (dif < 0)//受害者在加害者左边
            {
                defaultDir = NumShowDir.Left;//数值在左侧显示
            }
        }
        colInfo.CostHP(attackNum, 0, defaultDir);
        vertigoState = State.CreatVertigo("钩子初始眩晕", 8);
        State.AddState(colInfo, vertigoState);

    }


    /// <summary>
    /// 钩子消失
    /// </summary>
    void OverHook()
    {
        if (beHookedUnit!=null)
        {
            beHookedUnit.GetComponent<NavMeshAgent>().enabled = true;
            beHookedUnit.beHooked = false;
        }
        if(vertigoState!=null)
        {
            vertigoState.remainTime = leaveTime;
        }
        ObjectsPool.Instance.EnqueueInstance(nowPool, gameObject);
    }








}
