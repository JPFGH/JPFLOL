using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//引入导航命名空间



public enum UnitToAction
{
    PG,
    S1,
    S2,
    S3,
    SX,
}

/// <summary>
/// 所有单位的基类
/// </summary>
public class Unit :MonoBehaviour{
    //单位基类

    public string Name = "单位";
    public int ID;                      //整个游戏每个单位独一无二的ID
    public int level = 1;


    public AttributeType attributeType; //当前单位所使用的属性数据模型

    public NavMeshAgent agent;//英雄导航组件

    public bool alive = true;           //存活状态
    public bool isVertigo = false;      //正在眩晕状态
    public bool alreadyDeath = false;   //已经播放了死亡动画

    public bool beHooked = false;       //是否被勾中

    //下面两个在动画中改变
    //public bool isMove = false;         //正在移动
    public bool isForwardRoll = false;    //PG前摇标志位
    public bool isSkill = false;          //正在技能动画

    float timer = 0;//时间计时

    //下面四个通过给单位添加状态改变
    public bool isPGCD = false;         //正在普攻CD
    public bool isS1CD = false;         //正在一技能CD
    public bool isS2CD = false;         //正在二技能CD
    public bool isS3CD = false;         //正在三技能CD

    public bool isSXCD = false;         //正在闪现CD
    public bool isGZCD = false;         //正在钩子CD

    //遍历状态时赋值，方便UI显示
    public float remainS1CDTime = 0;        //一技能剩余CD
    public float remainS2CDTime = 0;        //二技能剩余CD
    public float remainS3CDTime = 0;        //三技能剩余CD
    public float remainSXCDTime = 0;        //闪现剩余CD
    public float remainGZCDTime = 0;        //钩子剩余CD

    public int HP_Max;
    public int MP_Max;

    private int _hp;
    public int HP//设置生命值
    {
        get
        {
            return _hp;
        }
        set
        {
            if(value<0)
            {
                alive = false;
                GetComponent<Collider>().enabled = false;//关掉碰撞体
                if (gameObject.tag=="Enemy")
                {
                    //玩家得到某物
                    InventoryManager.Instance.GetSomething(level);


                    Transform tran= transform.Find("TargetCircle");
                    if(tran!=null)
                    {
                        tran.SetParent(GameObject.Find("SkillArea").transform);
                    }
                    Destroy(gameObject, deathAnimationTime);
                }
                else
                {
                    //玩家死亡了怎么样
                    UIManager.Instance.ShowDeathPanel();
                }
                
            }
            _hp = Mathf.Clamp(value, 0, HP_Max);
        }
    }
    private int _mp;
    public int MP//设置魔法值
    {
        get
        {
            return _mp;
        }
        set
        {
            if (value > MP_Max)//限制恢复魔法上限
            {
                _mp = MP_Max;
            }
            else if (value < 0)
            {
                Debug.LogError("魔法属性即将降为0，报错");
            }
            else
            {
                _mp = value;
            }
        }
    }

    public Skill PG;
    public Skill S1;
    public Skill S2;
    public Skill S3;
    public Skill SX = new Skill(0, 3, SkillArea.ShortArrow, null, 4, 0, 1.5f);
    public Skill GZ = new Skill(0, 6, SkillArea.ShortArrow, null, 12, 0, 2);

    public float deathAnimationTime;//死亡动画时长

    public float defaultSpeed;    //初始移动速度-动画默认播放速度-----修改速度时不要修改这个
    public float nowSpeed;        //当前移动速度-只会受到减速状态影响
    public int attackNum;         //攻击力
    public float attackSpeed;     //普攻攻速，默认为1
    public float damageReduct;    //伤害减免比率


    public float suckPercent;         //吸血比率，上限为100
    public int violentProbability;   //暴击几率指数，上限为100
    public float violentMul;       //暴击倍率

    public List<State> states = new List<State>(); //状态列表


    public Vector3 dest;//设置组件导航终点


    public int CostHP(float beforeNum,int violentProbability, NumShowDir dir)//申请伤害计算，返回实际最终伤害
    {//理想伤害与暴击率进入
        bool isVio = false;//是否触发了暴击
        float Num = beforeNum+Random.Range(-20,21);//基础数值 随机偏移
        if (GameMode.Probability(violentProbability))//触发暴击
        {
            Num = Num * violentMul;//修改数值
            isVio = true;
        }

        int actualHurtNum= (int)(Num * (1 - damageReduct));//伤害折减


        GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.NumShow_Prefab);
        NumShow numShowInfo = obj.GetComponent<NumShow>();
        if (isVio)
        {
            //调用飘字预制体暴击显示----------------------------
            numShowInfo.InitFlutterAnimation(NumShowType.ViolentHurtNum, actualHurtNum, transform.position, dir);
        }
        else
        {
            //调用飘字预制体普通伤害显示-----------------------------
            numShowInfo.InitFlutterAnimation(NumShowType.NormalHurtNum, actualHurtNum, transform.position, dir);
        }

        HP -= actualHurtNum;
        return actualHurtNum;//返回最后的真实伤害数值

    }
    public void CostMP(float costMPNum)
    {
        MP -= (int)costMPNum;
    }
    public void RecoverHP(int plusHPNum)//生命恢复
    {
        int actualPlusNum = plusHPNum;

        if (plusHPNum>HP_Max-HP)
        {
            actualPlusNum = HP_Max - HP;
        }


        if(actualPlusNum > 0)//显示绿字的条件
        {
            //调用飘字预制体恢复显示---------------------------------
            GameObject obj = ObjectsPool.Instance.DequeueInstance(ObjectsPoolType.NumShow_Prefab);
            NumShow numShowInfo = obj.GetComponent<NumShow>();
            numShowInfo.InitFlutterAnimation(NumShowType.RecoverHPNum, plusHPNum, transform.position, NumShowDir.Null);
        }
        

        HP += actualPlusNum;
    }
    public void RecoverMP(float plusMPNum)
    {
        MP += (int)plusMPNum;
    }





    public void StateEffect()//单位身上状态生效，每帧执行
    {
        timer += Time.deltaTime;
        bool isSettlementFrame;//当前帧为结算帧
        if (timer > 1)//每隔1秒清算人物身上dot伤害
        {
            timer = 0;
            isSettlementFrame = true;
        }
        else
        {
            isSettlementFrame = false;
        }
        

        if (states!=null)
        {
            for (int i = 0; i < states.Count; i++)
            {
                states[i].remainTime -= Time.deltaTime;
                if(states[i].remainTime<=0)
                {
                    states.Remove(states[i]);
                }
            }
            //下面执行还存在单位身上的 具体状态所带来的效果

            #region 处理眩晕状态
            bool haveVertigo = false;//是否还存在眩晕状态
            for (int i = 0; i < states.Count; i++)
            {
                if(states[i].stateType== StateType.Vertigo)
                {
                    haveVertigo = true;
                }
            }
            if (haveVertigo)
            {
                isVertigo = true;
                //眩晕强行取消所有前摇和技能动画
                isForwardRoll = false;
                isSkill = false;
            }
            else
                isVertigo = false;
            #endregion



            #region 处理减速状态
            float maxDeceleratePercent = 0;
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.Decelerate)
                {
                    if (states[i].deceleratePercent >= maxDeceleratePercent)
                    {
                        maxDeceleratePercent = states[i].deceleratePercent;
                    }
                }
            }
            nowSpeed = (1 - maxDeceleratePercent) * defaultSpeed;
            #endregion

            #region 处理当前单位所有DOT状态
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.DOT)
                {
                    if (isSettlementFrame)//每隔1秒清算人物身上dot伤害
                    {
                        Debug.Log(gameObject.name + "dot"+"======");
                        CostHP(states[i].dotNum, 0, NumShowDir.Right);
                    }
                }
            }
            #endregion


            #region 处理PGCD状态
            bool havePGCD = false;//存在PGCD状态
            for(int i=0;i<states.Count;i++)
            {
                if(states[i].stateType== StateType.PGCD)
                {
                    havePGCD = true;
                }
            }
            if (havePGCD)
                isPGCD = true;
            else
            {
               
                isPGCD = false;
            }  
            #endregion

            #region 处理S1CD状态
            bool haveS1CD = false;//存在S1CD状态
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.S1CD)
                {
                    haveS1CD = true;
                    remainS1CDTime = states[i].remainTime;
                }
            }
            if (haveS1CD)
                isS1CD = true;
            else
            {
                isS1CD = false;
                remainS1CDTime = 0;
            }
            #endregion

            #region 处理S2CD状态
            bool haveS2CD = false;//存在S1CD状态
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.S2CD)
                {
                    haveS2CD = true;
                    remainS2CDTime = states[i].remainTime;
                }   
            }
            if (haveS2CD)
                isS2CD = true;
            else
            {
                isS2CD = false;
                remainS2CDTime = 0;
            }
            #endregion

            #region 处理S3CD状态
            bool haveS3CD = false;//存在S1CD状态
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.S3CD)
                {
                    haveS3CD = true;
                    remainS3CDTime = states[i].remainTime;
                }
            }
            if (haveS3CD)
                isS3CD = true;
            else
            {
                isS3CD = false;
                remainS3CDTime = 0;
            }
            #endregion

            #region 处理SXCD状态
            bool haveSXCD = false;//存在SXCD状态
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.SXCD)
                {
                    haveSXCD = true;
                    remainSXCDTime = states[i].remainTime;
                }
            }
            if (haveSXCD)
                isSXCD = true;
            else
            {
                isSXCD = false;
                remainSXCDTime = 0;
            }
            #endregion

            #region 处理GZCD状态
            bool haveGZCD = false;//存在GZCD状态
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].stateType == StateType.GZCD)
                {
                    haveGZCD = true;
                    remainGZCDTime = states[i].remainTime;
                }
            }
            if (haveGZCD)
                isGZCD = true;
            else
            {
                isGZCD = false;
                remainGZCDTime = 0;
            }
            #endregion

        }


    }



    //具体方法在子类重写
    public virtual void Attack()
    {
        
    }
    public virtual void Skill1()
    {

    }
    public virtual void Skill2()
    {

    }
    public virtual void Skill3()
    {

    }

    public void SkillSX()
    {
        if(alive==true)
        {
            if (isVertigo == true || isForwardRoll == true)
            {
                return;
            }
            else
            {
                UnitSkillRotate(UnitToAction.SX);//闪现转向
                SX.UseSX(this,SX.R);//开始闪现
                State.AddState(this, State.CreatSXCD("单位闪现CD状态",SX.CD));
            }
        }
    }
    public void SkillGZ()
    {
        if (alive == true)
        {
            if (isVertigo == true || isForwardRoll == true || isSkill==true )
            {
                return;
            }
            else
            {
                if(gameObject.tag=="Player")//玩家用钩子
                {
                    Quaternion startQua = GameObject.Find("UIRoot/Skills/GZ").GetComponent<SkillJoystick>().skillAreaQua;
                    GZ.UseGZ(this, GZ.R, startQua);//开始开始钩子
                    State.AddState(this, State.CreatGZCD("单位钩子CD状态", GZ.CD));
                }
                else
                {
                    GZ.UseGZ(this, GZ.R, transform.rotation);//开始开始钩子
                    State.AddState(this, State.CreatGZCD("单位钩子CD状态", GZ.CD*Random.Range(2.5f,3.5f)));
                }
            }
        }
    }







    public bool UnitPGRotate()//写在触发PG动画 那一段
    {
        if (gameObject.tag == "Player")//当前单位是玩家，转向交给PG摇杆控制
        {
            Quaternion endQua = PGMainButton.PGQua;
            if(endQua == transform.rotation)//并没有转向
            {
                return false;
            }
            else
            {
                transform.rotation = PGMainButton.PGQua;
                return true;
            }
        }
        else if (gameObject.tag == "Enemy")//如果当前单位是敌人
        {
            GameObject obj= ScopeManager.CheckTargetInCircle(gameObject, PG.R);
            if(obj==null)//敌人PG范围没有玩家
            {
                return false;
            }
            else//敌人PG范围有玩家
            {
                transform.rotation = Quaternion.LookRotation(obj.transform.position - transform.position);
                return true;
            }
        }
        else
        {
            Debug.LogError("无法识别当前单位标签");
            return false;
        }
    }







    public void UnitSkillRotate(UnitToAction choose)//写在触发技能动画 那一段
    {
        if(gameObject.tag=="Player")//当前单位是玩家，转向交给技能摇杆控制
        {
            switch (choose)
            {
                case UnitToAction.S1:
                    transform.rotation = GameObject.Find("UIRoot/Skills/S1").GetComponent<SkillJoystick>().skillAreaQua;
                    break;
                case UnitToAction.S2:
                    transform.rotation = GameObject.Find("UIRoot/Skills/S2").GetComponent<SkillJoystick>().skillAreaQua;
                    break;
                case UnitToAction.S3:
                    transform.rotation = GameObject.Find("UIRoot/Skills/S3").GetComponent<SkillJoystick>().skillAreaQua;
                    break;
                case UnitToAction.SX:
                    transform.rotation = GameObject.Find("UIRoot/Skills/SX").GetComponent<SkillJoystick>().skillAreaQua;
                    break;
                //钩子不用转向
                default:
                    Debug.LogError("应该写技能选择");
                    break;
            }
        }
        else if(gameObject.tag == "Enemy")//如果当前单位是敌人
        {
            transform.rotation = Quaternion.LookRotation(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position);
        }
        else
        {
            Debug.LogError("无法识别当前单位标签");
        }
    }







}






