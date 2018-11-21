using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//引入导航命名空间


/// <summary>
/// 一个寒冰射手就挂一个脚本
/// </summary>
public class HBSS : Unit{
    //单位速度是由组件的Speed统一控制

    
    
    public Animator animat;
    
    int isRunID = Animator.StringToHash("IsRun");
    int toAttackID = Animator.StringToHash("ToAttack");
    int isVertigoID = Animator.StringToHash("IsVertigo");
    int toDeathID = Animator.StringToHash("ToDeath");
    int toSkill2 = Animator.StringToHash("ToSkill2");
    int toSkill3 = Animator.StringToHash("ToSkill3");


    public int S1_remain = 0;


    void Start () {
        agent = GetComponent<NavMeshAgent>();
        animat = GetComponent<Animator>();

        Initialization();



    }
	
	void Update () {
        StateEffect();//所有状态生效并改变相应标志位
        StateToAnimation();// 在状态遍历完成后，根据现有移动速度，和攻速调整动画播放速度，和相关动画
        Move();//移动
    }

    /// <summary>
    /// 初始化人物
    /// </summary>
    public void Initialization()
    {
        //HBSSData点出来挨个赋值
        deathAnimationTime = 1.933f;//死亡动画时长
        agent.speed = defaultSpeed;
        attackSpeed = 1f;

        //创建技能
        PG = new Skill(0,2.133f, SkillArea.OuterCircle,this,8,0,0);//PGCD在具体执行时还需根据攻速计算一遍
        S1 = new Skill(150,8, SkillArea.Null,this,0,0,0);
        S2 = new Skill(150,8, SkillArea.Sector120,this,8,0,0);
        S3 = new Skill(200,12, SkillArea.LongArrow,this,0,0,3);
    }


    /// <summary>
    /// 移动可行性判断，并控制动画播放
    /// </summary>
    void Move()
    {
        if(alive==true&& GetComponent<NavMeshAgent>().enabled == true)
        {
            if (dest != Vector3.zero)//人物想动
            {
                if (isVertigo == true || isForwardRoll == true || isSkill == true)//但不能动
                {
                    agent.SetDestination(transform.position);
                    animat.SetBool(isRunID, false);
                    return;
                }
                //确认能动
                agent.SetDestination(dest);//最终设置目标点
                animat.SetBool(isRunID, true);//切换到跑步动画
            }
            else//人物不想动，想静止
            {
                agent.SetDestination(transform.position);
                animat.SetBool(isRunID, false);
            }
        }
        
    }


    public override void Attack()//人物想攻击
    {
        if(alive==true)
        {
            if (isVertigo == true || isForwardRoll == true || isSkill == true || isPGCD == true)//却不能攻击
            {//眩晕，PG前摇，技能动画，PGCD，无法进行PG动画的触发
                return;
            }
            else//确认能攻击
            {
                agent.SetDestination(transform.position);//打断行走
                animat.SetBool(isRunID, false);
                UnitPGRotate();//普攻转向

                animat.SetTrigger(toAttackID);//触发PG动画播放
                isForwardRoll = true;//开始前摇，到动画帧事件或突然眩晕结束前摇
                State.AddState(this, State.CreatPGCD("寒冰射手PGCD状态", PG.CD / attackSpeed));//算好初始普攻动画时长
            }
        }
    }


    //对于玩家而言，所有技能蓝量不足的情况已经被UI层过滤掉了，但敌人没过滤，这里要写一下
    //对于玩家而言，所有技能在人物眩晕情况已经被UI也过滤掉了，但敌人没过滤，这里要写一下
    public override void Skill1()//人物想使用一技能，抬起了技能摇杆
    {
        if(alive==true)
        {
            //强化箭矢，释放其他技能时可以使用该技能
            if (isVertigo == true || isForwardRoll == true || MP < S1.needMP)
            {
                return;
            }
            else//确认能使用一技能
            {
                CostMP(S1.needMP);
                //强化箭矢不会打断人物行走
                //没有动画，只有粒子特效
                S1_remain = 3;
                State.AddState(this, State.CreatS1CD("寒冰射手S1CD状态", S1.CD));
            }
        }
    }
    public override void Skill2()//人物想使用二技能，抬起了技能摇杆
    {
        if(alive==true)
        {
            if (isVertigo == true || isForwardRoll == true || isSkill == true || MP < S2.needMP)
            {
                return;
            }
            else
            {
                CostMP(S2.needMP);
                agent.SetDestination(transform.position);//打断行走
                animat.SetBool(isRunID, false);

                UnitSkillRotate(UnitToAction.S2);//二技能转向

                animat.SetTrigger(toSkill2);//触发二技能动画播放
                isSkill = true;//开始使用技能状态，到动画帧事件或突然眩晕结束状态
                State.AddState(this, State.CreatS2CD("寒冰射手S2CD状态", S2.CD));
            }
        }
        
        
    }
    public override void Skill3()//人物想使用三技能，抬起了技能摇杆
    {
        if(alive==true)
        {
            if (isVertigo == true || isForwardRoll == true || isSkill == true || MP < S3.needMP)
            {
                return;
            }
            else
            {
                CostMP(S3.needMP);
                agent.SetDestination(transform.position);//打断行走
                animat.SetBool(isRunID, false);

                UnitSkillRotate(UnitToAction.S3);//三技能转向

                animat.SetTrigger(toSkill3);//触发二技能动画播放
                isSkill = true;//开始使用技能状态，到动画帧事件或突然眩晕结束状态
                State.AddState(this, State.CreatS3CD("寒冰射手S3CD状态", S3.CD));
            }
        }
        
    }



    public void StateToAnimation()
    {
        
        if (alive == false&&alreadyDeath==false)
        {
            animat.SetBool(isVertigoID, false);
            animat.SetBool(toDeathID, true);
            alreadyDeath = true;
            animat.speed = 1;
            agent.speed = 0;
        }
        if (alive == true)
        {
            if (isVertigo == true)
                animat.SetBool(isVertigoID, true);
            else
                animat.SetBool(isVertigoID, false);

            if (animat.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                animat.speed = nowSpeed / defaultSpeed;//调整移动动画速度
            else if (animat.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                animat.speed = attackSpeed;//调整攻击动画速度
            else
                animat.speed = 1;//其它动画恢复默认状态


            agent.speed = nowSpeed;//应用当前速度
        }


    }


    public void OnAnimationEvent_HBSSAttack()//动画帧事件触发PG函数
    {
        //Debug.Log("发射箭矢"+Time.time);
        isForwardRoll = false;//结束前摇
        if (S1_remain > 0)//射出三支箭矢
        {
            S1_remain -= 1;
            Shot();
            Invoke("Shot", 0.1f);
            Invoke("Shot", 0.2f);
        }
        else
            Shot();

    }

    public void OnAnimationEvent_HBSSSkill2()//动画帧事件触发二技能函数
    {
        S2.HBSS_2(9,attackNum, State.CreatDecelerate("寒冰二技能法球减速", 0.5f, 3));
    }

    public void OnAnimationEvent_HBSSSkill3()//动画帧事件触发三技能函数
    {
        //Debug.Log("寒冰三技能发射" + Time.time);
        if(gameObject.tag=="Player")
        {
            GameObject.FindWithTag("MainCamera").GetComponent<FollowCamera>().ToShake(0.3f);
        }
        S3.HBSS_3(this,attackNum*3f, attackNum);
    }

    public void OnAnimationEvent_OverSkill()//动画帧事件触发结束使用技能状态
    {
        //Debug.Log("结束技能使用状态");
        isSkill=false;//结束状态
    }


    

    public void Shot()
    {
        PG.YC_PG(State.CreatDecelerate("寒冰PG减速", 0.5f, 3), ObjectsPoolType.HBSS_Arrow_Prefab);
    }





}
