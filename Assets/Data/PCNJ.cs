using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//引入导航命名空间

/// <summary>
/// 一个皮城女警就挂一个脚本
/// </summary>
public class PCNJ : Unit {

    public Animator animat;



    int isRunID = Animator.StringToHash("IsRun");
    int toAttackID = Animator.StringToHash("ToAttack");
    int isVertigoID = Animator.StringToHash("IsVertigo");
    int toDeathID = Animator.StringToHash("ToDeath");
    int toSkill1 = Animator.StringToHash("ToSkill1");
    int toSkill2 = Animator.StringToHash("ToSkill2");
    int toSkill3 = Animator.StringToHash("ToSkill3");












    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        animat = GetComponent<Animator>();

        Initialization();
    }
	
	// Update is called once per frame
	void Update () {
        StateEffect();//所有状态生效并改变相应标志位
        StateToAnimation();// 在状态遍历完成后，根据现有移动速度，和攻速调整动画播放速度，和相关动画
        Move();//移动
    }

    /// <summary>
    /// 初始化人物数据
    /// </summary>
    public void Initialization()
    {
        //PCNJData点出来挨个赋值
        deathAnimationTime = 4f;//死亡动画时长
        agent.speed = defaultSpeed;
        attackSpeed = 1f;
        


        //创建技能
        PG = new Skill(0, 2, SkillArea.OuterCircle, this, 8, 0, 0);//PGCD在具体执行时还需根据攻速计算一遍
        S1 = new Skill(150, 8, SkillArea.InnerCircle, this, 6, 3, 0);
        S2 = new Skill(100, 6, SkillArea.ShortArrow, this, 14, 0, 1);
        S3 = new Skill(200, 12, SkillArea.Sector60, this, 10, 0, 0);
    }

    /// <summary>
    /// 移动可行性判断，并控制动画播放
    /// </summary>
    void Move()
    {
        if (alive == true && GetComponent<NavMeshAgent>().enabled == true)
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
        if (alive == true)
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
                State.AddState(this, State.CreatPGCD("皮城女警PGCD状态", PG.CD / attackSpeed));//算好初始普攻动画时长
            }
        }
    }


    //对于玩家而言，所有技能蓝量不足的情况已经被UI层过滤掉了，但敌人没过滤，这里要写一下
    //对于玩家而言，所有技能在人物眩晕情况已经被UI也过滤掉了，但敌人没过滤，这里要写一下
    public override void Skill1()//人物想使用一技能，抬起了技能摇杆
    {
        if (alive == true)
        {
            if (isVertigo == true || isForwardRoll == true || isSkill == true || MP < S1.needMP)
            {
                return;
            }
            else
            {
                CostMP(S1.needMP);
                agent.SetDestination(transform.position);//打断行走
                animat.SetBool(isRunID, false);

                UnitSkillRotate(UnitToAction.S1);//一技能转向

                animat.SetTrigger(toSkill1);//触发一技能动画播放
                isSkill = true;//开始使用技能状态，到动画帧事件或突然眩晕结束状态
                State.AddState(this, State.CreatS1CD("皮城女警S1CD状态", S1.CD));
            }
        }
    }
    public override void Skill2()//人物想使用二技能，抬起了技能摇杆
    {
        if (alive == true)
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
                State.AddState(this, State.CreatS2CD("皮城女警S2CD状态", S2.CD));
            }
        }


    }
    public override void Skill3()//人物想使用三技能，抬起了技能摇杆
    {
        if (alive == true)
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

                animat.SetTrigger(toSkill3);//触发三技能动画播放
                isSkill = true;//开始使用技能状态，到动画帧事件或突然眩晕结束状态
                State.AddState(this, State.CreatS3CD("皮城女警S3CD状态", S3.CD));
            }
        }

    }



    public void StateToAnimation()
    {

        if (alive == false && alreadyDeath == false)
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


    public void OnAnimationEvent_PCNJAttack()//动画帧事件触发PG函数
    {
        //Debug.Log("女警开枪" + Time.time);
        isForwardRoll = false;//结束前摇
        PG.YC_PG(null, ObjectsPoolType.PCNJ_Bullet_Prefab);
    }

    public void OnAnimationEvent_PCNJSkill1()//动画帧事件触发一技能函数
    {
        //Debug.Log("女警一技能触发事件" + Time.time);
        S1.PCNJ_1(attackNum, State.CreatDecelerate("皮城女警一技能AOE减速", 0.5f, 3));
    }

    public void OnAnimationEvent_PCNJSkill2()//动画帧事件触发二技能函数
    {
        S2.PCNJ_2(2 * attackNum, State.CreatDecelerate("皮城女警二技能", 0.8f, 2));
    }

    public void OnAnimationEvent_PCNJSkill3()//动画帧事件触发三技能函数
    {
        //Debug.Log("女警三技能触发事件" + Time.time);
        S3.PCNJ_3(5, attackNum * 1.5f, State.CreatVertigo("皮城女警三技能眩晕", 1.5f));
    }

    public void OnAnimationEvent_OverSkill()//动画帧事件触发结束使用技能状态
    {
        //Debug.Log("结束技能使用状态");
        isSkill = false;//结束状态
    }




}
