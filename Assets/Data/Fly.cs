using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 飞行物类，挂载到场景中实例化的飞行物上，在生成飞行物的那部分程序时给字段赋值
/// </summary>
public class Fly : MonoBehaviour {

    public ObjectsPoolType nowPool;//当前使用了池的类型
    //这个在做预制体时手动选择

    //碰撞盒的大小由预制体自己控制
    public float flySpeed;//飞行速度

    float alreadyFly;//已经飞行的距离
    public float maxDistance;//最大飞行距离

    public bool isSuckVol;//是否触发暴击吸血

    public bool isPathInjury;//是否是路径伤害,对路径上的敌人造成伤害，到最大距离失效

    public State addState;//飞行物附加的状态

    public Unit fireUnit;//发射人引用
    public string targetTag;//将要碰撞目标的标签
    public GameObject targetObj;//目标物体为null则意为非跟踪类飞行物，有目标物体，则一直跟随目标，且只会与目标发生碰撞

    public Action something;//附加触发事件


    //以下为发射人信息
    public float attackNum;               //攻击力
    public float suckPercent;           //吸血比率，上限为100
    public int violentProbability;      //暴击几率指数，上限为100
    public float violentMul;            //暴击倍率

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {//每帧飞行
        if (targetObj != null && targetObj.GetComponent<Unit>().alive != false)//如果有目标且目标存活
        {
            transform.rotation = Quaternion.LookRotation(targetObj.transform.position + new Vector3(0, 1, 0) - transform.position);//飞行物朝向目标
            GetComponent<Rigidbody>().velocity = transform.forward * flySpeed;
        }
        else if(targetObj != null && targetObj.GetComponent<Unit>().alive == false)//有目标，但目标已死
        {
            ObjectsPool.Instance.EnqueueInstance(nowPool, gameObject);
        }
        else//没目标
        {
            GetComponent<Rigidbody>().velocity = transform.forward * flySpeed;
        }
        alreadyFly += flySpeed * Time.deltaTime;
        if (alreadyFly > maxDistance)
        {
            ObjectsPool.Instance.EnqueueInstance(nowPool, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.gameObject.GetComponent<Unit>();
        if (other.tag == targetTag && otherUnit != null)//目标标签符合
        {
            if (targetObj != null)//锁敌，只会触发锁定的目标
            {
                if (otherUnit.ID == targetObj.GetComponent<Unit>().ID)//通过唯一ID识别锁敌目标
                {
                    HurtAndState(other);
                    ObjectsPool.Instance.EnqueueInstance(nowPool, gameObject);
                }
            }
            else//无锁敌，只要是敌人就触发
            {
                if(isPathInjury)
                {
                    HurtAndState(other);
                }
                else
                {
                    HurtAndState(other);
                    ObjectsPool.Instance.EnqueueInstance(nowPool, gameObject);
                }
            }
        }
    }

    public void InitializeFly(float flySpeed,float hurtNum, Unit fireUnitInfo,Vector3 startPos,Quaternion startQua,State addState,bool isSuckVol, bool isPathInjury)//初始化飞行物数据
    {
        this.flySpeed = flySpeed;

        alreadyFly = 0;//所有数据都要初始化

        attackNum = hurtNum;//伤害不一定是攻击力
        fireUnit = fireUnitInfo;//发射人信息
        if(fireUnitInfo!=null)
        {
            suckPercent = fireUnitInfo.suckPercent;
            violentProbability = fireUnitInfo.violentProbability;
            violentMul = fireUnitInfo.violentMul;
        }
        

        gameObject.transform.position = startPos;
        gameObject.transform.rotation = startQua;

        this.addState = addState;
        this.isSuckVol = isSuckVol;

        this.isPathInjury = isPathInjury;

        if (fireUnitInfo==null||fireUnitInfo.gameObject.tag=="Player")
        {
            targetTag = "Enemy";
        }
        else if(fireUnitInfo.gameObject.tag == "Enemy")
        {
            targetTag = "Player";
        }
        else
        {
            Debug.LogError("发射人信息错误");
        }


        something = null;//清空委托
        //在初始化调用这个方法的下面给委托添加方法

    }
    public void InitializeFlyTarget(GameObject targetObj, float distance)//确认飞行物目标与射程，外部智能识别
    {
        this.targetObj = targetObj;
        maxDistance = distance;
    }






    void HurtAndState(Collider col)//对碰撞体伤害且添加状态
    {
        Unit colInfo = col.gameObject.GetComponent<Unit>();
        NumShowDir defaultDir = NumShowDir.Right;
        if (fireUnit != null)
        {
            float dif = colInfo.transform.position.x - fireUnit.transform.position.x;//x坐标的差
            if(dif>=0)//受害者在加害者右边
            {
                defaultDir = NumShowDir.Right;//数值在右侧显示
            }
            if (dif < 0)//受害者在加害者左边
            {
                defaultDir = NumShowDir.Left;//数值在左侧显示
            }
        }

        if (isSuckVol)//如果能触发吸血和暴击
        {
            int actualHurtNum = colInfo.CostHP(attackNum, violentProbability, defaultDir);
            if(fireUnit!=null)
            {
                fireUnit.RecoverHP((int)(actualHurtNum * suckPercent));//使用人吸血
            }

        }
        else
        {
            colInfo.CostHP(attackNum,0, defaultDir);
        }
        if(addState!=null)
        {
            State.AddState(colInfo, State.CopyState(addState));
        }


        if (something != null)
        {
            something();//实现委托
        }


    }

}
