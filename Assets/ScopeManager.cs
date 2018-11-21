using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeManager  {//范围管理器，工具类

	/// <summary>
    /// 根据玩家设置的锁敌模式，检测圆形范围内符合条件的目标单位
    /// </summary>
    /// <param name="useObj"></param>
    /// <returns></returns>
    public static GameObject CheckTargetInCircle(GameObject useObj,float R)
    {
        string targetTag = "";
        if (useObj.tag == "Player"|| useObj.tag == "Tower")
            targetTag = "Enemy";
        else
            targetTag = "Player";
        Collider[] cols = Physics.OverlapSphere(useObj.transform.position, R, LayerMask.GetMask("Unit"));
        bool condition;
        if(useObj.tag=="Tower")
        {
            condition = cols.Length > 0;
        }
        else
        {
            condition = cols.Length > 1;
        }

        GameObject distanceLeastObj = null;
        GameObject HPLeastObj = null;
        if (condition)//除了自身，还有别的单位
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.tag == targetTag && cols[i].gameObject.GetComponent<Unit>().alive)//是目标标签的单位并且存活
                {
                    if (distanceLeastObj == null || HPLeastObj == null)//只要攻击范围有敌人，就能赋值
                    {
                        distanceLeastObj = cols[i].gameObject;
                        HPLeastObj = cols[i].gameObject;
                    }
                    else
                    {
                        if (Vector3.Distance(cols[i].gameObject.transform.position, useObj.transform.position) < Vector3.Distance(distanceLeastObj.transform.position, useObj.transform.position))
                        {
                            distanceLeastObj = cols[i].gameObject;
                        }
                        if (cols[i].gameObject.GetComponent<Unit>().HP < HPLeastObj.GetComponent<Unit>().HP)
                        {
                            HPLeastObj = cols[i].gameObject;
                        }
                    }
                }
            }
        }
        switch (PlayerSettingData.Instance.PlayerPriorityMode)
        {
            case PriorityMode.Distance://优先距离最近
                if (distanceLeastObj == null)
                    return null;
                else
                    return distanceLeastObj;
            case PriorityMode.HPLeast://优先血量最少
                if (HPLeastObj == null)
                    return null;
                else
                    return HPLeastObj;
            default:
                return null;
        }
    }


    /// <summary>
    /// 圆形范围影响，伤害或附加状态
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="r"></param>
    /// <param name="attackNum"></param>
    /// <param name="addState"></param>
    public static void AOECircle(Unit useInfo, Vector3 pos,float r,float attackNum,State addState)
    {
        string targetTag = "";
        if (useInfo.gameObject.tag == "Player")
            targetTag = "Enemy";
        else
            targetTag = "Player";

        Collider[] cols = Physics.OverlapSphere(pos, r, LayerMask.GetMask("Unit"));
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.tag == targetTag && cols[i].gameObject.GetComponent<Unit>().alive)//是目标标签的单位并且存活
                {
                    //伤害和附加状态
                    cols[i].GetComponent<Unit>().CostHP(attackNum,0,NumShowDir.Right);
                    if(addState!=null)
                    {
                        State.AddState(cols[i].GetComponent<Unit>(), addState);
                    }
                }
            }
        }
        Debug.Log(pos);
    }



}
