using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// 技能范围的显示，不一定是真正的伤害区域计算
/// </summary>
public enum SkillArea
{
    Null,
    OuterCircle,
    InnerCircle,
    Sector120,
    Sector60,
    ShortArrow,
    LongArrow,


}


/// <summary>
/// 所有英雄的技能类
/// </summary>
public class Skill  {
    //约定好一个技能对象只能使用指定的方法
    //这里的技能方法只负责具体执行，不管可行性和蓝耗

    public int needMP;
    public float CD;
    public SkillArea viewArea;
    Unit useInfo;//使用人信息
    

    //下面是技能范围参数
    public float R;//外圆半径
    public float r;//内圆半径
    public float wide;//宽度

    public Skill(int needMP,float CD, SkillArea viewArea,Unit useInfo,float R,float r,float wide)
    {
        this.needMP = needMP;
        this.CD = CD;
        this.viewArea = viewArea;
        this.useInfo = useInfo;
        this.R = R;
        this.r = r;
        this.wide = wide;
    }


    //一次调用多次随机生成要使用复制状态






    public void YC_PG(State addState, ObjectsPoolType type)
    {
        GameObject obj= ObjectsPool.Instance.DequeueInstance(type);
        Fly flyInfo = obj.GetComponent<Fly>();
        Vector3 startPos = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f;//发射口位置
        flyInfo.InitializeFly(15,useInfo.attackNum,  useInfo, startPos, useInfo.transform.rotation,addState,true,false);
        GameObject targetObj = ScopeManager.CheckTargetInCircle(useInfo.gameObject, R);
        if(targetObj!=null)
        {
            flyInfo.InitializeFlyTarget(targetObj, R*2);//锁敌，跟踪距离稍长，大约两倍
        }
        else
        {
            flyInfo.InitializeFlyTarget(null, R );//无锁敌，原普攻射程
        }
    }

    public void JZ_PG()
    {

    }



    //寒冰射手技能==================================================================
    public void HBSS_1()
    {
        //强化普攻
        //下三次普攻，每普攻一次，发出三个飞行物
    }
    public void HBSS_2(int bulletNum,float attackNum, State addState)
    {
        Vector3 rayMother = useInfo.transform.forward +(- useInfo.transform.right) * Mathf.Tan(Mathf.PI / 3);//初始母线
        float angle = 120f / bulletNum;
        for (int i = 0; i < bulletNum; i++)
        {
            //绕正前方（圆锥中心）一定角度，然后把初始射线乘以这个旋转，获取到旋转后的射线
            Vector3 dir = Quaternion.AngleAxis(angle * i, useInfo.transform.up) * rayMother;//四元数可以乘以向量，相当于把母线绕transform.forward绕一定角度
            //下面发射子弹
            GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.HBSS_Arrow_Prefab);
            Fly flyInfo = obj.GetComponent<Fly>();
            Vector3 startPos = useInfo.transform.position /*+ useInfo.transform.forward * 0.8f*/ + useInfo.transform.up * 1.4f;//发射口位置
            flyInfo.InitializeFly(12, attackNum, useInfo, startPos, Quaternion.LookRotation(dir), addState, false, false);
            flyInfo.InitializeFlyTarget(null, R);
        }
    }
    public void HBSS_3(Unit useInfo,float attackNum,float aoeAttackNum)
    {
        GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.HBSS_BigArrow_Prefab);
        Fly flyInfo = obj.GetComponent<Fly>();
        Vector3 startPos = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f;//发射口位置
        flyInfo.InitializeFly(15, attackNum, useInfo, startPos, useInfo.transform.rotation, State.CreatVertigo("寒冰大招眩晕",2.5f), false, false);
        flyInfo.InitializeFlyTarget(null, 150);
        flyInfo.something = () =>
          {
              ScopeManager.AOECircle(useInfo, flyInfo.transform.position, 3, aoeAttackNum, null);//坐标是引用
          };
    }

    //皮城女警技能==================================================================
    public void PCNJ_1(float attackNum, State addState)
    {
        //有问题
        ScopeManager.AOECircle(useInfo, SkillAreaManager.Instance.InnerCircleTran.position, r, attackNum, addState);




    }

    

    public void PCNJ_2(float attackNum, State addState)
    {
        GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.PCNJ_Bullet_Prefab);
        Fly flyInfo = obj.GetComponent<Fly>();
        Vector3 startPos = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f;//发射口位置
        flyInfo.InitializeFly(18, attackNum, useInfo, startPos, useInfo.transform.rotation, addState, false, true);

        flyInfo.InitializeFlyTarget(null, R);//无锁敌，原普攻射程
    }

    public void PCNJ_3(int bulletNum, float attackNum, State addState)
    {
        Vector3 rayMother = useInfo.transform.forward + (-useInfo.transform.right) * Mathf.Tan(Mathf.PI / 6);//初始母线
        float angle = 60f / bulletNum;
        for (int i = 0; i < bulletNum; i++)
        {
            //绕正前方（圆锥中心）一定角度，然后把初始射线乘以这个旋转，获取到旋转后的射线
            Vector3 dir = Quaternion.AngleAxis(angle * i, useInfo.transform.up) * rayMother;//四元数可以乘以向量，相当于把母线绕transform.forward绕一定角度
            //下面发射子弹
            GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.PCNJ_Bullet_Prefab);
            Fly flyInfo = obj.GetComponent<Fly>();
            Vector3 startPos = useInfo.transform.position /*+ useInfo.transform.forward * 0.8f*/ + useInfo.transform.up * 1.4f;//发射口位置
            flyInfo.InitializeFly(14, attackNum, useInfo, startPos, Quaternion.LookRotation(dir), addState, false, true);
            flyInfo.InitializeFlyTarget(null, R);
        }
    }


    //赏金猎人技能==================================================================
    public void SJLR_1(float attackNum, State addState)
    {
        GameObject obj1 = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.SJLR_Bullet_Prefab);
        Fly flyInfo1 = obj1.GetComponent<Fly>();
        Vector3 startPos1 = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f + useInfo.transform.right * 0.3f;//发射口位置
        flyInfo1.InitializeFly(14, attackNum, useInfo, startPos1, useInfo.transform.rotation, addState, false, false);
        flyInfo1.InitializeFlyTarget(null, R);//无锁敌，原普攻射程

        GameObject obj2 = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.SJLR_Bullet_Prefab);
        Fly flyInfo2 = obj2.GetComponent<Fly>();
        Vector3 startPos2 = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f - useInfo.transform.right * 0.3f;//发射口位置
        flyInfo2.InitializeFly(14, attackNum, useInfo, startPos2, useInfo.transform.rotation, addState, false, false);
        flyInfo2.InitializeFlyTarget(null, R);//无锁敌，原普攻射程

    }

    //环形伤害附带dot
    public void SJLR_2(int bulletNum, float attackNum, State addState)
    {
        Vector3 rayMother = useInfo.transform.forward + (-useInfo.transform.right) * Mathf.Tan(Mathf.PI / 3);//初始母线
        float angle = 360f / bulletNum;
        for (int i = 0; i < bulletNum; i++)
        {
            //绕正前方（圆锥中心）一定角度，然后把初始射线乘以这个旋转，获取到旋转后的射线
            Vector3 dir = Quaternion.AngleAxis(angle * i, useInfo.transform.up) * rayMother;//四元数可以乘以向量，相当于把母线绕transform.forward绕一定角度
            //下面发射子弹
            GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.SJLR_Bullet_Prefab);
            Fly flyInfo = obj.GetComponent<Fly>();
            Vector3 startPos = useInfo.transform.position /*+ useInfo.transform.forward * 0.8f*/ + useInfo.transform.up * 1.4f;//发射口位置
            flyInfo.InitializeFly(12, attackNum, useInfo, startPos, Quaternion.LookRotation(dir), addState, false, false);
            flyInfo.InitializeFlyTarget(null, R);
        }
    }

    //随机位置子弹
    public void SJLR_3(float attackNum, State addState, float dis,float wide)
    {
        GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.SJLR_Bullet_Prefab);
        Fly flyInfo = obj.GetComponent<Fly>();
        float leftRightOffset = Random.Range(-(wide/2 - 0.2f), wide /2- 0.2f);//左右随机偏移
        Vector3 startPos = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f + useInfo.transform.right * leftRightOffset;//发射口位置
        flyInfo.InitializeFly(15, attackNum, useInfo, startPos, useInfo.transform.rotation, addState, true, false);
        flyInfo.InitializeFlyTarget(null, dis);//无锁敌，射程有限
    }



    //其他技能========================================================================================================
    public void UseSX(Unit useInfo,float dis)
    {
        Debug.Log("根据当前人物正向位移一段距离");
        NavMeshAgent navMesh = useInfo.GetComponent<NavMeshAgent>();
        if (navMesh!=null)
        {
            navMesh.enabled = false;
            useInfo.transform.position += useInfo.transform.forward * dis;
            navMesh.enabled = true;
        }




    }
    public void UseGZ(Unit useInfo,float distance,Quaternion startQua)
    {
        GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.Hook_Prefab);
        Hook hookInfo = obj.GetComponent<Hook>();
        Vector3 startPos = useInfo.transform.position + useInfo.transform.forward * 0.8f + useInfo.transform.up * 1.4f;//出钩位置
        hookInfo.InitializeHook(useInfo.attackNum, useInfo, distance,startPos, startQua);
    }


















}
