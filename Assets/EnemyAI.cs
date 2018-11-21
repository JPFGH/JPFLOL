using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType
{
    HBSS,
    PCNJ,
    SJLR,
}



public class EnemyAI : MonoBehaviour
{
    //一个怪物一个AI
    Unit nowEnemyInfo;
    public AIType nowAIType;//当前AI类型


    // Use this for initialization
    void Start()
    {
        nowEnemyInfo = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");



        if ((float)nowEnemyInfo.HP / nowEnemyInfo.HP_Max >= 0.4)//生命百分比足够
        {
            if (playerObj != null && playerObj.GetComponent<Unit>().alive)//玩家存活
            {
                GameObject pgTarget = ScopeManager.CheckTargetInCircle(gameObject, nowEnemyInfo.PG.R - 3);
                GameObject gzTarget = ScopeManager.CheckTargetInCircle(gameObject, nowEnemyInfo.GZ.R - 3);

                if (!nowEnemyInfo.isGZCD && gzTarget != null)//使用钩子
                {
                    nowEnemyInfo.SkillGZ();
                }
                else if (pgTarget == null)
                {
                    nowEnemyInfo.dest = playerObj.transform.position;
                }
                else
                {
                    nowEnemyInfo.dest = Vector3.zero;//取消步行
                    nowEnemyInfo.Attack();
                }
            }
            else//玩家死亡
            {
                nowEnemyInfo.dest = Vector3.zero;//取消步行
            }
        }
        else//生命不够，逃跑
        {
            nowEnemyInfo.dest = new Vector3(95, 0, 95);
        }



    }












}
